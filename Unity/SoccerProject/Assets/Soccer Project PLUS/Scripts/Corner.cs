using UnityEngine;

public class Corner : MonoBehaviour
{

    #region Public Fields

    public Ball ball;

    [Header("Others")]
    public Transform correspondingArea;
    public Transform downPosition;
    public GoalKeeper goalKeeper;
    public InGame inGame;
    public Transform point_goalkick;

    [Header("Positions")]
    public Transform upPosition;

    #endregion Public Fields

    #region Delegates and Events
    public delegate void OnCornerDelegate();
    public static event OnCornerDelegate cornerEvent;

    #endregion

    #region Private Methods

    private void OnTriggerEnter(Collider other)
    {
        if (inGame.state != InGame.InGameState.GOAL)
        {
            
            // Detect if Players are outside of field
            if (!other.gameObject.CompareTag("Ball") && inGame.state == InGame.InGameState.PLAYING)
            {
                if ( Ball.owner && other.gameObject != Ball.owner.gameObject)
                {
                    Player player = other.gameObject.GetComponent<Player>();
                    player.temporallyUnselectable = true;
                    player.timeToBeSelectable = Globals.periodToBeSelectable;
                    player.go_origin = true;
                }
            }

            // Chekc if is corner-kick or goal-kick
            if (other.gameObject.CompareTag("Ball") && inGame.state == InGame.InGameState.PLAYING)
            {
                Ball.owner = null;
                inGame.timeToChangeState = Globals.periodToChangeState;
                inGame.goal_kick = point_goalkick;
                inGame.goalKeeperToAct = goalKeeper;
                inGame.cornerTrigger = this.gameObject;

                // looking for the near corner point
                Vector3 positionBall = ball.gameObject.transform.position;
                if ((positionBall - downPosition.position).magnitude > (positionBall - upPosition.position).magnitude)
                {
                    inGame.cornerSource = upPosition;
                }
                else
                {
                    inGame.cornerSource = downPosition;
                }

                inGame.state = InGame.InGameState.CORNER;

                // Notify subscribers of corner event
                cornerEvent?.Invoke();
            }
        }
    }

    // Use this for initialization
    private void Start()
    {
        ball = (Ball)GameObject.FindObjectOfType(typeof(Ball));
    }

    #endregion Private Methods
}