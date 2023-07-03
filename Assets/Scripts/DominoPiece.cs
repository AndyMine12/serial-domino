using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Parent object that models a piece of domino. Can be linked to other domino pieces
public abstract class DominoPiece : MonoBehaviour
{
    //Domino pieces are linked via a double-linked-list
    protected DominoPiece _prevPiece = null;
    protected DominoPiece _nextPiece = null;
    //Domino pieces' id represent them as a two-integer array: bottomvalue first, then topvalue
    protected DominoID _id; 
    protected SpriteRenderer _renderer;
    public bool isAlternate = false;
    public bool isTopDown = true;
    public bool isVisible = true;

    public int TopValue => this._id.TopValue;
    public int BottomValue => this._id.BottomValue;

    protected virtual void Start() {
        this._renderer = this.GetComponent<SpriteRenderer>();
    }
    //Get self sprite name from current ID, rotation, and setting
    public virtual string getSpriteName()
    {
        string spriteName = "Domino";
        if (this.isTopDown)
        {
            spriteName += "_TD";
        }
        else
        {
            spriteName += "_ISO";
        }
        if (this.isAlternate)
        {
            spriteName += "_ALT";
        }
        if (this.isVisible)
        {
            spriteName += "_" + this._id.ConvertInt.ToString();
        }
        else
        {
            spriteName += "_BACK";
        }
        return spriteName;
    }

    //to-do Update self sprite automatically
    public abstract bool appendPiece(DominoPiece pieceToAppend);  
    public abstract bool stackPiece(DominoPiece pieceToStack);
    protected abstract bool connectToBottom(DominoPiece pieceToConnect);
    protected abstract bool connectToTop(DominoPiece pieceToConnect);
}