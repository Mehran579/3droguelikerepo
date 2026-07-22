using UnityEngine;

public class healthmanager : MonoBehaviour
{

    public float health;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void takedamage(float damageamount)
    {
        health -= damageamount;
        Debug.Log("damage taken"+ health);
    }
}
