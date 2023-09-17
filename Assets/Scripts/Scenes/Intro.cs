using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("ToLobby", 3.5f);
    }

    public void ToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}
