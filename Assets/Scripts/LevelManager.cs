using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private Transform player;
    [SerializeField]
    private Transform minion;

    public void Reset()
    {
        player.position = new Vector3(-51.56f, 1.7f, -41.71f);
        minion.position = new Vector3(-56.833f, 0.8f, -42.057f);

        Debug.Log("Resetando!");
    }
}
