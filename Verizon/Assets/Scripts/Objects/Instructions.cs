using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instructions : MonoBehaviour
{

    public Canvas canvasInstructions;
    public Camera mainCam;

    public static Instructions instance;

    public float distanceLerp = 10;
    public float speed = 10;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        
    }
}
