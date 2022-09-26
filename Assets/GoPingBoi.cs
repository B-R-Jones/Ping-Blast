using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoPingBoi : MonoBehaviour
{
    private Vector2 pos1;
    private Vector2 pos2;
    private float timePos;
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        pos1 = new Vector2(-54.0f, 3.5f);
        pos2 = new Vector2(-26.0f, 3.5f);
        timePos = 0.0f;
        speed = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        timePos = Mathf.PingPong(Time.time * speed, 1);
        transform.position = Vector2.Lerp(pos1, pos2, timePos);
    }
}
