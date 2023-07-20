using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Auxiliary class to point out spots where new pieces may be added to the table
public class MarkerPiece : MonoBehaviour
{
    private int _valid;
    private BoxCollider2D _collider;
    private SpriteRenderer _renderer;
    public Color standbyColor;
    public Color acceptColor;
    public Color rejectColor;
    public Color playColor;
    public bool sendLeft;
    private bool _doPlay = true;

    private void Awake() 
    {
        //Initialize collider
        this._collider = this.GetComponent<BoxCollider2D>();
    }
    private void Start() {
        this._renderer = this.GetComponent<SpriteRenderer>();
        this._renderer.color = this.standbyColor;
        if (this._collider == null)
        {
            Debug.Log("ERROR. Trigger BoxCollider not found. Deleting controller");
            Destroy(this.gameObject);
        }
    }
    
    public int Valid
    {
        set => this._valid = value;
        get => this._valid;
    }
    public bool validate(DominoID id)
    {
        return ( (this._valid == id.TopValue)||(this._valid == id.BottomValue) );
    }

    //If the piece found is a HandPiece, react to it. And also, try to validate it
    private void OnTriggerEnter2D(Collider2D other) 
    {
        HandPiece piece = other.gameObject.GetComponent<HandPiece>();
        if (piece == null) { this._renderer.color = this.standbyColor; }
        else
        {
            if (this.validate(piece.Id)) //Check if the play is valid
            {
                this._renderer.color = this.acceptColor;
            }
            else //Invalid play
            {
                this._renderer.color = this.rejectColor;
            }
        }
    }

    //Keep checking if the found piece tries to be played
    private void OnTriggerStay2D(Collider2D other) {
        HandPiece piece = other.gameObject.GetComponent<HandPiece>();
        if (piece != null) 
        { 
            if ( (piece.Play)&&(this._doPlay) ) //The piece is trying to be played and this marker may play
            {
                if (this.validate(piece.Id)) //Check if the play is valid
                {
                    piece.Interact = false;
                    this._doPlay = false;
                    this._renderer.color = this.playColor;
                    
                    //Send to table
                    TableController table = Controller.GetActiveController<TableController>("table");
                    DominoID targetId;
                    if (this.sendLeft)
                    {
                        if (piece.BottomValue == this._valid)
                        {
                            targetId = new DominoID(piece.Id.ConvertInt);
                        }
                        else
                        {
                            targetId = new DominoID(piece.Id.getRotate().ConvertInt);
                        }
                    }
                    else
                    {
                        if (piece.TopValue == this._valid)
                        {
                            targetId = new DominoID(piece.Id.ConvertInt);
                        }
                        else
                        {
                            targetId = new DominoID(piece.Id.getRotate().ConvertInt);
                        }
                    }
                    table.AddPiece(targetId, this.sendLeft, true);

                    //Delete piece from hand
                    HandController hand = Controller.GetActiveController<HandController>("hand");
                    hand.DeletePiece(piece.Id);
                }
                else //Invalid play
                {
                    piece.resetPos();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        this._renderer.color = this.standbyColor;
    }

    public void updatePos (Vector3 newPos)
    {
        if (this.sendLeft)
        {
            newPos.x -= this._collider.bounds.extents.x;
        }
        else
        {
            newPos.x += this._collider.bounds.extents.x;
        }
        this.transform.position = newPos;
    }
}
