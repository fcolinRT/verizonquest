using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityClass : MonoBehaviour
{
    #region PUBLIC_PROPERTIES
    public SerializableClasses.Entity entityAsociated;
    #endregion

    #region PRIVATE_PROPERTIES
    [SerializeField]
    private SerializableClasses.PositionTimeRelation[] positionsRelations;
    private int currentIndex;
    #endregion


    #region PUBLIC_METHODS
    public void SetEntity(SerializableClasses.Entity targetEntity)
    {
        entityAsociated = targetEntity;
        currentIndex = 0;
        SetPositionsAtTime();
        MoveToNextPoint();
    }

    public void SetPositionsAtTime()
    {
        positionsRelations = new SerializableClasses.PositionTimeRelation[entityAsociated.PositionGPS.Length];
        for (int i = 0; i < positionsRelations.Length; i++)
        {
            positionsRelations[i] = new SerializableClasses.PositionTimeRelation();
            positionsRelations[i].position = MapManager.instance.GetPositionFromCoordinates( double.Parse(entityAsociated.PositionGPS[i].Latitude), double.Parse(entityAsociated.PositionGPS[i].Longitude));
            positionsRelations[i].timePosition = (System.DateTime.Parse(entityAsociated.PositionGPS[i].Timestamp) - System.DateTime.Parse(Serializer.instance.startingNode.Timestamp)).Seconds;
        }
    }
    #endregion

    #region VIRTUAL_METHODS
    public virtual void MoveToNextPoint()
    {
        if (currentIndex >= (positionsRelations.Length-1))
            this.gameObject.SetActive(false);
        else
        {
            this.transform.position = positionsRelations[currentIndex].position;
            StartCoroutine(RoutineMoveAlong());
        }
        
            
    }
    #endregion

    #region COROUTINES
    IEnumerator RoutineMoveAlong()
    {
        Debug.Log(currentIndex);
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime/ positionsRelations[currentIndex + 1].timePosition;
            this.transform.position = Vector3.Lerp(positionsRelations[currentIndex].position, positionsRelations[currentIndex + 1].position,t);
            yield return new WaitForEndOfFrame();
        }
        currentIndex++;
        MoveToNextPoint();
    }
    #endregion
}
