using UnityEngine;

public class GoalKeeperJump_Down : MonoBehaviour
{
    #region Public Fields

    public GoalKeeper goalKeeper;

    #endregion Public Fields

    #region Private Fields

    private static readonly int JumpLeftDown = Animator.StringToHash("jump_left_down");
    private static readonly int JumpRightDown = Animator.StringToHash("jump_right_down");

    #endregion Private Fields

    #region Private Methods

    private void OnTriggerEnter(Collider other)
    {
        // Box triggers are used to know if goalkeeper need to throw to catch the ball
        if (other.CompareTag("Ball"))
        {
            var rb = other.gameObject.GetComponent<Rigidbody>();
            Vector3 dir_goalkeeper = goalKeeper.transform.forward;
            Vector3 dir_ball = other.GetComponent<Rigidbody>().velocity;
            dir_ball.Normalize();
            
            var degree = Vector3.Angle(dir_goalkeeper, dir_ball);
            if (degree > 90.0f && degree < 270.0f && rb.velocity.magnitude > 5.0f && !rb.isKinematic)
            {
                if (CompareTag("GoalKeeper_Jump_Left"))
                {
                    goalKeeper.animator.SetTrigger(JumpLeftDown);
                }

                if (CompareTag("GoalKeeper_Jump_Right"))
                {
                    goalKeeper.animator.SetTrigger(JumpRightDown);
                }
            }
        }
    }

    #endregion Private Methods
}