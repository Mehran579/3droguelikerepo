using UnityEngine;

public class railgunbulletbehaviour : MonoBehaviour
{
    Rigidbody rb;
    Vector3 dir;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log("hehe");
    }

    public void getdir(Vector3 direction)
    {
        dir = direction.normalized;
        rb.linearVelocity = dir*500f;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        rb.linearVelocity = dir * 500f;
    }
}
