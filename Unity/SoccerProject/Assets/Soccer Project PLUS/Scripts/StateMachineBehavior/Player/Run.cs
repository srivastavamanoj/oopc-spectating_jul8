using System;
using UnityEngine;

public class Run : StateMachineBehaviour
{
    #region Private Fields

    private static readonly int Speed = Animator.StringToHash("Speed");
    private Ball ball;
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
        if (team.inputPlayer != player)
            player.moveAutomatic = true;
        else
            player.moveAutomatic = false;

        if (player.moveAutomatic)
        {
            MoveAutomatic(animator);
        }
        else
        {
            if (team.inputPlayer == player)
            {
                Transform trans = player.transform;

                if (team.IsHuman)
                {
                    if (inputManager.bPassButton && Ball.owner != player)
                    {
                        animator.SetTrigger(Player.Tackle);
                        player.timeToBeSelectable = Globals.periodToBeSelectable;
                    }

                    if (inputManager.fVertical != 0.0f || inputManager.fHorizontal != 0.0f)
                    {
                        player.oldVelocityPlayer = player.actualVelocityPlayer;
                        Vector3 right = inGame.transform.right;
                        Vector3 forward = inGame.transform.forward;
                        right *= inputManager.fHorizontal;
                        forward *= inputManager.fVertical;
                        Vector3 target = trans.position + right + forward;
                        target.y = trans.position.y;
                        float speedForAnimation = 5.0f;

                        trans.LookAt(target);
                        float staminaTemp = Mathf.Clamp((player.stamina / Globals.STAMINA_DIVIDER), Globals.STAMINA_MIN, Globals.STAMINA_MAX);
                        player.actualVelocityPlayer = player.Speed * speedForAnimation * Time.deltaTime * staminaTemp * trans.forward;
                        trans.position += player.actualVelocityPlayer;
                        animator.SetFloat(Speed, player.Speed);
                    }
                    else
                    {
                        animator.SetFloat(Speed, 0f);
                    }
                }
                else
                {
                    Vector3 relPos = trans.InverseTransformPoint(ball.transform.position);
                    player.rotationSpeed = relPos.x / relPos.magnitude;
                    trans.Rotate(0, player.rotationSpeed * 20.0f, 0);
                    float staminaTemp3 = Mathf.Clamp((player.stamina / Globals.STAMINA_DIVIDER), Globals.STAMINA_MIN, Globals.STAMINA_MAX);
                    trans.position += player.Speed * (Globals.speedPlayer-1f) * Time.deltaTime * staminaTemp3 * trans.forward;
                    animator.SetFloat(Speed, player.Speed);
                }

                // if is owner of Ball....
                if (Ball.owner == player)
                {
                    animator.SetTrigger(Player.RunWithBall);
                }
            }
            else
            {
                if (player.moveAutomatic == false)
                    player.moveAutomatic = true;
            }
        }
    }

    #endregion Public Methods

    #region Private Methods

    private void MoveAutomatic(Animator animator)
    {
        var trans = player.transform;
        player.timeToRemove += Time.deltaTime;
        float distanceFromInitPos = (trans.position - player.initialPosition).magnitude;

        // know the distance of ball and player
        float distanceBallMove = (trans.position - ball.transform.position).magnitude;

        // if we get out of bounds of our player we come back to initial position
        if (distanceFromInitPos > Globals.maxDistanceFromPosition)
        {
            Vector3 relInitPos = trans.InverseTransformPoint(player.initialPosition);
            player.rotationSpeed = relInitPos.x / relInitPos.magnitude;
            if (Math.Abs(player.rotationSpeed) < Globals.EPSILON_INPUT && relInitPos.z < 0)
                player.rotationSpeed = 10.0f;
            trans.Rotate(0, player.rotationSpeed * 20.0f, 0);
            float staminaTemp2 = Mathf.Clamp((player.stamina / Globals.STAMINA_DIVIDER), Globals.STAMINA_MIN, Globals.STAMINA_MAX);
            trans.position += player.Speed * Globals.speedPlayer * Time.deltaTime * staminaTemp2 * trans.forward;
            animator.SetFloat(Speed, player.Speed);
        } // if not we go to Ball...
        else
        {
            Vector3 _ball = this.ball.transform.position;
            Vector3 direction = (_ball - trans.position).normalized;
            Vector3 posFinal = player.initialPosition + (direction * Globals.maxDistanceFromPosition);
            Vector3 relativeWaypointP = new Vector3(posFinal.x, posFinal.y, posFinal.z);

            // go to Ball position....
            if (distanceBallMove > 5.0f)
            {
                relativeWaypointP = trans.InverseTransformPoint(posFinal);
            }
            else if (distanceBallMove < 5.0f && distanceBallMove > 2.0f)
            {
                // if we are less than 5 meters of ball we stop
                relativeWaypointP = trans.InverseTransformPoint(trans.position);
            }
            else if (distanceBallMove < 2.0f)
            {
            }

            var inputSteer = player.rotationSpeed;
            inputSteer = relativeWaypointP.x / relativeWaypointP.magnitude;
            if (inputSteer == 0 && relativeWaypointP.z < 0)
                inputSteer = 10.0f;
            if (inputSteer > 0.0f)
                trans.Rotate(0, inputSteer * 20.0f, 0);

            // this just checks if the player's position is near enough.
            if (relativeWaypointP.magnitude < 1.5f)
            {
                trans.LookAt(new Vector3(this.ball.transform.position.x, trans.position.y, this.ball.transform.position.z));
                animator.SetFloat(Speed, 0f);
                player.timeToRemove = 0.0f;
            }
            else
            {
                if (player.timeToRemove > 1.0f)
                {
                    animator.SetTrigger(Player.Run);
                    var staminaTemp = Mathf.Clamp((player.stamina / Globals.STAMINA_DIVIDER), Globals.STAMINA_MIN, Globals.STAMINA_MAX);
                    trans.position += player.Speed * Globals.speedPlayer * Time.deltaTime * staminaTemp * trans.forward;
                    animator.SetFloat(Speed, player.Speed);
                }
            }
        }
    }

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

 
    #endregion Private Methods
}