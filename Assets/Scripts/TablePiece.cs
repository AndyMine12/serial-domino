using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TablePiece : DominoPiece
{
    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    // void Update()
    // {
        
    // }

    //Looks for lowest point of the bundle and tries to connects the piece there, if valid connection returns true
    public override bool appendPiece(DominoPiece pieceToAppend)
    {
        if(this._nextPiece != null)
        {
            return this._nextPiece.appendPiece(pieceToAppend);
        } else 
        {
            return connectToBottom(pieceToAppend);
        }
    }
    //Looks for highest point of the bundle and tries to connects the piece there, if valid connection returns true
    public override bool stackPiece(DominoPiece pieceToStack)
    {
        if(this._prevPiece != null)
        {
            return this._prevPiece.stackPiece(pieceToStack);
        } else
        {
            return connectToTop(pieceToStack);
        }
    }



    //Check permited connection on Bottom of the table hand, connect and return true if valid, only return false if not valid
    protected override bool connectToBottom(DominoPiece pieceToConnect)
    {
        TablePiece tablePiece = (TablePiece)pieceToConnect;
        if(this.BottomValue == tablePiece.TopValue)
        {
            this._nextPiece = tablePiece;
            tablePiece._prevPiece = this;
            return true;
        } else 
        {
            return false;
        }
    }

    //Check permited connection Top on the table hand, connect and return true if valid, only return false if not valid
    protected override bool connectToTop(DominoPiece pieceToConnect)
    {
        TablePiece tablePiece = (TablePiece)pieceToConnect;
        if(this.TopValue == tablePiece.BottomValue)
        {
            this._prevPiece = tablePiece;
            tablePiece._nextPiece = this;
            return true;
        } else 
        {
            return false;
        }
    }

    public override DominoPiece deleteById(DominoID id)
    {
        //to-do To do
        return null;
    }
}