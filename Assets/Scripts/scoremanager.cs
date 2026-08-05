using TMPro;
using UnityEngine;

public class scoremanager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float score;
    public TMP_Text score_text;
    public float NoofEnemies;
    public TMP_Text NoofEnemiestext;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        score_text.text = "Score : " + score;
        NoofEnemiestext.text = "No of Enemies : " + NoofEnemies;
    }
}
