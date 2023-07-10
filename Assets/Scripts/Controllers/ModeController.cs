using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeController : Controller
{
    private int _mode = -1;
    //Modes: -1 -> Wait, 0 -> Dealer, 1 -> Standby
    public int _players = 4;
    public int Mode => this._mode;
    public int PlayerCount => this._players;

    //Mode controller must be initialized and activated before all other controllers
    protected void Awake(){
        this.identifier = "mode";
        Random.InitState(Time.frameCount); //Initialize random number generator
        base.Start();
    }
    protected override void Start(){
        
        //to-do Set mode according to player selection
        //TEST Begin as Player 1: Dealing pieces
            this.generatePieces();
    }

    public void generatePieces(){
        //Get Deal Controller from active controllers
        Controller candidate = Controller.active["deal"];
        if (candidate == null) { throw new System.ArgumentNullException(nameof(candidate), "Deal Controller not found"); }
        if (candidate.GetType() != typeof(DealController))
        {
            throw new System.ArgumentException(nameof(candidate), "Controller registered as 'deal' is not a Deal Controller");
        }
        DealController dealer = (DealController)candidate;

        //Generate and deal pieces
        dealer.GenerateAll();
        //The player must pick their pieces, now we are in dealing mode
        this.dealTurn();
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
        this.SendNetwork();
    }

    //Send a specific action to the network adapter
    public bool SendNetwork()
    {
        Controller candidate = Controller.active["network"];
        if (candidate == null) { throw new System.ArgumentNullException(nameof(candidate), "Network Adapter not found");}
        if (candidate.GetType() != typeof(NetworkAdapter))
        {
            throw new System.ArgumentException(nameof(candidate), "Controller registered as 'network' is not a Network Adapter");
        }
        NetworkAdapter network = (NetworkAdapter)candidate;
        
        return ( network.setMode(this.identifier) && network.sendMode("play") );
    }
}
