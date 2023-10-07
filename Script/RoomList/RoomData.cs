using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomData
{
    public string RoomName { get; set; }
    public string GameMode { get; set; }
    public string Round { get; set; }
    public string Map { get; set; }
    public bool Status { get; set; }
    public int CurrentPlayers { get; set; }
    public int MaxPlayers { get; set; }

    public RoomData() { }
    public RoomData(string RoomName, string GameMode, string Round, string Map, 
        bool Status, int CurrentPlayers, int MaxPlayers)
    {
        this.RoomName = RoomName;
        this.GameMode = GameMode;
        this.Round = Round;
        this.Map = Map;
        this.Status = Status;
        this.CurrentPlayers = CurrentPlayers;
        this.MaxPlayers = MaxPlayers;
    }
}
