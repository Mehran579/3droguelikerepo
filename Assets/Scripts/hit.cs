using UnityEngine;
public class hit : MonoBehaviour
{
    Collider collider;
    public float knockbacktime;
    public float knockbackforce;
    public float damageamount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.root.CompareTag("Player"))
        {
            //Debug.Log(collision.transform.root);
            collision.transform.root.GetComponent<healthmanager>().takedamage(damageamount);
            collision.transform.root.GetComponent<playermanager>().StartCoroutine(collision.transform.root.GetComponent<playermanager>().knockback(knockbacktime,knockbackforce,collision.ClosestPoint(transform.position)));
        }
    }
    
}
 