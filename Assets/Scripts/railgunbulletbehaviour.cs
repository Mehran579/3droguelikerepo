using UnityEngine;

public class railgunbulletbehaviour : MonoBehaviour
{
    Rigidbody rb;
    Vector3 dir;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //Debug.Log("hehe");
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
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.root.gameObject.layer == 6)
        {
            collision.transform.root.gameObject.GetComponent<Rigidbody>().AddExplosionForce(13, collision.contacts[0].point, 10, 4, ForceMode.Impulse);
        }
        Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
    }
}
