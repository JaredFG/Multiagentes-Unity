// TC2008B Modelación de Sistemas Multiagentes con gráficas computacionales
// C# client to interact with Python server via POST
// Sergio Ruiz-Loza, Ph.D. March 2021

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class SphereController : MonoBehaviour
{
    List<List<Vector3>> positions;
    List<List<int>> lightBool;

    public List<GameObject> TrafficLights;
    public GameObject[] spheres;
    public float timeToUpdate = 5.0f;
    private float timer;
    public float dt;
    public GameObject verde1;
    public GameObject rojo1;
    public GameObject verde2;
    public GameObject rojo2;

    // IEnumerator - yield return
    IEnumerator SendData2(string data)
    {
        WWWForm form = new WWWForm();
        form.AddField("bundle", "the data");
        string url = "https://nubeunity.us-south.cf.appdomain.cloud/TrafficLightsAgentsStates";
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        //using (UnityWebRequest www = UnityWebRequest.Get(url))
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
                //Debug.Log("strs.Length:" + strs.Length);
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
    IEnumerator SendData(string data)
    {
        WWWForm form = new WWWForm();
        form.AddField("bundle", "the data");
        string url = "https://nubeunity.us-south.cf.appdomain.cloud/CarAgentsPositions";
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        //using (UnityWebRequest www = UnityWebRequest.Get(url))
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
                //Debug.Log(www.downloadHandler.text);    // Answer from Python
                //Debug.Log("Form upload complete!");
                //Data tPos = JsonUtility.FromJson<Data>(www.downloadHandler.text.Replace('\'', '\"'));
                //Debug.Log(tPos);
                List<Vector3> newPositions = new List<Vector3>();
                string txt = www.downloadHandler.text.Replace('\'', '\"');
                txt = txt.TrimStart('"', '{', 'd', 'a', 't', 'a', ':', '[');
                txt = "{\"" + txt;
                txt = txt.TrimEnd(']', '}');
                txt = txt + '}';
                string[] strs = txt.Split(new string[] { "}, {" }, StringSplitOptions.None);
                //Debug.Log("strs.Length:" + strs.Length);
                for (int i = 0; i < strs.Length; i++)
                {
                    strs[i] = strs[i].Trim();
                    if (i == 0) strs[i] = strs[i] + '}';
                    else if (i == strs.Length - 1) strs[i] = '{' + strs[i];
                    else strs[i] = '{' + strs[i] + '}';
                    //Debug.Log(strs[i]);
                    Vector3 test = JsonUtility.FromJson<Vector3>(strs[i]);
                    //Debug.Log(test);
                    newPositions.Add(test);
                }

                List<Vector3> poss = new List<Vector3>();
                for (int s = 0; s < spheres.Length; s++)
                {
                    //spheres[s].transform.localPosition = newPositions[s];
                    poss.Add(newPositions[s]);
                }
                positions.Add(poss);
            }
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        positions = new List<List<Vector3>>();
        lightBool = new List<List<int>>();
        //Debug.Log(spheres.Length);
#if UNITY_EDITOR
        //string call = "WAAAAASSSSSAAAAAAAAAAP?";
        Vector3 fakePos = new Vector3(3.44f, 0, -15.707f);
        string json = EditorJsonUtility.ToJson(fakePos);
        //StartCoroutine(SendData(call));
        StartCoroutine(SendData(json));
        StartCoroutine(SendData2(json));
        timer = timeToUpdate;
#endif
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
            StartCoroutine(SendData2(json));
            foreach (List<int> lista in lightBool)
        {
            //
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


        if (positions.Count > 1)
        {
            for (int s = 0; s < spheres.Length; s++)
            {
                // Get the last position for s
                List<Vector3> last = positions[positions.Count - 1];
                // Get the previous to last position for s
                List<Vector3> prevLast = positions[positions.Count - 2];
                // Interpolate using dt
                Vector3 interpolated = Vector3.Lerp(prevLast[s], last[s], dt);
                spheres[s].transform.localPosition = interpolated;

                if (last[s]!=prevLast[s])
                {
                    Vector3 dir = last[s] - prevLast[s];
                    spheres[s].transform.rotation = Quaternion.LookRotation(dir);
                }

                
            }
        }
    }
}