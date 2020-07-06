using UnityEngine;

public class Idle : StateMachineBehaviour
{
    #region Private Fields

    private InGame inGame;
    private InputManager inputManager;
    private Player player;
    private Team team;

    #endregion Private Fields

    #region Public Methods

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null)
        {
            player = animator.GetComponent<Player>();
            inGame = player.inGame;
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
        if (inGame.state == InGame.InGameState.PREPARE_TO_KICK_OFF)
        {
            if (Player.kickOffer == player)
            {
                if (inputManager.bPassButton || !player.team.IsHuman)
                {
                    Player.kickOffer = null;
                    animator.SetTrigger(Player.Pass);
                    player.timeToBeSelectable = Globals.periodToBeSelectable;
                    inGame.state = InGame.InGameState.PLAYING;
                }
            }
        }
        else if (inGame.state == InGame.InGameState.PLAYING)
        {
            if (team.inputPlayer == player)
            {
                player.moveAutomatic = false;
            }
            else
            {
                player.moveAutomatic = true;
            }

            animator.SetTrigger(Player.Run);
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