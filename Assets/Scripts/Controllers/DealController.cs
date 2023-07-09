using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controller to handle the dealing of pieces at the start of/during a game
public class DealController : Controller
{
    public DealPiece basePiece;
    private List<DealPiece> _spawned = new List<DealPiece>();
    private BoxCollider2D _spawnZone;
    private ModeController _mode;
    private List<DominoID> _selected = new List<DominoID>();
    private int _currentDepth = 100;

    protected override void Start()
    {
        //Activate controller
        this.identifier = "deal";
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
    }

    //Create a new full-pool of pieces
    public void GenerateAll()
    {
        for (int bottomVal = 0;bottomVal <= 6;bottomVal++)
        {
            for (int topVal = bottomVal;topVal <= 6;topVal++)
            {
                this.InstantiatePiece(new DominoID(new int[2] {bottomVal, topVal}));
            }
        }
        this.ActivatePieces();
    }

    //Create a new piece within the dealer's spawn zone
    public void InstantiatePiece (DominoID id) 
    {
        DealPiece newPiece = Instantiate(basePiece,this._spawnZone.bounds.center,Quaternion.identity);
        newPiece.changeId(id);
        if(newPiece.isVisible){newPiece.flip();}
        float seed = Random.Range(-1f, 1f);
        Vector3 modifier = this._spawnZone.bounds.extents - newPiece.PieceSize;
        modifier.x *= seed;
        seed = Random.Range(-1f,1f);
        modifier.y *= seed;
        modifier.z = 0f;
        Vector3 spawnPos = this._spawnZone.bounds.center + modifier;
        newPiece.updatePos(spawnPos);
        newPiece._controller = this;
        //Pieces that are instantiated first are 'deeper' than those instantiated later
        newPiece.setDepth(this._currentDepth);
        this._currentDepth -= 1; //Next piece is 'higher'
        this._spawned.Add(newPiece);
    }

    public void DeletePiece (DominoID id) {
        DealPiece objective = this.GetPieceById(id);
        this.DeletePiece(objective);
    }
    public void DeletePiece (DealPiece piece){
        this._spawned.Remove(piece);
        Destroy(piece.gameObject, 0.1f);
    }

    public DealPiece GetPieceById (DominoID id) {
        DealPiece found = null;
        foreach(DealPiece objective in this._spawned)
        {
            if(objective.Id.ConvertInt == id.ConvertInt)
            {
                found = objective;
                break;
            }
        }
        return found;
    }

    //Set all spawned pieces to be interact-able by the player
    public void ActivatePieces(){
        foreach(DealPiece objective in this._spawned)
        {
            objective.Interact = true;
        }
    }
    //Set all spawned pieces to be non interact-able by the player
    public void LockPieces(){
        foreach(DealPiece objective in this._spawned)
        {
            objective.Interact = false;
        }
    }

    //Select a specific piece from those dealt, when all seven pieces are selected, send all to hand. 
    public void QueueHand(DominoID id)
    {
        if(this._selected.Count < 7)
        {
            DealPiece select = this.GetPieceById(id);
            select.Select();
            this._selected.Add(id);
        }
        if(this._selected.Count == 7) //All pieces selected. Send to hand
        {
            this.LockPieces(); //Cannot select more pieces
            foreach(DominoID piece in this._selected)
            {
                this.SendHand(piece);
            }
        }
    }


    //Send a specific piece to the player hand
    public void SendHand(DominoID id)
    {
        if(this._mode.Mode == 0) //If game is in dealing phase
        {
            //Get Hand Controller from active controllers
            Controller candidate = Controller.active["hand"];
            if (candidate == null) { throw new System.ArgumentNullException(nameof(candidate), "Hand Controller not found"); }
            if (candidate.GetType() != typeof(HandController))
            {
                throw new System.ArgumentException(nameof(candidate), "Controller registered as 'hand' is not a Hand Controller");
            }
            HandController playerHand = (HandController)candidate;

            playerHand.AddPiece(new DominoID(id.ConvertInt));
            //to-do Must also send piece to network
            //to-do Must also set game to 'wait' && lock pile
            this.DeletePiece(id);
        }
    }

    //Send a specific piece to the network adapter
    public bool SendNetwork(DominoID id)
    {
        //Get network adapter from active controllers
        Controller candidate = Controller.active["network"];
        if (candidate == null) { throw new System.ArgumentNullException(nameof(candidate), "Network Adapter not found");}
        if (candidate.GetType() != typeof(NetworkAdapter))
        {
            throw new System.ArgumentException(nameof(candidate), "Controller registered as 'network' is not a Network Adapter");
        }
        NetworkAdapter network = (NetworkAdapter)candidate;
        
        return ( network.setMode(this.identifier) && network.queuePiece(new DominoID(id.ConvertInt)) );
    }
}
