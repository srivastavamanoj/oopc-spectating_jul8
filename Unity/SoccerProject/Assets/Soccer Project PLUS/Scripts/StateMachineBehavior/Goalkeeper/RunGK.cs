using UnityEngine;

public class RunGK : StateMachineBehaviour
{
    #region Private Fields

    private static readonly int DirStrafeX = Animator.StringToHash("dir_strafe_x");
    private static readonly int DirStrafeZ = Animator.StringToHash("dir_strafe_z");    
    private static readonly int Idle = Animator.StringToHash("Idle");
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

        if (player.go_origin)
        {
            trans.position = Vector3.MoveTowards(trans.position, player.initialPosition, Time.deltaTime * 2.5f);
            var relPosBall = trans.InverseTransformPoint(ball.transform.position).normalized;
            animator.SetFloat(DirStrafeX, relPosBall.x);
            animator.SetFloat(DirStrafeZ, relPosBall.z);

            if (Vector3.Distance(trans.position, player.initialPosition) < 1f)
            {
                player.go_origin = false;
                animator.SetBool("run", false);
                animator.SetTrigger(Idle);                
            }

            return;
        }

        if (player.stoleBall)
        {
            trans.position = Vector3.MoveTowards(trans.position, ball.transform.position, Time.deltaTime * 5f);
            var relPosBall = trans.InverseTransformPoint(ball.transform.position).normalized;
            animator.SetFloat(DirStrafeX, relPosBall.x);
            animator.SetFloat(DirStrafeZ, relPosBall.z);

            // if gk is too far of initialposition
            var distInit = (player.initialPosition - player.transform.position).magnitude;
            if (distInit > Globals.distanceToBackOrigin)
            {
                player.go_origin = true;                
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