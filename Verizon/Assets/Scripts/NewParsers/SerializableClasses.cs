using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializableClasses : MonoBehaviour
{
    [System.Serializable]
    public class StartingPoint
    {
        public string Timestamp;
        public Coordinates PositionGPS;
    }

    [System.Serializable]
    public class FullArray
    {
        public Entity[] data;
    }

    [System.Serializable]
    public class Entity
    {
        public Common Common;
        public Coordinates[] PositionGPS;
        public Velocity_Class Velocity;
    }

    [System.Serializable]
    public class Common
    {
        public string ObjectType;
        public string Uuid;
    }

    [System.Serializable]
    public class Coordinates
    {
        public string Timestamp;
        public string Latitude;
        public string Longitude;
        public string Altitude;
    }

    [System.Serializable]
    public class Velocity_Class
    {
        public string X;
        public string Y;
        public string Z;
    }

    [System.Serializable]
    public class PositionTimeRelation
    {
        public Vector3 position;
        public float timePosition;
    }


}
