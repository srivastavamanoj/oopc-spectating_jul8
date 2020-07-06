using UnityEngine;

public class Ball : MonoBehaviour
{
    #region Public Fields

    public static Player lastCandidatePlayer;
    public static Player owner;

    #endregion Public Fields

    #region Private Fields

    private InputManager inputManager;

    #endregion Private Fields

    #region Public Properties

    public Rigidbody RB { get; private set; }

    #endregion Public Properties

    #region Private Methods

    // Use this for initialization
    private void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
        RB = GetComponent<Rigidbody>();

        RB.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    // Update is called once per frame
    private void Update()
    {
        // if the ball has owner then just put on its feets
        if (owner)
        {
            transform.position = owner.transform.position + owner.transform.forward / 1.5f + owner.transform.up / 5.0f;
            float velocity = owner.actualVelocityPlayer.magnitude;

            if (inputManager.fVertical == 0.0f && inputManager.fHorizontal == 0.0f && owner.team.IsHuman)
            {
                velocity = 0.0f;
                RB.angularVelocity = Vector3.zero;
            }
            transform.Rotate(owner.transform.right, velocity * 10.0f);
        }
    }

    #endregion Private Methods
}