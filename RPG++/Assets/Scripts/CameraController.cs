using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    void Update()
    {
        //Debug.Log("Camera was called");

        // does the player exist?
        if (PlayerController.me != null && !PlayerController.me.dead)
        {
            //Debug.Log("Player Exist");
            Vector3 targetPos = PlayerController.me.transform.position;
            targetPos.z = -10;

            transform.position = targetPos;
        }
    }
    
}
