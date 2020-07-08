using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpectatingAPI : MonoBehaviour
{
    private SpectatingAPISoccer specApi;    
    private SpectatingCamera specCamScript;

    
    void Start()
    {
        specApi = GameObject.Find("SpectatingAPI").GetComponent<SpectatingAPISoccer>();
        specCamScript = GameObject.Find("SpectatingCamera").GetComponent<SpectatingCamera>();
        
        // Subscribe to events on spectating API 
        SubscribeToAPIEvents();
    }


    private void OnDisable()
    {
        UnsubscribeToAPIEvents();
    }


    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            TestAccessGameData();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            TestGetPlayerState("Messi");        

        if (Input.GetKeyDown(KeyCode.Alpha3))
            TestGetPlayerInfo();

        if (Input.GetKeyDown(KeyCode.Alpha4))
            TestGetPlayerInfoList();

        if (Input.GetKeyDown(KeyCode.Alpha5))
            TestGetPlayerTransform();

        if (Input.GetKeyDown(KeyCode.Alpha6))
            TestGetPlayerStats();

        if (Input.GetKeyDown(KeyCode.Alpha7))
            TestGetPlayerStatsList();

        if (Input.GetKeyDown(KeyCode.Alpha8))
            TestSpectatingCam();

        if (Input.GetKeyDown(KeyCode.Alpha9))
            TestSwapCamera();
    }
    */


    private void SubscribeToAPIEvents()
    {
        SpectatingAPISoccer.goalEvent += TestOnGoal;
        SpectatingAPISoccer.firstHalfStartedEvent += TestOnFirstHalfStarted;
        SpectatingAPISoccer.secondHalfStartedEvent += TestOnSecondHalfStarted;
        SpectatingAPISoccer.matchFinishedEvent += TestOnMatchFinished;
        SpectatingAPISoccer.throwInEvent += TestOnThrowIn;
        SpectatingAPISoccer.cornerEvent += TestOnCorner;
        SpectatingAPISoccer.goalKickEvent += TestGoalKick;
        SpectatingAPISoccer.passEvent += TestOnPass;
        SpectatingAPISoccer.shootEvent += TestOnShoot;
    }


    private void UnsubscribeToAPIEvents()
    {
        SpectatingAPISoccer.goalEvent -= TestOnGoal;
        SpectatingAPISoccer.firstHalfStartedEvent -= TestOnFirstHalfStarted;
        SpectatingAPISoccer.secondHalfStartedEvent -= TestOnSecondHalfStarted;
        SpectatingAPISoccer.matchFinishedEvent -= TestOnMatchFinished;
        SpectatingAPISoccer.throwInEvent -= TestOnThrowIn;
        SpectatingAPISoccer.cornerEvent -= TestOnCorner;
        SpectatingAPISoccer.goalKickEvent -= TestGoalKick;
        SpectatingAPISoccer.passEvent -= TestOnPass;
        SpectatingAPISoccer.shootEvent -= TestOnShoot;
    }


    #region Testing Access to Game Data
    private void TestAccessGameData()
    {
        InGame.InGameState gameState = specApi.GetGameState();
        Debug.Log("Game State: " + gameState.ToString());

        float matchElapsedTime = specApi.GetMatchElapsedTime();
        Debug.Log("Match elapsed time in seconds: " + matchElapsedTime);

        SpectatingAPISoccer.MatchClock matchClock = specApi.GetMatchClock();
        Debug.Log("Minutes: " + matchClock.mins + "     seconds: " + matchClock.secs);

        int scoreVisitingTeam = specApi.GetScoreVisitingTeam();
        int scoreLocalTeam = specApi.GetScoreLocalTeam();
        Debug.Log("Score: local team " + scoreLocalTeam + "     visiting team: " + scoreVisitingTeam);

        int matchHalf = specApi.GetMatchHalf();
        Debug.Log("Match half: " + matchHalf);

        Transform ball = specApi.GetBallTransform();
        Debug.Log("Ball position: " + ball.position.ToString());

        GameObject lastPlayerWithBall = specApi.GetLastPlayerWithBall();
        if (lastPlayerWithBall)
            Debug.Log("Last player with ball: " + lastPlayerWithBall.name);
        else
            Debug.Log("Last player with ball: none");

        GameObject ballOwner = specApi.GetBallOwner();
        if (ballOwner)
            Debug.Log("Current ball owner: " + ballOwner.name);
        else
            Debug.Log("Current ball owner: none");

        Debug.Log("----------");
    }
    #endregion


    #region Testing Event Callbacks
    private void TestOnGoal()
    {
        GameObject aPlayer = specApi.GetLastPlayerWithBall();

        Debug.Log("Event: a goal was just scored... 000");
        Debug.Log("The name of the player that score is: " + aPlayer.name);
    }   


    private void TestOnFirstHalfStarted()
    {
        //Debug.Log("Event: first half started... 111");
    }


    private void TestOnSecondHalfStarted()
    {
        //Debug.Log("Event: second half started... 222");
    }


    private void TestOnMatchFinished()
    {
        //Debug.Log("Event: match has finished... 333");
    }


    private void TestOnThrowIn()
    {
        Debug.Log("Event: throw in... 444");
    }


    private void TestOnCorner()
    {
        Debug.Log("Event: corner kick... 555");
    }    


    private void TestOnPass()
    {
        //Debug.Log("Event: a pass was made... 66");
    }


    private void TestOnShoot()
    {
        Debug.Log("Event: a shoot was made... 777");
    }


    private void TestGoalKick()
    {
        Debug.Log("Event: goal kick... 888");
    }
    #endregion


    #region Testing Access to Player Data
    private void TestGetPlayerState(string name)
    {        
        int id = specApi.GetPlayerUniqueId(name);
        if (id > -1)
        {
            string playerStateName = specApi.GetPlayerState(id);
            Debug.Log("Player id: " + id + "     name: " + name + "     current state: " + playerStateName);
            Debug.Log("----------");
        }
    }


    private void TestGetPlayerInfo()
    {
        int id = 7;
        SpectatingAPISoccer.PlayerInfo playerInfo;        
        playerInfo = specApi.GetPlayerInfo(id);
        PrintPlayerInfo(playerInfo);        
    }


    private void TestGetPlayerInfoList()
    {
        List<SpectatingAPISoccer.PlayerInfo> playersInfoList = specApi.GetAllPlayersInfoList();
        foreach (SpectatingAPISoccer.PlayerInfo playerInf in playersInfoList)
        {
            PrintPlayerInfo(playerInf);
        }
    }


    private void PrintPlayerInfo(SpectatingAPISoccer.PlayerInfo playerInfo)
    {
        Debug.Log("playerInfo.id   " + playerInfo.id);
        Debug.Log("playerInfo.gameObj   " + playerInfo.gameObj.name);
        Debug.Log("playerInfo.name   " + playerInfo.name);
        Debug.Log("playerInfo.team   " + playerInfo.team);
        Debug.Log("playerInfo.type   " + playerInfo.type.ToString());
        Debug.Log("playerInfo.stamina   " + playerInfo.stamina);
        Debug.Log("playerInfo.speed   " + playerInfo.speed);
        Debug.Log("-----------");
    }


    private void TestGetPlayerTransform()
    {
        int playerId = 15;
        Transform playerT = specApi.GetPlayerTransform(playerId);
        Debug.Log("Player id: " + playerId + "     position: " + playerT.position.ToString());
        Debug.Log("Player id: " + playerId + "     rotation: " + playerT.rotation.eulerAngles.ToString());
        Debug.Log("Player id: " + playerId + "     local scale: " + playerT.localScale.ToString());
        Debug.Log("-----------");
    }
    #endregion


    #region Testing Access to Player Stats
    private void TestGetPlayerStats()
    {
        int playerId = 9;
        SpectatingAPISoccer.PlayerMatchStats playerStats;
        playerStats = specApi.GetPlayerStats(playerId);

        PrintPlayerStats(playerStats);
    }


    private void TestGetPlayerStatsList()
    {
        List<SpectatingAPISoccer.PlayerMatchStats> playersStatsList = specApi.GetAllPlayersStatsList();
        foreach (SpectatingAPISoccer.PlayerMatchStats playerStats in playersStatsList)
        {
            PrintPlayerStats(playerStats);
        }
    }


    private void PrintPlayerStats(SpectatingAPISoccer.PlayerMatchStats playerStats)
    {
        Debug.Log("Player id: " + playerStats.id);
        Debug.Log("Player passes: " + playerStats.passes);
        Debug.Log("Player shoots: " + playerStats.shoots);
        Debug.Log("Player goals: " + playerStats.goals);
        Debug.Log("-----------");
    }
    #endregion


    #region Test Spectating Camera
    private void TestSpectatingCam()
    {
        Transform targetToLookAt = specApi.GetBallTransform();
        Transform positionRef = specApi.GetLastPlayerWithBall().transform;

        specCamScript.SetTargetToLook(targetToLookAt);
        specCamScript.SetPositionReference(positionRef);
        specCamScript.isEnabled = true;
        specCamScript.lookAtTarget = true;
    }


    private void TestSwapCamera()
    {        
        specCamScript.SwapCamera();
    }
    #endregion
}
