using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetRoundBtn : MonoBehaviour
{
    public void resetRnd()
    {
        ModeController mode = Controller.GetActiveController<ModeController>("mode");
        mode.resetScene();
    }
}
