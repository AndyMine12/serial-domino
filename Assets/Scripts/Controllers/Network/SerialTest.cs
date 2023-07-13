using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class SerialTest : MonoBehaviour
{
    private SerialPort port = new SerialPort();

    // Start is called before the first frame update
    void Start()
    {
        port.PortName = "COM1";
        port.BaudRate = 2400;
        port.Parity = Parity.None;
        port.DataBits = 8;
        port.StopBits = StopBits.One;
        port.Handshake = Handshake.None;
        port.ReadTimeout = 50;
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if(port.ReadTo("-") == "asd")
    //     {
    //         Debug.Log("Recibido");
    //         Debug.Log("Resto del mensaje: " + port.ReadLine());
    //     }
    // }

    
}
