using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeController : Controller
{
    private int _mode = 0;
    //Modes: 0 -> Wait, 1 -> Standby
    public int Mode => this._mode;

    //Mode controller must be initialized and activated before all other controllers
    protected void Awake(){
        this.identifier = "mode";
        base.Start();
    }
    protected override void Start(){
        
        //to-do Set mode according to player selection
        this._mode = 1;
    }

    public void endTurn(){
        this._mode = 0;
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
