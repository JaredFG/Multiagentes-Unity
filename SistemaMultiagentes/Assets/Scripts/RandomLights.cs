using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLights : MonoBehaviour
{
    public Light SpotLight;
    Color[] colours = new Color[7];
    public float time;
    public float repeatRate;
    // Start is called before the first frame update
    void Start()
    {
        colours[0] = Color.blue;
        colours[1] = Color.cyan;
        colours[2] = Color.green;
        colours[4] = Color.magenta;
        colours[5] = Color.red;
        colours[6] = Color.yellow;

        InvokeRepeating("ChangeColour", time, repeatRate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void ChangeColour()
    {
        SpotLight.color = colours[Random.Range(0, colours.Length -1)];     
    }
    
}
