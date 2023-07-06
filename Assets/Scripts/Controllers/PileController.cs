using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PileController : Controller
{
    public PilePiece basePiece;
    private List<PilePiece> _pile = new List<PilePiece>();
    private BoxCollider2D _spawnZone;
    private ModeController _mode;

    protected override void Start()
    {
        //Activate controller
        this.identifier = "pile";
        base.Start();
        //Initialize spawn zone
        this._spawnZone = this.GetComponent<BoxCollider2D>();
        if (this._spawnZone == null)
        {
            Debug.Log("ERROR. Spawn Zone BoxCollider not found. Deleting controller");
            Destroy(this.gameObject);
        }
        //Get mode controller
        Controller candidate = Controller.active["mode"];
        if (candidate == null) { throw new System.ArgumentNullException(nameof(candidate), "Mode Controller not found"); }
        if (candidate.GetType() != typeof(ModeController))
        {
            throw new System.ArgumentException(nameof(candidate), "Controller registered as 'mode' is not a Mode Controller");
        }
        this._mode = (ModeController)candidate;
        //TEST Le test
            this.InstantiatePiece (new DominoID(new int[2] {0, 0}));
            this.InstantiatePiece (new DominoID(new int[2] {2, 0}));
            this.InstantiatePiece (new DominoID(new int[2] {4, 3}));
            this.InstantiatePiece (new DominoID(new int[2] {6, 6}));
    }

    //Create a new piece within the pile's spawn zone
    public void InstantiatePiece (DominoID id) 
    {
        PilePiece newPiece = Instantiate(basePiece,this._spawnZone.bounds.center,Quaternion.identity);
        if(newPiece.isVisible){newPiece.flip();}
        newPiece.changeId(id);
        float seed = Random.Range(-1f, 1f);
        Vector3 spawnPos = this._spawnZone.bounds.center + seed * (this._spawnZone.bounds.extents - newPiece.PieceSize);
        newPiece.updatePos(spawnPos);
        newPiece._controller = this;
        this._pile.Add(newPiece);
    }

    public void DeletePiece (DominoID id) {
        PilePiece objective = this.GetPieceById(id);
        this.DeletePiece(objective);
    }
    public void DeletePiece (PilePiece piece){
        this._pile.Remove(piece);
        Destroy(piece.gameObject);
    }

    public PilePiece GetPieceById (DominoID id) {
        PilePiece found = null;
        foreach(PilePiece objective in this._pile)
        {
            if(objective.Id.ConvertInt == id.ConvertInt)
            {
                found = objective;
                break;
            }
        }
        return found;
    }

    //Send a specific piece to the player hand
    public void SendHand(DominoID id)
    {
        if(this._mode.Mode == 1) //If game is in standby phase
        {
            //Get Hand Controller from active controllers
            Controller candidate = Controller.active["hand"];
            if (candidate == null) { throw new System.ArgumentNullException(nameof(candidate), "Hand Controller not found"); }
            if (candidate.GetType() != typeof(HandController))
            {
                throw new System.ArgumentException(nameof(candidate), "Controller registered as 'hand' is not a Hand Controller");
            }
            HandController playerHand = (HandController)candidate;

            playerHand.AddPiece(id);
            //to-do Must also send piece to network
            //to-do Must also set game to 'wait'
            this.DeletePiece(id);
        }
    }

    //Send a specific piece to the network adapter
    public bool SendNetwork(DominoID id)
    {
        Controller candidate = Controller.active["network"];
        if (candidate == null) { throw new System.ArgumentNullException(nameof(candidate), "Network Adapter not found");}
        if (candidate.GetType() != typeof(NetworkAdapter))
        {
            throw new System.ArgumentException(nameof(candidate), "Controller registered as 'network' is not a Network Adapter");
        }
        NetworkAdapter network = (NetworkAdapter)candidate;
        
        return ( network.setMode(this.identifier) && network.queuePiece(id) );
    }
}