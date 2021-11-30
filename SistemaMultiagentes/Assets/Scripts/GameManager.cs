using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Robot List")]
    public List<GameObject>Robots;
    [Header("Waypoint Routes")]
    public List<Transform> Routes;
    [Header("Waypoint Spawns")]
    public List<Vector3> Spawns;
    

    List<List<Transform>> ListTranforms = new List<List<Transform>>();
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
        foreach(GameObject robot in Robots)
        {
            int index = Random.Range(0,list.Count -1);
            int i = list[index];
            //Debug.Log(i);
            //Debug.Log(Routes);
            robot.GetComponent<WaypointMover>().waypoints = ListTranforms[i];
            Debug.Log(Spawns[i]);
            //robot.GetComponent<WaypointMover>().StartPosition = Spawns[i];
            
            list.RemoveAt(index);
        }
    }

    void FixedUpdate()
    {
        
    }
}
