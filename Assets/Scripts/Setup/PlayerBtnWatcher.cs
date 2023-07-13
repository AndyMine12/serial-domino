using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBtnWatcher : MonoBehaviour
{
    public bool isCount = false; //Is used for player count or player id
    public int edit = 1; //Number to set

    public void setOption()
    {
        if (this.isCount)
        {
            Setup.SetPlayerCount(this.edit);
        }
        else
        {
            Setup.SetPlayer(this.edit);
        }
    }
}
