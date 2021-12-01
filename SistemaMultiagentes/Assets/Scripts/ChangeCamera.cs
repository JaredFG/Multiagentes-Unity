using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamera : MonoBehaviour
{
    public GameObject Top;
    public GameObject Side;
    public GameObject Cross;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setTop()
    {
        Top.SetActive(true);
        Side.SetActive(false);
        Cross.SetActive(false);
    }
    public void setSide()
    {
        Top.SetActive(false);
        Side.SetActive(true);
        Cross.SetActive(false);
    }
    public void setCross()
    {
        Top.SetActive(false);
        Side.SetActive(false);
        Cross.SetActive(true);
    }
}
