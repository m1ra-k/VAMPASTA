using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetryButton : MonoBehaviour
{
    public GameProgressionManager GameProgressionManager;

    void Awake()
    {
        GameProgressionManager = GameObject.Find("GameProgressionManager").GetComponent<GameProgressionManager>();
    }

    public void RetryGame()
    {
        GameProgressionManager.TransitionScene("retry");
    }
}
