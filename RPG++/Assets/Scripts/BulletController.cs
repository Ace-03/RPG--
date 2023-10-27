using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.Burst.CompilerServices;

public class BulletController : MonoBehaviourPun
{
    public int damage;
    private int attackerId;
    private bool isMine;

    public Rigidbody2D rigB;

    public void Initialize(int damage, int attackerId, bool isMine)
    {
        this.damage = damage;
        this.attackerId = attackerId;
        this.isMine = isMine;

       Destroy(gameObject, 5.0f);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        // did we hit an enemy?
        Debug.Log("Tried to shoot enemy");
        if (collision != null && collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemy was shot");
            // get the enemy and damage them
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.photonView.RPC("TakeDamage", RpcTarget.MasterClient, damage);
        }
    }
}
