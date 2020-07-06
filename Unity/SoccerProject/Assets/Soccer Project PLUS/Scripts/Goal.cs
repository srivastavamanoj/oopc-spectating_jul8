using UnityEngine;

public class Goal : MonoBehaviour
{
    #region Public Fields

    public GoalKeeper goalKeeper;
    public InGame inGame;

    public delegate void OnGoalDelegate();
    public static event OnGoalDelegate goalEvent;

    #endregion Public Fields

    #region Private Methods

    private void OnTriggerEnter(Collider other)
    {
        // if ball is inside then is GOAL
        if (other.gameObject.CompareTag("Ball"))
        {
            Ball.owner = null;
            goalKeeper.animator.SetTrigger(GoalKeeper.IdleGK);

            // add score depending of goal side
            if (goalKeeper.team == inGame.team2 && inGame.state != InGame.InGameState.GOAL)
            {
                inGame.scoreLocal++;
                inGame.scoredbylocal = true;
                inGame.scoredbyvisiting = false;

                // Notify subscribers that a goal was scored
                goalEvent?.Invoke();
            }

            if (goalKeeper.team == inGame.team1 && inGame.state != InGame.InGameState.GOAL)
            {
                inGame.scoreVisiting++;
                inGame.scoredbylocal = false;
                inGame.scoredbyvisiting = true;

                // Notify subscribers that a goal was scored
                goalEvent?.Invoke();
            }

            inGame.timeToChangeState = Globals.periodToChangeState;
            inGame.state = InGame.InGameState.GOAL;            
        }
    }

    // Use this for initialization

    #endregion Private Methods
}