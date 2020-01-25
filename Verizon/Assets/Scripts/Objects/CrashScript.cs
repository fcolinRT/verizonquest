using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashScript : MonoBehaviour
{

    public string id;
    public System.DateTime dateTimeInstanced;
    public SpriteRenderer spriteRender;
    private PlagerManager player;

    private Ray ray;
    private RaycastHit hit;

    private void Start()
    {
        player = GameObject.FindObjectOfType<PlagerManager>();
    }

    private void Update()
    {
        /*if (Mathf.Abs((dateTimeInstanced - ObjectPositioner.instance.currentTime).Seconds) > 10)
        {
            player.ActivateCanvas(false);
            Destroy(this.gameObject);
            return;
        }*/
            

        spriteRender.transform.LookAt(player.transform, Vector3.up);

        Vector3 projectedForwad = Vector3.ProjectOnPlane(player.cameraPlayer.transform.forward, Vector3.up);
        Vector3 projectedPoint = (Vector3.ProjectOnPlane((this.transform.position - player.cameraPlayer.transform.position).normalized, Vector3.up));
        float angle = Vector3.Angle(projectedForwad, projectedPoint);
        float yCrossComponent = Vector3.Cross(projectedForwad, projectedPoint).y;


        if (angle > 40)
            player.ActivateCanvas(true, yCrossComponent < 0 );
        else
            player.ActivateCanvas(false);
    }

   



}
