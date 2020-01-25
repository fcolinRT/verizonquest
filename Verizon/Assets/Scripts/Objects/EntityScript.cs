using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (BoxCollider))]
public class EntityScript : MonoBehaviour
{
    public string idEntity;
    public ObjectPositioner.EntityEnum type;

    public List<Vector3> positions;
    public List<string> positionsTimeStrings;
    public List<DateTime> positionsTime;
    public int targetNode;

    [SerializeField]
    private int currentIndex;
    [SerializeField]
    private int nextIndex;
    [SerializeField]
    private float startingTime;
    [SerializeField]
    private float timePassed;
    private Vector3 positionToLook;
    public float deltaTime;

    public Transform viewPoint;
    private bool readyToDie;
    private Vector3[] last2Points;

    [Range(0,500)]
    public float angleSpeed;
    public Vector3 axisVector;
    public Transform[] wheels;

    private bool isSelected;

    public Animator asociatedAnimator;

    public GameObject[] renderersToSelect;
    public AnimationCurve selfLocalSpeed;

    //public float 
    public Collider thisCollider;
    public SpriteRenderer iconIndicator;
    public Transform transformPlayer;

	void Start ()
    {
        isSelected = false;
        thisCollider = this.GetComponent<Collider>();
        if (type.Equals(ObjectPositioner.EntityEnum.Post))
            return;
        readyToDie = false;
        
        last2Points = new Vector3[2];
        startingTime = Time.time;
        deltaTime = 1;
        if (ObjectPositioner.instance.statePlayback.Equals(ObjectPositioner.EnumStatePlayback.bckwd))
        {
            nextIndex = positions.Count - 1;
            Debug.Log(nextIndex);
        }
            
        else
            currentIndex = 0;
        //nextIndex = (ObjectPositioner.instance.statePlayback.Equals(ObjectPositioner.EnumStatePlayback.bckwd) ? (positions.Count - 3) : 1);
        
        if (!ObjectPositioner.instance.useLocalInfo)
            CalculateNextPoints();
        
    }

    public void SelectObject()
    {
        if (!isSelected)
        {
            isSelected = true;
            for (int i = 0; i < renderersToSelect.Length;i++)
            {
                renderersToSelect[i].SetActive(true);
            }
        }
            
        
    }

    public void UnselectObject()
    {
        isSelected = false;
        for (int i = 0; i < renderersToSelect.Length; i++)
        {
            renderersToSelect[i].SetActive(false);
        }
    }

    public EntityScript TransportObject(Transform playerTransform)
    {
        playerTransform.SetParent(viewPoint);
        playerTransform.localPosition = Vector3.down * RaycasSelector.instance.currentCamera.transform.localPosition.y;
        playerTransform.localEulerAngles = Vector3.zero;
        thisCollider.enabled = false;
        if (iconIndicator != null)
            iconIndicator.enabled = false;

        return this;
    }

    public void LeaveThisObject()
    {
        thisCollider.enabled = true;
        if (iconIndicator != null)
            iconIndicator.enabled = true;
    }

    public void CrashEntity()
    {
        for (int i = currentIndex; i< positions.Count; i++)
        {
            positions[i] = positions[currentIndex];
        }
    }

