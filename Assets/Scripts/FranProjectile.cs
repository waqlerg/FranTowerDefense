﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FranProjectile : MonoBehaviour
{
    public int damage;
    private GameObject gameManager;

    void Start()
    {
        StartCoroutine(DestroyProjectile());
        gameManager = GameObject.FindGameObjectWithTag("GameController");
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Enemy")
        {
            RemoveEnemyHealth(collider);
            UpdateEnemySpeed(collider);

            GetComponent<BoxCollider2D>().enabled = false;
            gameManager.GetComponent<GameManager>().PlayPopNoise(collider.gameObject);

            Destroy(gameObject);
        }
    }
    
    void UpdateEnemySpeed(Collider2D collider)
    {
        collider.GetComponent<Enemy>().speed -= 0.5f;
    }

    void RemoveEnemyHealth(Collider2D collider)
    {
        collider.GetComponent<Enemy>().health -= damage;
    }

    IEnumerator DestroyProjectile()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}