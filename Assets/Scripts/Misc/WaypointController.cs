using System.Collections.Generic;
using UnityEngine;

public class WaypointController : MonoBehaviour
{
    public bool isPaused = false;
    public int currentWaypointIdx = 0;
    public bool isReversing = false;
    public int currentPauseIdx = -1; // sets pause true when index reached
    public bool lookTowardTarget = true;

    public enum WaypointListType
    {
        REVERSE,
        LOOP,
        ONCE
    }

    [SerializeField] List<Transform> waypoints = new List<Transform>();
    [SerializeField] WaypointListType waypointListType = WaypointListType.REVERSE;
    public float speed = 0;
    [SerializeField] float destinationRadius = 0;

    Vector3 currentDirection = Vector3.zero;

    private void Start()
    {
        if (isPaused == false)
        {
            currentDirection = (waypoints[currentWaypointIdx].position - transform.position).normalized;
            
            if (lookTowardTarget)
                transform.forward = currentDirection;
        }
    }

    private void Update()
    {
        if (isPaused == false)
        {
            // check if at target waypoint
            if (Vector3.Distance(waypoints[currentWaypointIdx].position, transform.position) <= destinationRadius)
            {
                if (currentWaypointIdx == currentPauseIdx)
                {
                    isPaused = true;
                    return;
                }

                // get next waypoint index based on type
                switch (waypointListType)
                {
                    case WaypointListType.REVERSE:
                        currentWaypointIdx += isReversing ? -1 : 1;

                        if (currentWaypointIdx == 0 || currentWaypointIdx == waypoints.Count - 1)
                            isReversing = !isReversing;

                        break;

                    case WaypointListType.LOOP:
                        ++currentWaypointIdx;

                        if (currentWaypointIdx >= waypoints.Count)
                            currentWaypointIdx = 0;

                        break;

                    case WaypointListType.ONCE:
                        ++currentWaypointIdx;

                        if (currentWaypointIdx >= waypoints.Count)
                        {
                            --currentWaypointIdx;
                            isPaused = true;
                            return;
                        }

                        break;
                }

                currentDirection = (waypoints[currentWaypointIdx].position - transform.position).normalized;
                
                if (lookTowardTarget)
                    transform.forward = currentDirection;
            }

            // go to waypoint
            transform.position += currentDirection * Time.deltaTime * speed;
        }
    }

    public void Resume()
    {
        isPaused = false;
        currentDirection = (waypoints[currentWaypointIdx].position - transform.position).normalized;

        if (lookTowardTarget)
            transform.forward = currentDirection;
    }
}