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
}
