using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Target;

    Vector3 _offset = new Vector3(0, 10, -5);

    private void LateUpdate()
    {
        if(Target == null)
        {
            return;
        }
        transform.position = Target.transform.position + _offset;
    }
}
