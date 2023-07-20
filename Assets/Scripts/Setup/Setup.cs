using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{
    public static string portName = "COM1";
    public static int baudRate = 4800;
    public static int PlayerId = 1;
    public static int PlayerCount = 2;
    public static int threshold = 100;
    public static int[] Score = new int[2] {0,0};

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
    public static void AddScore(int score, int teamId)
    {
        Setup.Score[teamId] += score;
    }
}
