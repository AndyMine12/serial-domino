using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TablePiece : DominoPiece
{
    public TableController _controller;

    //If the table piece is set to be a double piece, place it 'perpendicular' to the rest
    public override void changeId(DominoID newId)
    {
        if (this.isAlternate != newId.isDouble)
        {
            this.toggleAlternate();
        }
        base.changeId(newId);
    }

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

    //Table pieces may only be deleted in cascade, through using id 'null' as identifier for this instruction
    public override DominoPiece deleteById(DominoID id)
    {
        if (id == null) //Do cascade elimination
        {
            if (this.NextPiece != null)
            {
                this.NextPiece = this._nextPiece.deleteById(id);
            }
            Destroy(this.gameObject, 0.1f);
        }
        return null;
    }

    public override void updatePos(Vector3 newPos){
        base.updatePos(newPos);
    }
    public void updatePos(Vector3 newPos, bool goLeft, bool goRight)
    {
        base.updatePos(newPos);
        Vector3 padding;

        if ( (this._nextPiece != null)&&(goRight) )
        {
            padding = new Vector3 (this.PieceSize.x + this._nextPiece.PieceSize.x, 0f, 0f);
            TablePiece next = (TablePiece) this._nextPiece;
            next.updatePos(newPos + padding, false, true);
        }
        if ( (this._prevPiece != null)&&(goLeft) )
        {
            padding = new Vector3 (this.PieceSize.x + this._prevPiece.PieceSize.x, 0f, 0f);
            TablePiece prev = (TablePiece) this._prevPiece;
            prev.updatePos(newPos - padding, true, false);
        }
    }


    //Gets the leftmost and rightmost borders of the table's length as a two-float array. First left, then right
    public float[] getBorders(bool doLeftScan = true, bool doRightScan = true)
    {
        float[] borders = new float[2];

        if (doRightScan)
        {
            if (this._nextPiece != null)
            {
                TablePiece nextPiece = (TablePiece) this._nextPiece;
                borders[1] = nextPiece.getBorders(false, true)[1];
            }
            else
            {
                borders[1] = this.transform.position.x + this.PieceSize.x;
            }
        }
        if (doLeftScan)
        {
            if (this._prevPiece != null)
            {
                TablePiece prevPiece = (TablePiece) this._prevPiece;
                borders[0] = prevPiece.getBorders(true, false)[0];
            }
            else
            {
                borders[0] = this.transform.position.x - this.PieceSize.x;
            }
        }

        return borders;
    }

    //Gets the piece values that may be played at the leftmost or rightmost borders of the table. First left, then right
    public int[] getPlayable(bool doLeftScan = true, bool doRightScan = true)
    {
        int[] borders = new int[2];

        if (doRightScan)
        {
            if (this._nextPiece != null)
            {
                TablePiece nextPiece = (TablePiece) this._nextPiece;
                borders[1] = nextPiece.getPlayable(false, true)[1];
            }
            else
            {
                borders[1] = this.BottomValue;
            }
        }
        if (doLeftScan)
        {
            if (this._prevPiece != null)
            {
                TablePiece prevPiece = (TablePiece) this._prevPiece;
                borders[0] = prevPiece.getPlayable(true, false)[0];
            }
            else
            {
                borders[0] = this.TopValue;
            }
        }

        return borders;
    }

    //Counts the amount of pieces that include a given value
    public int getCount(int value, bool doLeftScan = true, bool doRightScan = true)
    {
        int count = 0;

        if ( (doRightScan)&&(this._nextPiece != null) )
        {
            TablePiece nextPiece = (TablePiece) this._nextPiece;
            count += nextPiece.getCount(value, false, true);
        }
        if ( (doLeftScan)&&(this._prevPiece != null) )
        {
            TablePiece prevPiece = (TablePiece) this._prevPiece;
            count += prevPiece.getCount(value, true, false);
        }
        if ( (this.TopValue == value)||(this.BottomValue == value) )
        {
            count += 1;
        }

        return count;
    }
    
}