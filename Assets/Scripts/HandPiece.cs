using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPiece : DominoPiece
{
    // Start is called before the first frame update
    // override void Start()
    // {
        
    // }

    // Update is called once per frame
    // void Update()
    // {
        
    // }

    public override bool appendPiece(DominoPiece pieceToAppend)
    {
        if(this._nextPiece != null)
        {
            return this._nextPiece.appendPiece(pieceToAppend);
        } else 
        {
            return this.connectToBottom(pieceToAppend);
        }
    }

    public override bool stackPiece(DominoPiece pieceToStack)
    {
        if(this._prevPiece != null)
        {
            return this._prevPiece.stackPiece(pieceToStack);
        } else 
        {
            return this.connectToTop(pieceToStack);
        }
    }

    //No restrictions on hand connections, just connects and always returns true
    protected override bool connectToBottom(DominoPiece pieceToConnect)
    {
        HandPiece handPiece = (HandPiece)pieceToConnect;
        this._nextPiece = handPiece;
        handPiece._prevPiece = this;
        return true;
    }
    //No restrictions on hand connections, just connects and always returns true
    protected override bool connectToTop(DominoPiece pieceToConnect)
    {
        HandPiece handPiece = (HandPiece)pieceToConnect;
        this._prevPiece = handPiece;
        handPiece._nextPiece = this;
        return true;
    }
}