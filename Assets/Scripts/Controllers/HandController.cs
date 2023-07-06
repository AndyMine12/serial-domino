using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : Controller
{
    //to-do private List<HandPiece> _hand;
    public bool show = false;

    protected override void Start(){
        this.identifier = "hand";
        base.Start();
    }

    public void AddPiece(DominoID id){

    }
}
