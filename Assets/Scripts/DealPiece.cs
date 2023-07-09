using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Pieces used for dealing  
public class DealPiece : DominoPiece
{
    public DealController _controller;
    protected bool _doColor = true;
    public override bool Colorize => this._doColor;

    protected override void Start()
    {
        this.isVisible = false;
        if (Random.Range(0,2) == 1) //50% chance of creating alternate piece (have more variety on pieces on table)
        {
            this.isAlternate = !this.isAlternate;
        }
        base.Start();
    }

    private void OnMouseUpAsButton() {
        if(this.Interact)
            this._controller.QueueHand(this._id);
    }

    //Indicate to this piece that it has been selected
    public void Select() {
        this._doColor = false;
        this.Interact = false;
        if(!this.isVisible){ this.flip(); }
    }

    protected override bool connectToBottom(DominoPiece pieceToConnect)
    {
        //Deal pieces do not connect to one another. Thus, always return false
        return false;
    }
    protected override bool connectToTop(DominoPiece pieceToConnect)
    {
        //Deal pieces do not connect to one another. Thus, always return false
        return false;
    }
    public override bool appendPiece(DominoPiece pieceToAppend)
    {
        //Deal pieces do not connect to one another. Thus, always return false
        return false;
    }
    public override bool stackPiece(DominoPiece pieceToAppend)
    {
        //Deal pieces do not connect to one another. Thus, always return false
        return false;
    }
    public override DominoPiece deleteById(DominoID id)
    {
        //Since Deal pieces do not connect to one another, try to delete self if Id matches
        if(this.Id.ConvertInt == id.ConvertInt)
        {
            Destroy(this.gameObject, 0.1f);
            return null;
        }
        else
        {
            return this;
        }
    }
}
