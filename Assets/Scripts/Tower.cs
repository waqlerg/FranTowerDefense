﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public string towerType;
    public float rangeRadius;
    public float fireRate;
    public float projectileSpeed;
    public int price;

    [Space(20)]

    public int upgrade11Price;
    public int upgrade12Price;
    public int upgrade21Price;
    public int upgrade22Price;

    [Space(20)]

    public int bulletsShot;
    public LayerMask layer;

    [Space(20)]

    public GameObject ammo;

    private GameObject currentEnemy;
    private List<GameObject> enemies;

    private GameObject gameManager;
    private Upgrade upgrade;

    private GameObject rangeIndicator;
    private bool enemyWithinRange;
    private float timer;
    private float angle;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");

        rangeIndicator = transform.GetChild(1).gameObject;
        UpdateRange();

        enemies = new List<GameObject>();
        upgrade = new Upgrade();

        transform.SetParent(GameObject.Find("Towers").transform);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (enemies.Count > 0)
        {
            enemyWithinRange = true;
            currentEnemy = enemies[0];
        }
        else
        {
            enemyWithinRange = false;
        }

        if (enemyWithinRange && currentEnemy != null) //Shoot enemy
        {
            ShootEnemy(currentEnemy);
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, ~layer);
            if (hit.collider == GetComponent<CapsuleCollider2D>())
            {
                this.tag = "SelectedTower";
                ShowRange();
            }
            else if(hit.collider != null && hit.collider.tag == "Upgrade")
            {
                   // do nothing..for now
            }
            else
            {
                this.tag = "Tower";
                HideRange();
            }
        }

        UpdateRange();
    }

    void ShootEnemy(GameObject enemy)
    {
        if (timer > fireRate && GetComponent<PlaceTower>().placedTower)
        {
            Vector2 direction = PointAtEnemy(ref enemy);

            bulletsShot++;

            GameObject projectile = Instantiate(ammo);
            projectile.transform.position = transform.position;
            projectile.transform.rotation = transform.rotation;
            FlipImage(angle, projectile, projectile.transform.localScale.x);
            projectile.GetComponent<Rigidbody2D>().AddForce(direction * projectileSpeed);

            gameManager.GetComponent<GameManager>().PlayWhishNoise();

            timer = 0;
        }
    }

    Vector2 PointAtEnemy(ref GameObject enemy)
    {
        Vector3 targ = enemy.transform.position;
        targ.z = 0f;

        Vector3 objectPos = transform.position;
        targ.x = targ.x - objectPos.x;
        targ.y = targ.y - objectPos.y;

        angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;

        FlipImage(angle, gameObject, transform.localScale.x);

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        return new Vector2(targ.x, targ.y);
    }

    void FlipImage(float angle, GameObject obj, float scale)
    {
        if(angle < -90 || angle > 90)
        {
            obj.transform.localScale = new Vector2(scale, -scale);
        }
        else
        {
            obj.transform.localScale = new Vector2(scale, scale);
        }
    }

    void UpdateRange()
    {
        rangeIndicator.transform.localScale = new Vector3(rangeRadius * 2, rangeRadius * 2, 0);
        if (GetComponent<PlaceTower>().placedTower)
        {
            transform.GetComponentInChildren<CircleCollider2D>().radius = rangeRadius;
        }
    }

    void ShowRange()
    {
        rangeIndicator.SetActive(true);
    }

    void HideRange()
    {
        rangeIndicator.SetActive(false);
    }

    public Upgrade GetUpgradeInstance()
    {
        return upgrade;
    }

    public void SetRange(float range)
    {
        rangeRadius = range;
    }

    public void SetFireRate(float fireRate)
    {
        this.fireRate = fireRate;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Enemy")
        {
            enemies.Add(collider.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "Enemy" && enemies.Count > 0)
        {
            enemies.RemoveAt(0);
        }
    }
}