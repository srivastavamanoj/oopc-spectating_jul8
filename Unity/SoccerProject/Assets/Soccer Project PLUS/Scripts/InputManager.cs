using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region Public Properties

    public bool bPassButton { get; set; }
    public bool bShootButtonFinished { get; set; }
    public float fHorizontal { get; set; }
    public float fVertical { get; set; }
    public float timeShootButtonPressed { get; set; }

    #endregion Public Properties

    #region Private Properties

    private bool bShootButton { get; set; }

    #endregion Private Properties

    #region Private Methods

    private void Update()
    {
        // get input
        fVertical = ControlFreak2.CF2Input.GetAxis("Vertical");
        fHorizontal = ControlFreak2.CF2Input.GetAxis("Horizontal");

        bPassButton = ControlFreak2.CF2Input.GetKey(KeyCode.Z);
        bShootButton = ControlFreak2.CF2Input.GetKey(KeyCode.X);

        if (ControlFreak2.CF2Input.GetKeyUp(KeyCode.X))
        {
            bShootButtonFinished = true;
        }

        if (bShootButton)
        {
            timeShootButtonPressed += Time.deltaTime;
        }
        else
        {
            timeShootButtonPressed = 0.0f;
        }
    }

    #endregion Private Methods
}