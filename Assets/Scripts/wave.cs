using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class wave : MonoBehaviour
{
    public int WaveNumber;
    public GameObject[] enemies;
    float WaveInterval;
    public Transform player;
    public TMP_Text currentwaveshow;
    public scoremanager scoremanager;
    [Header("Multiplier")]
    public TMP_Text speedx;
    public TMP_Text damagex;
    public TMP_Text healthx;
    public bool check;
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
        if (check)
        {
            if(scoremanager.NoofEnemies <= 0)
            {
                check = false;
                WaveNumber += 1;
                StartCoroutine(THEWAVE());
            }
        }
    }
    IEnumerator Wave1()
    {
        for(int i = 0;  i < tinycubic(WaveNumber); i++)
        {
            Vector3 Spawnpos = new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30));
            if(Vector3.Distance(Spawnpos, player.position)> 5)
            {
                GameObject enemy = Instantiate(enemies[0], Spawnpos, Quaternion.identity);
                scoremanager.NoofEnemies++;
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
                scoremanager.NoofEnemies++;
                yield return new WaitForSeconds(Random.Range(0.75f,1.2f));
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
                scoremanager.NoofEnemies++;
                yield return new WaitForSeconds(Random.Range(0.5f, 1f));
            }
        }
        yield return new WaitForSeconds(20);
        WaveNumber += 1;
        StartCoroutine(THEWAVE());
    }
    IEnumerator THEWAVE()
    {
        speedx.text = "  Speed : " + (slowingpendulum(WaveNumber) / 2).ToString("F2");
        damagex.text = "  Damage : " + (softhorizon(WaveNumber) / 2).ToString("F2");
        healthx.text = "  Health : " + centimeterruler(WaveNumber).ToString("F2");
        for (int i = 0;i < tinycubic(WaveNumber);i++)
        {
            Vector3 spawnpos = new Vector3(Random.Range(-75, 75), 0, Random.Range(-75, 75));
            if (Vector3.Distance(spawnpos, player.position) > 5)
            {
                GameObject enemy = Instantiate(enemies[Random.Range(0,5)],spawnpos, Quaternion.identity);
                enemy.GetComponent<enemy_manager>().speed += slowingpendulum(WaveNumber) / 2;
                enemy.GetComponentInChildren<hit>().damageamount += softhorizon(WaveNumber) / 2;
                enemy.GetComponent<healthmanager>().health += centimeterruler(WaveNumber);
                scoremanager.NoofEnemies++;
                yield return new WaitForSeconds(Random.Range(0.3f,1.2f));
            }
        }
        check = true;
        //yield return new WaitForSeconds(wavetime(WaveNumber));
        //WaveNumber += 1;
        //StartCoroutine(THEWAVE());
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
    float wavetime(float wavenumber)
    {
        return Mathf.FloorToInt(1.2f * wavenumber * wavenumber + 1.7f * wavenumber + 6.2f);
    }
}
