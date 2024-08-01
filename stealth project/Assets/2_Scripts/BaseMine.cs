using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMine : MonoBehaviour
{

    public Mine so_mine;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Setup()
    {
        if(so_mine.sprite != null)
            GetComponent<SpriteRenderer>().sprite = so_mine.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider other)
    {

    }
}
