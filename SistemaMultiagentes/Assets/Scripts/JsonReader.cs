using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonReader : MonoBehaviour
{
    // Start is called before the first frame update
    public TextAsset textJSON;
    [System.Serializable]
    
    public class Positions
    {
        public float x;
        public float y;
        public float z;
    
    }
    //public class Player
    //{
    //    public string name;
    //    public int health;
    //    public int mana;
    //}
    [System.Serializable]
    public class positionList
    {
        public Positions[] positions;
    }

    //public class PlayerList
    //{
    //    public Player[] player;
    //}
    public positionList myPositionList = new positionList();
    //public PlayerList myPlayerList = new PlayerList();
    void Start()
    {
        myPositionList =JsonUtility.FromJson<positionList>(textJSON.text);
        //myPlayerList =JsonUtility.FromJson<PlayerList>(textJSON.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
