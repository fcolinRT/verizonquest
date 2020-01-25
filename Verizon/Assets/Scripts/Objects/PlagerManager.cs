using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlagerManager : MonoBehaviour
{
    public Camera cameraPlayer;

    public Canvas canvasSpriteCrash;

	public void ActivateCanvas(bool activate, bool right = false)
    {
        canvasSpriteCrash.enabled = activate;
        if (!activate)
            return;

        canvasSpriteCrash.transform.localScale = new Vector3( right ? 1 : -1 , 1, 1 );
    }

    private void Update()
    {
        if (canvasSpriteCrash!=null)
        {
            canvasSpriteCrash.transform.position = cameraPlayer.transform.position + (cameraPlayer.transform.forward * 5);
            canvasSpriteCrash.transform.LookAt(cameraPlayer.transform);
        }
        
    }

    
}
