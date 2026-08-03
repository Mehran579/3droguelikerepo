using System.Collections;
using UnityEngine;

public class dashtrailplayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public float activetime;
    bool istrailactive;
    public float meshrefreshrate;
    public float meshdestroydelay;
    public Material mat;
    SkinnedMeshRenderer[] skinnedMeshRenderers;
    public IEnumerator dashtrail(float activetimegiven,float refreshrategiven,Material matgiven)
    {
        activetime = activetimegiven;
        meshrefreshrate = refreshrategiven;
        mat = matgiven;
        if (istrailactive) yield break;
        istrailactive = true;
        float tospawntime = activetime;
        while(tospawntime> 0)
        {
            tospawntime -= meshrefreshrate;
            if(skinnedMeshRenderers == null)
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                GameObject obj = new GameObject();
                obj.transform.SetPositionAndRotation(skinnedMeshRenderers[i].transform.position, skinnedMeshRenderers[i].transform.rotation);
                MeshRenderer mr =  obj.AddComponent<MeshRenderer>();
                MeshFilter mf = obj.AddComponent<MeshFilter>();
                Mesh mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);
                mf.mesh = mesh;
                mr.material = mat;
                StartCoroutine(fade(mr.material,0,shaderfaderate, shaderfaderefreshrate));
                Destroy(obj, meshdestroydelay); 
            }
            yield return new WaitForSeconds(meshrefreshrate);
        }
        istrailactive = false;
    }
    public string shadervarref;
    public float shaderfaderate;
    public float shaderfaderefreshrate;
    IEnumerator fade(Material mat,float goal, float rate, float refreshrate)
    {
        float valuetofade = mat.GetFloat(shadervarref);
        while(valuetofade > goal)
        {
            valuetofade -= rate;
            mat.SetFloat(shadervarref, valuetofade); 
            yield return new WaitForSeconds(refreshrate);
            //Debug.Log('c');
            //Debug.Log(valuetofade);
        }
    }
}
