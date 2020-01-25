using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimsManager : MonoBehaviour
{
    public Animator anim;

    public EntityScript[] videoEntities;

    public void PlayAnimation(string nameState)
    {
        anim.Play(nameState);
    }

    public void ChangePoint(int index)
    {
        Debug.Log("Changin with " + index);
        if (RaycasSelector.instance.asociatedEntity != null)
            RaycasSelector.instance.asociatedEntity.LeaveThisObject();
        RaycasSelector.instance.asociatedEntity = videoEntities[index].TransportObject(RaycasSelector.instance.cameraTransform);
    }
}
