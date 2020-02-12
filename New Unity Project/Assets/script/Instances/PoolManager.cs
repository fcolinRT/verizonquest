using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    #region PROPERTIES
    public GameObject prefabGameObject;
    private List<GameObject> objectsCreated;
    #endregion

    #region PUBLIC_METHODS
    public GameObject AskForDisableObject()
    {
        return objectsCreated[ActiveObjectInList()];
    }

    public int ActiveObjectInList()
    {
        int indexObject = -1;
        
        for (int i = 0; i < objectsCreated.Count; i++)
        {
            if (!objectsCreated[i].activeInHierarchy)
            {
                indexObject = i;
                break;
            }
        }

        if (indexObject < 0)
        {
            CreateNewInstance();
            indexObject = objectsCreated.Count - 1;
        }
            
        return indexObject;
    }

    public void CreateNewInstance()
    {
        objectsCreated.Add((GameObject)Instantiate(prefabGameObject));
    }
    #endregion


}
