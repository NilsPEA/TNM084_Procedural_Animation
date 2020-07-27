using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegStepper : MonoBehaviour
{
    // The position and rotation we want to stay in range of
    [SerializeField] Transform homeTransform;

    [SerializeField] bool isLeft;

    [SerializeField] Transform arm;
    // Is the leg moving?
    public bool Moving;
    
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

        // Total distance to overshoot by
        float overshootDistance = wantStepAtDistance * stepOvershootFraction;
        Vector3 overshootVector = towardHome * overshootDistance;

        //Restrict the overshoot to be level with the ground by projecting it on the world XZ plane
        overshootVector = Vector3.ProjectOnPlane(overshootVector, Vector3.up);

        //Apply the overshoot
        Vector3 endPoint = homeTransform.position + overshootVector;

        // We want to pass through the center point
        Vector3 centerPoint = (startPoint + endPoint) / 2;
        
        // But also lift off, so we  move it up by half the step distance
        centerPoint += homeTransform.up * Vector3.Distance(startPoint, endPoint) / 2f;

        // Time since step started
        float timeElapsed = 0;

        // Here we use a do-while loop so the normalized time goes past 1.0 on the last iteration,
        // placing us at the end position before ending.
        do
        {
            // Add time since last frame to the time elapsed
            timeElapsed += Time.deltaTime;

            float normalizedTime = timeElapsed / moveDuration;
            normalizedTime = Easing.Cubic.InOut(normalizedTime);

            // Quadratic bezier curve
            transform.position = Vector3.Lerp(
                Vector3.Lerp(startPoint, centerPoint, normalizedTime),
                Vector3.Lerp(centerPoint, endPoint, normalizedTime),
                normalizedTime);
            Vector3 temp;
            if (isLeft)
                temp = new Vector3(transform.position.x - 0.05f, arm.position.y, transform.position.z+0.1f);
            else
                temp = new Vector3(transform.position.x + 0.05f, arm.position.y, transform.position.z + 0.1f);
            arm.position = temp;

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

        if (Moving) return;

        float distFromHome = Vector3.Distance(transform.position, homeTransform.position);


        if (distFromHome > stepAtDistance)
        {
            StartCoroutine(Move(stepAtDistance, moveDuration, stepOvershoot));
        }
    }

}