	void Update ()
    {
        if (type.Equals(ObjectPositioner.EntityEnum.Post))
        {
            iconIndicator.transform.LookAt(transformPlayer, Vector3.up);
            return;
        }
            
            

        switch (ObjectPositioner.instance.statePlayback)
        {
            case ObjectPositioner.EnumStatePlayback.bckwd:

                if (ObjectPositioner.instance.useLocalInfo)
                {
                    //timePassed = (startingTime - Time.time);
                    //this.transform.Translate(Vector3.back * Time.deltaTime * ObjectPositioner.speed * selfLocalSpeed.Evaluate(timePassed));
                    break;
                }

                for (int i = 0; i < wheels.Length; i++)
                {
                    wheels[i].Rotate(axisVector * Time.deltaTime * -angleSpeed * ObjectPositioner.speed);
                }

                if (currentIndex > 0)
                {   

                    TimeSpan deltaDateTime = positionsTime[nextIndex] - positionsTime[currentIndex];
                    deltaTime = Math.Abs((float)(deltaDateTime.Seconds + ((deltaDateTime.Milliseconds) / 1000)));
                    //deltaTime = Math.Abs((float)(positionsTime[currentIndex] - positionsTime[currentIndex + 1]).TotalSeconds);
                    timePassed = (Time.time - startingTime) / (deltaTime / ObjectPositioner.speed);
                    this.transform.position = Vector3.Lerp
                        (
                        positions[nextIndex],
                        positions[currentIndex],
                        timePassed
                        );

                    positionToLook = positions[nextIndex] - positions[currentIndex];
                    if (positionToLook.magnitude != 0)
                    {
                        this.transform.rotation = Quaternion.Lerp(
                            this.transform.rotation,
                            Quaternion.LookRotation(positionToLook),
                            Time.time - startingTime);
                    }

                    if (asociatedAnimator != null)
                    {

                        asociatedAnimator.Play((Vector3.Distance(positions[currentIndex], positions[nextIndex]) <= 0.1f) ? "Idle" : "Walking");
                    }

                    if (timePassed >= 1)
                    {
                        startingTime = Time.time;
                        nextIndex = currentIndex;
                        CalculateNextPoints();
                    }
                }
                else
                    DestroyEntity();
            break;

            case ObjectPositioner.EnumStatePlayback.ffwd:

                if (ObjectPositioner.instance.useLocalInfo)
                {
                    //timePassed = (Time.time - startingTime);
                    //this.transform.Translate(Vector3.forward * Time.deltaTime * ObjectPositioner.speed * selfLocalSpeed.Evaluate(timePassed));
                    break;
                }

                for (int i = 0; i < wheels.Length; i++)
                {
                    wheels[i].Rotate(axisVector * Time.deltaTime * angleSpeed * ObjectPositioner.speed);
                }

                if (currentIndex < (positions.Count - 1))
                {
                    TimeSpan deltaDateTime = positionsTime[nextIndex] - positionsTime[currentIndex];
                    deltaTime = Math.Abs((float)(deltaDateTime.Seconds + ((deltaDateTime.Milliseconds) / 1000)));
                    //deltaTime = Math.Abs((float)(positionsTime[currentIndex] - positionsTime[currentIndex + 1]).Milliseconds)/1000;
                    timePassed = (Time.time - startingTime) / (deltaTime / ObjectPositioner.speed);
                    this.transform.position = Vector3.Lerp
                        (
                        positions[currentIndex],
                        positions[nextIndex],
                        timePassed
                        );

                    positionToLook = positions[nextIndex] - positions[currentIndex];
                    if (positionToLook.magnitude != 0)
                    {
                        this.transform.rotation = Quaternion.Lerp(
                            this.transform.rotation,
                            Quaternion.LookRotation(positionToLook),
                            Time.time - startingTime);
                    }

                    if (asociatedAnimator != null)
                    {
                        
                        asociatedAnimator.Play((Vector3.Distance(positions[currentIndex], positions[nextIndex]) <= 0.1f) ? "Idle" : "Walking");
                    }

                    if (timePassed >= 1)
                    {
                        startingTime = Time.time;
                        currentIndex = nextIndex;
                        CalculateNextPoints();
                    }
                }
                else
                    DestroyEntity();
            break;

            case ObjectPositioner.EnumStatePlayback.pause:
                startingTime = Time.time - (timePassed * deltaTime);
            break;

            case ObjectPositioner.EnumStatePlayback.realTime:

                if (ObjectPositioner.instance.useLocalInfo)
                {
                    //timePassed = (Time.time - startingTime);
                    //this.transform.Translate(Vector3.forward * Time.deltaTime * ObjectPositioner.speed * selfLocalSpeed.Evaluate(timePassed ));
                    break;
                }


                for (int i = 0; i < wheels.Length; i++)
                {
                    wheels[i].Rotate(axisVector * Time.deltaTime * angleSpeed);
                }

                if (currentIndex < (positions.Count - 1) )
                {
                    TimeSpan deltaDateTime = positionsTime[nextIndex] - positionsTime[currentIndex];
                    deltaTime = Math.Abs((float)(deltaDateTime.Seconds + ((deltaDateTime.Milliseconds)/1000)));

                    timePassed = (Time.time - startingTime) / deltaTime;
                    this.transform.position = Vector3.Lerp
                        (
                        positions[currentIndex],
                        positions[nextIndex],
                        timePassed
                        );

                    positionToLook = positions[nextIndex] - positions[currentIndex];
                    if (positionToLook.magnitude != 0)
                    {
                        this.transform.rotation = Quaternion.Lerp(
                            this.transform.rotation,
                            Quaternion.LookRotation(positionToLook),
                            Time.time - startingTime);
                    }

                    if (asociatedAnimator != null)
                    {
                        asociatedAnimator.Play( ( Vector3.Distance(positions[currentIndex],positions[nextIndex]) <= 0.1f ) ? "Idle" : "Walking");
                    }


                    if (timePassed >= 1)
                    {
                        startingTime = Time.time;
                        currentIndex = nextIndex;
                        CalculateNextPoints();
                    }
                }
                else
                {
                    DestroyEntity();
                    //startingTime = Time.time;
                }
                
                break;

            default:
            break;
        }
	}
    
