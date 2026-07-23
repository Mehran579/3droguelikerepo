using System.Collections;
using UnityEngine;

public class wave : MonoBehaviour
{
    public int WaveNumber;
    public GameObject[] enemies;
    float WaveInterval;
    public Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        WaveInterval = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(WaveInterval < Time.time)
        {
            WaveInterval += 75f;
            WaveNumber += 1;
        }
    }
    IEnumerator Wave()
    {
        while (true)
        {
            if (WaveNumber == 1)
            {

            }
        }
    }

    IEnumerator enemyspawner(float noofenemies,float spawnInterval, Vector3 minspawn, Vector3 maxspawn)
    {
        if (WaveNumber == 1) 
        {
            for (int i = 0; i < noofenemies; i++)
            {

            }
            Vector3 spawnlocation = new Vector3(player.position.x, 0, player.position.z);
            if(Vector3.Distance(player.position,spawnlocation) > 6)
            {
                GameObject enemy = Instantiate(enemies[0], player.position, Quaternion.identity);
                yield return null;
            }
        }
        for (int i = 0; i < noofenemies; i++)
        {
            GameObject enemy = Instantiate
            yield return new WaitForSeconds(spawnInterval);

        }
    }
}
