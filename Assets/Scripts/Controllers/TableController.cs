using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableController : Controller
{
    private TablePiece _tableHead = null;
    private Vector3 _spawnPos;
    public TablePiece basePiece;
    public TablePiece baseAltPiece;
    public MarkerPiece baseMarker;
    private MarkerPiece _markerLeft;
    private MarkerPiece _markerRight;
    public bool isEmpty => this._tableHead == null;
    public int[] setMarkers
    {
        set
        {
            if (this._markerLeft != null)
            {
                this._markerLeft.Valid = value[0];
            }
            if (this._markerRight != null)
            {
                this._markerRight.Valid = value[1];
            }
        }
    }

    protected override void Awake(){
        this.identifier = "table";
        base.Awake();
    }

    protected void Start(){
        this._spawnPos = this.transform.position;
    }

    public bool AddPiece (DominoID id, bool isLeftAdd, bool playerPlayed)
    {
        TablePiece newPiece = this.InstantiatePiece(id);
        bool success = false;
        if (this._tableHead == null) { this._tableHead = newPiece; success = true; }
        else
        {
            if (isLeftAdd)
            {
                success = this._tableHead.stackPiece(newPiece);
            }
            else //Is adding to the right of the table
            {
                success = this._tableHead.appendPiece(newPiece);
            }
        }
        this._tableHead.updatePos(this.transform.position, true, true);
        if (success&&playerPlayed) { this.SendNetwork(id, isLeftAdd); }
        if (!success) { Destroy(newPiece.gameObject, 0.1f); }
        
        //Check if table is stuck
        int[] playable = this._tableHead.getPlayable();
        bool isStuck = false;
        ModeController mode = Controller.GetActiveController<ModeController>("mode");
        if ( playable.Length == 1) //If only one type of value is playable, the table might be stuck
        {
            if ( this._tableHead.getCount(playable[0]) == 7 ) //Table is stuck indeed
            {
                isStuck = true;
                mode.stuckCondition();
            }
        }
        if (!isStuck)
        {
            mode.endTurn();
            //TEST
                Debug.Log("turn ended!");
        }

        return success;
    }

    public void ActivateTable()
    {
        this._markerLeft = this.InstantiateMarker(true);
        if (this._tableHead != null)
        {
            this.ActivateHand(this._tableHead.getPlayable());

            float[] borders;
            borders = this._tableHead.getBorders();
            int[] playable = this._tableHead.getPlayable();
            this._markerRight = this.InstantiateMarker(false);

            this._markerLeft.updatePos(new Vector3 (borders[0], this.transform.position.y, this.transform.position.z));
            this._markerRight.updatePos(new Vector3 (borders[1], this.transform.position.y, this.transform.position.z));
            this.setMarkers = playable;
        }
    }
    public void ActivateTable(int leftMark, int rightMark)
    {
        this.ActivateTable();
        if (this._tableHead == null)
        {
            if(rightMark != -1)
            {
                this.ActivateHand(new DominoID(new int[2] {leftMark, rightMark}));
            }
            else
            {
                this.ActivateHand(new int[1] {leftMark});
            }
        }
        this.setMarkers = new int[2] {leftMark, rightMark};
    }

    public TablePiece InstantiatePiece(DominoID id){
        TablePiece newPiece;
        if (id.isDouble)
        {
            newPiece = Object.Instantiate(this.baseAltPiece,this._spawnPos,Quaternion.identity,this.transform);
        }
        else
        {
            newPiece = Object.Instantiate(this.basePiece,this._spawnPos,Quaternion.identity,this.transform);
        }
        newPiece.changeId(id);
        newPiece._controller = this;
        
        return newPiece;
    }
    public MarkerPiece InstantiateMarker(bool isLeftMarker){
        MarkerPiece newMarker;
        newMarker = Object.Instantiate(this.baseMarker,this._spawnPos,Quaternion.identity,this.transform);
        newMarker.sendLeft = isLeftMarker;
        
        return newMarker;
    }

    //Activates pieces with values in 'valid' within their own values
    public void ActivateHand(int[] valid)
    {
        HandController hand = Controller.GetActiveController<HandController>("hand");
        hand.ActivateHand(valid);
    }
    //Activates all playable pieces in the player's hand
    public void ActivateHand()
    {
        if (this._tableHead != null)
        {
            this.ActivateHand(this._tableHead.getPlayable());
        }
    }
    //Activates specified piece, if found
    public void ActivateHand(DominoID target)
    {
        HandController hand = Controller.GetActiveController<HandController>("hand");
        hand.ActivateHand(target);
    }
    //Deactivates the markers
    public void LockTable()
    {
        if (this._markerLeft != null)
        {
            Destroy(this._markerLeft.gameObject, 0.1f);
            this._markerLeft = null;
        }
        if (this._markerRight != null)
        {
            Destroy(this._markerRight.gameObject, 0.1f);
            this._markerRight = null;
        }
    }
    
    //Send a specific piece to the network adapter
    public bool SendNetwork(DominoID id, bool isLeftAdd)
    {
        //Get network adapter from active controllers
        NetworkAdapter network = Controller.GetActiveController<NetworkAdapter>("network");
        string modeset;
        if (isLeftAdd)
        {
            modeset = "table0";
        }
        else
        {
            modeset = "table1";
        }
        
        return ( network.setMode(modeset) && network.queuePiece(new DominoID(id.ConvertInt)) );
    }
}
