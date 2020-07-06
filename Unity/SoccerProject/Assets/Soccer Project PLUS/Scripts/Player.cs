using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Player : MonoBehaviour
{
    public static Player kickOffer;

    // Delegates and events
    public delegate void OnPassDelegate();
    public delegate void OnShootDelegate();
    public static event OnPassDelegate passEvent;
    public static event OnShootDelegate shootEvent;


    #region Public Fields

    public Vector3 actualVelocityPlayer;
    public Ball ball;
    public Transform hand_bone;
    public InGame inGame;
    public float Speed = 1.0f;
    public float stamina = 64.0f;
    public TypePlayer type = TypePlayer.DEFENDER;

    #endregion Public Fields

    #region Public Properties

    public Vector3 resetPosition { get; set; }
    public bool canReleaseBall { get; set; } = false;
    public Vector3 initialPosition { get; set; }
    public bool temporallyUnselectable { get; set; } = true;
    public float timeToBeSelectable { get; set; } = 1f;
    public InputManager inputManager { get; private set; }
    public Team team { get; private set; }
    public Vector3 oldVelocityPlayer { get; set; }
    public bool moveAutomatic { get; set; } = false;
    public int dirCorner { get; set; }
    public int uniqueId { get; set; }
    

    #endregion Public Properties

    #region Private Fields

    public Animator animator { get; private set; }
    private Quaternion initialRotation;
    public float rotationSpeed;
    public float timeToPass = Globals.periodToPass;
    public float timeToRemove = Globals.periodToRemove;
    public CapsuleCollider capsuleCollider { get; private set; }
    public bool go_origin { get; set; }
    // Animator parameters
    public static readonly int Pass = Animator.StringToHash("Pass");
    public static readonly int Shoot = Animator.StringToHash("Shoot");
    public static readonly int Tackle = Animator.StringToHash("Tackle");
    public static readonly int Idle = Animator.StringToHash("Idle");
    public static readonly int Run = Animator.StringToHash("Run");
    public static readonly int RunWithBall = Animator.StringToHash("RunWithBall");

    private Color originalColor;
    private float posYfloor;    

    #endregion Private Fields

    #region Public Enums

    public enum TypePlayer
    {
        GOALKEEPER,
        DEFENDER,
        MIDDLER,
        ATTACKER
    };

    #endregion Public Enums

    #region Events Methods

    public void CornerGKSequence(int indexDirection)
    {
        dirCorner = indexDirection;
    }

    public void ThrowSide()
    {
        StartCoroutine(addforce());

        IEnumerator addforce()
        {
            ball.RB.isKinematic = false;
            yield return 0;
            ball.RB.AddForce(inGame.candidateToThrowIn.transform.forward * Globals.forceReleaseBall + new Vector3(0.0f, 1300.0f, 0.0f));
            canReleaseBall = true;
        }
    }

    public void OnPass(int indexAction)
    {        
        switch (indexAction)
        {
            case 2:
                animator.ResetTrigger(Player.Pass);
                animator.SetTrigger(Idle);
                return;

            case 1:
                {
                    Player bestCandidatePlayer = null;
                    float bestCandidateCoord = 1000.0f;
                    foreach (Player go in team.players)
                    {
                        if (go != this)
                        {
                            var position = go.transform.position;
                            Vector3 relativePos = this.transform.InverseTransformPoint(position);
                            float distanceOtherPlayer = relativePos.magnitude;
                            float direction = Mathf.Abs(relativePos.x);
                            if (relativePos.z > 0.0f && direction < 5.0f && distanceOtherPlayer < Globals.distanceOtherPlayerForPass && (direction < bestCandidateCoord))
                            {
                                bestCandidateCoord = direction;
                                bestCandidatePlayer = go;
                            }
                        }
                    }

                    if (Ball.owner == this)
                    {
                        if (bestCandidateCoord != 1000.0f)
                        {
                            var delta = bestCandidatePlayer.transform.position - this.transform.position;
                            Vector3 directionBall = delta.normalized;
                            float distanceBall = delta.magnitude * 1.4f;
                            distanceBall = Mathf.Clamp(distanceBall, 15.0f, 40.0f);
                            ball.RB.velocity = new Vector3(directionBall.x * distanceBall, distanceBall / 5.0f, directionBall.z * distanceBall);
                        }
                        else
                        {
                            // if not found a candidate just throw the ball forward....
                            ball.RB.velocity = this.transform.forward * 20.0f;
                        }

                        Ball.owner = null;

                        //Notify subscribers
                        passEvent?.Invoke();
                    }

                    break;
                }
        }
    }

    public void OnShoot(int indexAction)
    {
        switch (indexAction)
        {
            case 2:
                animator.ResetTrigger(Player.Shoot);
                animator.SetTrigger(Idle);
                return;

            case 1:

                var forward = transform.forward;

                if (Ball.owner == this)
                {
                    Ball.owner = null;
                    if (team.IsHuman)
                    {
                        ball.RB.velocity = new Vector3 (Globals.velocityShoot.x * forward.x, Globals.velocityShoot.y, Globals.velocityShoot.z * forward.z) ;
                    }
                    else
                    {
                        float valueRndY = UnityEngine.Random.Range(4.0f, 10.0f);
                        ball.RB.velocity = new Vector3 (Globals.velocityShoot.x * forward.x, valueRndY, Globals.velocityShoot.z * forward.z) ;
                    }

                    // Notify subscribers
                    shootEvent?.Invoke();
                }

                break;
        }
    }

    #endregion Events Methods    

    #region Private Methods

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // ask if someone is in front of me
    public bool NoOneInFront(List<Player> teamPlayers)
    {
        foreach (Player go in teamPlayers)
        {
            Vector3 relativePos = transform.InverseTransformPoint(go.transform.position);

            if (relativePos.z > 0.0f)
                return true;
        }

        return false;
    }

    protected virtual void OnCollisionStay(Collision coll)
    {
        if (coll.collider.CompareTag("Ball") && !temporallyUnselectable)
        {
            inGame.lastTouched = this;
            var currentAnimatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);
            foreach (var animatorClipInfo in currentAnimatorClipInfo)
            {
                if (animatorClipInfo.clip.name.Equals("tackle"))
                    ball.transform.position += transform.forward;
            }

            Vector3 relativePos = transform.InverseTransformPoint(ball.gameObject.transform.position);

            // only "glue" the ball to player if the collision is at bottom
            if (relativePos.y < 0.35f)
            {
                coll.rigidbody.rotation = Quaternion.identity;
                Ball.owner = this;
            }
        }
    }

    protected virtual void Start()
    {
        posYfloor = transform.position.y;
        originalColor = GetComponentInChildren<SkinnedMeshRenderer>().material.color;

        team = GetComponentInParent<Team>();
        inputManager = FindObjectOfType<InputManager>();
        capsuleCollider = gameObject.GetComponentInChildren<CapsuleCollider>();
        var position = resetPosition = transform.position;
        if (team == inGame.team1)
            initialPosition = new Vector3(position.x, position.y, position.z + Globals.initialDisplacement);
        else if (team == inGame.team2)
            initialPosition = new Vector3(position.x, position.y, position.z - Globals.initialDisplacement);
    }

    private void Update()
    {
        stamina += Globals.staminaRecovery * Time.deltaTime;
        stamina = Mathf.Clamp(stamina, 1, 64);

        // after pass or shoot player get in a Unselectable state some little time
        timeToBeSelectable -= Time.deltaTime;

        if (timeToBeSelectable < 0.0f)
            temporallyUnselectable = false;
        else
            temporallyUnselectable = true;

        
/*        
        if (go_origin)
        {
            GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.black;
        }
        else
        {
            var materialColor = GetComponentInChildren<SkinnedMeshRenderer>().material.color;
            if (!materialColor.Equals(originalColor))
                GetComponentInChildren<SkinnedMeshRenderer>().material.color = originalColor;
        }
        */
    }

    private void LateUpdate()
    {
        transform.position = new Vector3( transform.position.x, posYfloor, transform.position.z );
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (animator)
        {
            var currentAnimatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);
            float i = 0f;
            foreach (var clipInfo in currentAnimatorClipInfo)
            {
                Handles.Label(transform.position + transform.up * (2f - i), clipInfo.clip.name);
                i -= 1f;
            }
        }
#endif
    }

    #endregion Private Methods
}