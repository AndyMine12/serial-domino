using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;

public class SerialAdapter : NetworkAdapter
{
    public String portName = "COM1";
    public int baudRate = 9600;
    public Parity parity = Parity.None;
    public int dataBits = 8;
    public StopBits stopBits = StopBits.None;
    public Handshake handshake = Handshake.None;
    public int readTimeout = 50;

    private SerialPort serialPort;
    new void Start()
    {
        serialPort = new SerialPort(portName,baudRate, parity, dataBits, stopBits);
        serialPort.Handshake = handshake;
        serialPort.ReadTimeout = readTimeout;

        serialPort.Open();
    }
    
    public void openPort()
    {
        serialPort.Open();
    }

    public void closePort()
    {
        serialPort.Close();
    }

    void sendSerialMessage(String message)
    {
        serialPort.Write(message);
    }

    String readSerialLine()
    {
        try
        {
            return serialPort.ReadLine();
        }
        catch (TimeoutException)
        {
            throw;
        }
        
    }

    String readSerialUntil(String stopChar)
    {
        try
        {
            return serialPort.ReadTo(stopChar);
        }
        catch (TimeoutException)
        {
            throw;
        }
        
    }
}
