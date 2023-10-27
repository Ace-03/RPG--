using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Teleport : MonoBehaviourPun
{
    [Header("Info")]
    public Transform teleporterTarget;
    public float teleportBuffer;

    [Header("Components")]
    public PlayerController player;

    private float lastTeleport;


    void OnTriggerEnter2D(Collider2D collision)
    {


        if (Time.time - lastTeleport < teleportBuffer)
            return;

        lastTeleport = Time.time;

        Debug.Log("Player Touched the teleporter");
        player = GameManager.instance.GetPlayer(collision.gameObject);
        player.transform.position = teleporterTarget.transform.position - new Vector3(2,0,0);
    }
}
