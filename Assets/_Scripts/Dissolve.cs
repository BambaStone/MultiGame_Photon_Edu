using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    public float dissolves = 1f;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Q))
        {
            dissolves = dissolves - Time.deltaTime*2;
            if(dissolves<0)
            {
                dissolves = 0;
            }
            GetComponent<Renderer>().material.SetFloat("_Cut", dissolves);
        }
        else
        {
            if(dissolves<1)
            {
                dissolves = dissolves + Time.deltaTime*0.5f;
            }
            GetComponent<Renderer>().material.SetFloat("_Cut", dissolves);
        }

    }
}
