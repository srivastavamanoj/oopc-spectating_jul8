using UnityEngine;
using UnityEngine.UI;

public class ScorerTimeHUD : MonoBehaviour
{
    #region Public Fields

    public int minutes = 0;
    public int seconds = 0;
    public float timeMatch = 0.0f;
    public float TRANSFORM_TIME = 1.0f;

    #endregion Public Fields

    #region Private Fields

    private InGame inGame;
    private Text text;

    #endregion Private Fields

    #region Private Methods

    // Use this for initialization
    private void Start()
    {
        inGame = GameObject.FindObjectOfType(typeof(InGame)) as InGame;
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (inGame.state == InGame.InGameState.PLAYING)
        {
            timeMatch += Time.deltaTime * TRANSFORM_TIME;
        }

        int d = (int)(timeMatch * 100.0f);
        minutes = d / (60 * 100);
        seconds = (d % (60 * 100)) / 100;

        string time = string.Format("{0:00}:{1:00}", minutes, seconds);
        text.text = time;
    }

    #endregion Private Methods
}