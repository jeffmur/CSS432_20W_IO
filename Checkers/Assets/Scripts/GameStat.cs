using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStat : MonoBehaviour
{
    private Client client;

    public Text showDown;
    public string whoAmI;
    public string opponent;
    // Start is called before the first frame update
    void Awake()
    {
        if (GameManager.Instance != null)
            client = GameManager.Instance.clientObject.GetComponent<Client>();
        else
            Debug.LogError("GameManager is null at start of Game!");
    }

    private void Start()
    {
        OpponentPopUp();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpponentPopUp()
    {
        opponent = GameManager.Instance.oponentUsername;
        whoAmI = client.clientName;
        // only show for online
        if (opponent != "" && whoAmI != "")
            showDown.text = $"{whoAmI} vs {opponent}";
        else
            showDown.text = "Practice";
            
        StartCoroutine(hideAfter(showDown.transform.parent.gameObject, 2));
    }

    private IEnumerator hideAfter(GameObject toHide, int sec)
    {
        yield return new WaitForSeconds(sec);
        toHide.SetActive(false);
    }

    public void Quit()
    {
        // Close socket if online
        if(client != null)
            client.CloseSocket();
        SceneManager.LoadScene("Menu");
    }
}
