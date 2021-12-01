using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Agent List")]
    public List<GameObject>Agents;
    [Header("TrafficLight List")]
    public List<GameObject>TrafficLights;
    [Header("Waypoint Routes")]
    public List<Transform> Routes;
    [Header("Waypoint Spawns")]
    public List<Transform> Spawns;
    

    List<List<Transform>> ListTranforms = new List<List<Transform>>();
    List<Vector3> spawnList = new List<Vector3>();
    List<int> list = new List<int>{0,1,2,3,4};

    
    void Start()
    {
        foreach(Transform lista in Routes)
        {
            List<Transform> T = new List<Transform>();
            foreach(Transform position in lista)
            {
                T.Add(position);
            }
            ListTranforms.Add(T);
        }
        foreach(Transform lista in Spawns)
        {
            //List<Vector3> T = new List<Vector3>();
            spawnList.Add(lista.position);


        }
        foreach(GameObject agent in Agents)
        {
            int index = Random.Range(0,list.Count -1);
            int i = list[index];
            //Debug.Log(i);
            //Debug.Log(Routes);
            agent.GetComponent<WaypointMover>().waypoints = ListTranforms[i];
            Debug.Log(Spawns[i]);
            //robot.GetComponent<WaypointMover>().StartPosition = Spawns[i];
            
            list.RemoveAt(index);
        }
    }

    void FixedUpdate()
    {
        
    }
}
