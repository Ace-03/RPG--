using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletController : MonoBehaviourPun
{
    private int damage;
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
    void Update()
    {
        Debug.Log("Tried to shoot enemy");
        // calculate the direction
        Vector3 dir = (this.transform.position);

        // shoot a raycast in the direction
        RaycastHit2D hit = Physics2D.Raycast(transform.position + dir, dir);

        // did we hit an enemy?
        if (hit.collider != null && hit.collider.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemy was shot");
            // get the enemy and damage them
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            enemy.photonView.RPC("TakeDamage", RpcTarget.MasterClient, damage);
        }
    }
}
