using System.Collections;
using UnityEngine;

public class rail_gun_ripples : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    float rippleactivetime;
    public GameObject shockwave;
    void Start()
    {
        rippleactivetime = Time.time + 2f;
        StartCoroutine(shockwaveripples());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public string shadervarref;
    public float rate;
    public float refreshrate;
    public float duration;
    IEnumerator shockwaveripples()
    {
        while (Time.time < rippleactivetime)
        {
            GameObject ripple = Instantiate(shockwave,transform.position, transform.rotation);
            Material mat = ripple.GetComponent<Renderer>().material;
            StartCoroutine(fade(mat, 0, duration));
            Destroy(ripple, 0.2f);
            yield return new WaitForSeconds(0.001f);
        }
    }
    IEnumerator fade(Material mat, float goal, float duration)
    {
        Debug.Log("called");
        float start = mat.GetFloat(shadervarref);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float valuetofade = Mathf.Lerp(start, goal, elapsed / duration);
            mat.SetFloat(shadervarref, valuetofade);
            Debug.Log("fading");
            yield return null;
        }
        mat.SetFloat(shadervarref, goal);
    }
    //IEnumerator fade(Material mat, float goal, float rate, float refreshrate)
    //{
    //    Debug.Log(mat.GetFloat(shadervarref));
    //    float valuetofade = mat.GetFloat(shadervarref);
    //    while (valuetofade > goal)
    //    {
    //        valuetofade -= rate;
    //        mat.SetFloat(shadervarref, valuetofade);
    //        yield return new WaitForSeconds(refreshrate);
    //    }
    //}
}
