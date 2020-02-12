using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    public GameObject[] prefabs;

    public void InstanceRiverObject()
    {
        GameObject pivotInstance = (GameObject)Instantiate(prefabs[Random.Range(0, prefabs.Length)]);
        pivotInstance.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
    }
}
