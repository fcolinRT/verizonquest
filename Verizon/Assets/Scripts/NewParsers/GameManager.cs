using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region PUBLIC_PROPERTIES
    [Header("INTERNAL VALUES")]
    public static System.DateTime managerTime;
    public enum EnumInteractionState
    {
        normalTime,
        paused,
        backTime
    }
    public EnumInteractionState interactionState;

    [Header("EXAMPLES")]
    public string exampleStartPoint;
    public string exampleData;
    #endregion

    #region PRIVATE_PROPERTIES
    private bool interactionInPlay;
    #endregion


    #region UNITY_METHODS
    private void Start()
    {
        interactionInPlay = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartExampleStartinPoint();

        if (Input.GetKeyDown(KeyCode.Return))
            StartExampleData();

        if (Input.GetKeyDown(KeyCode.RightShift))
            StartInteration();
    }
    #endregion

    #region PUBLIC_METHODS
    public void StartExampleStartinPoint()
    {
        Serializer.instance.GetInitialMapPoint(exampleStartPoint);
        double lat = double.Parse(Serializer.instance.startingNode.PositionGPS.Latitude);
        double lon = double.Parse(Serializer.instance.startingNode.PositionGPS.Longitude);
        MapManager.instance.SetCoordinates(lat, lon);
        MapManager.instance.InitializeMap();
        managerTime = System.DateTime.Parse(Serializer.instance.startingNode.Timestamp);
    }

    public void StartExampleData()
    {
        Serializer.instance.TranslateFromString(exampleData);
        EntityManager.instance.SpawnEntities(Serializer.instance.completeArray);
    }

    public void StartInteration()
    {
        interactionState = EnumInteractionState.normalTime;
        interactionInPlay = true;
        StartCoroutine(RoutineTimeLapse());

    }

    public void FinishInteraction()
    {
        interactionInPlay = false;
    }
    #endregion

    #region COROUTINES
    IEnumerator RoutineTimeLapse()
    {
        while(interactionInPlay)
        {
            switch (interactionState)
            {
                case EnumInteractionState.normalTime:
                    yield return new WaitForSeconds(1);
                    managerTime.AddSeconds(1);
                    Debug.Log(managerTime);
                    EntityManager.instance.SpawnEntities(Serializer.instance.completeArray);
                    break;

                case EnumInteractionState.backTime:
                    yield return new WaitForSeconds(1);
                    //managerTime.Subtract(1);
                    break;

                case EnumInteractionState.paused:
                    yield return null;
                    break;

                default:
                    yield return null;
                    break;
            }
        }
        
    }
    #endregion
}
