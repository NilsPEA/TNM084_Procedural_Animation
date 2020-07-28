using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    // This is a basic script to move the follow target using arrow keys or wasd
    
        //Speed can be set from unity UI
    public float speed = 1.0f;
    void Update()
    {
        var keyBoardX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        var keyBoardZ = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        transform.Translate(keyBoardX, 0, keyBoardZ);
    }
}
