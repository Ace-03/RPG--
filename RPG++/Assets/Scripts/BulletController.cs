using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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

        // calculate the direction
        //Vector3 dir = (Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position)).normalized;

        // shoot a raycast in the direction
        //RaycastHit2D hit = Physics2D.Raycast(transform.position + this.transform.position, this.transform.position);

        // did we hit an enemy?
        Debug.Log("Tried to shoot enemy");
        if (collision.gameObject.name == "Enemy(Clone)")
        {
            Debug.Log("Enemy was shot");
            // get the enemy and damage them
            Enemy enemy = GetComponent<Enemy>();
            enemy.photonView.RPC("TakeDamage", RpcTarget.MasterClient, damage);
        }
    }
    
}
