using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

//Loads a list of all available ports onto a dropdown option field
public class PortLoader : MonoBehaviour
{
    private List<string> _ports = new List<string>();
    private TMPro.TMP_Dropdown _dropdown;
    // Start is called before the first frame update
    void Start()
    {
        this._dropdown = this.GetComponent<TMPro.TMP_Dropdown>();
        foreach(string portName in SerialPort.GetPortNames())
        {
            this._ports.Add(portName);
        }
        this._dropdown.ClearOptions();
        this._dropdown.AddOptions(this._ports);
    }

    void Update()
    {
        if (this._dropdown.options[this._dropdown.value].text != Setup.portName)
            Setup.SetPort(this._dropdown.options[this._dropdown.value].text);
    }
}
