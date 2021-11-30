using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles moving the attached game object between waypoints
/// </summary>
public class WaypointMover : MonoBehaviour
{
    
    [Header("Waypoint List")]
    [SerializeField] GameManager GameManager;
    public List<Transform> waypoints = new List<Transform>();
    public float moveSpeed = 1f;
    public float waitTime = 3f;
    public enum FacingStyle { LookAlongYOnly, LookDirectly, DontLook };

    public FacingStyle facingStyle = FacingStyle.LookDirectly;
    public bool stopped = false;
    private float timeToStartMovingAgain = 0f;
    private Vector3 previousTarget;
    private Vector3 currentTarget;
    private int currentTargetIndex;
    public Vector3 travelDirection;
    public Vector3 StartPosition = new Vector3(1,1,1);
    void Awake()
    {
        
    }

    void Start()
    {
        //transform.position = StartPosition;
        //Debug.Log(StartPosition);
        InitializeInformation();
    }

    void FixedUpdate()
    {
        ProcessMovementState();
    }

    void ProcessMovementState()
    {
        if (stopped)
        {
            StartCheck();
        }
        else
        {
            Travel();
        }
    }
    void StartCheck()
    {
        if (Time.time >= timeToStartMovingAgain)
        {
            stopped = false;
            previousTarget = currentTarget;
            currentTargetIndex += 1;
            if (currentTargetIndex >= waypoints.Count)
            {
                currentTargetIndex = 0;
            }
            currentTarget = waypoints[currentTargetIndex].position;
            CalculateTravelInformation();
        }
    }

    void InitializeInformation()
    {
        previousTarget = this.transform.position;
        currentTargetIndex = 0;
        if (waypoints.Count > 0)
        {
            currentTarget = waypoints[0].position;
        }
        else
        {
            waypoints.Add(this.transform);        
            currentTarget = previousTarget;
        }
        
        CalculateTravelInformation();
    }
    void CalculateTravelInformation()
    {
        travelDirection = (currentTarget - previousTarget).normalized;
    }


    void Travel()
    {
        transform.Translate(travelDirection * moveSpeed * Time.deltaTime, Space.World);
        bool overX = false;
        bool overY = false;
        bool overZ = false;

        Vector3 directionFromCurrentPositionToTarget = currentTarget - transform.position;

        if (directionFromCurrentPositionToTarget.x == 0 || Mathf.Sign(directionFromCurrentPositionToTarget.x) != Mathf.Sign(travelDirection.x))
        {
            overX = true;
            transform.position = new Vector3(currentTarget.x, transform.position.y, transform.position.z);
        }
        if (directionFromCurrentPositionToTarget.y == 0 || Mathf.Sign(directionFromCurrentPositionToTarget.y) != Mathf.Sign(travelDirection.y))
        {
            overY = true;
            transform.position = new Vector3(transform.position.x, currentTarget.y, transform.position.z);
        }
        if (directionFromCurrentPositionToTarget.z == 0 || Mathf.Sign(directionFromCurrentPositionToTarget.z) != Mathf.Sign(travelDirection.z))
        {
            overZ = true;
            transform.position = new Vector3(transform.position.x, transform.position.y, currentTarget.z);
        }

        ChangeRotation();

        if (overX && overY && overZ)
        {
            BeginWait();
        }
    }

    void ChangeRotation()
    {
        if (facingStyle == FacingStyle.DontLook)
        {
            
        }
        else if (facingStyle == FacingStyle.LookDirectly)
        {
            transform.LookAt(currentTarget);
        }
        else if (facingStyle == FacingStyle.LookAlongYOnly)
        {
            Vector3 targetPositionForRotation = new Vector3(currentTarget.x, transform.position.y, currentTarget.z);
            transform.LookAt(targetPositionForRotation);
        }
    }

    
    void BeginWait()
    {
        stopped = true;
        timeToStartMovingAgain = Time.time + waitTime;
    }
}
