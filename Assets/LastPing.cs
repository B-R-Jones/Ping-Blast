using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LastPing : MonoBehaviour
{
    private GameObject manager;
    public TextMeshPro myText;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameController");
        myText = gameObject.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        myText.text = "LAST PING: " + manager.GetComponent<ManagerScript>().lastScore;
    }
}
