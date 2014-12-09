using UnityEngine;
using System.Collections;

public class LobbyManager : MonoBehaviour {

	void Start () {
	}

    public void LoadGame1()
    {
        Application.LoadLevel("Game2");
    }
    
    public void LoadGame2()
    {
        Application.LoadLevel("Game3");
    }

    public void LoadGame3()
    {
        Application.LoadLevel("Game");
    }
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
	}
}
