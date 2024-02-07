using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    public GameObject target;
    public GameObject center;

    public float fixedDistance;
    public float percDistance;

    public bool touch;
    public float touchFixedOffset;
    public float touchPercOffset;
    public bool atTarget;
    public float moveSpeed = 2f; // Set your move speed here
    public float stayDuration = 2f; // Set the duration to stay at the target position

    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (touch)
        {
            MoveToTarget();
        }
        else
        {
            // Calculate the position based on the chosen method
            Vector3 newPosition = CalculateEffectorPosition();

            // Set the position of the effector object
            transform.position = newPosition;
        }

    }
    Vector3 CalculateEffectorPosition(float fD = 0, float pD = 0)
    {
        if(fD==0 && pD == 0)
        {
            fD = fixedDistance;
            pD = percDistance;
        }
        // Calculate the distance between target and center
        float distanceBetweenTargets = Vector3.Distance(target.transform.position, center.transform.position);

        // Calculate the position based on the chosen method
        if (fD > 0)
        {
            // If using a fixed distance
            return center.transform.position + (target.transform.position - center.transform.position).normalized * fD;
        }
        else if (pD > 0 && pD < 1)
        {
            // If using a percentage of the distance
            float percentageDistance = distanceBetweenTargets * pD;
            return center.transform.position + (target.transform.position - center.transform.position).normalized * percentageDistance;
        }
        else
        {
            // Default to the center position if no valid options are set
            return center.transform.position;
        }
    }

    void MoveToTarget()
    {
        if (!atTarget)
        {
            // Calculate the target position
            Vector3 targetPosition = CalculateEffectorPosition(touchFixedOffset, touchPercOffset);

            // Move towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Check if the effector has reached the target position
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                atTarget = true;
                timer = 0f;
            }
        }
        else
        {
            // Stay at the target position for the specified duration
            timer += Time.deltaTime;
            if (timer >= stayDuration)
            {
                // Go back to the original position after staying for the specified duration
                atTarget = false;
                touch = false;
            }
        }
    }
}
