using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Modifies the color of a Domino based on whether or not it may interact and user interactions with mouse (hover, click)
public class DominoColorizer : MonoBehaviour
{
    public Color onMouseEnterColor = new Color(1, 0.992f, 0.553f, 1f); //Yellow-ish by default
    public Color onMouseClickColor = new Color(0.506f, 1, 0.439f, 1f); //Green-ish by default
    public Color onMouseDownColor = new Color(0.663f, 0.827f, 1, 1f); //Blue-ish by default
    public Color standbyColor;
    public Color onLockColor = new Color(0.639f, 0.639f, 0.639f, 1f); //Gray-ish by default
    public DominoPiece domino;
    private SpriteRenderer _renderer;
    private bool _isLocked = false;

    private void Start() { 
        this._renderer = this.GetComponent<SpriteRenderer>();
        this.standbyColor = this._renderer.color;
    }
    private void Update() {
        if (this.domino.Colorize)
        {
            if (!this.domino.Interact)
            {
                this._renderer.color = this.onLockColor;
                this._isLocked = true;
            }
            else 
            {
                if (this._isLocked)
                {
                    this._renderer.color = this.standbyColor;
                }
                this._isLocked = false;
            }
        }
    }

    private void OnMouseDown() {
        if(this.domino.Interact && this.domino.Colorize)
            this._renderer.color = this.onMouseDownColor;
    }
    private void OnMouseUpAsButton() {
        if(this.domino.Interact && this.domino.Colorize)
            this._renderer.color = this.onMouseClickColor;
    }
    private void OnMouseEnter() {
        if(this.domino.Interact && this.domino.Colorize)
            this._renderer.color = this.onMouseEnterColor;
    }
    private void OnMouseExit() {
        if(this.domino.Interact && this.domino.Colorize)
            this._renderer.color = this.standbyColor;
    }
}
