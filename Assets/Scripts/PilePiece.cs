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
}
