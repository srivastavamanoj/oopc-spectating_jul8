using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventNotifier : MonoBehaviour
{
    private SpectatingAPISoccer specApi;
    private UDPSend udpSend;
    private string localTeamName;
    private string visitingTeamName;
    private InGame inGame;


    private void Start()
    {
        specApi = GameObject.Find("SpectatingAPI").GetComponent<SpectatingAPISoccer>();
        udpSend = GetComponent<UDPSend>();

        inGame = GameObject.Find("GameManager").GetComponent<InGame>();
        localTeamName = inGame.team1.name;
        visitingTeamName = inGame.team2.name;

        SubscribeToEvents();        
    }


    private void SubscribeToEvents()
    {
        SpectatingAPISoccer.goalEvent += OnGoal;
        SpectatingAPISoccer.cornerEvent += OnCorner;
        SpectatingAPISoccer.goalKickEvent += OnGoalKick;
    }


    private void UnsubscribeToEvents()
    {
        SpectatingAPISoccer.goalEvent -= OnGoal;
        SpectatingAPISoccer.cornerEvent -= OnCorner;
        SpectatingAPISoccer.goalKickEvent -= OnGoalKick;
    }


    private void OnDisable()
    {
        UnsubscribeToEvents();
    }


    private void OnGoal()
    {
        GameObject aPlayer = specApi.GetLastPlayerWithBall();
        SpectatingAPISoccer.PlayerInfo playerInfo = specApi.GetPlayerInfo(aPlayer);
        
        // TO DO
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


    private void OnCorner()
    {        
        GameObject aPlayer = specApi.GetLastPlayerWithBall();
        string teamAwardedCorner = aPlayer.transform.GetComponent<Team>().otherTeam.transform.name;

        MsgCorner msgCorner = new MsgCorner();
        msgCorner.eventType = "corner";
        msgCorner.playerConcededCorner = aPlayer.name;
        msgCorner.teamAwardedCorner = teamAwardedCorner;
        msgCorner.playerToThrowIn = inGame.candidateToThrowIn.transform.name;

        string json = JsonUtility.ToJson(msgCorner);
        udpSend.SendString(json);
    }


    private void OnGoalKick()
    {
        GameObject aPlayer = specApi.GetLastPlayerWithBall();
        string teamAwardedGk = aPlayer.transform.parent.GetComponent<Team>().otherTeam.transform.name;

        MsgGoalKick msgGoalKick = new MsgGoalKick();
        msgGoalKick.eventType = "goal_kick";
        msgGoalKick.playerConcededGk = aPlayer.name;
        msgGoalKick.teamAwardedGk = teamAwardedGk;
        msgGoalKick.goalKeeperToAct = inGame.goalKeeperToAct.transform.name;

        string json = JsonUtility.ToJson(msgGoalKick);
        udpSend.SendString(json);
    }


    #region TO DO
    private void OnShootDeflected()
    {

    }


    
    private void OnOwnGoal()
    {        
    }

        
    private bool IsOwnGoal()
    {
        bool isOwnGoal = false;


        return isOwnGoal;
    }
    #endregion

}
