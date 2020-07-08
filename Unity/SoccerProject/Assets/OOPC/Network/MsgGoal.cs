using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class MsgGoal
{
    public string eventType;
    public int minute;
    public int second;
    public string playerName;
    public string teamName;
    public int localTeamScore;    
    public int visitingTeamScore;
    public string localTeamName;
    public string visitingTeamName;
}


[Serializable]
public class MsgShoot
{
    public string eventType;    
    public string playerName;
    public string teamName;    
}


[Serializable]
public class MsgCorner
{
    public string eventType;    
    public string playerConcededCorner;
    public string teamAwardedCorner;
    public string playerToThrowIn;
}


public class MsgGoalKick
{
    public string eventType;
    public string playerConcededGk;
    public string teamAwardedGk;
    public string goalKeeperToAct;
}
