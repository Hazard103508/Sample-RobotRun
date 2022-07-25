using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Player player;

    void Start()
    {
        Invoke("StarRunning", 1);
    }

    void Update()
    {
        print(player.State);
        CheckAction();

        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene("SampleScene");
    }

    private void StarRunning()
    {
        player.Run();
    }
    private void CheckAction()
    {
        if (player.State != PlayerState.Running)
            return;

        if (Input.GetKey(KeyCode.D))
            player.Move_Right();
        else if (Input.GetKey(KeyCode.A))
            player.Move_Left();
        else if (Input.GetKey(KeyCode.W))
            player.Jump();
    }
}

