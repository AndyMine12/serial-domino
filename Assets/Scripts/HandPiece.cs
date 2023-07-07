using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPiece : DominoPiece
{
    public HandController _controller;
    public float hopDistance = 0.5f;
    protected Vector3 _startPos;
    protected bool _doColor = true;
    public override bool Colorize => this._doColor;

    protected override void Start()
    {
        this._startPos = this.transform.position;
        base.Start();
    }

    public override void updatePos(Vector3 newPos){
        base.updatePos(newPos);
        this._startPos = this.transform.position;
        Vector3 padding;
        if(this.isAlternate) //Pieces are vertical, hand is horizontally shown
        {
            padding = new Vector3 (this.PieceSize.x * 2, 0f, 0f);
        }
        else //Pieces are horizontal, hand is vertically shown
        {
            padding = new Vector3 (0f, this.PieceSize.y * 2, 0f);
        }

        if (this._nextPiece != null)
        {
            this._nextPiece.updatePos(newPos + padding);
        }
    }
    public void updatePos(Vector3 newPos, bool doCascade)
    {
        if(doCascade)
        {
            this.updatePos(newPos);
        }
        else
        {
            base.updatePos(newPos);
        }
    }
    
    //Deletes the piece with the given ID from the hand.
    public override DominoPiece deleteById(DominoID id)
    {
        if (this.Id.ConvertInt != id.ConvertInt) //This is not the piece you are looking for
        {
            if (this._nextPiece != null) {this._nextPiece = this._nextPiece.deleteById(id);}
            if (this._nextPiece != null) {this._nextPiece.PrevPiece = this;}
            return this;
        }
        else //This is the piece you are looking for
        {
            Destroy(this.gameObject, 0.1f);
            return this._nextPiece;
        }
    }
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

    //Interaction related
    //Activate all hand pieces that include *any* (isStrict = false) valid value in their ID, or that include *all* (isStrict = true) valid value in their ID
    public override void Activate(int[] valid, bool isStrict = false)
    {
        if (!isStrict) //Any value match is alright
        {
            this.Interact = false; //Assume that no valid activation was found
            foreach(int value in valid)
            {
                this.Interact = this.Interact || ( (this.BottomValue == value)||(this.TopValue == value) );
            }
        }
        else //Needs to be an exact match
        {
            if(valid.Length == 2)
            {
                this.Interact = ((this.BottomValue == valid[0])&&(this.TopValue == valid[1])) || ((this.BottomValue == valid[1])&&(this.TopValue == valid[0]));
            }
            else
            {
                this.Interact = false;
            }
        }
        if (this._nextPiece != null) {this._nextPiece.Activate(valid, isStrict);}
    }
    //Deactivate all hand pieces
    public override void Lock()
    {
        this.Interact = false;
        if (this._nextPiece != null) {this._nextPiece.Lock();}
    }

    //To-do: Hand piece tries to play itself when released
    //Mouse-interaction related
    private void OnMouseEnter() {
        if(this.Interact) //Only interact-able pieces react
        {
            if(this.isAlternate) //Vertical piece, horizontal hand
            {
                this.updatePos(this.transform.position + new Vector3 (0f, this.hopDistance, 0f), false);
            }
            else //Horizontal piece, vertical hand
            {
                this.updatePos(this.transform.position - new Vector3 (this.hopDistance, 0f, 0f), false);
            }
        }
    }

    private void OnMouseExit() {
        this.updatePos(this._startPos, false);
    }

    private void OnMouseDrag() {
        if(this.Interact) //Only interact-able pieces react
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = this.transform.position.z;
            this.updatePos(mousePos, false);
        }
    }
}