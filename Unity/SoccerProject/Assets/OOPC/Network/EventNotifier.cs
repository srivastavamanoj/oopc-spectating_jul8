using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventNotifier : MonoBehaviour
{
    private SpectatingAPISoccer specApi;
    private UDPSend udpSend;
    private string localTeamName;
    private string visitingTeamName;


    private void Start()
    {
        specApi = GameObject.Find("SpectatingAPI").GetComponent<SpectatingAPISoccer>();
        udpSend = GetComponent<UDPSend>();

        InGame inGame = GameObject.Find("GameManager").GetComponent<InGame>();
        localTeamName = inGame.team1.name;
        visitingTeamName = inGame.team2.name;

        SubscribeToEvents();

        //Output the current screen window width in the console
        //Debug.Log("Screen Width : " + Screen.width);
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
        SpectatingAPISoccer.PlayerInfo playerInfo = specApi.GetPlayerInfo(aPlayer);
        
        //If last player is goalkeeper then change for the prevToLastPlayerWithBall

        SpectatingAPISoccer.MatchClock matchClock = specApi.GetMatchClock();        

        MsgGoal msgGoal = new MsgGoal();
        msgGoal.eventType = "goal";
        msgGoal.minute = matchClock.mins;
        msgGoal.second = matchClock.secs;
        msgGoal.playerName = aPlayer.name;
        msgGoal.teamName = playerInfo.team;
        msgGoal.localTeamScore = specApi.GetScoreLocalTeam();
        msgGoal.visitingTeamScore = specApi.GetScoreVisitingTeam();
        msgGoal.localTeamName = localTeamName;
        msgGoal.visitingTeamName = visitingTeamName;

        string json = JsonUtility.ToJson(msgGoal);
        udpSend.SendString(json);
    }


    private void OnShootDeflected()
    {
        
    }


    private void OnCornerKick()
    {

    }


    private void OnGoalKick()
    {

    }



    // TO DO
    private void OnOwnGoal()
    {        
    }


    // TO DO
    private bool IsOwnGoal()
    {
        bool isOwnGoal = false;


        return isOwnGoal;
    }


}
