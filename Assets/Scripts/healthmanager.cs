using TMPro;
using UnityEngine;

public class healthmanager : MonoBehaviour
{
    public Transform damagespawn;
    public GameObject damageprefab;
    public float health;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(health == 0)
        {
            Destroy(gameObject);
        }
    }
    public void takedamage(float damageamount)
    {
        health -= damageamount;
        Debug.Log("damage taken"+ health);
        if (!gameObject.CompareTag("Player"))
        {
            GameObject text = Instantiate(damageprefab, damagespawn.position + (Random.insideUnitSphere*0.2f), Quaternion.LookRotation(-GameObject.FindWithTag("Player").transform.position + transform.position));
            //text.transform.localRotation = Quaternion.LookRotation(GameObject.FindWithTag("Player").transform.position - text.transform.localPosition);
            text.GetComponent<TextMeshPro>().text = health.ToString();
            Destroy(text,1f);
        }
    }
}
