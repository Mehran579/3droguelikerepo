using System.Collections;
using TMPro;
using UnityEngine;

public class wave : MonoBehaviour
{
    public int WaveNumber;
    public GameObject[] enemies;
    float WaveInterval;
    public Transform player;
    public TMP_Text currentwaveshow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        WaveInterval = Time.time;
        StartCoroutine(Wave1());
        WaveNumber = 1;
    }

    // Update is called once per frame
    void Update()
    {
        currentwaveshow.text = "Current Wave : "+ WaveNumber;
    }
    IEnumerator Wave1()
    {
        for(int i = 0;  i < tinycubic(WaveNumber); i++)
        {
            Vector3 Spawnpos = new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30));
            if(Vector3.Distance(Spawnpos, player.position)> 5)
            {
                GameObject enemy = Instantiate(enemies[0], Spawnpos, Quaternion.identity);
                yield return new WaitForSeconds(Random.Range(2,4));
            }
        }
        yield return new WaitForSeconds(10);
        WaveNumber = 2;
        StartCoroutine(Wave2());
    }
    IEnumerator Wave2() 
    {
        for (int i = 0; i < tinycubic(WaveNumber); i++)
        {
            Vector3 Spawnpos = new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30));
            if(Vector3.Distance(Spawnpos, player.position)> 5)
            {
                GameObject enemy = Instantiate(enemies[Random.Range(0,4)], Spawnpos, Quaternion.identity);
                yield return new WaitForSeconds(Random.Range(0.75f,1.5f));
            }
        }
        yield return new WaitForSeconds(10);
        WaveNumber = 3;
        StartCoroutine(wave3());
    }
    IEnumerator wave3()
    {
        for (int i = 0; i < tinycubic(WaveNumber); i++)
        {
            Vector3 Spawnpos = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
            if (Vector3.Distance(Spawnpos, player.position) > 5)
            {
                GameObject enemy = Instantiate(enemies[Random.Range(0, 5)], Spawnpos, Quaternion.identity);
                yield return new WaitForSeconds(0.5f);
            }
        }
        yield return new WaitForSeconds(20);
        WaveNumber += 1;
        StartCoroutine(THEWAVE());
    }
    IEnumerator THEWAVE()
    {
        for(int i = 0;i < tinycubic(WaveNumber);i++)
        {
            Vector3 spawnpos = new Vector3(Random.Range(-75, 75), 0, Random.Range(-75, 75));
            if (Vector3.Distance(spawnpos, player.position) > 5)
            {
                GameObject enemy = Instantiate(enemies[Random.Range(0,5)],spawnpos, Quaternion.identity);
                enemy.GetComponent<enemy_manager>().speed += slowingpendulum(WaveNumber) / 2;
                //hit _enemyhit = enemy.GetComponent<enemy_manager>()._hit; //+= softhorizon(WaveNumber) / 2;
                //_enemyhit.damageamount += softhorizon(WaveNumber) / 2;
                enemy.GetComponentInChildren<hit>().damageamount += softhorizon(WaveNumber) / 2;
                enemy.GetComponent<healthmanager>().health += centimeterruler(WaveNumber);
                yield return new WaitForSeconds(Random.Range(0.3f,1.5f));
            }
        }
            yield return new WaitForSeconds(75f);
        WaveNumber += 1;
        StartCoroutine(THEWAVE());
    }
    float tinycubic(float wavenumber)
    {
        //Debug.Log("current wave no is "+ WaveNumber + " and no of enemies to spawn is " + (((wavenumber - 4) * wavenumber + 5) * wavenumber + 1));
        return ((0.002f * wavenumber + 0.05f) * wavenumber + 1f) * wavenumber + 1f;
    }
    float centimeterruler(float wavenumber)
    {
        return (0.05f * wavenumber * wavenumber) + (0.1f * wavenumber) + 2f;
    }
    float slowingpendulum(float wavenumber)
    {
        return (2.5f * Mathf.Log(wavenumber)) + 2f;
    }
    float softhorizon(float wavenumber)
    {
        return (0.8f * wavenumber) + Mathf.Sqrt(wavenumber);
    }
}
