using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegStepper3 : MonoBehaviour
{
    // The position and rotation we want to stay in range of
    [SerializeField] Transform LeftHomeTransform;
    [SerializeField] Transform RightHomeTransform;


    [SerializeField] GameObject LeftLeg;
    [SerializeField] GameObject RightLeg;
    // Stay within this distance of home
    [SerializeField] float wantStepAtDistance;
    // How long a step takes to complete
    [SerializeField] float moveDuration;

    // Fraction of the max distance from home we want to overshoot by
    [SerializeField] float stepOvershootFraction;

    // Is the leg moving?
    public bool Moving;
    
    IEnumerator MoveLeft()
    {
        // Indicate we're moving (used later)
        Moving = true;
    
        // Store the initial conditions
        Quaternion LeftStartRot = LeftLeg.transform.rotation;
        Vector3 LeftStartPoint = LeftLeg.transform.position;


        Quaternion LeftEndRot = LeftHomeTransform.rotation;
    
        // Directional vector from the foot to the home position
        Vector3 LeftTowardHome = (LeftHomeTransform.position - transform.position);
     

        // Total distance to overshoot by
        float overshootDistance = wantStepAtDistance * stepOvershootFraction;
        Vector3 LeftOvershootVector = LeftTowardHome* overshootDistance;
      
        //Restrict the overshoot to be level with the ground by projecting it on the world XZ plane
        LeftOvershootVector = Vector3.ProjectOnPlane(LeftOvershootVector, Vector3.up);
      

        //Apply the overshoot
        Vector3 LeftEndPoint = LeftHomeTransform.position + LeftOvershootVector;
     
        // We want to pass through the center point
        Vector3 LeftCenterPoint = (LeftStartPoint + LeftEndPoint) / 2;
     

        // But also lift off, so we  move it up by half the step distance
        LeftCenterPoint += LeftHomeTransform.up * Vector3.Distance(LeftStartPoint, LeftEndPoint) / 2f;
       

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
            LeftLeg.transform.position = Vector3.Lerp(
                Vector3.Lerp(LeftStartPoint, LeftCenterPoint, normalizedTime),
                Vector3.Lerp(LeftCenterPoint, LeftEndPoint, normalizedTime),
                normalizedTime);

            LeftLeg.transform.rotation = Quaternion.Slerp(LeftStartRot, LeftEndRot, normalizedTime);
            // Wait for one frame
            yield return null;
        }
        while (timeElapsed < moveDuration);

        // Done moving
        Moving = false;
    }

    IEnumerator MoveRight()
    {

        Quaternion RightStartRot = RightLeg.transform.rotation;
        Vector3 RightStartPoint = RightLeg.transform.position;

        Quaternion RightEndRot = RightHomeTransform.rotation;

        Vector3 RightTowardHome = (RightHomeTransform.position - transform.position);

        float overshootDistance = wantStepAtDistance * stepOvershootFraction;
        Vector3 RightOvershootVector = RightTowardHome * overshootDistance;
        RightOvershootVector = Vector3.ProjectOnPlane(RightOvershootVector, Vector3.up);

        Vector3 RightEndPoint = RightHomeTransform.position + RightOvershootVector;

        Vector3 RightCenterPoint = (RightStartPoint + RightEndPoint) / 2;
        RightCenterPoint += RightHomeTransform.up * Vector3.Distance(RightStartPoint, RightEndPoint) / 2f;

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
            RightLeg.transform.position = Vector3.Lerp(
                Vector3.Lerp(RightStartPoint, RightCenterPoint, normalizedTime),
                Vector3.Lerp(RightCenterPoint, RightEndPoint, normalizedTime),
                normalizedTime);

            RightLeg.transform.rotation = Quaternion.Slerp(RightStartRot, RightEndRot, normalizedTime);

            // Wait for one frame
            yield return null;
        }
        while (timeElapsed < moveDuration);

        // Done moving
        Moving = false;

    }
    public void TryMove()
    {
        if (Moving) return;

        float LeftDistFromHome = Vector3.Distance(LeftLeg.transform.position, LeftHomeTransform.position);
        float RightDistFromHome = Vector3.Distance(RightLeg.transform.position, RightHomeTransform.position);


        if (LeftDistFromHome > wantStepAtDistance)
        {
            StartCoroutine(MoveLeft());
        }
        if (RightDistFromHome > wantStepAtDistance * 2)
        {
            StartCoroutine(MoveRight());
        }

    }

}
