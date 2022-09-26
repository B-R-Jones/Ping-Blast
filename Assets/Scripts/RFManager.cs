using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RFManager : MonoBehaviour
{
    public float lifeTimer;
    // Start is called before the first frame update
    void Start()
    {
        lifeTimer = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (lifeTimer < 0) { Destroy(gameObject); } else { lifeTimer -= Time.deltaTime; }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.transform.GetComponent<PlayerController>().rapidFireOn = true;
                collision.transform.GetComponent<PlayerController>().shotTimer = 0.25f;
            }

            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.transform.GetComponent<FireControl>().rapidFireOn = true;
                collision.transform.GetComponent<FireControl>().shotTimer = 0.25f;
            }

            Destroy(gameObject);
        }
    }
}
