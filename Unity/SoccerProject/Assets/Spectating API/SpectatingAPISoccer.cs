using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using UnityEngine;

public class SpectatingAPISoccer : MonoBehaviour
{
    public struct MatchClock
    {
        public int mins;
        public int secs;
    }

    public struct PlayerInfo
    {
        public int id;
        public GameObject gameObj;
        public string name;
        public string team;
        public Player.TypePlayer type;
        public float stamina;
        public float speed;        
    }

    public struct PlayerMatchStats
    {
        public int id;                
        public int passes;
        public int shoots;
        public int goals;
        //public int ownGoals;  // TO DO
    }

    // Player animator states    
    public static readonly int ps_idle = Animator.StringToHash("idle");
    public static readonly int ps_pass = Animator.StringToHash("pass");
    public static readonly int ps_shoot = Animator.StringToHash("shoot");
    public static readonly int ps_turn = Animator.StringToHash("turn");
    public static readonly int ps_corner_kick = Animator.StringToHash("corner_kick");
    public static readonly int ps_throw_side = Animator.StringToHash("throw_side");
    public static readonly int ps_running_ball = Animator.StringToHash("running_ball");
    public static readonly int ps_running = Animator.StringToHash("running");
    public static readonly int ps_tackle = Animator.StringToHash("tackle");
    public static Dictionary<int, string> playerStatesDic;

    // Goalkeeper animator states    
    public static readonly int gs_idle = Animator.StringToHash("idle");
    public static readonly int gs_run = Animator.StringToHash("run");
    public static readonly int gs_jump_left = Animator.StringToHash("jump_left");
    public static readonly int gs_jump_left_down = Animator.StringToHash("jump_left_down");
    public static readonly int gs_jump_right = Animator.StringToHash("jump_right");
    public static readonly int gs_jump_right_down = Animator.StringToHash("jump_right_down");
    public static readonly int gs_get_ball_front = Animator.StringToHash("get_ball_front");
    public static readonly int gs_throw_out = Animator.StringToHash("throw_out");
    public static readonly int gs_set_ball = Animator.StringToHash("set_ball");
    public static readonly int gs_corner_kick = Animator.StringToHash("corner_kick");        
    public static readonly int gs_runBlendTree = Animator.StringToHash("Blend Tree");

    public Dictionary<int, string> goalkeeperStatesDic;

    // Delegates and events
    public delegate void OnGoalDelegate();    
    public delegate void OnPassDelegate();
    public delegate void OnShootDelegate();
    public delegate void OnFirstHalfStartedDelegate();
    public delegate void OnSecondHalfStartedDelegate();
    public delegate void OnMatchFinishedDelegate();
    public delegate void OnThrowInDelegate();
    public delegate void OnCornerDelegate();
    public delegate void OnGoalKickDelegate();

    public static event OnGoalDelegate goalEvent;
    public static event OnPassDelegate passEvent;
    public static event OnShootDelegate shootEvent;
    public static event OnFirstHalfStartedDelegate firstHalfStartedEvent;
    public static event OnSecondHalfStartedDelegate secondHalfStartedEvent;
    public static event OnMatchFinishedDelegate matchFinishedEvent;
    public static event OnThrowInDelegate throwInEvent;
    public static event OnCornerDelegate cornerEvent;
    public static event OnGoalKickDelegate goalKickEvent;

    private InGame inGame;
    private ScorerTimeHUD scorerTimeHUD;
    private GameObject ball;
    private Ball ballScript;    
    private Dictionary<int, PlayerInfo> allPlayersInfoDic;
    private Dictionary<int, PlayerMatchStats> allPlayersStatsDic;
    private enum playerStatsType
    {
        Pass,
        Shoot,
        Goal
    }



    void Start()
    {
        inGame = GameObject.Find("GameManager").GetComponent<InGame>();
        scorerTimeHUD = GameObject.Find("Time").GetComponent<ScorerTimeHUD>();
        ball = GameObject.Find("soccer_ball");
        ballScript = ball.GetComponent<Ball>();        

        //Subscribe to different events in other scripts 
        SubscribeToEvents();

        // Initialize dictioanary of players states
        playerStatesDic = new Dictionary<int, string>();
        goalkeeperStatesDic = new Dictionary<int, string>();
        InitPlayersStates();

        // Initialize dictionaries of players info and players stats
        allPlayersInfoDic = new Dictionary<int, PlayerInfo>();
        allPlayersStatsDic = new Dictionary<int, PlayerMatchStats>();
        InitPlayersInfo();
    }


