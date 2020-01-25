using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaycasSelector : MonoBehaviour
{

    public Transform cameraTransform;
    public Camera currentCamera;

    private RaycastHit hit;
    private Ray ray;
    public EntityScript entityReference;

    public LineRenderer lineRenderer;

    public Color[] colorsUnselected;
    public Color[] colorsSelected;

    public Collider coliderPost;
    public SpriteRenderer spritePost;

    public Image imageConfirm;
    public EntityScript asociatedEntity;

    public static RaycasSelector instance;
    public Button currentButton;
    private Animator animButton;

    private void Awake()
    {
        instance = this;
    }

    void Start ()
    {
        if (ObjectPositioner.instance != null)
            asociatedEntity = ObjectPositioner.instance.post;
	}

    public void AsociateEntity()
    {

    }
	
	void FixedUpdate ()
    {
        ray = new Ray(this.transform.position, this.transform.forward);
        lineRenderer.SetPosition(0,this.transform.position);
        if (Physics.Raycast(ray, out hit))
        {
            lineRenderer.SetPosition(1, hit.point);
            lineRenderer.startColor = colorsSelected[0];
            lineRenderer.endColor = colorsSelected[1];

            
            currentButton = hit.transform.GetComponent<Button>();
            if (animButton != null)
                if (!currentButton.name.Equals(animButton.name))
                    animButton.Play("Normal");
            if (currentButton != null)
            {
                animButton = currentButton.GetComponent<Animator>();
                if (animButton != null)
                    animButton.Play("Highlighted");
                return;
            }
                

            if (entityReference != null)
                entityReference.UnselectObject();
            entityReference = hit.transform.GetComponent<EntityScript>();
            entityReference.SelectObject();
            imageConfirm.enabled = true;
            return;
        }
        else
        {
            if (animButton != null)
                animButton.Play("Normal");
            currentButton = null;
            lineRenderer.SetPosition(1, this.transform.position + this.transform.forward * 20);
            lineRenderer.startColor = colorsUnselected[0];
            lineRenderer.endColor = colorsUnselected[1];
            if (entityReference != null)
                entityReference.UnselectObject();
            entityReference = null;
            if (imageConfirm != null)
                imageConfirm.enabled = false;
        }

        //Debug.DrawRay(this.transform.position, this.transform.forward * 200);
	}

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") > 0) && currentButton != null)
        {
            if (currentButton != null)
                currentButton.onClick.Invoke();
            return;
        }

        if ( (Input.GetKeyDown(KeyCode.Space) || Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") > 0) && entityReference != null )
        {
            Debug.Log("Pressing");
            
            

            asociatedEntity.LeaveThisObject();
            asociatedEntity = entityReference.TransportObject(cameraTransform);
            //Instructions.instance.canvasInstructions.enabled = false;
            
        }
    }
}
