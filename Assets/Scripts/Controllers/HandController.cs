using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : Controller
{
    private HandPiece _handHead = null;
    private Vector3 _spawnPos;
    private int handSize = 0;
    public bool show = false; //Is this hand visible to the player?
    public HandPiece basePiece;
    public bool doHorizontalDisp = true; //Is this hand displayed horizontally?

    protected override void Awake(){
        if (this.identifier == "") {this.identifier = "hand";}
        base.Awake();
    }

    protected void Start(){
        this._spawnPos = this.transform.position;
    }

    public void AddPiece(DominoID id){
        //TEST AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            Debug.Log("Reached AddPiece. ID: " + this.identifier + "///" + id.ToString(), this);
        
        HandPiece newPiece = this.InstantiatePiece(id);
        //TEST AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            Debug.Log("Outside InstantiatePiece()");
        this.handSize += 1; //Hand is bigger

        if (this._handHead == null) 
        {
            Debug.Log("Created head!");
            this._handHead = newPiece;
        }
        else
        {
            if(this._handHead.appendPiece(newPiece))
            {
                Vector3 spawnAdjust;
                if (this.doHorizontalDisp)
                {
                    spawnAdjust = new Vector3(this._handHead.PieceSize.x * 2, 0f, 0f);
                }
                else
                {
                    spawnAdjust = new Vector3(0f, this._handHead.PieceSize.y * 2, 0f);
                }
                Vector3 spawnPos = this._spawnPos - ((this.handSize / 2) * spawnAdjust);
                this._handHead.updatePos(spawnPos);
            }
        }

        //TEST BBBBBBBBBBB
            Debug.Log("Head: " + this._handHead.ToString());
            Debug.Log("Size: " + this.handSize);
    }

    public HandPiece InstantiatePiece(DominoID id){
        //TEST tracerouting
            Debug.Log("Inside InstantiatePiece!", this);
        HandPiece newPiece;
        //TEST
            Debug.Log("Local declaration");
        newPiece = Object.Instantiate(this.basePiece,this._spawnPos,Quaternion.identity);
        //TEST tracin' v3
            Debug.Log("Instantiated!");
        newPiece.changeId(id);
        //TEST moar tracin
            Debug.Log("Created le piece: " + newPiece.Id.ToString());
        if(newPiece.isVisible != this.show){newPiece.flip();}
        if(newPiece.isAlternate != this.doHorizontalDisp){newPiece.toggleAlternate();}
        newPiece._controller = this;
        
        return newPiece;
    }

    //Activates pieces with values in 'valid' within their own values
    public void ActivateHand(int[] valid)
    {
        this._handHead.Activate(valid, false);
    }
    //Activates all pieces in the player's hand
    public void ActivateHand()
    {
        this.ActivateHand(new int[] {0,1,2,3,4,5,6});
    }
    //Activates specified piece, if found
    public void ActivateHand(DominoID target)
    {
        this._handHead.Activate(new int[2] {target.BottomValue, target.TopValue}, true);
    }
    //Deactivates all pieces in the player's hand
    public void LockHand()
    {
        this._handHead.Lock();
    }
}
