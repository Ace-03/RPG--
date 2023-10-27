using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Teleport : MonoBehaviourPun
{
    [Header("Components")]
    public PlayerController player;
    public Transform teleporterTarget;


    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player Touched the teleporter");
        player = GameManager.instance.GetPlayer(collision.gameObject);
        player.transform.position = teleporterTarget.transform.position;
    }
}
