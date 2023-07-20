using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeController : Controller
{
    private int _mode = -1;
    //Modes: -2 -> Wait (Round End), -1 -> Wait, 0 -> Dealer, 1 -> Standby, 2 -> End
    private int _players = Setup.PlayerCount;
    private int playerId = Setup.PlayerId; //Is this Player 1? Player 2? Etc
    public int Mode => this._mode;
    public int PlayerCount => this._players;
    public int Player => this.playerId;
    public GameObject endRoundPanel;
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    //Mode controller must be initialized and activated before all other controllers
    protected override void Awake(){
        this.identifier = "mode";
        Random.InitState(Time.frameCount); //Initialize random number generator
        base.Awake();
    }
    protected void Start(){
        this.generatePieces();
        if(this.playerId == 1)
        {
            this.dealTurn();
        }
    }

    public void generatePieces(){
        //Get Deal Controller from active controllers
        DealController dealer = Controller.GetActiveController<DealController>("deal");

        //Generate and deal pieces
        dealer.GenerateAll();
    }

    public void dealTurn()
    {
        this._mode = 0;

        //Get Deal Controller from active controllers. Set generated pieces to be picked
        DealController dealer = Controller.GetActiveController<DealController>("deal");
        dealer.ActivatePieces();
    }

    public void beginTurn(){
        this._mode = 1; //Set game to standby/play phase
        //Check for win condition
        this.winCondition();
        if (this._mode == -2) { return; } //If already in 'round end' mode, do not go any further

        //Get Pile Controller, and unlock pile
        PileController pile = Controller.GetActiveController<PileController>("pile");
        pile.ActivatePile();

        //Unlock table, making table unlock certain hand pieces
        TableController table = Controller.GetActiveController<TableController>("table");
        if (table.isEmpty)
        {
            HandController hand = Controller.GetActiveController<HandController>("hand");
            if (this.PlayerCount == 4)
            {
                if (hand.Contains(new DominoID(new int[2] {6,6})))
                {
                    table.ActivateTable(6,6);
                }
                else
                {
                    this.endTurn();
                }
            }
            if (this.PlayerCount == 2)
            {
                HandController op2 = Controller.GetActiveController<HandController>("opponent2");
                int[] op2Double = op2.highDouble();
                int[] handDouble = hand.highDouble();
                bool playerBegin = true;
                if (op2Double[1] > handDouble[1])
                {
                    playerBegin = false;
                }
                else
                {
                    if (op2Double[0] > handDouble[0])
                    {
                        playerBegin = false;
                    }
                }

                if (playerBegin)
                {
                    if(handDouble[1] == 1)
                    {
                        table.ActivateTable(handDouble[0], handDouble[0]);
                    }
                    else
                    {
                        table.ActivateTable(handDouble[0], -1);
                    }
                }
                else
                {
                    this.endTurn();
                }
            }
        }
        else
        {
            table.ActivateTable();
        }
    }

    public void endTurn(){
        if (this._mode == 1) //If in standby, check for win condition
        {
            this.winCondition();
        }
        this._mode = -1; //Set game to wait phase
        
        //Get Deal Controller from active controllers
        DealController dealer = Controller.GetActiveController<DealController>("deal");
        //Generated pieces cannot be picked up
        dealer.LockPieces();

        //Get Pile Controller, and lock pile
        PileController pile = Controller.GetActiveController<PileController>("pile");
        pile.LockPile();

        //Lock player's hand
        HandController hand = Controller.GetActiveController<HandController>("hand");
        hand.LockHand();

        //Lock table
        TableController table = Controller.GetActiveController<TableController>("table");
        table.LockTable();

        this.SendStartTurn();
    }

    //Tell the network adapter to let the next player take their turn
    public bool SendStartTurn()
    {
        NetworkAdapter network = Controller.GetActiveController<NetworkAdapter>("network");
        
        return ( network.setMode(this.identifier) && network.sendMode("play") );
    }

    //Resolve a stuck table in a given round
    public void stuckCondition()
    {
        HandController hand = Controller.GetActiveController<HandController>("hand");
        HandController op2 = Controller.GetActiveController<HandController>("opponent2");
        if (this.PlayerCount == 4)
        {
            HandController op1 = Controller.GetActiveController<HandController>("opponent1");
            HandController op3 = Controller.GetActiveController<HandController>("opponent3");
            int scoreTeam0 = hand.getScore() + op2.getScore();
            int scoreTeam1 = op1.getScore() + op3.getScore();
            if ( scoreTeam0 > scoreTeam1 )
            {
                Setup.Score[0] += scoreTeam0;
            }
            else
            {
                if (scoreTeam0 == scoreTeam1)
                {
                    Setup.Score[0] += scoreTeam0;
                    Setup.Score[1] += scoreTeam1;
                }
                else
                {
                    Setup.Score[1] += scoreTeam1;
                }
            }
        }
        if (this.PlayerCount == 2)
        {
            PileController pile = Controller.GetActiveController<PileController>("pile");
            if (this._mode == -1) //If the stuck happened during 'waiting', the rival managed to stuck it
            {
                Setup.AddScore((pile.getScore() + hand.getScore() - op2.getScore()),1);
            }
            else
            {
                Setup.AddScore((pile.getScore() + op2.getScore() - hand.getScore()),0);
            }
        }
        this.endRound();
    }

    //Resolve a win-condition in a given round
    public void winCondition()
    {
        if (this._mode == -2) { return; } //If already in 'round end' mode, do not check any further
        HandController hand = Controller.GetActiveController<HandController>("hand");
        HandController op2 = Controller.GetActiveController<HandController>("opponent2");
        HandController op1 = Controller.GetActiveController<HandController>("opponent1");
        HandController op3 = Controller.GetActiveController<HandController>("opponent3");
        bool doEnd = false;

        if (this.PlayerCount == 4)
        {
            if ( (hand.Size == 0)||(op2.Size == 0) )
            {
                Setup.AddScore(op1.getScore() + op3.getScore(), 0);
                doEnd = true;
            }
            if ( (op1.Size == 0)||(op3.Size == 0) )
            {
                Setup.AddScore(hand.getScore() + op2.getScore(), 0);
                doEnd = true;
            }
        }
        if (this.PlayerCount == 2)
        {
            if (hand.Size == 0)
            {
                Setup.AddScore(op2.getScore(), 0);
                doEnd = true;
            }
            if (op2.Size == 0)
            {
                Setup.AddScore(hand.getScore(), 1);
                doEnd = true;
            }
        }

        if (doEnd) { this.endRound(); }
    }

    //Call the ending of a round
    public void endRound()
    {
        //Generated pieces cannot be picked up
        DealController dealer = Controller.GetActiveController<DealController>("deal");
        dealer.LockPieces();

        //Get Pile Controller, and lock pile
        PileController pile = Controller.GetActiveController<PileController>("pile");
        pile.LockPile();

        //Lock player's hand
        HandController hand = Controller.GetActiveController<HandController>("hand");
        hand.LockHand();

        //Lock table
        TableController table = Controller.GetActiveController<TableController>("table");
        table.LockTable();

        //Show all remaining pieces
        HandController op2 = Controller.GetActiveController<HandController>("opponent2");
        HandController op1 = Controller.GetActiveController<HandController>("opponent1");
        HandController op3 = Controller.GetActiveController<HandController>("opponent3");
        op1.Show(); 
        op2.Show();
        op3.Show();
        this._mode = -2; //Set mode to round end

        //Adjust score
        if(Setup.Score[0] >= Setup.threshold)
        {
            this.victoryPanel.SetActive(true);
        }
        else
        {
            if(Setup.Score[1] >= Setup.threshold)
            {
                if(Setup.Score[1] > Setup.Score[0])
                {
                    this.defeatPanel.SetActive(true);
                }
                else
                {
                    this.victoryPanel.SetActive(true);
                }
            }
            else
            {
                this.endRoundPanel.SetActive(true);
            }
        }
    }
    //Call the ending of a game
    public void endGame()
    {
        if(this._mode != 2)
        {
            NetworkAdapter network = Controller.GetActiveController<NetworkAdapter>("network");
            network.disconnect();
        }
        this._mode = 2;
        this.resetMain();
    }

    //Reset scene
    public void resetScene()
    {
        SceneManager.LoadScene(1);
    }

    //Go to main menu
    public void resetMain()
    {
        SceneManager.LoadScene(0);
    }
}
