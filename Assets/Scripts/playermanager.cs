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
            if (!canjump) return;
            if (isjumping && !isdiving && !isdashing)
            {
                isdiving = true;
                StartCoroutine(desolatedive());
            }
            else
            {
                inputtimer = inputtime;
                if (coyotetimer > 0)
                {
                    dojump();
                }
            }

        }
        else if (context.canceled)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f, rb.linearVelocity.z);
            coyotetimer = 0;
        }
    }
    public Material dashmat;
    public Material divemat;
    public bool canjump = true;
    public bool isdiving;
    public bool candiveattack;
    IEnumerator desolatedive()
    {
        candash = false;
        canjump = false;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        anim.SetTrigger("dive");
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(GetComponent<dashtrailplayer>().dashtrail(0.1f, 0.01f, divemat));
        rb.linearVelocity = new Vector3(0, -100, 0);
        Physics.IgnoreLayerCollision(gameObject.layer, 6, true);
        rb.useGravity = true;
        candiveattack = true;
        //isdiving = false;
        yield return new WaitForSeconds(0.5f);
        Physics.IgnoreLayerCollision(gameObject.layer, 6, true);
        canjump = true;
    }
    public void enddive()
    {
        candash = true;
        isdiving = false;
    }
    public void OnRifleAttack(InputAction.CallbackContext context)
    {
        if (!weapon1.activeSelf || knockedback || isdiving) return;
        if (context.performed)
        {
            firepressed = true;
            anim.SetBool("aiming", true);
            //anim.SetTrigger("attacking");   

            //StartCoroutine(attackcooldown());
        }
        else if (context.canceled)
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
    IEnumerator DashRoutine(float dashtime, float dashvelocity, float dashcooldown)
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
        Physics.IgnoreLayerCollision(gameObject.layer, 6, true);
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
        StartCoroutine(fovchanger(_dashzoomout, 30f));
        StartCoroutine(GetComponent<dashtrailplayer>().dashtrail(0.27f, 0.02f, dashmat));
        yield return new WaitForSeconds(dashtime);
        Physics.IgnoreLayerCollision(7, 6, false);
        rb.useGravity = true;
        isdashing = false;
        //cinecamera.Lens.FieldOfView = 80f;
        StartCoroutine(fovchanger(80f, 30f));
        yield return new WaitForSeconds(dashcooldown);
        candash = true;
    }
    public CinemachineInputAxisController inputAxisController;
    public float[] savedGains;
    public float aim_sensi_multiplier;
    public void OnAim(InputAction.CallbackContext context)
    {
        if (knockedback) return;
        var controllers = inputAxisController.Controllers;
        if (context.performed)
        {
            savedGains = new float[controllers.Count];
            for (int i = 0; i < controllers.Count; i++)
            {
                savedGains[i] = controllers[i].Input.Gain;
                if (controllers[i].Name == "Look Orbit X" || controllers[i].Name == "Look Orbit Y")
                    controllers[i].Input.Gain *= aim_sensi_multiplier;
            }
            if (!firepressed)
            {
                anim.SetBool("aiming", true);
            }
            //cinecamera.Lens.FieldOfView = 40f;
            if (fovroutine != null)
            {
                StopCoroutine(fovroutine);
            }
            fovroutine = StartCoroutine(fovchanger(40f, 40f));
        }
        if (context.canceled)
        {
            //cinecamera.Lens.FieldOfView = 80f;
            for (int i = 0; i < controllers.Count; i++)
                controllers[i].Input.Gain = savedGains[i];
            if (fovroutine != null)
            {
                StopCoroutine(fovroutine);
            }
            fovroutine = StartCoroutine(fovchanger(80f, 40f));

            if (!holdingrailgun)
            {
                anim.SetBool("aiming", false);
            }
            //StartCoroutine(fovchanger(80f));
        }
    }

    IEnumerator fovchanger(float targetlens, float changetime)
    {
        while (Mathf.Abs(cinecamera.Lens.FieldOfView - targetlens) > 0.1f)
        {
            cinecamera.Lens.FieldOfView = Mathf.Lerp(cinecamera.Lens.FieldOfView, targetlens, changetime * Time.deltaTime);
            yield return null;
        }
        cinecamera.Lens.FieldOfView = targetlens;
        fovroutine = null;
    }
    Coroutine chargeVfxRoutine;
    public void OnRailgun(InputAction.CallbackContext context)
    {
        //float initialvelocity = speed;
        if (!weapon2.activeSelf || knockedback || isdiving) return;
        if (context.started)
        {
            railgunchargevfxobj.gameObject.SetActive(true);
            chargeVfxRoutine = StartCoroutine(railgunchargevfx());
            speed *= 0.5f;
            holdingrailgun = true;
            anim.SetBool("aiming", true);
            if (chargeloop != null) StopCoroutine(chargeloop);
            chargeloop = StartCoroutine(chargingrail());
        }
        if (context.canceled)
        {
            StopCoroutine(chargeVfxRoutine);
            railgunchargevfxobj.gameObject.SetActive(false);
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
    public Transform railgunchargevfxobj;
    IEnumerator railgunchargevfx()
    {
        float elapsed = 0;
        float timebuff = 0.2f;
        while (elapsed < railguncooldown + timebuff)
        {
            elapsed += Time.deltaTime;
            railgunchargevfxobj.localScale = new Vector3(Mathf.Lerp(0,0.22f, elapsed/railguncooldown + timebuff), railgunchargevfxobj.localScale.y, railgunchargevfxobj.localScale.z);
            yield return null;
        }
        if (holdingrailgun)
        {
            chargeVfxRoutine = StartCoroutine(railgunchargevfx());
        }
    }
    IEnumerator chargingrail()
    {
        while (holdingrailgun)
        {
            //railgunchargevfxobj.gameObject.SetActive(true);
            //StartCoroutine(railgunchargevfx());
            yield return new WaitForSeconds(railguncooldown);
            if (!holdingrailgun) yield break;
            anim.SetTrigger("attacking");
        }
    }
    public void shootrailgun()
    {
        //StopCoroutine(railgunchargevfx());
        //railgunchargevfxobj.gameObject.SetActive(false);
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
        GameObject realBullet = Instantiate(railgunbullet, realbulletorigin, Quaternion.identity * Quaternion.Euler(90f, 0, 0));
        realBullet.transform.right = -ray.direction;
        Debug.DrawLine(realbulletorigin, targetpoint, Color.blue, 5f);
        realBullet.GetComponent<Rigidbody>().linearVelocity = ray.direction * 250;
        realBullet.GetComponent<railgunbulletbehaviour>().getdir(ray.direction);


        //visual bulllet
        bullet.transform.right = -(targetpoint - shootpoint.position).normalized;
        bullet.GetComponent<Rigidbody>().linearVelocity = (targetpoint - shootpoint.position).normalized * 250;
        Debug.DrawLine(shootpoint.position, targetpoint, Color.red, 5f);
        //player knockback;
        rb.AddForce(-transform.forward * 30, ForceMode.Impulse);
        //Destroy(bulletskin, 0.25f);
        //Destroy(realbullet, 0.25f);
        Destroy(bullet, 0.5f);
        Destroy(realBullet, 0.5f);
    }
    void Update()
    {
        //if(anim.GetCurrentAnimatorStateInfo(0).IsName("desolate dive"))
        //{
        //    isdiving = true;
        //}
        //else
        //{
        //    isdiving = false;
        //}
        if (knockedback || isdashing)
        {
            riflemuzzle.Stop();
            //riflemuzzle.gameObject.SetActive(false);
            return;
        }
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Ranged_2H_Shooting"))
        {
            //riflemuzzle.gameObject.SetActive(false);
            riflemuzzle.Stop();
        }
        if (Mouse.current.scroll.ReadValue() != Vector2.zero)
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
        GameObject realbullet = Instantiate(riflebullet, realBulletOrigin, Quaternion.identity);
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
        if (knockedback || isdashing || isdiving) return;
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
            else if (rb.linearVelocity.y <= 0.01f)
            {
                isjumping = false;
            }
        }
        else if (!isGrounded())
        {
            coyotetimer -= Time.deltaTime;
        }
        inputtimer -= Time.deltaTime;
    }
    public float diveradius;
    public float diveforce;
    public GameObject Expo;
    public GameObject expoimpact;
    public void DiveAttack()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, diveradius);

        foreach (Collider hit in hits)
        {
            Rigidbody hitrb = hit.attachedRigidbody;
            if (hitrb == null || hitrb == rb) continue;
            //Debug.Log(hit);
            //Debug.Log($"Hit: {hit.name}");
            //Debug.Log($"Attached RB: {hit.attachedRigidbody}");
            hitrb.AddExplosionForce(diveforce, transform.position, diveradius, 3f, ForceMode.Impulse);
            if(hit.GetComponent<healthmanager>() != null)
                hit.GetComponent<healthmanager>().takedamage(0.5f);
        }
        Expo.SetActive(true);
        expoimpact.transform.position = transform.position + new Vector3(0, 0.1f, 0);
        expoimpact.SetActive(true);
        Invoke(nameof(disablexpo), 1f);
    }
    void disablexpo()
    {
        Expo.SetActive(false);
        expoimpact.SetActive(false);
    }
    public bool isjumping;
    public void dojump()
    {
        coyotetimer = 0;
        inputtimer = 0;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jump, rb.linearVelocity.z);
        isjumping = true;
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position - transform.up * castdistance, raycastradius);

        Gizmos.color = Color.violet;
        Gizmos.DrawWireSphere(transform.position, diveradius);
    }
    public bool knockedback;
    public IEnumerator knockback(float knockbacktime, float knockbackforce, Vector3 pointofcontact)
    {
        if (isdashing || isdiving) yield break;
        //float initailspeed = speed;
        knockedback = true;
        Vector3 dir = transform.position - pointofcontact;
        dir.y = 0;
        rb.AddForce(dir.normalized * knockbackforce, ForceMode.Impulse);
        if (!holdingrailgun)
        {
            anim.SetBool("knockback", true);
        }
        //Debug.Log(rb.linearVelocity);
        //Debug.DrawLine(transform.position, pointofcontact, Color.darkCyan, 2f);
        //Debug.Log("the direction"+ dir);
        //Debug.Log("the distance between "+Vector3.Magnitude(transform.position - pointofcontact));
        yield return new WaitForSeconds(knockbacktime);
        anim.SetBool("knockback", false);
        knockedback = false;
        //speed = initailspeed;
    }
    public float expo_hit_stop;
    public float expo_cam_shake;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            if (candiveattack)
            {
                isdiving = false;
                candiveattack = false;
                StartCoroutine(hitstop(0, expo_hit_stop));
                DiveAttack();
                GetComponent<CinemachineImpulseSource>().GenerateImpulse(expo_cam_shake);
            }
        }
    }
    public IEnumerator hitstop(float multiplier, float duration)
    {
        //Debug.Log("called");
        float originalTimeScale = Time.timeScale;
        float orgiginalfixedDeltaTime = Time.fixedDeltaTime;
        Time.timeScale *= multiplier;
        Time.fixedDeltaTime *= multiplier;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = orgiginalfixedDeltaTime;
    }
}
