using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeController : Controller
{
    private int _mode = -1;
    //Modes: -1 -> Wait, 0 -> Dealer, 1 -> Standby
    private int _players = Setup.PlayerCount;
    private int playerId = Setup.PlayerId; //Is this Player 1? Player 2? Etc
    public int Mode => this._mode;
    public int PlayerCount => this._players;
    public int Player => this.playerId;

    //Mode controller must be initialized and activated before all other controllers
    protected void Awake(){
        this.identifier = "mode";
        Random.InitState(Time.frameCount); //Initialize random number generator
        base.Start();
    }
    protected override void Start(){
        
        //to-do Set mode according to player selection
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
        //to-do Make other controllers react to dealing mode
        this._mode = 0;
    }

    public void beginTurn(){
        //to-do Tell all other controllers to unlock themselves, also, do starting-turn logic such as unlocking certain hand pieces and the pile
    }

    public void endTurn(){
        this._mode = -1;
        //to-do Tell all other controllers to lock themselves
        this.SendStartTurn();
    }

    //Tell the network adapter to let the next player take their turn
    public bool SendStartTurn()
    {
        NetworkAdapter network = Controller.GetActiveController<NetworkAdapter>("network");
        
        return ( network.setMode(this.identifier) && network.sendMode("play") );
    }
}
