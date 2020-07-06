using System;
using UnityEngine;

public class JumpGK : StateMachineBehaviour
{
    #region Public Fields

    public DirectionJump direction;

    #endregion Public Fields

    #region Private Fields

    private Ball ball;

    private InGame inGame;

    private InputManager inputManager;

    private GoalKeeper player;

    private Team team;

    #endregion Private Fields

    #region Public Enums

    public enum DirectionJump
    {
        JUMP_LEFT,
        JUMP_RIGHT,
        JUMP_LEFT_DOWN,
        JUMP_RIGHT_DOWN
    }

    #endregion Public Enums

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
        var norm = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        switch (direction)
        {
            case DirectionJump.JUMP_LEFT:

                if (norm < 0.45f)
                {
                    trans.position -= Globals.jumpSpeedUp * Time.deltaTime * trans.right;
                }

                break;

            case DirectionJump.JUMP_RIGHT:

                if (norm < 0.45f)
                {
                    trans.position += Globals.jumpSpeedUp * Time.deltaTime * trans.right;
                }

                break;

            case DirectionJump.JUMP_LEFT_DOWN:

                trans.position -= Globals.jumpSpeedDown * Time.deltaTime * trans.right;

                break;

            case DirectionJump.JUMP_RIGHT_DOWN:

                trans.position += Globals.jumpSpeedDown * Time.deltaTime * trans.right;

                break;

            default:
                throw new ArgumentOutOfRangeException();
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