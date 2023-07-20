using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Abstract class to define a network adapter of some type
public abstract class NetworkAdapter : Controller
{
    private bool isAvailable = false; //Is the adapter available for requests
    public bool Available => this.isAvailable;
    protected string _commType = "standby"; //The 'mode' of communications that the adapter is on

    //It must activate itself, as a controller
    protected override void Awake()
    {
        this.identifier = "network";
        base.Awake();
    }
    protected virtual void Start()
    {
        this.isAvailable = true;

        //Try to establish connection. If failed, handle it.
        bool isLinked = this.initNetwork();
        if(!isLinked) {this.failNetwork();}
    }

    //Set transmission mode. Is it sending a pile piece, hand piece, dealing pool... Is it sending a configuration (i.e. Network mode, 'can play' instruction)... Etc
    public abstract bool setMode(string mode);
    //Prepare a specific piece to be sent through the network
    public abstract bool queuePiece(DominoID id);
    //Send a mode setting through the network
    public abstract bool sendMode(string mode);
    //Send a specific piece through the network
    public abstract bool sendPiece(DominoID id);
    //Recieve a piece through the network
    public abstract void recievePiece(DominoID id);
    // Recieve a mode setting through the network
    public abstract void recieveMode(string mode);
    //Initialize network communications
    public abstract bool initNetwork();
    //Handle exception: Trigger if initialization fails
    public abstract bool failNetwork();
    //Cancel operation and reset availability of the network adapter
    public abstract bool refresh();
    //Set network adapter to 'available'
    protected void listen()
    {
        this.isAvailable = true;
    }
    //Set network adapter to 'not available'
    protected void ignore()
    {
        this.isAvailable = false;
    }
    //Handle disconnection from network
    public abstract bool disconnect();
}
