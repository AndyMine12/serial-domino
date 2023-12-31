using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using System.Threading;

public class SerialAdapter : NetworkAdapter
{
    protected String portName = Setup.portName;
    protected int baudRate = Setup.baudRate;
    public Parity parity = Parity.None;
    public int dataBits = 8;
    public StopBits stopBits = StopBits.One;
    public Handshake handshake = Handshake.None;
    public int readTimeout = 50;
    public float actionDelay = 0.1f; //Delay between execution of actions queued in the message pool
    public string endOfMessageChar = "#";
    public char parserSepareChar = '/';
    private ModeController _mode;
    private SerialPort serialPort;
    private int _senderId = 0; //Identifies the number of the player that sent the message
    private int _partnerId = 0; //Identifies this player's teammate in a four-player match
    private Thread _readThread;
    private bool _doComms = true;
    private List<String> messagePool = new List<String>();

    protected override void Start()
    {
        base.Start();

        //Get 'mode' controller from active controllers
        this._mode = Controller.GetActiveController<ModeController>("mode");
        //Set partner ID if in four-player mode
        if (this._mode.PlayerCount == 4)
        {
            if(this._mode.Player % 2 == 0)
            {
                this._partnerId = this._mode.Player - 1;
            }
            else
            {
                this._partnerId = this._mode.Player + 1;
            }
        }
        if (this._mode.PlayerCount == 2) //In a 2P game, the partner is actually the rival
        {
            if(this._mode.Player == 1)
            {
                this._partnerId = 2;
            }
            else
            {
                this._partnerId = 1;
            }
        }
    }

    public override bool initNetwork()
    {
        serialPort = new SerialPort(portName,baudRate, parity, dataBits, stopBits);
        serialPort.Handshake = handshake;
        serialPort.ReadTimeout = readTimeout;

        serialPort.Open();

        this._readThread = new Thread(readMethod);
        this._readThread.Start();
        StartCoroutine(this.actionExecuter());

        return true; //Network has been setup successfully
    }

    public void readMethod()
    {
        while(this._doComms)
        {
            try
            {
                string message = this.readSerialUntil(this.endOfMessageChar);
                this.messagePool.Add(message);
            }
            catch (System.Exception) 
            { 
                //If read fails, just catch exception
            }
        }
    }

    //If any messages are logged, execute them
    public IEnumerator actionExecuter()
    {
        while(this._doComms)
        {
            if (this.messagePool.Count > 0)
            {
                string message = this.messagePool[0];
                DominoID recievedPiece = this.ParseIn(message);
                if (recievedPiece != null) { this.recievePiece(recievedPiece); }
                this.messagePool.Remove(message);
            }
            yield return new WaitForSeconds(this.actionDelay);
        }
    }

    //Parse incoming messages and set communications mode/recieved piece from string recieved
    public DominoID ParseIn(string message)
    {
        string[] parsed = new string[3] {"", "", ""}; //The message is sent in order player->mode->id, separated by the special char
        int index = 0;
        foreach (char letter in message)
        {
            if (letter != this.parserSepareChar)
            {
                parsed[index] += letter;
            }
            else
            {
                index += 1;
            }
        }

        this._senderId = Convert.ToInt32(parsed[0]);
        //If this player did send the message, throw it
        if (this._senderId == this._mode.Player)
        {
            return null;
        }
        else //If this player did not send it, resend and process it
        {
            this.recieveMode(parsed[1]); //Modify this adapter's mode
            this.sendSerialMessage(message + this.endOfMessageChar); //Resend message

            if (parsed[2] != "") //If the message included a piece, return it
            {
                return new DominoID(Convert.ToInt32(parsed[2]));
            }
            else //Did not contain a piece
            {
                return null;
            }
        }
    }
    //Parse outgoing message and format a string from parameters given
    public string ParseOut(DominoID id)
    {
        string message = "";
        message += this._mode.Player.ToString();
        message += this.parserSepareChar + this._commType;
        if (id != null)
        {
            message += this.parserSepareChar + id.ConvertInt.ToString();
        }
        message += this.endOfMessageChar;
        return message;
    }
    public string ParseOut(string mode)
    {
        string message = "";
        message += this._mode.Player.ToString();
        message += this.parserSepareChar + mode;
        message += this.endOfMessageChar;
        return message;
    }

    public override bool failNetwork()
    {
        //to-do Show error message and send user back to main menu
        
        return true; //Failure handled successfully
    }

    public override bool queuePiece(DominoID id)
    {
        //May send anytime. Duplex communications
        return this.sendPiece(id); //Try and send
    }
    public override bool sendMode(string mode)
    {
        this.sendSerialMessage(this.ParseOut(mode));
        return true; //Sent sucessfully?
    }
    public override bool sendPiece(DominoID id)
    {
        this.sendSerialMessage(this.ParseOut(id));
        return true; //Sent sucessfully?
    }

