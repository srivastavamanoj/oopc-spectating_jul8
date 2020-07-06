using UnityEngine;

public class CornerKickGK : StateMachineBehaviour
{
    #region Private Fields

    private Ball ball;
    private InGame inGame;
    private GoalKeeper player;

    #endregion Private Fields

    #region Public Methods

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null)
        {
            player = animator.GetComponent<GoalKeeper>();
            inGame = player.inGame;
            ball = player.ball;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var trans = player.transform;

        switch (player.dirThrowOutFoot)
        {
            case 0:
                break;

            case 1:
                trans.position = Vector3.MoveTowards(trans.position, ball.transform.position, 5f * Time.deltaTime);
                break;

            case -1:
                trans.position -= trans.forward * 1.5f * Time.deltaTime;
                break;

            case 2:

                ball.RB.isKinematic = false;
                float force = Random.Range(Globals.forceReleaseBall, Globals.forceReleaseBall*2f);
                ball.RB.AddForce((inGame.goalKeeperToAct.transform.forward * force) + new Vector3(0, 3000.0f, 0));

                player.dirThrowOutFoot = 0;

                break;
        }
    }

    #endregion Public Methods

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}