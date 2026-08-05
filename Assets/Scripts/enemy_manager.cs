using UnityEngine;

public class enemy_manager : MonoBehaviour
{
    public hit _hit;
    public Collider weapon;
    //public Transform[] waypoints;
    public Transform myfollowpoint;
    public float speed;
    public float attack_distance;
    Rigidbody rb;
    Animator anim;
    GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //_hit = hit;
        rb = GetComponent<Rigidbody>();
        //myfollowpoint = waypoints[Random.Range(0, waypoints.Length)];
        anim = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        myfollowpoint = player.transform.Find("way point "+Random.Range(0,5));
        //transform.rotation = Quaternion.LookRotation(player.transform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 dir = (player.transform.position - rb.position).normalized;
        dir.y = 0;
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir),100 * Time.fixedDeltaTime));
        if (Vector3.Distance(transform.position, player.transform.position) < attack_distance)
        {
            anim.SetBool("attacking", true);
        }
        else
        {
            anim.SetBool("attacking", false);
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("walking"))
        {
            rb.MovePosition(Vector3.MoveTowards(transform.position, myfollowpoint.position, speed * Time.fixedDeltaTime));
        }
    }
    private void Update()
    {
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.transform.position), 100f * Time.deltaTime);
    }
    public void enablecollider()
    {
        weapon.enabled = true;
    }
    public void disablecollider()
    {
        weapon.enabled = false;
    }
    public GameObject impactparticle;
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("bullet"))
        {
            Instantiate(impactparticle, other.GetContact(0).point, Quaternion.LookRotation(player.transform.position - transform.position));
            GetComponent<healthmanager>().takedamage(1);
        }
        else if (other.gameObject.CompareTag("railgunbullet"))
        {
            GetComponent<healthmanager>().takedamage(100);
            Debug.Log("duh am a trigger");
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("railgunbullet"))
        {
        }
    }
}
