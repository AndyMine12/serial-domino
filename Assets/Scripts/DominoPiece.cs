using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Parent object that models a piece of domino. Can be linked to other domino pieces
public abstract class DominoPiece : Colorable
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
    protected bool _canInteract = false;

    public int TopValue => this._id.TopValue;
    public int BottomValue => this._id.BottomValue;
    public DominoID Id => this._id;
    public override bool Interact {get => this._canInteract; set => this._canInteract = value;}
    public override bool Colorize => true;
    public DominoPiece PrevPiece {get => this._prevPiece; set => this._prevPiece = value;}
    public DominoPiece NextPiece {get => this._nextPiece; set => this._nextPiece = value;}

    public Vector3 PieceSize 
    {
        get
        {
            Collider2D collider = this.GetComponent<Collider2D>();
            return collider.bounds.extents;
        }
    }

    protected virtual void Start() {
        this._renderer = this.GetComponent<SpriteRenderer>();
        StartCoroutine (this.updateSprite());
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

    //Add piece to the last position of the linked list
    public abstract bool appendPiece(DominoPiece pieceToAppend);  
    //Add piece to the first position of the linked list
    public abstract bool stackPiece(DominoPiece pieceToStack);
    //Connect a new piece, next to this one, through this one's bottom
    protected abstract bool connectToBottom(DominoPiece pieceToConnect);
    //Connect a new piece, before this one, through this one's top
    protected abstract bool connectToTop(DominoPiece pieceToConnect);
    //Delete a piece from the list using their ID
    public abstract DominoPiece deleteById(DominoID id);
    
    //Async-load the new sprite requested
    public IEnumerator updateSprite()
    {
        string path = "Domino/" + this.getSpriteName();
        ResourceRequest loader = Resources.LoadAsync(path, typeof(UnityEngine.Sprite));
        do
            yield return loader;
        while (!loader.isDone); 

        Object loadedAsset = loader.asset;
        if (loadedAsset != null)
        {
            if (loadedAsset.GetType() == typeof(UnityEngine.Sprite))
            {
                this._renderer.sprite = loadedAsset as UnityEngine.Sprite;
            }
            else
            {
                throw new System.ArgumentException(nameof(loader), "Sprite found in specified path is not a Sprite");
            }
        }
        else
        {
            throw new System.ArgumentNullException(nameof(loader), "Cannot find the sprite in the path specified");
        }
    }

    //View- and Piece-transformations
    //Set alternate (TopView-Vertical and Isometric-Right) sprites on/off
    public void toggleAlternate(){
        this.isAlternate = !this.isAlternate;
        this.StopCoroutine(this.updateSprite());
        this.StartCoroutine(this.updateSprite());
    }
    //Flip the piece (make the piece visible or not)
    public void flip(){
        this.isVisible = !this.isVisible;
        this.StopCoroutine(this.updateSprite());
        this.StartCoroutine(this.updateSprite());
    }
    //Set drawing style (isometric or top-down view)
    public void toggleView(){
        this.isTopDown = !this.isTopDown;
        this.StopCoroutine(this.updateSprite());
        this.StartCoroutine(this.updateSprite());
    }
    //Rotate the piece (swap top- and bottom-value)
    public void rotate(){
        this.changeId(new DominoID(new int[2] {this.TopValue, this.BottomValue}));
    }
    //Modify the piece's ID for a new one
    public void changeId(DominoID newId){
        this._id = newId;
        this.StopCoroutine(this.updateSprite());
        this.StartCoroutine(this.updateSprite());
    }
    //Update the piece's position
    public virtual void updatePos(Vector3 newPos){
        this.transform.position = newPos;
    }
    //Modify the piece's drawing order and depth
    public virtual void setDepth(int order)
    {
        if (this._renderer == null) { this._renderer = this.GetComponent<SpriteRenderer>(); } //Renderer may not be null
        this._renderer.sortingOrder = order;
    }
    
    //Interaction-related
    public virtual void Activate(int[] valid, bool isStrict = false){ this._canInteract = true; }
    public virtual void Lock(){ this._canInteract = false; }
}
