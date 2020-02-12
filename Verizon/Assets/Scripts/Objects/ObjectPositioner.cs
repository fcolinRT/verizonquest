using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPositioner : MonoBehaviour
{
    public static ObjectPositioner instance;

    public bool replaceURL = true;
    public string urlBaseAPI;
    [Range(0.1f,10f)]
    public float refreshTime = 1;

    public AbstractMap Map;
    public List<string> Coordinates;
    public List<EntityScript> currentEntities;

    public GameObject prefabCar;
    public GameObject prefabPeople;
    public GameObject prefabBike;

    public GameObject prefabCrash;

    // map example coords   40.776988, -73.963708
    public FullStructure fullExample;
    private List<string> activeNodes;
    public List<int> activeNodesIndex;
    public int lastIndexActivated;

    // private DateTime startDate;
    public bool useLocalInfo = false;
    public FullStructure getStructure;
    public FullNodeArray getStructureNode;
    public ConsultasAPI verizonAPI;
    public ConsultasAPI verizonMapAPI;

    public DateTime currentTime;
    private DateTime startTime;

    public EntityScript post;

    public static float speed = 1;

    public float additionalLifetime = 200;

    public int startingDay = 19;
    public int startingMont = 09;
    public int hoursStarting = 20;
    public int minutesStarting = 0;
    public int secondsStarting = 0;

    public float defaultDegrees;

    public enum EntityEnum
    {
        Post,
        Car,
        Bike,
        Person
    }
    public enum EnumStatePlayback
    {
        realTime,
        pause,
        ffwd,
        bckwd
    }

    public EnumStatePlayback statePlayback;
    private void Awake()
    {
        instance = this;
    }

    public Animator videoAnimator;

    public FullStructureArray fsa;

    private void Start()
    {
         fsa = new FullStructureArray();
        

        if (useLocalInfo)
        {
            //currentTime = startTime = new DateTime(2018, startingMont, startingDay, hoursStarting, minutesStarting, secondsStarting);
            StartCoroutine("RoutineExampleVideo");
        }
        else
        {
            verizonMapAPI.Get();
        }
        
        speed = 1;
        activeNodes = new List<string>();
        activeNodesIndex = new List<int>();
        activeNodesIndex.Add(0);
        lastIndexActivated = 0;

        
    }

    private void Update()
    {
        Debug.Log(JsonUtility.ToJson(fsa));
    }

    public void InitializeMap()
    {
        Posts posts = new Posts();
        posts = JsonUtility.FromJson<Posts>(verizonMapAPI.result);
        Vector2d centerMap = new Vector2d( posts.nodes[0].coordinates.latitude, posts.nodes[0].coordinates.longitude);
        Map.Initialize(centerMap, 18);

        if (UIManager.instance.gpsData != null)
            UIManager.instance.gpsData.text = centerMap.ToString();
    }

    public void StartRetrievingData()
    {
        currentTime = startTime = new DateTime(2018,startingMont,startingDay,hoursStarting,minutesStarting,secondsStarting);
        /*currentTime = startTime = new DateTime(2018, 09, 
            DateTime.Now.Hour, 
            DateTime.Now.Minute, 
            DateTime.Now.Second, 0);*/


        //verizonAPI.addressAPI = verizonAPI.addressAPI + startTime.ToString();

        currentEntities = new List<EntityScript>();
        statePlayback = EnumStatePlayback.realTime;


        Map.OnInitialized += () =>
        {
            foreach (var item in Coordinates)
            {
                var latLonSplit = item.Split(',');
                var llpos = new Vector2d(double.Parse(latLonSplit[1]), double.Parse(latLonSplit[0]));
                var pos = Conversions.GeoToWorldPosition(llpos, Map.CenterMercator, Map.WorldRelativeScale);
                var gg = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                gg.transform.position = new Vector3((float)pos.x, 0, (float)pos.y);
            }
        };

        if (replaceURL)
            verizonAPI.addressAPI = urlBaseAPI + startTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(" ", "%20");
        else
            verizonAPI.addressAPI = verizonAPI.addressAPI + SceneAdmin.typeCrash;
            //Debug.Log( urlBaseAPI + startTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(" ","%20"));
        verizonAPI.Get();
            
    }

    public void InstanceCrash(Crashes targetCrash)
    {
        DateTime crashTime = DateTime.Parse(targetCrash.time);
        //Debug.Log("Try to crash " + Mathf.Abs((crashTime - ObjectPositioner.instance.currentTime).Seconds));
        if (Mathf.Abs((crashTime - ObjectPositioner.instance.currentTime).Seconds) > 5)
            return;
        //Debug.Log("Creating crash");
        CrashScript[] scriptArray = GameObject.FindObjectsOfType<CrashScript>();

        for (int i = 0; i < scriptArray.Length; i++)
            if (scriptArray[i].id == targetCrash.id)
                return;

        Vector2d llpos;
        llpos = new Vector2d(
                    targetCrash.latitude,
                    targetCrash.longitude);

        Vector3 newPosition = Map.GeoToWorldPosition(llpos);
        newPosition.y = 5;
        GameObject pivotCrash = (GameObject)Instantiate(prefabCrash, newPosition, Quaternion.identity );
        CrashScript newCrash = pivotCrash.GetComponent<CrashScript>();
        newCrash.id = targetCrash.id;
        newCrash.dateTimeInstanced = crashTime;

        EntityScript[] currentEntities = GameObject.FindObjectsOfType<EntityScript>();
        for (int j = 0; j < currentEntities.Length; j++)
        {
            if (currentEntities[j].idEntity == newCrash.id || currentEntities[j].idEntity == "crash-id")
            {
                currentEntities[j].positions.Add(currentEntities[j].positions[currentEntities[j].positions.Count - 1]);
                currentEntities[j].positionsTime.Add(currentEntities[j].positionsTime[currentEntities[j].positionsTime.Count - 1].AddSeconds(5));
                Transform[] childs = currentEntities[j].GetComponentsInChildren<Transform>();
                for (int k = 0; k < childs.Length;k++)
                {
                    childs[k].gameObject.layer = 10;
                }

                currentEntities[j].CrashEntity();
            }
                
        }

    }
    
    public void InstanceEntity(Entity targetEntity)
    {
        //Debug.Log("Trying to instance");

        if (currentEntities.Count > 50)
            return;

        bool createdEntity = false;
        Vector2d llpos;

        for (int j = 0; j < currentEntities.Count; j++)
        {
            if (currentEntities[j].idEntity.Equals(targetEntity.id))
            {
                createdEntity = true;
//                Debug.Log("Ya existe " + currentEntities[j].idEntity + " con " + targetEntity.id);
                /*for (int k = 0; k < targetEntity.coordinates.Count; k++)
                {
                    llpos = new Vector2d(
                        targetEntity.coordinates[k].latitude,
                        targetEntity.coordinates[k].longitude);

                    Vector3 positionVector = Map.GeoToWorldPosition(llpos);
                    //currentEntities[j].transform.position = positionVector;
                    currentEntities[j].positions.Add(positionVector);
                    currentEntities[j].positionsTime.Add(DateTime.Parse(targetEntity.coordinates[k].time));
                }*/     // NO HA LLEGADO AL PUNTO FINAL POR ESO AUN SE INSTANCIA
                return;
            }
        }

        //Debug.Log("Entity doesnt exist");

        if (targetEntity.coordinates.Count > 1)
        {
            GameObject newEntity = null;
            Vector3 spawnEntityPos = Vector3.zero;
            switch (targetEntity.type)
            {
                case "car":
                    newEntity = (GameObject)Instantiate(prefabCar);
                    break;

                case "bicycle":
                    newEntity = (GameObject)Instantiate(prefabBike);
                    break;

                case "person":
                    newEntity = (GameObject)Instantiate(prefabPeople);
                    break;

                default:
                    Debug.LogWarning("Entity doesn't exist " + targetEntity.type);
                break;
            }

            // Debug.Log("entity created");

            if (newEntity != null)
            {
                EntityScript scriptEntity = newEntity.GetComponent<EntityScript>();
                currentEntities.Add(scriptEntity);

                scriptEntity.idEntity = targetEntity.id;
                for (int k = 0; k < targetEntity.coordinates.Count; k++)
                {
                    llpos = new Vector2d(
                    targetEntity.coordinates[k].latitude,
                    targetEntity.coordinates[k].longitude);


                    Vector3 positionVector = Map.GeoToWorldPosition(llpos);
                    targetEntity.coordinates[k].unityCoords = positionVector;
                    if (k==0)
                    {
                        scriptEntity.transform.position = positionVector;
                    }
                    if (k==1)
                    {
                        
                        
                        if (Vector3.Distance(scriptEntity.positions[scriptEntity.positions.Count - 1], positionVector) == 0)
                            scriptEntity.transform.localEulerAngles = Vector3.up * defaultDegrees;
                        else
                            scriptEntity.transform.LookAt(positionVector);
                    }

                    if (scriptEntity.positions == null)
                        scriptEntity.positions = new List<Vector3>();

                    //if (!scriptEntity.positions.Contains(positionVector))
                    //{
                    scriptEntity.positions.Add(positionVector);
                    if (scriptEntity.positionsTime == null)
                        scriptEntity.positionsTime = new List<DateTime>();

                    scriptEntity.positionsTime.Add(
                        DateTime.ParseExact(targetEntity.coordinates[k].time.Replace("T"," ").Replace("Z",""),
                         "yyyy-MM-dd HH:mm:ss.fff",
                        System.Globalization.CultureInfo.InvariantCulture));

                    scriptEntity.positionsTimeStrings.Add(targetEntity.coordinates[k].time);
                    //}
                    
                }

                // Enlarge movement time
                // Add front position
                Vector3 movementVector = scriptEntity.positions[scriptEntity.positions.Count - 1] - 
                    scriptEntity.positions[scriptEntity.positions.Count - 2];

                float deltaTime = (float) (scriptEntity.positionsTime[scriptEntity.positionsTime.Count - 2] - 
                    scriptEntity.positionsTime[scriptEntity.positionsTime.Count - 1]).Milliseconds;
                deltaTime = deltaTime / 1000;
                deltaTime = Mathf.Abs(deltaTime);
                //deltaTime = (movementVector.magnitude == 0) ? 10 : (additionalLifetime * deltaTime / movementVector.magnitude);

                //  Debug.Log(movementVector + " " + additionalLifetime + " " + deltaTime);
                if (deltaTime == 0)
                    scriptEntity.positions.Add(scriptEntity.positions[scriptEntity.positions.Count - 1]);
                else
                scriptEntity.positions.Add(scriptEntity.positions[scriptEntity.positions.Count - 1] +
                    (movementVector * (additionalLifetime/deltaTime) ));

                //Debug.Log("Total positions: " + targetEntity.coordinates.Count + " Total times: " + targetEntity.coordinates.Count + " agregando " + deltaTime.ToString() );
                scriptEntity.positionsTime.Add( scriptEntity.positionsTime[scriptEntity.positionsTime.Count - 1].AddSeconds(additionalLifetime) );
                scriptEntity.positionsTimeStrings.Add(scriptEntity.positionsTime[scriptEntity.positionsTime.Count - 1].ToString());


            }
        }
    }

    public void ParseData()
    {

        getStructureNode = new FullNodeArray();
        getStructureNode = JsonUtility.FromJson<FullNodeArray>(verizonAPI.result);

        for (int i = 0; i < getStructureNode.vehicles.Count; i++)
        {
            activeNodes.Add(getStructureNode.vehicles[i].nodeId);
            for (int j = 0; j < getStructureNode.vehicles[i].objects.Count; j++)
            {
                getStructureNode.vehicles[i].objects[j].coordinates.Sort
                (
                delegate (GPSCoords coord1, GPSCoords coord2)
                { return coord1.time.CompareTo(coord2.time); });
            }

            getStructureNode.vehicles[i].objects.Sort(
            delegate (Entity entity1, Entity entity2)
            { return entity1.coordinates[0].time.CompareTo(entity2.coordinates[0].time); });
        }

        

        StartCoroutine("RoutineInstanceManager");

        // USING OLD JSON STRUCTURE
        /*
        getStructure = new FullStructure();
        getStructure = JsonUtility.FromJson<FullStructure>(verizonAPI.result);

        for (int i = 0; i < getStructure.entities.Count; i++)
        {
            getStructure.entities[i].coordinates.Sort(
                delegate(GPSCoords coord1, GPSCoords coord2) 
                { return coord1.time.CompareTo(coord2.time); });
        }

        getStructure.entities.Sort(
            delegate(Entity entity1, Entity entity2) 
            { return entity1.coordinates[0].time.CompareTo(entity2.coordinates[0].time); });

        StartCoroutine("RoutineInstanceManager");
        */
    }

    public void ParseDataFast()
    {
        getStructure = new FullStructure();
        getStructure = JsonUtility.FromJson<FullStructure>(verizonAPI.result);

        for (int i = 0; i < getStructure.entities.Count; i++)
        {
            getStructure.entities[i].coordinates.Sort(
                delegate (GPSCoords coord1, GPSCoords coord2)
                { return coord1.time.CompareTo(coord2.time); });
        }

        getStructure.entities.Sort(
            delegate (Entity entity1, Entity entity2)
            { return entity1.coordinates[0].time.CompareTo(entity2.coordinates[0].time); });

        // Harcode
        /*getStructure.crashes = new List<Crashes>();
        Crashes crash = new Crashes();
        crash.id = "Z%q0eaj0uAN@SKJBrqc^";
        crash.type = "car";
        crash.latitude = 38.510590f;
        crash.longitude = -121.436025f;
        DateTime dtNew = new DateTime(2018,9,19,20,00,03);
        crash.time = dtNew.ToString("yyyy-MM-dd HH:mm:ss");
        getStructure.crashes.Add(crash);*/

        StartCoroutine("RoutineInstanceManagerFast");
    }

    public void InstanceEntityExample(Entity targetEntity)
    {
        if (currentEntities.Count > 50)
            return;

        for (int j = 0; j < currentEntities.Count; j++)
        {
            if (currentEntities[j].idEntity.Equals(targetEntity.id))
                return;
        }

        GameObject newEntity = null;
        Vector3 spawnEntityPos = Vector3.zero;
        switch (targetEntity.type)
        {
            case "car":
                newEntity = (GameObject)Instantiate(prefabCar);
            break;

            case "bicycle":
                newEntity = (GameObject)Instantiate(prefabBike);
            break;

            case "person":
                newEntity = (GameObject)Instantiate(prefabPeople);
            break;

            default:
                Debug.LogWarning("Entity doesn't exist " + targetEntity.type);
            break;
        }

        // Debug.Log("entity created");

        if (newEntity != null)
        {
            EntityScript scriptEntity = newEntity.GetComponent<EntityScript>();
            currentEntities.Add(scriptEntity);

            scriptEntity.idEntity = targetEntity.id;
            for (int k = 0; k < targetEntity.coordinates.Count; k++)
            {

                if (k == 0)
                {
                    scriptEntity.transform.position = targetEntity.coordinates[k].unityCoords;
                }
                if (k == 1)
                {
                    if (Vector3.Distance(scriptEntity.positions[scriptEntity.positions.Count - 1], targetEntity.coordinates[k].unityCoords) == 0)
                        scriptEntity.transform.localEulerAngles = Vector3.up * defaultDegrees;
                    else
                        scriptEntity.transform.LookAt(targetEntity.coordinates[k].unityCoords);
                }

                if (scriptEntity.positions == null)
                    scriptEntity.positions = new List<Vector3>();

                //if (!scriptEntity.positions.Contains(positionVector))
                //{
                scriptEntity.positions.Add(targetEntity.coordinates[k].unityCoords);
                if (scriptEntity.positionsTime == null)
                    scriptEntity.positionsTime = new List<DateTime>();

                scriptEntity.positionsTime.Add(
                    DateTime.ParseExact(targetEntity.coordinates[k].time.Replace("T", " ").Replace("Z", ""),
                     "yyyy-MM-dd HH:mm:ss.fff",
                    System.Globalization.CultureInfo.InvariantCulture));

                scriptEntity.positionsTimeStrings.Add(targetEntity.coordinates[k].time);
                //}

            }

            // Enlarge movement time
            // Add front position
            Vector3 movementVector = scriptEntity.positions[scriptEntity.positions.Count - 1] -
                scriptEntity.positions[scriptEntity.positions.Count - 2];

            float deltaTime = (float)(scriptEntity.positionsTime[scriptEntity.positionsTime.Count - 2] -
                scriptEntity.positionsTime[scriptEntity.positionsTime.Count - 1]).Milliseconds;
            deltaTime = deltaTime / 1000;
            deltaTime = Mathf.Abs(deltaTime);
            //deltaTime = (movementVector.magnitude == 0) ? 10 : (additionalLifetime * deltaTime / movementVector.magnitude);

            //  Debug.Log(movementVector + " " + additionalLifetime + " " + deltaTime);
            if (deltaTime == 0)
                scriptEntity.positions.Add(scriptEntity.positions[scriptEntity.positions.Count - 1]);
            else
                scriptEntity.positions.Add(scriptEntity.positions[scriptEntity.positions.Count - 1] +
                    (movementVector * (additionalLifetime / deltaTime)));

            //Debug.Log("Total positions: " + targetEntity.coordinates.Count + " Total times: " + targetEntity.coordinates.Count + " agregando " + deltaTime.ToString() );
            scriptEntity.positionsTime.Add(scriptEntity.positionsTime[scriptEntity.positionsTime.Count - 1].AddSeconds(additionalLifetime));
            scriptEntity.positionsTimeStrings.Add(scriptEntity.positionsTime[scriptEntity.positionsTime.Count - 1].ToString());


        }
    }

    // For Example...
    /*
    public void ParseData(ConsultasAPI apiRequest)
    {
        getStructure = new FullStructure();
        getStructure = fullExample;

        Vector2d llpos;
        bool createdEntity = false;
        
        for (int i = 0; i < getStructure.entities.Length; i++)
        {
            createdEntity = false;
            for (int j = 0; j < currentEntities.Count; j++)
            {
                if (currentEntities[j].idEntity.Equals(getStructure.entities[i].id))
                {
                    createdEntity = true;
                    for (int k = 0; k < getStructure.entities[i].coordinates.Count; k++)
                    {
                        llpos = new Vector2d(
                            getStructure.entities[i].coordinates[k].latitude, 
                            getStructure.entities[i].coordinates[k].longitude);

                        Vector3 positionVector = Map.GeoToWorldPosition(llpos);
                        currentEntities[j].transform.position = positionVector;
                        currentEntities[j].positions.Add(positionVector);
                    }
                }
            }


            if (!createdEntity)
            {
                GameObject newEntity = null;
                Vector3 spawnEntityPos = Vector3.zero;

                switch(getStructure.entities[i].type)
                {
                    case "Car":
                        newEntity = (GameObject)Instantiate(prefabCar);
                    break;

                    case "Bike":
                        newEntity = (GameObject)Instantiate(prefabBike);
                    break;

                    case "Person":
                        newEntity = (GameObject)Instantiate(prefabPeople);
                    break;

                    default:
                        Debug.LogError("Entity doesn't exist");
                    break;
                }

                if (newEntity != null)
                {
                    EntityScript scriptEntity = newEntity.GetComponent<EntityScript>();
                    currentEntities.Add(scriptEntity);

                    scriptEntity.idEntity = getStructure.entities[i].id;

                    llpos = new Vector2d
                        (getStructure.entities[i].coordinates[0].latitude,
                        getStructure.entities[i].coordinates[0].longitude);


                    Vector3 positionVector = Map.GeoToWorldPosition(llpos);
                    scriptEntity.transform.position = positionVector;
                    scriptEntity.positions.Add(positionVector);
                    
                }
            }
        }
        

    }
    */
    // ...until here
    
    IEnumerator RoutineInstanceManager()
    {
        DateTime pivot;
        int removeIndex = 0;

        while ((currentTime - startTime).TotalSeconds < 300)
        {
            if (!statePlayback.Equals(EnumStatePlayback.pause))
            {
                for (int j = 0; j < getStructureNode.vehicles.Count; j++)
                {
                    if (!activeNodesIndex.Contains(j))
                        continue;

                    for (int i = removeIndex; i < getStructureNode.vehicles[j].objects.Count; i++)
                    {

                        pivot = DateTime.ParseExact(
                            getStructureNode.vehicles[j].objects[i].coordinates[0].time.Replace("T", " ").Replace("Z", ""),
                            "yyyy-MM-dd HH:mm:ss.fff",
                            System.Globalization.CultureInfo.InvariantCulture);

                        /*Debug.Log(getStructureNode.vehicles[j].objects[i].id + " " +
                            (pivot - currentTime).Seconds.ToString() + " " +
                            pivot.CompareTo(currentTime)
                            );*/

                        if (pivot.CompareTo(currentTime) > 0)
                            //if ( (pivot - currentTime).TotalSeconds > 1 )
                            break;
                        else
                        {
                            if (Math.Abs((pivot - currentTime).Seconds) <= 1)
                                InstanceEntity(getStructureNode.vehicles[j].objects[i]);
                        }
                    }
                }
                


                currentTime = currentTime.AddSeconds(1);
                // Debug.Log("Tiempo objetivo " + currentTime.ToString());
                if (UIManager.instance.timeData != null)
                    UIManager.instance.timeData.text = currentTime.ToString();
            }

            yield return new WaitForSeconds(1);
        }

        Debug.Log("Finish a query");
        getStructureNode.vehicles.Clear();
        startTime = currentTime;
        verizonAPI.addressAPI = urlBaseAPI + startTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(" ", "%20");
        verizonAPI.Get();

        // USING OLD JSON STRUCTURE
        /*
        while ((currentTime - startTime).TotalSeconds < 300)
        {
            if (!statePlayback.Equals(EnumStatePlayback.pause))
            {
                for (int i = removeIndex; i < getStructure.entities.Count; i++)
                {

                    pivot = DateTime.ParseExact(
                        getStructure.entities[i].coordinates[0].time.Replace("T", " ").Replace("Z", ""), 
                        "yyyy-MM-dd HH:mm:ss.fff", 
                        System.Globalization.CultureInfo.InvariantCulture);
                    
                    if (pivot.CompareTo(currentTime) > 0)
                        //if ( (pivot - currentTime).TotalSeconds > 1 )
                        break;
                    else
                    {
                        if (Math.Abs((pivot - currentTime).Seconds) <= 1)
                            InstanceEntity(getStructure.entities[i]);
                    }

                }


                currentTime = currentTime.AddSeconds(1);
                Debug.Log("Tiempo objetivo " + currentTime.ToString());
                if (UIManager.instance.timeData != null)
                    UIManager.instance.timeData.text = currentTime.ToString();
            }
            
            yield return new WaitForSeconds(1);
        }

        Debug.Log("Finish a query");
        getStructure.entities.Clear();
        startTime = currentTime;
        verizonAPI.addressAPI = urlBaseAPI + startTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(" ", "%20");
        verizonAPI.Get();
        */

    }

    IEnumerator RoutineInstanceManagerFast()
    {
        DateTime pivot;

        while (true)
        {
            if (statePlayback.Equals(EnumStatePlayback.ffwd) || statePlayback.Equals(EnumStatePlayback.realTime))
            {
                for (int i = 0; i < getStructure.entities.Count; i++)
                {
                    
                    pivot = DateTime.ParseExact(getStructure.entities[i].coordinates[0].time.Replace("T"," ").Replace("Z",""), "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                    if (pivot.CompareTo(currentTime) > 0)
                    //if ( (pivot - currentTime).TotalSeconds > 1 )
                        break;
                    else
                    {
                        if (Math.Abs((pivot - currentTime).Seconds) <= 1 )
                            InstanceEntity(getStructure.entities[i]);
                    }
                }


                for (int j = 0; j < getStructure.crashes.Count; j++)
                {
                    pivot = DateTime.ParseExact(getStructure.crashes[j].time.Replace("T", " ").Replace("Z", ""), "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                    //Debug.Log("analizyng crash " + pivot.ToString() + " " + currentTime.ToString());
                    if (pivot.CompareTo(currentTime) < 0)
                    {
                        InstanceCrash(getStructure.crashes[j]);
                        break;
                    }
                }

                currentTime = currentTime.AddSeconds(1);
                if (UIManager.instance.timeData != null)
                    UIManager.instance.timeData.text = currentTime.ToString();
                //Debug.Log("Tiempo objetivo " + currentTime.ToString());
            }
            else if (statePlayback.Equals(EnumStatePlayback.bckwd) )
            {
                for (int i = 0; i < getStructure.entities.Count; i++)
                {
                    pivot = DateTime.ParseExact(getStructure.entities[i].coordinates[getStructure.entities[i].coordinates.Count - 1].time.Replace("T", " ").Replace("Z", ""), "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                    //Debug.Log(getStructure.entities[i].id + ": " + (pivot - currentTime).Seconds); 
                    if (Math.Abs((pivot - currentTime).Seconds) <= 1)
                    {
                        InstanceEntity(getStructure.entities[i]);
                    }
                }

                for (int j = 0; j < getStructure.crashes.Count; j++)
                {
                    pivot = DateTime.ParseExact(getStructure.crashes[j].time.Replace("T", " ").Replace("Z", ""), "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                    //Debug.Log("analizyng crash " + pivot.ToString() + " " + currentTime.ToString());
                    if (Math.Abs((pivot - currentTime).Seconds) <= 1)
                    {
                        InstanceCrash(getStructure.crashes[j]);
                    }
                }

                currentTime = currentTime.AddSeconds(-1);
                //Debug.Log("Tiempo objetivo " + currentTime.ToString());
                if (UIManager.instance.timeData != null)
                    UIManager.instance.timeData.text = currentTime.ToString();
            }

            yield return new WaitForSeconds(1/speed);
        }
    }

    IEnumerator RoutineExampleVideo()
    {
        yield return new WaitForSeconds(4);
        videoAnimator.gameObject.SetActive(true);
        
    }

    public void ChangeFastForward()
    {
        
        videoAnimator.Play("Forward",0, 
            (videoAnimator.GetCurrentAnimatorStateInfo(0).IsName("Backward")) ?
                1 - videoAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime :
                videoAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime
                );
    }

    public void ChangeFastBackward()
    {
        videoAnimator.Play("Backward", 0, 1 - videoAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
    }

    public void ChangeRealtime()
    {
        Debug.Log("REALTIME");
        videoAnimator.Play("Realtime", 0,
            (videoAnimator.GetCurrentAnimatorStateInfo(0).IsName("Backward")) ?
            1 - videoAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime :
            videoAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime
            );
    }
}



#region EntitiesClasses
[System.Serializable]
public class FullNodeArray
{
    public List<NodeArray> vehicles;
}

[System.Serializable]
public class NodeArray
{
    public string nodeId;
    public List<Entity> objects;
}

[System.Serializable]
public class FullStructureArray
{
    public Entity[] entities;
    public Crashes[] crashes;
}

[System.Serializable]
public class FullStructure
{
    public List<Entity> entities;
    public List<Crashes> crashes;
}

[System.Serializable]
public class Entity
{
    public string id;
    public string type;
    public List<GPSCoords> coordinates;
    
}

[System.Serializable]
public class Crashes
{
    public string id;
    public string type;
    public float latitude;
    public float longitude;
    public string time;
}


[System.Serializable]
public class GPSCoords
{
    public float latitude;
    public float longitude;
    public string time;
    public Vector3 unityCoords;

}
#endregion




#region NodesClasses
[System.Serializable]
public class Posts
{
    public NodePost[] nodes;
}

[System.Serializable]
public class NodePost
{
    public string nodeId;
    public NodeCoords coordinates;

}

[System.Serializable]
public class NodeCoords
{
    public double latitude;
    public double longitude;
}
#endregion