    public void CalculateNextPoints()
    {
        if (ObjectPositioner.instance.statePlayback.Equals(ObjectPositioner.EnumStatePlayback.bckwd))
        {
            for (int i = nextIndex; i >= 0; i--)
            {
                if ( (positionsTime[nextIndex] - positionsTime[i]).Seconds >= 1)
                {
                    currentIndex = i;
                    return;
                }
            }
            currentIndex = 0;
        }
        else
        {
            for (int i = 1; i < positionsTime.Count; i++)
            {
                if ((positionsTime[i] - positionsTime[currentIndex]).Seconds >= 1)
                {
                    nextIndex = i;
                    return;
                }
            }
            currentIndex = positions.Count;
        }

        
    }

    public void DestroyEntity()
    {
        //Debug.Log("Destroying object");
        PlagerManager player = GetComponentInChildren<PlagerManager>();
        if (player != null)
        {
            /*player.transform.SetParent(ObjectPositioner.instance.post.transform);
            player.transform.localPosition = Vector3.zero;
            player.transform.localEulerAngles = Vector3.zero;*/
            LeaveThisObject();
            RaycasSelector.instance.asociatedEntity = ObjectPositioner.instance.post.TransportObject(player.transform);
        }

        for (int i = 0; i < ObjectPositioner.instance.currentEntities.Count;i++)
        {
            if (ObjectPositioner.instance.currentEntities[i].idEntity == idEntity)
            {
                ObjectPositioner.instance.currentEntities.RemoveAt(i);
                break;
            }
        }
        Destroy(this.gameObject);
    }

    public void ChangeOnPause()
    {

    }

    public void ChangeOnResume()
    {
        //startingTime = Time.time - ((1 - timePassed) * ObjectPositioner.instance.refreshTime);
        startingTime = Time.time - ( deltaTime * timePassed);
    }

    public void ChangeOnResumeFromBack()
    {
        // startingTime = Time.time - (deltaTime * ( 1 - timePassed));
        startingTime = Time.time + timePassed;
    }

    public void ChangeOnForward()
    {
        //startingTime = Time.time - ((deltaTime - timePassed) * (ObjectPositioner.speed));
        Debug.Log(idEntity + ":" + deltaTime + " " + timePassed);
        //startingTime = Time.time - ( (deltaTime * timePassed) / ObjectPositioner.speed);
        
        //startingTime = Time.time + timePassed;
    }

    public void ChangeOnForwardFromBack()
    {
        Debug.Log(idEntity + ":" + deltaTime + " " + timePassed);
        //startingTime = Time.time - ((deltaTime * (1 - timePassed)) / ObjectPositioner.speed);
        startingTime = Time.time + timePassed;
    }

    public void ChangeOnBackward()
    {
        //startingTime = Time.time - ( ( deltaTime - timePassed ) * (ObjectPositioner.speed) );

        //startingTime = Time.time - ( (deltaTime * ( 1 - timePassed))/ ObjectPositioner.speed);
        startingTime = Time.time + timePassed;
    }
}
