using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilePiece : DominoPiece
{
    public PileController _controller;

    protected override void Start()
    {
        this.isVisible = false;
        base.Start();
    }

    private void OnMouseUpAsButton() {
        if(this.Interact)
            this._controller.SendHand(this._id);
    }

    protected override bool connectToBottom(DominoPiece pieceToConnect)
    {
        //Pile pieces do not connect to one another. Thus, always return false
        return false;
    }
    protected override bool connectToTop(DominoPiece pieceToConnect)
    {
        //Pile pieces do not connect to one another. Thus, always return false
        return false;
    }
    public override bool appendPiece(DominoPiece pieceToAppend)
    {
        //Pile pieces do not connect to one another. Thus, always return false
        return false;
    }
    public override bool stackPiece(DominoPiece pieceToAppend)
    {
        //Pile pieces do not connect to one another. Thus, always return false
        return false;
    }
    public override DominoPiece deleteById(DominoID id)
    {
        //Since Pile pieces do not connect to one another, try to delete self if Id matches
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

    public int getScore()
    {
        return (this.TopValue + this.BottomValue);
    }
}
