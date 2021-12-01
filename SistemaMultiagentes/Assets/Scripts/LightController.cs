using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
public class LightController : MonoBehaviour
{
    // Start is called before the first frame update
    List<List<int>> lightBool;

    public List<GameObject> TrafficLights;
    public float timeToUpdate = 5.0f;
    private float timer;
    public float dt;
    public int cont = 0;
    public GameObject verde1;
    public GameObject rojo1;
    public GameObject verde2;
    public GameObject rojo2;
    IEnumerator SendData(string data)
    {
        WWWForm form = new WWWForm();
        form.AddField("bundle", "the data");
        string url = "http://localhost:8585/TrafficLightsAgentsStates";
        //using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(data);
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            //www.SetRequestHeader("Content-Type", "text/html");
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();          // Talk to Python
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                
                List<int> newLights = new List<int>();
                string txt = www.downloadHandler.text.Replace('\'', '\"');
                //Debug.Log(txt);
                txt = txt.TrimStart('"', '{', 'd', 'a', 't', 'a', ':', '[');
                txt = "{\"" + txt;
                txt = txt.TrimEnd(']', '}');
                txt = txt + '}';
                //Debug.Log(txt);
                string[] strs = txt.Split(new string[] { "}, {" }, StringSplitOptions.None);
                //foreach( string pal in strs)
                //{
                //    Debug.Log(pal);
                //}
                Debug.Log("strs.Length:" + strs.Length);
                for (int i = 0; i < strs.Length; i++)
                {
                    strs[i] = strs[i].Trim();
                    if (i == 0) strs[i] = strs[i] + '}';
                    else if (i == strs.Length - 1) strs[i] = '{' + strs[i];
                    else strs[i] = '{' + strs[i] + '}';
                    //Debug.Log(strs[i]);
                    Vector3 test = JsonUtility.FromJson<Vector3>(strs[i]);
                    //Debug.Log(test.x);
                    //if(i==2 ||i==4)
                    //{
                    newLights.Add((int)test.x);
                    //}
                    
                }

                List<int> poss = new List<int>();
                for (int s = 0; s < 4; s++)
                {
                    //spheres[s].transform.localPosition = newPositions[s];
                    //Debug.Log(newLights[s]);
                    poss.Add(newLights[s]);
                }
                //Debug.Log(poss);
                lightBool.Add(poss);
            }
        }
        
        //Debug.Log(lightBool);

    }

    void Start()
    {
        
        {
        lightBool = new List<List<int>>();
        //Debug.Log(spheres.Length);
#if UNITY_EDITOR
        //string call = "WAAAAASSSSSAAAAAAAAAAP?";
        Vector3 fakePos = new Vector3(3.44f, 0, -15.707f);
        string json = EditorJsonUtility.ToJson(fakePos);
        //StartCoroutine(SendData(call));
        StartCoroutine(SendData(json));
        timer = timeToUpdate;
#endif
    }
    }

    // Update is called once per frame
    void Update()
    {
        /*
         *    5 -------- 100
         *    timer ----  ?
         */
        timer -= Time.deltaTime;
        dt = 1.0f - (timer / timeToUpdate);

        if(timer < 0)
        {
#if UNITY_EDITOR
            timer = timeToUpdate; // reset the timer
            Vector3 fakePos = new Vector3(3.44f, 0, -15.707f);
            string json = EditorJsonUtility.ToJson(fakePos);
            StartCoroutine(SendData(json));

            foreach (List<int> lista in lightBool)
        {
            //Debug.Log("pene");
            //cont =0;
            if(lista[1]==1)
            {
                verde1.SetActive(true);
                rojo1.SetActive(false);
            }
            else
            {
                verde1.SetActive(false);
                rojo1.SetActive(true);
            }

            if(lista[3]==1)
            {
                verde2.SetActive(true);
                rojo2.SetActive(false);
            }
            else
            {
                verde2.SetActive(false);
                rojo2.SetActive(true);
            }
            

            //Debug.Log();
            //Debug.Log(lista[3]);
            //foreach(int pos in lista)
            //{
                //Debug.Log("culo");
                //Debug.Log(pos);
                
              //  if((cont == 2))
                //{
                  //  Debug.Log(pos);
                //}
                //else if((cont ==4))
                //{
                 //   Debug.Log(pos);
                //}
            //cont++;
            //}
        }
        
#endif
        }
    }
    
}
