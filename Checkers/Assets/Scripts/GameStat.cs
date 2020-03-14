using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStat : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Quit()
    {
        // Close socket if online
        if(GameManager.Instance != null)
        {
            GameManager.Instance.clientObject.GetComponent<Client>().CloseSocket();
        }
        SceneManager.LoadScene("Menu");
    }
}
