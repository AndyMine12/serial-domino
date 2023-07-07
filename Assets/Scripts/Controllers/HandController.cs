using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : Controller
{
    private HandPiece _handHead = null;
    private int handSize = 0;
    public bool show = false; //Is this hand visible to the player?
    public HandPiece basePiece;
    public bool doHorizontalDisp = true; //Is this hand displayed horizontally?

    protected override void Start(){
        if (this.identifier == "") {this.identifier = "hand";}
        base.Start();
    }

    public void AddPiece(DominoID id){
        HandPiece newPiece = this.InstantiatePiece(id);
        this.handSize += 1; //Hand is bigger

        if (this._handHead == null) {this._handHead = newPiece;}
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
                Vector3 spawnPos = this.transform.position - ((this.handSize / 2) * spawnAdjust);
                this._handHead.updatePos(spawnPos);
            }
        }

        //TEST Activate full hand, on pair inserts. On size 4, only activate 6|6
            if(this.handSize % 2 == 0)
            {
                if(this.handSize == 4)
                {
                    this.ActivateHand(new DominoID(47));
                }
                else
                    this.ActivateHand();
            }
            else
            {
                this.LockHand();
            }
    }

    public HandPiece InstantiatePiece(DominoID id){
        HandPiece newPiece = Instantiate(basePiece,this.transform.position,Quaternion.identity);
        newPiece.changeId(id);
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