    public override void recieveMode(string mode)
    {
        this.setMode(mode);
        switch(mode)
        {
            case "deal": //No additional actions are needed for these modes
            case "pile":
            case "table0":
            case "table1":
            case "mode":
            {
                break;
            }
            case "play": //Begin turn if previous player played
            {
                if(this._senderId == this.playerWrap(this._mode.Player - 1))
                {
                    DealController dealer = Controller.GetActiveController<DealController>("deal");
                    if (dealer.isDealing)
                    {
                        this._mode.dealTurn(); //Can now grab from dealt pieces
                    }
                    else
                    {
                        this._mode.beginTurn(); //Begin self turn
                    }
                }
                break;
            }
            case "disconnect": //Disconnect from game
            {
                this.disconnect();
                break;
            }
            default: //If there is no match, do not do additional actions
            {
                break;
            }
        }
    }
    public override void recievePiece(DominoID id)
    {
        switch(this._commType)
        {
            case "deal":
            {
                string opponentId = this.getOpponentID(this._senderId);
                DealController dealer = Controller.GetActiveController<DealController>("deal");
                dealer.SendHandOpponent(new DominoID(id.ConvertInt), opponentId);
                break;
            }
            case "pile":
            {
                string opponentId = this.getOpponentID(this._senderId);
                PileController pile = Controller.GetActiveController<PileController>("pile");
                pile.SendHandOpponent(new DominoID(id.ConvertInt), opponentId);
                break;
            }
            case "table0":
            {
                HandController opponentHand = Controller.GetActiveController<HandController>(this.getOpponentID(this._senderId));
                opponentHand.DeletePiece(id);
                TableController table = Controller.GetActiveController<TableController>("table");

                bool success = table.AddPiece(id, true, false);
                if (!success) { table.AddPiece(id.getRotate(), true, false); }
                break;
            }
            case "table1":
            {
                HandController opponentHand = Controller.GetActiveController<HandController>(this.getOpponentID(this._senderId));
                opponentHand.DeletePiece(id);
                TableController table = Controller.GetActiveController<TableController>("table");

                bool success = table.AddPiece(id, false, false);
                if (!success) { table.AddPiece(id.getRotate(), false, false); }
                break;
            }
        }
    }

    public override bool disconnect()
    {
        this.sendMode("disconnect");
        this._doComms = false; //Stop communications
        this._readThread.Join(); //Close reading thread
        this.serialPort.Close(); //Close port
        StopAllCoroutines(); //Stop coroutines

        this._mode.endGame();
        return true;
    }

    public override bool setMode(string mode)
    {
        this._commType = mode; //to-do If adapter not available, return false?
        return true; //Mode set sucessfully
    }

    public override bool refresh()
    {
        this.listen();
        return true; //Adapter now available
    }

    //"Wraps" the player id amongst all integers, so that player Id's outside of [1, 4] get reduced to said interval
    private int playerWrap (int playerId)
    {
        int remain;
        if (playerId < 0) 
        { 
            remain = (playerId % this._mode.PlayerCount) + this._mode.PlayerCount;
        } 
        else
        {
            remain = playerId % this._mode.PlayerCount;
        }
        switch (remain)
        {
            case 0: return this._mode.PlayerCount;
            default: return remain;
        }
    }
    //Transform a certain integer ID into an opponent's string identifier
    private string getOpponentID (int senderId)
    {
        if (senderId == this._partnerId)
        {
            return "opponent2";
        }
        switch (this._mode.Player)
        {
            case 1:
            {
                switch (senderId)
                {
                    case 3: return "opponent1";
                    case 4: return "opponent3";
                    default: return "";
                }
            }
            case 2:
            {
                switch (senderId)
                {
                    case 3: return "opponent3";
                    case 4: return "opponent1";
                    default: return "";
                }
            }
            case 3:
            {
                switch (senderId)
                {
                    case 1: return "opponent3";
                    case 2: return "opponent1";
                    default: return "";
                }
            }
            case 4:
            {
                switch (senderId)
                {
                    case 1: return "opponent1";
                    case 2: return "opponent3";
                    default: return "";
                }
            }
        }
        return "";
    }

    public void openPort()
    {
        serialPort.Open();
    }

    public void closePort()
    {
        serialPort.Close();
    }

    void sendSerialMessage(String message)
    {
        serialPort.Write(message);
    }

    String readSerialLine()
    {
        try
        {
            return serialPort.ReadLine();
        }
        catch (TimeoutException)
        {
            throw;
        }
        
    }

    String readSerialUntil(String stopChar)
    {
        try
        {
            return serialPort.ReadTo(stopChar);
        }
        catch (TimeoutException)
        {
            throw;
        }
        
    }
}
