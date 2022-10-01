using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadShotPowerupController : MonoBehaviour
{
    // Entity attributes
    public float lifeTimer;

    // Start is called before the first frame update
    void Start()
    {
        LoadPlaySettings();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        Decay();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.transform.GetComponent<PlayerController>().spreadShotOn = true;
                collision.transform.GetComponent<PlayerController>().shotNumber = 3;
            }

            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.transform.GetComponent<EnemyController>().spreadShotOn = true;
                collision.transform.GetComponent<EnemyController>().shotNumber = 3;
            }

            Destroy(gameObject);
        }
    }

    private void LoadPlaySettings()
    {
        lifeTimer = 5.0f;
    }

    private void Decay()
    {
        if (lifeTimer < 0) { Destroy(gameObject); } else { lifeTimer -= Time.deltaTime; }
    }
}