    private void OnDisable()
    {
        UnsubscribeToEvents();
    }


    private void SubscribeToEvents()
    {
        Goal.goalEvent += OnGoal;
        InGame.firstHalfStartedEvent += OnFirstHalfStarted;
        InGame.secondHalfStartedEvent += OnSecondHalfStarted;
        InGame.matchFinishedEvent += OnMatchFinished;
        Side.throwInEvent += OnThrowIn;
        //Corner.cornerEvent += OnCorner;
        InGame.cornerEvent += OnCorner;
        InGame.goalKickEvent += OnGoalKick;
        Player.passEvent += OnPass;
        Player.shootEvent += OnShoot;
    }


    private void UnsubscribeToEvents()
    {
        Goal.goalEvent -= OnGoal;
        InGame.firstHalfStartedEvent -= OnFirstHalfStarted;
        InGame.secondHalfStartedEvent -= OnSecondHalfStarted;
        InGame.matchFinishedEvent -= OnMatchFinished;
        Side.throwInEvent -= OnThrowIn;
        //Corner.cornerEvent -= OnCorner;
        InGame.cornerEvent -= OnCorner;
        InGame.goalKickEvent -= OnGoalKick;
        Player.passEvent -= OnPass;
        Player.shootEvent -= OnShoot;
    }


    private void InitPlayersStates()
    {
        playerStatesDic.Add(ps_idle, "idle");
        playerStatesDic.Add(ps_pass, "pass");
        playerStatesDic.Add(ps_shoot, "shoot");
        playerStatesDic.Add(ps_turn, "turn");
        playerStatesDic.Add(ps_corner_kick, "corner_kick");
        playerStatesDic.Add(ps_throw_side, "throw_side");
        playerStatesDic.Add(ps_running_ball, "running_ball");
        playerStatesDic.Add(ps_running, "running");
        playerStatesDic.Add(ps_tackle, "tackle");

        goalkeeperStatesDic.Add(gs_idle, "idle");
        goalkeeperStatesDic.Add(gs_run, "run");
        goalkeeperStatesDic.Add(gs_jump_left, "jump_left");
        goalkeeperStatesDic.Add(gs_jump_left_down, "jump_left_down");
        goalkeeperStatesDic.Add(gs_jump_right, "jump_right");
        goalkeeperStatesDic.Add(gs_jump_right_down, "jump_right_down");
        goalkeeperStatesDic.Add(gs_get_ball_front, "get_ball_front");
        goalkeeperStatesDic.Add(gs_throw_out, "throw_out");
        goalkeeperStatesDic.Add(gs_set_ball, "set_ball");
        goalkeeperStatesDic.Add(gs_corner_kick, "corner_kick");
        goalkeeperStatesDic.Add(gs_runBlendTree, "run Blend Tree");        
    }


    private void InitPlayersInfo()
    {
        int counter = 0;        
        int numPlayersT1 = inGame.team1.players.Count;
        int numPlayersT2 = inGame.team2.players.Count;
        
        foreach (Player player in inGame.team1.players)               
        {            
            player.uniqueId = counter;

            // Player info
            PlayerInfo playerInfo;
            playerInfo.id = counter;
            playerInfo.gameObj = player.gameObject;
            playerInfo.name = player.gameObject.name;
            playerInfo.team = player.gameObject.transform.parent.gameObject.name;
            playerInfo.type = player.type;
            playerInfo.speed = player.Speed;
            playerInfo.stamina = player.stamina;
            allPlayersInfoDic.Add(counter, playerInfo);

            // Player match stats
            PlayerMatchStats playerStats;
            playerStats.id = counter;                      
            playerStats.passes = 0;
            playerStats.shoots = 0;
            playerStats.goals = 0;
            allPlayersStatsDic.Add(counter, playerStats);

            counter++;
        }

        foreach (Player player in inGame.team2.players)        
        {            
            player.uniqueId = counter;
            PlayerInfo tempPlayerInfo;
            tempPlayerInfo.id = player.uniqueId;
            tempPlayerInfo.gameObj = player.gameObject;
            tempPlayerInfo.name = player.gameObject.name;
            tempPlayerInfo.team = player.gameObject.transform.parent.gameObject.name;
            tempPlayerInfo.type = player.type;
            tempPlayerInfo.speed = player.Speed;
            tempPlayerInfo.stamina = player.stamina;
            allPlayersInfoDic.Add(counter, tempPlayerInfo);

            // Player match stats
            PlayerMatchStats playerStats;
            playerStats.id = counter;
            playerStats.passes = 0;
            playerStats.shoots = 0;
            playerStats.goals = 0;
            allPlayersStatsDic.Add(counter, playerStats);

            counter++;
        }
    }


