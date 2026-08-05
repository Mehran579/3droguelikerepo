using UnityEngine;

public class score_increaser : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float score_points;
    bool flag;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponent<healthmanager>().dead && !flag)
        {
            GameObject.FindWithTag("scoremanager").GetComponent<scoremanager>().score += score_points;
            GameObject.FindWithTag("scoremanager").GetComponent<scoremanager>().NoofEnemies--;
            flag = true;
        }
    }
}
