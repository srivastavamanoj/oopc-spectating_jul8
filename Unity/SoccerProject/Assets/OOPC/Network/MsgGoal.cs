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

}
