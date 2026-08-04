using UnityEngine;
using UnityEngine.SceneManagement;

public class game_load : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public GameObject loading_panel;
    public void change()
    {
        loading_panel.SetActive(true);
        SceneManager.LoadScene(1);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
