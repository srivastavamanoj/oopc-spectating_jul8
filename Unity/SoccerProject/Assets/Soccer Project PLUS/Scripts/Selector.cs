using UnityEngine;

public class Selector : MonoBehaviour
{
    #region Public Fields

    public Team team;

    #endregion Public Fields

    #region Private Methods

    // Update is called once per frame
    private void Update()
    {
        if (team.inputPlayer != null  && team.IsHuman)
            transform.position = team.inputPlayer.transform.position + (Vector3.up * 0.10f) ;
    }

    #endregion Private Methods
}