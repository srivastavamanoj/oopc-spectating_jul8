using UnityEngine;

public class Globals
{
    #region Public Fields

    // general
    public const double EPSILON_INPUT = 0.1f;
    public const float initialDisplacement = 20.0f;
    public const float maxDistanceFromPosition = 10;
    public const float STAMINA_DIVIDER = 64.0f;
    public const float STAMINA_MAX = 1.0f;
    public const float STAMINA_MIN = 0.5f;
    public const float periodToChangeState = 2.0f;
    public const float periodToBeSelectable = 0.5f;

    // player
    public const float periodToPass = 1.0f;
    public const float periodToRemove = 3.0f;
    public const float distanceOtherPlayerForPass = 15.0f;
    public static readonly Vector3 velocityShoot = new Vector3(30.0f, 5.0f, 30.0f);
    public const float speedTackle = 8f;
    public const float distanceToShoot = 20f;
    public const float staminaRecovery = 2f;
    public const float speedPlayer = 5.5f;
    
    // team
    public const float periodSelectAgain = 0.5f;
    
    // goalkeeper
    public const float forceReleaseBall = 5000.0f;
    public const float minBallSpeedToJump = 5.0f;
    public const float jumpSpeedUp = 7.0f;
    public const float jumpSpeedDown = 4.0f;
    public const float distanceToBackOrigin = 10;
    
    

    // InGame
    public const float perdiodToKickOff = 4.0f;
    public const float perdiodToThrowCPU = 3.0f;
    
    
    
    
    #endregion Public Fields
}