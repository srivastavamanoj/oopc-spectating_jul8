using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class InGame : MonoBehaviour
{
    #region Public Fields
    [Header("Ball")]
    public Ball ball;

    [Header("Camera")]
    public Camera cameraGame;

    [Header("Field")]
    public Transform center;
    public Transform cornerSource;
    public GameObject cornerTrigger;
    public Transform goal_kick;
    public Vector3 target_throw_in;

    [Header("Input Manager")]
    public InputManager inputManager;

    [Header("Other")]
    public Player lastTouched;

    [Header("Kick-Off players")]
    public Player passed;
    public Player passed_oponent;
    public Player passer;
    public Player passer_oponent;
    public Vector3 positionSide;

    [field: Header("Scorer")]
    public ScorerTimeHUD scorerTime;
    public int scoreVisiting = 0;
    public int scoreLocal = 0;

    [Header("Game State")]
    public InGameState state;

    [Header("Teams")]
    public Team team1;
    public Team team2;
    public float timeToChangeState = 0.0f;
    #endregion Public Fields

    #region Private Fields
    private static readonly int CornerKick = Animator.StringToHash("Corner_Kick");
    private static readonly int Idle = Animator.StringToHash("idle");
    private static readonly int ThowOutFoot = Animator.StringToHash("thow_out_foot");
    private static readonly int ThrowSide = Animator.StringToHash("ThrowSide");
    private float timeToKickOff = Globals.perdiodToKickOff;
    private float timeToThrowCPU = Globals.perdiodToThrowCPU;
    private Player whoLastTouched;
    #endregion Private Fields

    #region Public Enums
    public enum InGameState
    {
        PLAYING,
        PREPARE_TO_KICK_OFF,
        KICK_OFF,
        GOAL,
        THROW_IN,
        THROW_IN_CHASING,
        THROW_IN_DONE,
        CORNER,
        CORNER_CHASING,
        CORNER_DONE,
        GOAL_KICK,
    };
    #endregion Public Enums

    #region Public Properties
    public Player candidateToThrowIn { get; set; }
    public int firstHalf { get; set; } = 0;
    public GoalKeeper goalKeeperToAct { get; set; }
    public bool scoredbylocal { get; set; } = false;
    public bool scoredbyvisiting { get; set; } = true;
    #endregion Public Properties

    #region Delegates and events    
    public delegate void OnFirstHalfStartedDelegate();
    public delegate void OnSecondHalfStartedDelegate();
    public delegate void OnMatchFinishedDelegate();
    public delegate void OnCornerDelegate();
    public delegate void OnGoalKickDelegate();
    public static event OnFirstHalfStartedDelegate firstHalfStartedEvent;
    public static event OnSecondHalfStartedDelegate secondHalfStartedEvent;
    public static event OnMatchFinishedDelegate matchFinishedEvent;
    public static event OnCornerDelegate cornerEvent;
    public static event OnGoalKickDelegate goalKickEvent;
    #endregion


    #region Private Methods
    private void PutPlayersInCornerArea(List<Player> arrayPlayers, Player.TypePlayer type)
    {
        foreach (Player player in arrayPlayers)
        {
            var pl = player;
            if (pl.type == type)
            {
                var areaCorner = this.cornerTrigger.GetComponent<Corner>().correspondingArea;
                var boxCollider = areaCorner.GetComponent<BoxCollider>();
                var bounds = boxCollider.bounds;
                float xmin = bounds.min.x;
                float xmax = bounds.max.x;
                float zmin = bounds.min.z;
                float zmax = bounds.max.z;

                float x = Random.Range(xmin, xmax);
                float z = Random.Range(zmin, zmax);

                player.transform.position = new Vector3(x, player.transform.position.y, z);
            }
        }
    }

    private Player SearchPlayerNearBall(List<Player> arrayPlayers)
    {
        Player candidatePlayer = null;
        float distance = 1000.0f;
        foreach (Player player in arrayPlayers)
        {
            if (!player.temporallyUnselectable)
            {
                Vector3 relativePos = ball.transform.InverseTransformPoint(player.transform.position);
                float newdistance = relativePos.magnitude;

                if (newdistance < distance)
                {
                    distance = newdistance;
                    candidatePlayer = player;
                }
            }
        }

        return candidatePlayer;
    }

    // Use this for initialization
    private void Start()
    {
        state = InGameState.PREPARE_TO_KICK_OFF;
        firstHalf = 0;
    }

    // Update is called once per frame
    private void Update()
    {        
        // little time between states
        timeToChangeState -= Time.deltaTime;

        if (timeToChangeState < 0.0f)
        {
            // Handle all states related to match
            switch (state)
            {
                case InGameState.PLAYING:
                    
                    if (scorerTime.minutes < 44.0f && firstHalf == 0)
                    {
                        firstHalf = 1;

                        // Notify subscribers first half started
                        firstHalfStartedEvent?.Invoke();
                    }

                    if (scorerTime.minutes > 45.0f && firstHalf == 1)
                    {
                        ball.transform.position = center.position;

                        foreach (Player player in team1.players)
                        {
                            player.transform.position = player.resetPosition;
                            player.animator.SetTrigger(Player.Idle);
                        }

                        foreach (Player player in team2.players)
                        {
                            player.transform.position = player.resetPosition;
                            player.animator.SetTrigger(Player.Idle);
                        }

                        firstHalf = 2;

                        scoredbylocal = true;
                        scoredbyvisiting = false;
                        state = InGameState.PREPARE_TO_KICK_OFF;

                        // Notify subscribers second half started
                        secondHalfStartedEvent?.Invoke();
                    }

                    if (scorerTime.minutes > 90.0f && firstHalf == 2)
                    {
                        // Notify subscribers match finished
                        matchFinishedEvent?.Invoke();

                        PlayerPrefs.SetInt("ScoreLocal", scoreLocal);
                        PlayerPrefs.SetInt("ScoreVisit", scoreVisiting);

                        // Restart game
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);                        
                    }

                    break;

                case InGameState.THROW_IN:

                    whoLastTouched = lastTouched;

                    foreach (Player go in team1.players)
                    {
                        go.animator.Play("idle");
                    }
                    foreach (Player go in team2.players)
                    {
                        go.animator.Play("idle");
                    }

                    Ball.owner = null;
                    candidateToThrowIn = SearchPlayerNearBall(whoLastTouched.team.otherTeam.players);
                    candidateToThrowIn.transform.position = new Vector3(positionSide.x, candidateToThrowIn.transform.position.y, positionSide.z);

                    if (whoLastTouched.team.IsHuman)
                    {
                        candidateToThrowIn.temporallyUnselectable = true;
                        candidateToThrowIn.timeToBeSelectable = Globals.periodToBeSelectable;
                        candidateToThrowIn.transform.LookAt(SearchPlayerNearBall(whoLastTouched.team.otherTeam.players).transform.position);
                    }
                    else
                        candidateToThrowIn.transform.LookAt(center);

                    candidateToThrowIn.transform.Rotate(0, inputManager.fHorizontal * 10.0f, 0);
                    candidateToThrowIn.animator.SetTrigger(ThrowSide);
                    ball.RB.isKinematic = true;
                    target_throw_in = candidateToThrowIn.transform.position + candidateToThrowIn.transform.forward;

                    state = InGameState.THROW_IN_CHASING;

                    break;

                case InGameState.THROW_IN_CHASING:
                    var trans = candidateToThrowIn.transform;
                    trans.position = new Vector3(positionSide.x, trans.position.y, positionSide.z);
                    candidateToThrowIn.transform.LookAt(target_throw_in);

                    //if (!whoLastTouched.team.IsHuman)
                    if (whoLastTouched.team.IsHuman)
                    {
                        target_throw_in += new Vector3(0, 0, inputManager.fHorizontal / 10.0f);

                        if (inputManager.bPassButton)
                        {
                            candidateToThrowIn.animator.speed = 1f;
                            state = InGameState.THROW_IN_DONE;
                        }
                    }
                    else
                    {
                        timeToThrowCPU -= Time.deltaTime;

                        if (timeToThrowCPU < 0.0f)
                        {
                            timeToThrowCPU = Globals.perdiodToThrowCPU;
                            state = InGameState.THROW_IN_DONE;
                        }
                    }

                    break;

                case InGameState.THROW_IN_DONE:
                    candidateToThrowIn.moveAutomatic = true;
                    state = InGameState.PLAYING;

                    break;

                case InGameState.CORNER:

                    whoLastTouched = lastTouched;
                    if (!cornerTrigger.CompareTag(whoLastTouched.tag))
                    {
                        // it is not corner-kick, it is goal-kick
                        state = InGameState.GOAL_KICK;
                        
                        // Notify subscribers there is a goal kick                        
                        goalKickEvent?.Invoke();

                        break;
                    }                    

                    foreach (Player go in team1.players)
                    {
                        go.animator.Play("idle");
                    }
                    foreach (Player go in team2.players)
                    {
                        go.animator.Play("idle");
                    }

                    Ball.owner = null;
                    PutPlayersInCornerArea(whoLastTouched.team.players, Player.TypePlayer.DEFENDER);
                    PutPlayersInCornerArea(whoLastTouched.team.players, Player.TypePlayer.MIDDLER);
                    PutPlayersInCornerArea(whoLastTouched.team.otherTeam.players, Player.TypePlayer.ATTACKER);
                    PutPlayersInCornerArea(whoLastTouched.team.otherTeam.players, Player.TypePlayer.MIDDLER);
                    candidateToThrowIn = SearchPlayerNearBall(whoLastTouched.team.otherTeam.players);

                    // Notify subscribers there is a corner                        
                    cornerEvent?.Invoke();

                    candidateToThrowIn.transform.position = new Vector3(cornerSource.position.x, candidateToThrowIn.transform.position.y, cornerSource.position.z);
                    candidateToThrowIn.transform.rotation = cornerSource.rotation;
                    candidateToThrowIn.transform.position -= candidateToThrowIn.transform.forward * 3f;

                    if (whoLastTouched.team.IsHuman)
                    {
                        whoLastTouched.temporallyUnselectable = true;
                        whoLastTouched.timeToBeSelectable = Globals.periodToBeSelectable;

                        candidateToThrowIn.transform.LookAt(SearchPlayerNearBall(whoLastTouched.team.otherTeam.players).transform.position);
                    }
                    else
                        candidateToThrowIn.transform.LookAt(center);

                    candidateToThrowIn.transform.Rotate(0, inputManager.fHorizontal * 10.0f, 0);
                    candidateToThrowIn.animator.SetTrigger(CornerKick);

                    ball.RB.isKinematic = true;
                    ball.transform.position = cornerSource.position;
                    target_throw_in = candidateToThrowIn.transform.position + candidateToThrowIn.transform.forward;
                    state = InGameState.CORNER_CHASING;

                    break;

                case InGameState.CORNER_CHASING:

                    candidateToThrowIn.transform.LookAt(ball.transform.position);
                    
                    if (whoLastTouched.team.IsHuman)
                    {
                        if (inputManager.bPassButton)
                        {
                            candidateToThrowIn.animator.speed = 1f;
                            state = InGameState.CORNER_DONE;
                        }
                    }
                    else
                    {
                        timeToThrowCPU -= Time.deltaTime;

                        if (timeToThrowCPU < 0.0f)
                        {
                            timeToThrowCPU = Globals.perdiodToThrowCPU;
                            //ball.RB.isKinematic = true;
                            state = InGameState.CORNER_DONE;
                        }
                    }

                    break;

                case InGameState.CORNER_DONE:

                    whoLastTouched.moveAutomatic = true;
                    state = InGameState.PLAYING;

                    break;

                case InGameState.GOAL_KICK:

                    ball.transform.position = goal_kick.position;
                    ball.RB.isKinematic = true;
                    goalKeeperToAct.transform.rotation = goal_kick.transform.rotation;
                    goalKeeperToAct.transform.position = new Vector3(goal_kick.position.x, goalKeeperToAct.transform.position.y, goal_kick.position.z) - (goalKeeperToAct.transform.forward * 1.0f);
                    goalKeeperToAct.animator.SetTrigger(ThowOutFoot);

                    foreach (Player go in team1.players)
                    {                        
                        go.animator.Play("idle");
                    }
                    foreach (Player go in team2.players)
                    {                        
                        go.animator.Play("idle");
                    }

                    Ball.owner = null;
                    state = InGameState.PLAYING;

                    break;

                case InGameState.GOAL:

                    foreach (Player go in team1.players)
                    {
                        go.animator.Play("idle");
                    }
                    foreach (Player go in team2.players)
                    {
                        go.animator.Play("idle");
                    }                    

                    timeToKickOff -= Time.deltaTime;

                    if (timeToKickOff < 0.0f)
                    {
                        timeToKickOff = 4.0f;
                        state = InGame.InGameState.PREPARE_TO_KICK_OFF;
                    }

                    break;

                case InGameState.KICK_OFF:

                    foreach (Player go in team1.players)
                    {
                        go.moveAutomatic = true;
                        go.transform.position = go.initialPosition;
                    }
                    foreach (Player go in team2.players)
                    {
                        go.moveAutomatic = true;
                        go.transform.position = go.initialPosition;
                    }
                                        
                    team1.players[0].animator.Play("idle");
                    team2.players[0].animator.Play("idle");

                    Ball.owner = null;
                    ball.gameObject.transform.position = center.position;
                    ball.RB.drag = 0.5f;
                    state = InGameState.PLAYING;

                    break;

                case InGameState.PREPARE_TO_KICK_OFF:

                    ball.transform.position = center.position;

                    foreach (Player go in team1.players)
                    {
                        go.transform.LookAt(ball.transform);
                    }
                    foreach (Player go in team2.players)
                    {
                        go.transform.LookAt(ball.transform);
                    }

                    if (scoredbyvisiting)
                    {
                        var ballPos = new Vector3( ball.transform.position.x, passer.transform.position.y, ball.transform.position.z) ;
                        passer.transform.position =  ballPos + new Vector3(0.0f, 0, 1.0f);
                        passer.transform.LookAt(ballPos);
                        passed.transform.position = passer.transform.position + (passer.transform.forward * 5.0f);
                        Player.kickOffer = passer;
                        Ball.owner = passer;                        
                    }

                    if (scoredbylocal)
                    {
                        var ballPos = new Vector3( ball.transform.position.x, passer_oponent.transform.position.y, ball.transform.position.z) ;
                        passer_oponent.transform.position = ballPos + new Vector3(0.0f, 0, -1.0f);
                        passer_oponent.transform.LookAt(ballPos);
                        passed_oponent.transform.position = passer_oponent.transform.position + (passer_oponent.transform.forward * 5.0f);
                        Player.kickOffer = passer_oponent;
                        Ball.owner = passer_oponent;
                    }

                    scoredbylocal = false;
                    scoredbyvisiting = false;

                    break;
            }
        }
    }
    #endregion Private Methods
}