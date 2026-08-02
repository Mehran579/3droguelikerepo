using System.Collections;
using UnityEngine;

public class rogue : MonoBehaviour
{
    Transform player;
    Renderer[] rends;

    public float visible_Distance;
    bool player_reached;
    bool ishidden;
    void Start()
    {
        rends = GetComponentsInChildren<Renderer>();
        player = GameObject.FindWithTag("Player").transform;
        foreach (Renderer r in rends)
        {
            r.enabled = false;
        }
    }

    void Update()
    {
        if(GetComponent<healthmanager>().health <= 0)
        {
            foreach (Renderer r in rends)
            {
                r.enabled = true;
            }
            return;
        }
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist < visible_Distance && !ishidden) 
        {
            ishidden = true;
            StartCoroutine(Unhide());
        }
        else
        {
        }


        //bool shouldBeVisible = dist <= visible_Distance;

        //foreach (Renderer r in rends)
        //{
        //    r.enabled = shouldBeVisible;
        //}
    }
    IEnumerator Unhide()
    {
        foreach (Renderer r in rends)
        {
            r.enabled = true;
        }
        yield return new WaitForSeconds(5);
        ishidden = false;
            foreach (Renderer r in rends)
            {
                r.enabled = false;
            }
    }
}
