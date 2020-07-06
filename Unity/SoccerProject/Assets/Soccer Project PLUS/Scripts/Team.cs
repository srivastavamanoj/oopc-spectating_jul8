using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    #region Public Fields

    [Header("Only Necessary if team is CPU")]
    public Transform goalTarget;

    public InGame inGame;
    public bool IsHuman = true;
    public Team otherTeam;

    [HideInInspector]
    public List<Player> players = new List<Player>();

    #endregion Public Fields

    #region Public Properties

    public Player inputPlayer { get; private set; }
    public float timeToSelectAgain { get; set; }

    #endregion Public Properties

    #region Private Methods

    // activate nearest player to ball
    private void ActivateNearestPlayer()
    {
        var ball = players[0].ball;

        float distance = 1000000.0f;
        Player candidatePlayer = null;
        foreach (Player player in players)
        {
            var pl = player;
            if (!pl.temporallyUnselectable)
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

        timeToSelectAgain += Time.deltaTime;
        if (timeToSelectAgain > Globals.periodSelectAgain)
        {
            inputPlayer = candidatePlayer;
            timeToSelectAgain = 0.0f;
        }
        else
        {
            candidatePlayer = Ball.lastCandidatePlayer;
        }

        Ball.lastCandidatePlayer = candidatePlayer;
    }
    
    private void Awake()
    {
        var childsNumber = transform.childCount;
        for (int i = 0; i < childsNumber; i++)
        {
            players.Add(transform.GetChild(i).GetComponent<Player>());
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (inGame.state == InGame.InGameState.PLAYING)
        {
            ActivateNearestPlayer();
        }
    }

    #endregion Private Methods
}