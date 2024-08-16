using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationTrigger : MonoBehaviour
{

    public bool attackTrigger = false;
    public bool footstepTrigger = false;
    public bool footstepPrev = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (attackTrigger)
        {
            attackTrigger = false;
            transform.parent.gameObject.SendMessage("TriggerAttack");
            
        }
        if (footstepTrigger && !footstepPrev)
        {
            footstepPrev = true;
            transform.parent.gameObject.SendMessage("TriggerFootstep");
        }
        else if (!footstepTrigger)
        {
            footstepPrev = false;
        }
    }
}