    private void AddToPlayerStats(playerStatsType type)
    {
        GameObject aPlayer = GetLastPlayerWithBall();
        
        if (!aPlayer)
            aPlayer = GetBallOwner();
        
        if (aPlayer != null)
        {
            int id = GetPlayerUniqueId(aPlayer);
            if (id > -1)
            {
                if (allPlayersStatsDic.ContainsKey(id))
                {
                    PlayerMatchStats playerStats = allPlayersStatsDic[id];
                    if (type == playerStatsType.Goal)
                        playerStats.goals += 1;
                    else if (type == playerStatsType.Pass)
                        playerStats.passes += 1;
                    else if (type == playerStatsType.Shoot)
                        playerStats.shoots += 1;

                    allPlayersStatsDic[id] = playerStats;
                }
                else
                {
                    Debug.LogError("allPlayersStatatsDic doesn't contain id: " + id);
                }        
            }
        }
        else
        {
            Debug.Log("Last player with ball returned null...");    // Should only occur at start of the match
        }
    }


    #region Accessing Game Data
    // Returns the state of the game
    public InGame.InGameState GetGameState()
    {
        return inGame.state;
    }


    // Returns the elapsed time of the soccer match in seconds (increases as time passes)
    public float GetMatchElapsedTime()
    {
        return scorerTimeHUD.timeMatch;
    }


    // Returns a structure with the time of the match (minutes and seconds)
    public MatchClock GetMatchClock()
    {
        MatchClock matchClock = new MatchClock();
        matchClock.mins = scorerTimeHUD.minutes;
        matchClock.secs = scorerTimeHUD.seconds;
        return matchClock;
    }


    // Returns the number of goals by the visiting team (team 2)
    public int GetScoreVisitingTeam()
    {
        return inGame.scoreVisiting;
    }


    // Returns the number of goals by the local team (team 1)
    public int GetScoreLocalTeam()
    {
        return inGame.scoreLocal;
    }


    // Returns a number that represents the half of the match (0 = match not started; 1 = first half; 2 = second half)
    public int GetMatchHalf()
    {
        return inGame.firstHalf;
    }


    // Returns the ball transform (position, rotation and game object can be obtained from the transform)
    public Transform GetBallTransform()
    {
        return ball.transform;
    }


    // Returns game object of player who last touched the ball
    public GameObject GetLastPlayerWithBall()
    {
        GameObject go = null;

        if (inGame.lastTouched)
            go = inGame.lastTouched.gameObject;

        return go;
    }


    // Returns game object of player that currently owns the ball or null if nobody owns the ball
    public GameObject GetBallOwner()
    {
        GameObject go = null;

        if (Ball.owner)
            go = Ball.owner.gameObject;

        return go;
    }
    #endregion


    #region Game Event Callbacks
    private void OnGoal()
    {
        // Add goal to player stats   
        AddToPlayerStats(playerStatsType.Goal);

        // Notify subscribers that a goal has been scored
        goalEvent?.Invoke();
    }    


    private void OnFirstHalfStarted()
    {
        firstHalfStartedEvent?.Invoke();
    }


    private void OnSecondHalfStarted()
    {
        secondHalfStartedEvent?.Invoke();
    }


    private void OnMatchFinished()
    {
        matchFinishedEvent?.Invoke();
    }


    private void OnCorner()
    {
        cornerEvent?.Invoke();
    }


    private void OnGoalKick()
    {
        goalKickEvent?.Invoke();
    }


