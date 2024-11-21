using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTriggerZone : MonoBehaviour
{

    public bool shoveTrigger = false;
    public float shoveCooldown = 0.5f;
    private float t_shoveCooldown = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(t_shoveCooldown > 0) t_shoveCooldown -= Time.deltaTime;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(!shoveTrigger)
                transform.parent.SendMessage("AttackTriggerEnter");

            else
            {
                // do shove stuff
                PlayerController pc = collision.GetComponent<PlayerController>();
                EnemyStateMachine ec = transform.parent.GetComponent<EnemyStateMachine>();
                if (pc != null)
                {
                    if(pc.CurrentPlayerState != e_PlayerControllerStates.WallGrab &&
                        pc.playerFacingVector.x != ec.GetFacingDirection() &&
                        ec.currentState != e_EnemyStates.damageFlinch &&
                        ec.currentState != e_EnemyStates.attacking &&
                        t_shoveCooldown <= 0)
                    {
                        pc.SendMessage("TriggerKnockback", (int)ec.GetFacingDirection());
                        t_shoveCooldown = shoveCooldown;
                        Debug.Log("Shove!");
                    }
                    
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!shoveTrigger)
                transform.parent.SendMessage("AttackTriggerExit");
        }
    }

}
