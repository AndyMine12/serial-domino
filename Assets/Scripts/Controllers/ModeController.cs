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

        //Get Pile Controller, and unlock pile
        PileController pile = Controller.GetActiveController<PileController>("pile");
        pile.ActivatePile();

        //to-do Unlock table, making table unlock certain hand pieces
        //TEST Unlock player's hand
            HandController hand = Controller.GetActiveController<HandController>("hand");
            hand.ActivateHand();
    }

    public void endTurn(){
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

        //to-do Lock table

        this.SendStartTurn();
        //to-do Show banner that other people are playing
    }

    //Tell the network adapter to let the next player take their turn
    public bool SendStartTurn()
    {
        NetworkAdapter network = Controller.GetActiveController<NetworkAdapter>("network");
        
        return ( network.setMode(this.identifier) && network.sendMode("play") );
    }
}
