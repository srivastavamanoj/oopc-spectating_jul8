using UnityEngine;

public class Camera_Game : MonoBehaviour
{
    #region Public Fields

    public Transform target;

    #endregion Public Fields

    #region Private Fields

    private float deltaY;
    private Vector3 oldPos;

    #endregion Private Fields

    #region Private Methods

    // Behaviour of camera to follow the ball
    private void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position + new Vector3(0, deltaY, 0), 1f);
    }

    private void Start()
    {
        deltaY = transform.position.y - target.position.y;
    }

    #endregion Private Methods
}