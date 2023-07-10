using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipButton : Colorable
{
    public PileController _controller;
    private bool _canInteract;
    public override bool Interact {get => this._canInteract; set => this._canInteract = value;}
    public override bool Colorize => true;

    private void OnMouseUpAsButton() {
        if(this.Interact)
            this._controller.SkipTurn();
    }
}
