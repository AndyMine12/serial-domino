using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreLoader : MonoBehaviour
{
    public int teamId;
    private TMPro.TextMeshProUGUI text;

    private void Start() 
    {
        text = this.GetComponent<TMPro.TextMeshProUGUI>();
    }

    private void Update() 
    {
        text.SetText("EQUIPO " + (teamId + 1).ToString() + ": " + Setup.Score[teamId].ToString());
    }
}
