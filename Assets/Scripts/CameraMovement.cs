using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject Player;
    public bool FollowX;
    public bool FollowY;
    [Header("Offset")]
    public float XOffset;
    public float YOffset;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (Player == null) return;

        if (FollowX)
        {
            Vector3 position = transform.position;
            position.x = Player.transform.position.x + XOffset;
            transform.position = position;
        }

        if (FollowY)
        {
            Vector3 position = transform.position;
            position.y = Player.transform.position.y + YOffset;
            transform.position = position;
        }
    }
}

