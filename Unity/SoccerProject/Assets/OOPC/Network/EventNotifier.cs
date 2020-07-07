using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventNotifier : MonoBehaviour
{
    private SpectatingAPISoccer specApi;
    private UDPSend udpSend;



    private void Start()
    {
        specApi = GameObject.Find("SpectatingAPI").GetComponent<SpectatingAPISoccer>();
        udpSend = GetComponent<UDPSend>();

        SubscribeToEvents();
    }


    private void SubscribeToEvents()
    {
        SpectatingAPISoccer.goalEvent += OnGoal;
    }


    private void UnsubscribeToEvents()
    {
        SpectatingAPISoccer.goalEvent += OnGoal;
    }


    private void OnDisable()
    {
        UnsubscribeToEvents();
    }


    private void OnGoal()
    {
        GameObject aPlayer = specApi.GetLastPlayerWithBall();
        SpectatingAPISoccer.MatchClock matchClock = specApi.GetMatchClock();

        MsgPython msgPython = new MsgPython();
        msgPython.eventType = "goal";
        msgPython.minute = matchClock.mins;
        msgPython.second = matchClock.secs;
        msgPython.playerName = aPlayer.name;
        msgPython.localTeamScore = specApi.GetScoreLocalTeam();
        msgPython.visitingTeamScore = specApi.GetScoreVisitingTeam();

        string json = JsonUtility.ToJson(msgPython);
        udpSend.SendString(json);
    }


}
