using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegStepper : MonoBehaviour
{
    // The position and rotation we want to stay in range of when deciding to move or not
    [SerializeField] Transform homeTransform;

    // Is the leg moving?
    public bool Moving;
    public float randomOffsetRange = 0.1f;
    
    IEnumerator Move(float wantStepAtDistance, float moveDuration, float stepOvershootFraction)
    {

        // Indicate we're moving (used later)
        Moving = true;

        // Store the initial conditions
        Quaternion startRot = transform.rotation;
        Vector3 startPoint = transform.position;

        Quaternion endRot = homeTransform.rotation;

        // Directional vector from the foot to the home position
        Vector3 towardHome = (homeTransform.position - transform.position);

        // Total distance to overshoot the target by
        float overshootDistance = wantStepAtDistance * stepOvershootFraction;
        Vector3 overshootVector = towardHome * overshootDistance;

        // Make sure the overshoot is level with the ground by projecting it to the XZ plane
        overshootVector = Vector3.ProjectOnPlane(overshootVector, Vector3.up);

        //Vector that set x and z in a random range
        Vector3 randomVec = new Vector3(Random.Range(-randomOffsetRange, randomOffsetRange), 0, Random.Range(-randomOffsetRange, randomOffsetRange));
        
        //Apply overshoot + random range to give enpoint a slightly random position every time. Making it look
        // a little more realistic / drunken depending on the range size.
        Vector3 endPoint = (homeTransform.position + overshootVector) + randomVec;
        
        // Center point which we want to pass through
        Vector3 centerPoint = (startPoint + endPoint) / 2;
        
        // We want the foot to lift slightly from the ground to make it look realistic so lift with half the overshoot
        centerPoint += homeTransform.up * Vector3.Distance(startPoint, endPoint) / 2f;

        // Time since step started
        float timeElapsed = 0;

        // Use a do-while loop so the normalized time goes past 1.0 on the last iteration,
        // placing us at the end position before ending.
        do
        {
            // Add time since last frame to the time elapsed
            timeElapsed += Time.deltaTime;

            float normalizedTime = timeElapsed / moveDuration;
            
            // Easing function and code in Easing.cs taken from Tween.js
            //We use this because it is frame independent compared to lerp which is not
            normalizedTime = Easing.Cubic.InOut(normalizedTime);

            // Move and lift the foot according to a Quadratic bezier curve in order to make it look slightly more realistic
            transform.position = Vector3.Lerp(
                Vector3.Lerp(startPoint, centerPoint, normalizedTime),
                Vector3.Lerp(centerPoint, endPoint, normalizedTime),
                normalizedTime);

           
            transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);

            // Wait for one frame
            yield return null;
        }
        while (timeElapsed < moveDuration);

        // Done moving
        Moving = false;
    }

    public void TryMove(float stepAtDistance, float moveDuration, float stepOvershoot)
    {
        // If the foot is moving we do not want to try and move it again
        if (Moving) return;


        float distFromHome = Vector3.Distance(transform.position, homeTransform.position);

        // If we foot is too far from the bodys position, move it
        if (distFromHome > stepAtDistance)
        {
            StartCoroutine(Move(stepAtDistance, moveDuration, stepOvershoot));
        }
    }

}
