using UnityEngine;

public class IdleGK : StateMachineBehaviour
{
    #region Private Fields

    private static readonly int Run = Animator.StringToHash("run");
    private Ball ball;
    private CapsuleCollider capsuleCollider;
    private InGame inGame;
    private InputManager inputManager;
    private GoalKeeper player;
    private Team team;

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
            inputManager = player.inputManager;
            team = player.team;
            capsuleCollider = player.capsuleCollider;
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

        float distanceBall = (trans.position - ball.transform.position).magnitude;

        if (distanceBall < Globals.distanceToBackOrigin)
        {
            if (player.stoleBall == false)
            {
                player.stoleBall = true;
                animator.SetBool(Run, true);
            }
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