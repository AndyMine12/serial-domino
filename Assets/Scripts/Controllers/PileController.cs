using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controller to handle the 'steal' pile for 2P games
public class PileController : Controller
{
    public PilePiece basePiece;
    public SkipButton baseButton;
    private List<PilePiece> _pile = new List<PilePiece>();
    private BoxCollider2D _spawnZone;
    private ModeController _mode;
    private SkipButton _button;

    protected override void Awake()
    {
        //Activate controller
        this.identifier = "pile";
        base.Awake();
    }
    protected void Start()
    {
        //Initialize spawn zone
        this._spawnZone = this.GetComponent<BoxCollider2D>();
        if (this._spawnZone == null)
        {
            Debug.Log("ERROR. Spawn Zone BoxCollider not found. Deleting controller");
            Destroy(this.gameObject);
        }
        //Get mode controller from active controllers
        this._mode = Controller.GetActiveController<ModeController>("mode");
    }

    protected void Update() {
        this.CheckPile();
    }

    //If pile is empty, show skip button
    public void CheckPile()
    {
        if( (this._pile.Count == 0)&&(this._mode.Mode == 1) ) //No pieces within 'steal' pile, also, game is in standby phase
        {
            if(this._button == null)
            {
                this._button = Instantiate(this.baseButton, this._spawnZone.bounds.center, Quaternion.identity);
                this._button._controller = this;
                this._button.Interact = this._mode.Mode == 1; //If the button is created outside of 'Standby' phase, lock it
            }
        }
        else //Some pieces within 'steal' pile
        {
            if(this._button != null)
            {
                Destroy(this._button.gameObject);
                this._button = null;
            }
        }
    }

    //Skip button was pressed
    public void SkipTurn()
    {
        this._mode.endTurn(); //Turn was ended
    }

    //Create a new piece within the pile's spawn zone
    public void InstantiatePiece (DominoID id) 
    {
        PilePiece newPiece = Instantiate(basePiece,this._spawnZone.bounds.center,Quaternion.identity);
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
        this._pile.Add(newPiece);
    }

    public void DeletePiece (DominoID id) {
        PilePiece objective = this.GetPieceById(id);
        this.DeletePiece(objective);
    }
    public void DeletePiece (PilePiece piece){
        this._pile.Remove(piece);
        Destroy(piece.gameObject, 0.1f);
    }

    public PilePiece GetPieceById (DominoID id) {
        PilePiece found = null;
        foreach(PilePiece objective in this._pile)
        {
            if((objective.Id.ConvertInt == id.ConvertInt)||(objective.Id.ConvertInt == id.getRotate().ConvertInt))
            {
                found = objective;
                break;
            }
        }
        return found;
    }

    //Set all pile pieces to be interact-able by the player
    public void ActivatePile(){
        foreach(PilePiece objective in this._pile)
        {
            objective.Interact = true;
        }
        if(this._button != null) { this._button.Interact = true; }
    }
    //Set all pile pieces to be non interact-able by the player
    public void LockPile(){
        foreach(PilePiece objective in this._pile)
        {
            objective.Interact = false;
        }
        if(this._button != null) { this._button.Interact = false; }
    }

    //Send a specific piece to the player hand
    public void SendHand(DominoID id)
    {
        if(this._mode.Mode == 1) //If game is in standby phase
        {
            //Get Hand Controller from active controllers
            HandController playerHand = Controller.GetActiveController<HandController>("hand");

            playerHand.AddPiece(new DominoID(id.ConvertInt));
            this.SendNetwork(new DominoID (id.ConvertInt));
            this.DeletePiece(id);

            //Refresh hand
            TableController table = Controller.GetActiveController<TableController>("table");
            table.ActivateHand();
        }
    }
    //Send a specific piece to an opponent's hand, using said opponent's number [1~3]. In a two-player game, always remains opponent2
    public void SendHandOpponent(DominoID id, string opponentId)
    {
        //Get Opponent Hand Controller from active controllers
        HandController opponentHand = Controller.GetActiveController<HandController>(opponentId);

        opponentHand.AddPiece(new DominoID(id.ConvertInt));
        this.DeletePiece(id);
    }

    //Send a specific piece to the network adapter
    public bool SendNetwork(DominoID id)
    {
        //Get network adapter from active controllers
        NetworkAdapter network = Controller.GetActiveController<NetworkAdapter>("network");
        
        return ( network.setMode(this.identifier) && network.queuePiece(new DominoID(id.ConvertInt)) );
    }

    //Get score kept in the pile
    public int getScore()
    {
        int score = 0;
        foreach(PilePiece piece in this._pile)
        {
            piece.flip();
            score += piece.getScore();
        }
        return score;
    }
}
