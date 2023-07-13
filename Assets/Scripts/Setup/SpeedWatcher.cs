using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedWatcher : MonoBehaviour
{
    private TMPro.TMP_Dropdown _dropdown;
    private int _baudRate = 9600;
    // Start is called before the first frame update
    void Start()
    {
        this._dropdown = this.GetComponent<TMPro.TMP_Dropdown>();
    }

    void Update()
    {
        this._baudRate = System.Convert.ToInt32(this._dropdown.options[this._dropdown.value].text);
        if (this._baudRate != Setup.baudRate)
            Setup.SetBaudRate(this._baudRate);
    }
}
