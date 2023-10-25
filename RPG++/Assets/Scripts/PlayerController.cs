using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    [HideInInspector]
    public int id;


    [Header("Info")]
    public float moveSpeed;
    public int gold;
    public int curHP;
    public int maxHP;
    public bool dead;
    public bool isGun;
    public float bulletSpeed;
    public Transform bulletSpawnPos;


    [Header("Attack")]
    public int damage;
    public float attackRange;
    public float attackRate;
    public float lastAttackTime;

    [Header("Components")]
    public Rigidbody2D rig;
    public Player photonPlayer;
    public SpriteRenderer sr;
    public Animator weaponAnim;
    public HeaderInfo headerInfo;
    public string bulletPrefabsPath;

    [Header("Weapons")]
    public GameObject Axe;
    public GameObject Gun;
    //public GameObject bulletObj;

    // local player
    public static PlayerController me;

    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;

        Debug.Log("Player is Initialized");
        GameManager.instance.players[id - 1] = this;

        // initialize the health bar
        headerInfo.Initialize(player.NickName, maxHP);

        if (player.IsLocal)
            me = this;
        else
            rig.isKinematic = true;
    }
    

    void Update()
    {
        // only if the local player can control this player controller
        if (!photonView.IsMine)
            return;

        Move();

        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime > attackRate)
            Attack();

        float mouseX = (Screen.width / 2) - Input.mousePosition.x;

        if (mouseX < 0)
            weaponAnim.transform.parent.localScale = new Vector3(1, 1, 1);
        else
            weaponAnim.transform.parent.localScale = new Vector3(-1, 1, 1);
    }

    void Move()
    {
        // get the horizontal and vetical inputs
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        // apply that to our velocity
        rig.velocity = new Vector2(x, y) * moveSpeed;
    }

    // melee attacks towards the mouse
    void Attack()
    {
        lastAttackTime = Time.time;


        if (!isGun)
        {
            // calculate the direction
            Vector3 dir = (Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position)).normalized;

            // shoot a raycast in the direction
            RaycastHit2D hit = Physics2D.Raycast(transform.position + dir, dir, attackRange);

            // did we hit an enemy?
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Enemy"))
            {
                // get the enemy and damage them
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                enemy.photonView.RPC("TakeDamage", RpcTarget.MasterClient, damage);
            }

            // player attack animation
            weaponAnim.SetTrigger("Attack");
        }
        else if (isGun)
        {

            Vector2 mouseX = (Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position)).normalized;

                this.photonView.RPC("SpawnBullet", RpcTarget.All, bulletSpawnPos.transform.position, new Vector3(mouseX.x,mouseX.y,0));
         

            //Bullet = PhotonNetwork.Instantiate(bulletPrefabsPath, Gun.transform.position, Gun.transform.rotation);
            //Bullet.transform.forward = dir;
        }
    }
       
    [PunRPC]
    void SpawnBullet(Vector3 pos, Vector3 dir)
    {
        // spawn and orientate it
        GameObject bulletObj = PhotonNetwork.Instantiate(bulletPrefabsPath, pos, Quaternion.identity);
        bulletObj.transform.right = dir;

        // get bullet script
        BulletController bulletScript = bulletObj.GetComponent<BulletController>();

        //initialize it and set the velocity
        bulletScript.Initialize(damage, this.id, this.photonView.IsMine);
        bulletScript.rigB.velocity = dir * bulletSpeed;
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        curHP -= damage;

        // update the health bar
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHP);

        if (curHP <= 0)
            Die();
        else
        {
            StartCoroutine(DamageFlash());

            IEnumerator DamageFlash()
            {
                sr.color = Color.red;
                yield return new WaitForSeconds(0.05f);
                sr.color = Color.white;
            }
        }
    }

    void Die()
    {
        dead = true;
        rig.isKinematic = true;

        transform.position = new Vector3(0, 99, 0);

        Vector3 spawnPos = GameManager.instance.spawnPoints[Random.Range(0, GameManager.instance.spawnPoints.Length)].position;
        StartCoroutine(Spawn(spawnPos, GameManager.instance.respawnTime));
    }

    IEnumerator Spawn(Vector3 spawnPos, float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);

        dead = false;
        transform.position = spawnPos;
        curHP = maxHP;
        rig.isKinematic = false;

        // update the health bar
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHP);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject.CompareTag("Teleporter"))
        {
            Debug.Log("Touched the teleporter");
        }
    }


    [PunRPC]
    void Heal(int amountToHeal)
    {
        curHP = Mathf.Clamp(curHP + amountToHeal, 0, maxHP);

        //update the health bar
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHP);
    }

    [PunRPC]
    void GiveGold(int goldToGive)
    {
        gold += goldToGive;

        // update the ui
        GameUI.instance.UpdateGoldText(gold);
    }

    [PunRPC]
    bool GiveGun(bool hasGun)
    {
        if (hasGun)
        {
            Axe.SetActive(false);
            Gun.SetActive(true);
            return isGun = true;
        }
        return isGun = false;
    }
}
