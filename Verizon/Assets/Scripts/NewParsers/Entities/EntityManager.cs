using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager instance;

    #region PUBLIC_PROPERTIES
    public List<EntityClass> entitiesInScene;
    public GameObject carPrefab;
    public GameObject bikePrefab;
    public GameObject pedestrianPrefab;
    #endregion


    #region UNITY_METHODS
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }
    #endregion

    #region PUBLIC_METHODS
    public void SpawnEntities(SerializableClasses.FullArray arrayData)
    {
        for (int i = 0; i < arrayData.data.Length; i++)
        {
            CreateNewEntity(arrayData.data[i]);
        }
    }

    public void CreateNewEntity(SerializableClasses.Entity entity)
    {
        for (int i = 0; i < entitiesInScene.Count;i++)
        {
            if (entitiesInScene[i].entityAsociated.Common.Uuid.Equals(entity.Common.Uuid))
                return;
        }

        GameObject pivotObject;
        EntityClass pivotClass;
        switch (entity.Common.ObjectType)
        {
            case "car":
                pivotObject = (GameObject)Instantiate(carPrefab);
            break;

            case "bike":
                pivotObject = (GameObject)Instantiate(bikePrefab);
            break;

            case "pedestrian":
                pivotObject = (GameObject)Instantiate(pedestrianPrefab);
            break;

            default:
                pivotObject = null;
            break;
        }

        if (pivotObject != null)
        {
            pivotClass = pivotObject.GetComponent<EntityClass>();
            pivotClass.SetEntity(entity);
            entitiesInScene.Add(pivotClass);

        }
            

    }
    #endregion
}
