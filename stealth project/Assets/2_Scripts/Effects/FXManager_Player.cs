using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager_Player : MonoBehaviour
{

    public bool spawn_footDust = false;
    private bool last_footDust = false;
    [SerializeField]
    private GameObject footStepDust;
    [SerializeField]
    private GameObject footDustPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!spawn_footDust) last_footDust = false;

        if (spawn_footDust && !last_footDust)
        {
            spawn_footDust = false;
            last_footDust = true;
            GameObject dust = Instantiate(footStepDust, footDustPos.transform.position, Quaternion.identity);
            dust.transform.localScale = this.transform.localScale;
        }


    }
}
