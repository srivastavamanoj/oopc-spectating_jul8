using UnityEngine;

public class GoalKeeper : Player
{

    #region Public Fields
    
    public static readonly int IdleGK = Animator.StringToHash("Idle");

    #endregion Public Fields

    #region Private Fields

    private static readonly int GetBallFront = Animator.StringToHash("get_ball_front");

    #endregion Private Fields

    #region Public Properties

    public int dirThrowOutFoot { get; set; }
    public bool stoleBall { get; set; }

    #endregion Public Properties

    #region Public Methods

    
    public new void CornerGKSequence(int indexDirection)
    {
        dirThrowOutFoot = indexDirection;
    }

    public void ReleaseBall()
    {
        ball.RB.isKinematic = false;
        ball.RB.AddForce(transform.forward * Globals.forceReleaseBall + new Vector3(0.0f, 1300.0f, 0.0f));
        canReleaseBall = true;
    }

    #endregion Public Methods

    #region Protected Methods

    // To know if GoalKeeper is touching Ball
    protected override void OnCollisionStay(Collision coll)
    {
        if (inGame.state == InGame.InGameState.PLAYING)
        {
            if (coll.collider.transform.gameObject.CompareTag("Ball"))
            {
                inGame.lastTouched = this;

                Vector3 relativePos = transform.InverseTransformPoint(ball.gameObject.transform.position);

                // only get ball if the altitude is 0.35f (relative)
                if (relativePos.y < 0.35f)
                {
                    Ball.owner = null;
                    animator.SetTrigger(GetBallFront);
                }
            }
        }
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        initialPosition = transform.position;
    }

    #endregion Protected Methods
}