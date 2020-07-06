using UnityEngine;

public class GoalKeeperJump : MonoBehaviour
{
    #region Public Fields

    public GoalKeeper goalKeeper;

    #endregion Public Fields

    #region Private Fields

    private static readonly int JumpLeft = Animator.StringToHash("jump_left");
    private static readonly int JumpRight = Animator.StringToHash("jump_right");

    #endregion Private Fields

    #region Private Methods

    private void OnTriggerEnter(Collider other)
    {
        // Box triggers are used to know if goalkeeper need to throw to catch the ball
        if (other.CompareTag("Ball"))
        {
            var rb = other.gameObject.GetComponent<Rigidbody>();
            Vector3 dir_goalkeeper = goalKeeper.transform.forward;
            Vector3 dir_ball = rb.velocity;
            dir_ball.Normalize();
            var degree = Vector3.Angle(dir_goalkeeper, dir_ball);
            
          
            if (degree > 90.0f && degree < 270.0f && rb.velocity.magnitude > Globals.minBallSpeedToJump && !rb.isKinematic)
            {
                if (CompareTag("GoalKeeper_Jump_Left"))
                {
                    goalKeeper.animator.SetTrigger(JumpLeft);
                }

                if (CompareTag("GoalKeeper_Jump_Right"))
                {
                    goalKeeper.animator.SetTrigger(JumpRight);
                }
            }
        }
    }

    #endregion Private Methods
}