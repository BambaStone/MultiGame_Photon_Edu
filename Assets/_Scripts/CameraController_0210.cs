using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController_0210 : MonoBehaviour
{
    public GameObject Target;


    private void LateUpdate()
    {
        if(Target == null)
        {
            return;
        }
        transform.position = Target.transform.position;
        transform.rotation = Target.transform.rotation;
    }
}
