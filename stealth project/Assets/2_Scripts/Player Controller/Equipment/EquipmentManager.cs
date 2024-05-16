using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public int maxEquipment = 4;
    public GameObject[] equipment;
    public GameObject[] borders;
    public float boxSpacing = 1;
    public GameObject borderPrefab;

    private PlayerController playerScript;

    // Start is called before the first frame update
    void Start()
    {
        equipment = new GameObject[maxEquipment];

        playerScript = GameObject.FindWithTag("Player").
                        GetComponent<PlayerController>();
        InitSprites();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitSprites()
    {
        borders = new GameObject[maxEquipment];


        for (int i = 0; i < borders.Length; i++)
        {
            Vector3 pos = transform.position;
            pos.x = pos.x + boxSpacing * i;
            borders[i] = Instantiate(borderPrefab, pos, Quaternion.identity, transform);
        }
    }
}
