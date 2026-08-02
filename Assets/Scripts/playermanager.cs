using JetBrains.Annotations;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class playermanager : MonoBehaviour
{
    Rigidbody rb;
    Animator anim;
    public float jump;
    public float speed;
    public Transform shootpoint;
    public GameObject railgunbullet;
    public GameObject railgunbulletskin;
    public GameObject riflebullet;
    public GameObject riflebulletskin;
    public LayerMask aimMask;
    Vector2 MoveInput;
    public GameObject weapon1;
    public GameObject weapon2;
    public CinemachineCamera cinecamera;
    bool firepressed;
    Coroutine fovroutine;
    public bool holdingrailgun;
    Coroutine chargeloop;
    float nextfiretime;
    public float firerate;
    //float yaw;
    //bool canattack = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        weapon1.SetActive(true);
        weapon2.SetActive(false);
        //riflemuzzle.gameObject.SetActive(false);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if ((knockedback))
        {
            return;
        }
        if (context.performed)
        {
            if (!holdingrailgun)
            {
                anim.SetBool("running", true);
            }
        }
        else
        {
            anim.SetBool("running", false);
        }
        MoveInput = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (knockedback) return;
        if (context.performed)
        {
            //Debug.Log($"Jump pressed. coyotetimer: {coyotetimer}");
            inputtimer = inputtime;
            if (coyotetimer > 0)
            {
                dojump();
            }
            //playerrb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);

        }
        else if (context.canceled)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f,rb.linearVelocity.z);
            //playerrb.AddForce(Vector2.up * jump * 0.5f, ForceMode2D.Impulse);
            coyotetimer = 0;
        }
    }
    public void OnRifleAttack(InputAction.CallbackContext context)
    {
        if (!weapon1.activeSelf || knockedback) return;
        if (context.performed)
        {
            firepressed = true;
            anim.SetBool("aiming", true);
            //anim.SetTrigger("attacking");   

            //StartCoroutine(attackcooldown());
        }else if (context.canceled)
        {
            anim.SetBool("shooting", false);
            anim.SetBool("aiming", false);
            firepressed = false;
        }
    }
    public bool isdashing;
    public bool candash;
    public float _Dashtime;
    public float _dashvelocity;
    public float _dashcooldown;
    public float _dashzoomout;
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && !isdashing && candash)
        {
            StartCoroutine(DashRoutine(_Dashtime, _dashvelocity, _dashcooldown));
        }
    }
    IEnumerator DashRoutine(float dashtime,float dashvelocity, float dashcooldown)
    {
        candash = false;
        isdashing = true;
        Vector3 lockedf = transform.forward;
        Vector3 lockedr = transform.right;
        //cinecamera.Lens.FieldOfView = 120f;
        if (MoveInput.y > 0 || MoveInput == Vector2.zero)
            anim.SetTrigger("df");
        else if (MoveInput.y < 0)
            anim.SetTrigger("db");
        else if (MoveInput.x > 0)
            anim.SetTrigger("dr");
        else
            anim.SetTrigger("dl");
        yield return new WaitForSeconds(0.024f);
        rb.useGravity = false;
        Physics.IgnoreLayerCollision(gameObject.layer , 6, true);
        Vector3 dashdirection;
        if (MoveInput == Vector2.zero)
        {
            dashdirection = lockedf;
        }
        else
        {
            dashdirection = (lockedr * MoveInput.x + lockedf * MoveInput.y).normalized;
        }
        rb.linearVelocity = new Vector3(dashdirection.x * dashvelocity, rb.linearVelocity.y, dashdirection.z * dashvelocity);
        StartCoroutine(fovchanger(_dashzoomout,30f));
        StartCoroutine(GetComponent<dashtrailplayer>().dashtrail());
        yield return new WaitForSeconds(dashtime);
        Physics.IgnoreLayerCollision(7 , 6, false);
        rb.useGravity = true;
        isdashing = false;
        //cinecamera.Lens.FieldOfView = 80f;
        StartCoroutine(fovchanger(80f, 30f));
        yield return new WaitForSeconds(dashcooldown);
        candash = true;
    }
    public void OnAim(InputAction.CallbackContext context)
    {
        if (knockedback) return;
        if (context.performed)
        {
            if (!firepressed)
            {
                anim.SetBool("aiming", true);
            }
            //cinecamera.Lens.FieldOfView = 40f;
            if(fovroutine != null)
            {
                StopCoroutine(fovroutine);
            }
            fovroutine = StartCoroutine(fovchanger(40f,40f));
        }
        if(context.canceled)
        {
            //cinecamera.Lens.FieldOfView = 80f;
            if(fovroutine != null)
            {
                StopCoroutine(fovroutine);
            }
            fovroutine = StartCoroutine(fovchanger(80f,40f));

            if (!holdingrailgun)
            {
                anim.SetBool("aiming", false);
            }
            //StartCoroutine(fovchanger(80f));
        }
    }
    
    IEnumerator fovchanger(float targetlens,float changetime)
    {
        while(Mathf.Abs(cinecamera.Lens.FieldOfView - targetlens) > 0.1f)
        {
            cinecamera.Lens.FieldOfView = Mathf.Lerp(cinecamera.Lens.FieldOfView, targetlens, changetime * Time.deltaTime);
            yield return null;
        }
        cinecamera.Lens.FieldOfView = targetlens;
        fovroutine = null;
    }
    
    public void OnRailgun(InputAction.CallbackContext context)
    {
        //float initialvelocity = speed;
        if (!weapon2.activeSelf || knockedback) return;
        if (context.started)
        {
            speed *= 0.5f;
            holdingrailgun = true;
            anim.SetBool("aiming", true);
            if (chargeloop != null) StopCoroutine(chargeloop);
            chargeloop = StartCoroutine(chargingrail());
        }
        if (context.canceled)
        {
            speed *= 2;
            holdingrailgun = false;
            anim.SetBool("aiming", false);
            if (chargeloop != null)
            {
                StopCoroutine(chargeloop);
                chargeloop = null;
            }
        }
    }
    public float railguncooldown;
    IEnumerator chargingrail()
    {
        while (holdingrailgun)
        {
            yield return new WaitForSeconds(railguncooldown);
            if (!holdingrailgun) yield break;
            anim.SetTrigger("attacking");
        }
    }
    public void shootrailgun()
    {
        GameObject bullet = Instantiate(railgunbulletskin, shootpoint.position, shootpoint.rotation);
        Vector3 targetpoint;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, aimMask))
        {
            targetpoint = hit.point;
            //Debug.Log(hit.collider.name);
        }
        else
        {
            targetpoint = ray.GetPoint(1000);
        }


        //real bullet
        Vector3 realbulletorigin = ray.origin + ray.direction * 0.5f;
        Quaternion aimrotation = Quaternion.FromToRotation(Vector3.up, ray.direction);
        GameObject realBullet = Instantiate(railgunbullet, realbulletorigin, Quaternion.identity * Quaternion.Euler(90f,0,0));
        realBullet.transform.right = -ray.direction;
        Debug.DrawLine(realbulletorigin, targetpoint, Color.blue, 5f);
        realBullet.GetComponent<Rigidbody>().linearVelocity = ray.direction * 500;
        realBullet.GetComponent<railgunbulletbehaviour>().getdir(ray.direction);


        //visual bulllet
        bullet.transform.right = -(targetpoint - shootpoint.position).normalized;
        bullet.GetComponent<Rigidbody>().linearVelocity = (targetpoint - shootpoint.position).normalized * 500;
        Debug.DrawLine(shootpoint.position, targetpoint, Color.red, 5f);
        //player knockback;
        rb.AddForce(-transform.forward * 30, ForceMode.Impulse);
        //Destroy(bullet, 0.5f);
        //Destroy(realBullet,0.5f);
    }
    void Update()
    {
        if (knockedback || isdashing) 
        {
            riflemuzzle.Stop();
            //riflemuzzle.gameObject.SetActive(false);
            return;
        }
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Ranged_2H_Shooting"))
        {
            //riflemuzzle.gameObject.SetActive(false);
            riflemuzzle.Stop();
        }
        if (Mouse.current.scroll.ReadValue()!= Vector2.zero)
        {
            if (weapon1.activeInHierarchy)
            {
                weapon1.SetActive(false);
                weapon2.SetActive(true);
            }
            else
            {
                weapon2.SetActive(false);
                weapon1.SetActive(true);
            }
        }
        if (firepressed)
        {
            tryfire();
        }
    }
    void tryfire()
    {
        if (Time.time < nextfiretime) return;
        //shoot();
        nextfiretime = Time.time + firerate;
        //anim.SetTrigger("attacking");
        shootrifle();
        anim.SetBool("shooting", true);
    }
    public ParticleSystem riflemuzzle;
    public void shootrifle()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle_B")) return;
        if (!riflemuzzle.isPlaying)
        {
            riflemuzzle.Play();
        }
        //if (!riflemuzzle.gameObject.activeSelf)
        //{
        //    riflemuzzle.gameObject.SetActive(true);
        //}
        GameObject bulletskin = Instantiate(riflebulletskin, shootpoint.position, shootpoint.rotation);
        Vector3 targetpoint;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, aimMask))
        {
            targetpoint = hit.point;
            Debug.Log(hit.collider.name);
        }
        else
        {
            targetpoint = ray.GetPoint(1000);
        }
        //realbullet
        Vector3 realBulletOrigin = ray.origin + ray.direction * 0.5f;
        GameObject realbullet = Instantiate(riflebullet,realBulletOrigin,Quaternion.identity);
        realbullet.GetComponent<Rigidbody>().linearVelocity = (targetpoint - realBulletOrigin).normalized * 300f;
        Debug.DrawLine(realBulletOrigin, targetpoint, Color.pink, 5f);
        //visual bullet;
        Debug.DrawLine(shootpoint.position, targetpoint, Color.red, 10f);
        bulletskin.transform.right = -(targetpoint - shootpoint.position).normalized;
        bulletskin.GetComponent<Rigidbody>().linearVelocity = (targetpoint - shootpoint.position).normalized * 300;
        Destroy(bulletskin, 0.25f);
        Destroy(realbullet, 0.25f);
        //player knockback;
        rb.AddForce(-transform.forward * 10, ForceMode.Impulse);
    }
    private void FixedUpdate()
    {
        if (knockedback || isdashing) return;
        Vector3 camf = Camera.main.transform.forward;
        camf.y = 0;
        camf.Normalize();
        Vector3 camr = Camera.main.transform.right;
        camr.y = 0;
        camr.Normalize();
        Vector3 move = camf * MoveInput.y + camr * MoveInput.x;
        rb.linearVelocity = new Vector3(move.x * speed, rb.linearVelocity.y, move.z * speed);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(camf), 15f * Time.fixedDeltaTime);
        if (isGrounded())
        {
            coyotetimer = coyotetime;
            if (inputtimer > 0)
            {
                dojump();
            }
        }
        else if (!isGrounded())
        {
            coyotetimer -= Time.deltaTime;
        }
        inputtimer -= Time.deltaTime;
    }
    public void dojump()
    {
        coyotetimer = 0;
        inputtimer = 0;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jump, rb.linearVelocity.z);
    }
    public float coyotetime;
    float coyotetimer;
    public float inputtime;
    float inputtimer;
    public float raycastradius;
    public float castdistance;
    public LayerMask groundlayer;
    public bool isGrounded()
    {
        if (Physics.CheckSphere(transform.position - transform.up * castdistance, raycastradius, groundlayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position - transform.up * castdistance, raycastradius);
    }
    public bool knockedback;
    public IEnumerator knockback(float knockbacktime, float knockbackforce,Vector3 pointofcontact)
    {
        if (isdashing) yield break;
        //float initailspeed = speed;
        knockedback = true;
        Vector3 dir = transform.position - pointofcontact;
        dir.y = 0;
        rb.AddForce(dir.normalized * knockbackforce, ForceMode.Impulse);
        anim.SetBool("knockback",true);
        //Debug.Log(rb.linearVelocity);
        //Debug.DrawLine(transform.position, pointofcontact, Color.darkCyan, 2f);
        //Debug.Log("the direction"+ dir);
        //Debug.Log("the distance between "+Vector3.Magnitude(transform.position - pointofcontact));
        yield return new WaitForSeconds(knockbacktime);
        anim.SetBool("knockback",false);
        knockedback = false;
        //speed = initailspeed;
    }

}
