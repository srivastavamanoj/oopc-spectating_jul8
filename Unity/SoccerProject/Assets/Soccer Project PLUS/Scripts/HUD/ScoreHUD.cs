using UnityEngine;
using UnityEngine.UI;

public class ScoreHUD : MonoBehaviour
{
    #region Private Fields

    private InGame inGame;

    private Text text;

    #endregion Private Fields

    #region Private Methods

    // Update is called once per frame
    private void LateUpdate()
    {
        text.text = inGame.scoreLocal + " - " + inGame.scoreVisiting;
    }

    // Use this for initialization
    private void Start()
    {
        inGame = GameObject.FindObjectOfType(typeof(InGame)) as InGame;
        text = GetComponent<Text>();
    }

    #endregion Private Methods
}