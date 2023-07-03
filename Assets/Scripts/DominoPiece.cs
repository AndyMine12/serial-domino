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

    //Async-load the new sprite requested
    public IEnumerator updateSprite()
    {
        ResourceRequest loader = Resources.LoadAsync("Sprites/Domino/" + this.getSpriteName(), typeof(UnityEngine.Sprite));
        do
            yield return loader;
        while (!loader.isDone);

        Object loadedAsset = loader.asset;
        if (loadedAsset.GetType() == typeof(UnityEngine.Sprite))
        {
            this._renderer.sprite = loadedAsset as UnityEngine.Sprite;
        }
        else
        {
            throw new System.ArgumentNullException(nameof(loader), "Cannot find the sprite in the path specified");
        }
    }

    //View- and Piece-transformations
    public void toggleAlternate(){
        this.isAlternate = !this.isAlternate;
        this.StopCoroutine(this.updateSprite());
        this.StartCoroutine(this.updateSprite());
    }
    public void flip(){
        this.isVisible = !this.isVisible;
        this.StopCoroutine(this.updateSprite());
        this.StartCoroutine(this.updateSprite());
    }
    public void toggleView(){
        this.isTopDown = !this.isTopDown;
        this.StopCoroutine(this.updateSprite());
        this.StartCoroutine(this.updateSprite());
    }
    public void rotate(){
        this._id = new DominoID(new int[2] {this.TopValue, this.BottomValue});
        this.StopCoroutine(this.updateSprite());
        this.StartCoroutine(this.updateSprite());
    }
    public void changeId(DominoID newId){
        this._id = newId;
        this.StopCoroutine(this.updateSprite());
        this.StartCoroutine(this.updateSprite());
    }

}