    private void OnThrowIn()
    {
        throwInEvent?.Invoke();
    }


    private void OnPass()
    {
        // Add pass to player stats   
        AddToPlayerStats(playerStatsType.Pass);

        // Notify subscribers
        passEvent?.Invoke();
    }


    private void OnShoot()
    {
        // Add shoot to player stats   
        AddToPlayerStats(playerStatsType.Shoot);

        // Notify subscribers
        shootEvent?.Invoke();
    }
    #endregion


    #region Accessing Player Data
    // Returns player id
    public int GetPlayerUniqueId(GameObject go)
    {
        int id = -1;
        Player player = go.GetComponent<Player>(); 

        if (player != null)        
            id = player.uniqueId;        
        else        
            Debug.LogError("Game Object is not a player...");
        
        return id;
    }


    // Returns player id
    public int GetPlayerUniqueId(string name)
    {
        int id = -1;
        GameObject go = GameObject.Find(name);
        
        if (go != null)
            id = GetPlayerUniqueId(go);
        else
            Debug.LogError("Game object with name " + name + " was not found...");
                
        return id;
    }


    // Returns a struct with player info (id, gameobject, name, team, type, stamina, speed)
    public PlayerInfo GetPlayerInfo(int playerId)
    {
        PlayerInfo playerInfo = new PlayerInfo();

        if (allPlayersInfoDic.ContainsKey(playerId))
            playerInfo = allPlayersInfoDic[playerId];
        else
            Debug.LogError("allPlayersInfoDic does not contain id " + playerId);

        return playerInfo;
    }


    // Returns a struct with player info (id, gameobject, name, team, type, stamina, speed)
    public PlayerInfo GetPlayerInfo(GameObject goPlayer)
    {
        int id = GetPlayerUniqueId(goPlayer);
        return allPlayersInfoDic[id];
    }


    // Returns the player's transform
    public Transform GetPlayerTransform(int playerId)
    {
        Transform t = null;

        if (allPlayersInfoDic.ContainsKey(playerId))
            t = allPlayersInfoDic[playerId].gameObj.transform;
        else
            Debug.LogError("allPlayersInfoDic does not contains key: " + playerId);

        return t;
    }


    // Returns the player's state name (based on the animator controller states)
    public string GetPlayerState(int playerId)
    {
        string state = "State not found";
        Animator anim = allPlayersInfoDic[playerId].gameObj.GetComponent<Animator>();
        int hash = anim.GetCurrentAnimatorStateInfo(0).shortNameHash;
        Player.TypePlayer playerType = allPlayersInfoDic[playerId].type;

        if (playerType == Player.TypePlayer.GOALKEEPER)
        {
            if (goalkeeperStatesDic.ContainsKey(hash))
                state = goalkeeperStatesDic[hash];   
        }
        else
        {            
            if (playerStatesDic.ContainsKey(hash))
                state = playerStatesDic[hash];
        }
        
        return state;
    }


    // Returns the dictionary that contains the PlayerInfo struct for all players
    public Dictionary<int, PlayerInfo> GetAllPlayersInfoDic()
    {        
        return allPlayersInfoDic;
    }


    // Returns a list of PlayerInfo structs for all players
    public List<PlayerInfo> GetAllPlayersInfoList()
    {
        List<PlayerInfo> allPlayersInfoList = new List<PlayerInfo>(allPlayersInfoDic.Values);
        return allPlayersInfoList;
    }
    #endregion


    #region Accessing player match performance stats
    // Returns a struct with the player's statistics during the match
    public PlayerMatchStats GetPlayerStats(int id)
    {
        PlayerMatchStats playerStats = new PlayerMatchStats();

        if (allPlayersStatsDic.ContainsKey(id))
            playerStats = allPlayersStatsDic[id];
        else
            Debug.LogError("allPlayersStatsDic doesn't contain key: " + id);

        return playerStats;
    }


    // Returns a list of PlayerMatchStats structs for all players
    public List<PlayerMatchStats> GetAllPlayersStatsList()
    {
        List<PlayerMatchStats> allPlayersStatsList = new List<PlayerMatchStats>(allPlayersStatsDic.Values);
        return allPlayersStatsList;
    }
    #endregion
}
