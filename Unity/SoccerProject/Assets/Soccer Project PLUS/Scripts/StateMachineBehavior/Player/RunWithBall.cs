using UnityEngine;

public class RunWithBall : StateMachineBehaviour
{
    #region Private Fields

    private Ball ball;
    private InGame inGame;
    private InputManager inputManager;
    private Player player;

    #endregion Private Fields

    #region Public Methods

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null)
        {
            player = animator.GetComponent<Player>();
            inGame = player.inGame;
            ball = player.ball;
            inputManager = player.inputManager;
        }

        player.moveAutomatic = false;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Implement code that sets up animation IK (inverse kinematics)
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Implement code that processes and affects root motion
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Ball.owner != player)
        {
            player.moveAutomatic = true;
            animator.SetTrigger(Player.Run);
        }

        Transform trans = player.transform;

        if (player.team.IsHuman)
        {
            if (inputManager.fVertical != 0.0f || inputManager.fHorizontal != 0.0f)
            {
                player.oldVelocityPlayer = player.actualVelocityPlayer;
                var transform = inGame.transform;
                Vector3 right = transform.right;
                Vector3 forward = transform.forward;
                right *= inputManager.fHorizontal;
                forward *= inputManager.fVertical;
                var position = trans.position;
                Vector3 target = position + right + forward;
                target.y = position.y;
                float speedForAnimation = 5.0f;
                trans.LookAt(target);
                float staminaTemp = Mathf.Clamp((player.stamina / Globals.STAMINA_DIVIDER), Globals.STAMINA_MIN, Globals.STAMINA_MAX);
                player.actualVelocityPlayer = player.Speed * speedForAnimation * Time.deltaTime * staminaTemp * trans.forward;
                position += player.actualVelocityPlayer;
                trans.position = position;
            }
            else
            {
                animator.SetTrigger(Player.Idle);
            }

            // pass
            if (inputManager.bPassButton && Ball.owner == player)
            {
                animator.SetTrigger(Player.Pass);
                player.timeToBeSelectable = Globals.periodToBeSelectable;
            }

            // shoot
            if (inputManager.bShootButtonFinished && Ball.owner == player)
            {
                animator.SetTrigger(Player.Shoot);
                player.timeToBeSelectable = Globals.periodToBeSelectable;
                inputManager.bShootButtonFinished = false;
            }

            if (Ball.owner != player)
            {
                animator.SetTrigger(Player.Idle);
                player.moveAutomatic = true;
            }
        }
        else
        {
            player.actualVelocityPlayer = Time.deltaTime * 5.0f * trans.forward;
            Vector3 relativeWaypointPosition = trans.InverseTransformPoint(player.team.goalTarget.position);
            player.rotationSpeed = relativeWaypointPosition.x / relativeWaypointPosition.magnitude;
            trans.Rotate(0, player.rotationSpeed * 10.0f, 0);
            float staminaTemp = Mathf.Clamp((player.stamina / Globals.STAMINA_DIVIDER), Globals.STAMINA_MIN, Globals.STAMINA_MAX);
            trans.position += player.Speed * (Globals.speedPlayer-1f) * Time.deltaTime * staminaTemp * trans.forward;

            player.timeToPass -= Time.deltaTime;

            if (player.timeToPass < 0.0f && player.NoOneInFront(player.team.otherTeam.players))
            {
                player.timeToPass = UnityEngine.Random.Range(1.0f, 5.0f);
                animator.SetTrigger(Player.Pass);
                player.timeToBeSelectable = Globals.periodToBeSelectable;
                player.temporallyUnselectable = true;
            }

            float distance = (player.team.goalTarget.position - trans.position).magnitude;
            Vector3 relative = trans.InverseTransformPoint(player.team.goalTarget.position);

            if (distance < Globals.distanceToShoot && relative.z > 0)
            {
                animator.SetTrigger(Player.Shoot);
                player.timeToBeSelectable = Globals.periodToBeSelectable;
                player.temporallyUnselectable = true;
            }
        }
    }

    #endregion Public Methods
}