using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    #region PUBLIC_PROPERTIES
    public AbstractMap map;
    public Vector2d coordinates;
    public int zoom;
    #endregion

    #region UNITY_METHODS
    private void Awake()
    {
        instance = this;
    }
    #endregion

    #region PUBLIC_METHODS
    public void SetCoordinates(double lattitude, double longitude)
    {
        coordinates = new Vector2d(lattitude, longitude);
    }

    public void InitializeMap()
    {
        map.Initialize(coordinates, zoom);
    }

    public Vector3 GetPositionFromCoordinates(double lattitude, double longitude)
    {
        Debug.Log(lattitude + "," + longitude);
        Vector3 position = Vector3.zero;
        Vector2d gpsPosition = new Vector2d(lattitude, longitude);
        position = map.GeoToWorldPosition(gpsPosition);
        return position;
    }
    #endregion
}
