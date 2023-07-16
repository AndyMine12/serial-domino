using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{
    public static string portName = "COM1";
    public static int baudRate = 4800;
    public static int PlayerId = 1;
    public static int PlayerCount = 4;

    public static void SetPlayer(int playerId)
    {
        Setup.PlayerId = playerId;
    }
    public static void SetPlayerCount(int count)
    {
        Setup.PlayerCount = count;
    }
    public static void SetBaudRate(int rate)
    {
        Setup.baudRate = rate;
    }
    public static void SetPort(string portName)
    {
        Setup.portName = portName;
    }
}
