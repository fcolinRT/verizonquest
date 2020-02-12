using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serializer : MonoBehaviour
{

    public static Serializer instance;

    #region PUBLIC_PROPERTIES
    public SerializableClasses.StartingPoint startingNode;
    public SerializableClasses.FullArray completeArray;
    #endregion

    #region UNITY_METHODS
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitValues();
    }
    #endregion

    #region PUBLIC_METHODS
    public void InitValues()
    {
        startingNode = new SerializableClasses.StartingPoint();
        completeArray = new SerializableClasses.FullArray();
    }

    public void GetInitialMapPoint(string data)
    {
        startingNode = JsonUtility.FromJson<SerializableClasses.StartingPoint>(data);
        Debug.Log("Parsed initial point");
    }

    public void TranslateFromString(string incomingData)
    {
        completeArray = JsonUtility.FromJson<SerializableClasses.FullArray  >(incomingData);
        Debug.Log("Parsed data");
    }
    #endregion
}
