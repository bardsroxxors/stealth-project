using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum e_PlayerAttacks
{
    ThreeHitCombo,
    ChargeUp,
    HeavySlash,
    Kick,
    None
}


public class PlayerAttackManager : MonoBehaviour
{
    /*
    private PlayerController playerController;
    public e_PlayerAttacks currentAttack = e_PlayerAttacks.None;

    [Header("General")]
    // can attack flag
    public bool FlagCanMeleeAttack = true;

    [Header("Three Hit Combo")]
    public GameObject SwordSwingObject;
    private SwordHitboxScript swordHitboxScript;
    // init flag
    private bool InitFlag3Hit = false;
    public bool FlagNewAttackInput = false;
    
    public int maxConsecutive = 3;
    public int currentConsecutive = 0;
    // timer to go through lifetime of a swing
    private float TimerThreeSwingLifespan = 0;
    // Lifespan for each swing
    public float threeSlashUptime = 0.3f;
    // min time to wait before next attack
    public float comboMinTime = 0.4f;
    // time to enter next input before chain is dropped
    public float comboChainTime = 0.8f;
    // cooldown timer
    [Tooltip("Cooldown after attacks have finished to be able to attack again")]
    public float meleeAttackCooldownTime = 0.1f;
    private float TimerMeleeAttackCooldown = 0;
    // other stuff
    public float hitboxPositionOffset = 2;
    public float attackMoveSpeed = 1;
    public float attackMoveDecay = 0.1f;

    // i want the kick to be from a dash or a ziptotarget and the player sort of flies
    // in that direction for a sec and boots the first thing they hit
    [Header("Kick")]
    public float kickMoveSpeed = 3;
    public float kickUptime = 0.25f;
    private float TimerKickUptime = 0;
    private bool InitFlagKick = false;
    private Vector2 kickDirection = Vector2.zero;


    [Header("Charge Up and Heavy Slash")]
    public float minChargeTime = 0.8f;
    public bool FlagChargeReleaseInput = false;
    public bool FlagHoldAttackTriggered = false;
    public bool InitFlagChargeUp = false;
    private bool InitFlagHeavySlash = false;
    public float heavyMaxConsecutive = 2;
    public float heavySlashCooldown = 0.8f;
    private float TimerHeavySlashLifetime = 0;
    private float TimerChargeUp = 0;
    public float heavySlashUptime = 0.3f;
    // min time before next attack
    public float heavyComboMinTime = 0.2f;
    // time before combo is dropped
    public float heavyComboChainTime = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        swordHitboxScript = SwordSwingObject.GetComponent<SwordHitboxScript>();
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TimerMeleeAttackCooldown > 0) TimerMeleeAttackCooldown -= Time.deltaTime;
        else FlagCanMeleeAttack = true;
    }

    public void ProcessAttack()
    {
        switch (currentAttack)
        {
            case e_PlayerAttacks.ThreeHitCombo:
                ProcessThreeHit();
                break;
            case e_PlayerAttacks.ChargeUp:
                ProcessChargeUp();
                break;
            case e_PlayerAttacks.HeavySlash:
                ProcessHeavySlash();
                break;
            case e_PlayerAttacks.Kick:
                ProcessKick();
                break;
        }
    }

    public void ProcessThreeHit()
    {
        if(FlagNewAttackInput && TimerThreeSwingLifespan < threeSlashUptime) FlagNewAttackInput = false;

        // Initialise state
        if(currentConsecutive < maxConsecutive)
        {
            if (!InitFlag3Hit || (FlagNewAttackInput && TimerThreeSwingLifespan > comboMinTime))
            {
                InitFlag3Hit = true;
                FlagNewAttackInput = false;
                SwordSwingObject.SetActive(true);
                TimerThreeSwingLifespan = 0;

                currentConsecutive += 1;

                Vector2 aimStick = playerController.GetVector2Input("aimStick");

                // Set the position of the hitbox
                //if (aimStick == Vector2.zero) aimStick = playerController.GetPlayerFacing();
                Vector2 hitboxPosition = aimStick;
                SwordSwingObject.transform.right = hitboxPosition;
                SwordSwingObject.transform.localPosition = hitboxPosition * hitboxPositionOffset;

                // trigger activation method on hitbox
                swordHitboxScript.AttackActivated();

                // set input vector for the movement inherent to the attack
                playerController.inputVector = hitboxPosition.normalized * attackMoveSpeed;

            }
        }
        

        TimerThreeSwingLifespan += Time.deltaTime;


        // make the movement slow down to zero
        if (playerController.inputVector.magnitude > 0)
        {
            playerController.inputVector = playerController.inputVector - (playerController.inputVector * attackMoveDecay * Time.deltaTime);
        }
        else if (playerController.inputVector.magnitude < 0.1) playerController.inputVector = Vector2.zero;



            if (TimerThreeSwingLifespan > threeSlashUptime)
        {
            swordHitboxScript.Deactivate();
            SwordSwingObject.SetActive(false);
        }


        // DeInitialise state
        if (TimerThreeSwingLifespan > comboChainTime)
        {
            InitFlag3Hit = false;
            playerController.ChangeState(e_PlayerControllerStates.FreeMove);
            currentAttack = e_PlayerAttacks.None;
            TimerMeleeAttackCooldown = meleeAttackCooldownTime;
            FlagCanMeleeAttack = false;
            currentConsecutive = 0;
        }
    }

    public void ProcessChargeUp()
    {
        // initialise state
        if (!InitFlagChargeUp)
        {
            InitFlagChargeUp = true;
            TimerChargeUp = 0;
            FlagChargeReleaseInput = false;
        }

        TimerChargeUp += Time.deltaTime;


        // go to heavy slash if charged for long enough and input released
        if (FlagChargeReleaseInput && TimerChargeUp > minChargeTime)
        {
            FlagChargeReleaseInput = false;
            InitFlagChargeUp = false;
            //playerController.ChangeState(e_PlayerControllerStates.Attacking);
            currentAttack = e_PlayerAttacks.HeavySlash;
            Debug.Log("heavy slash");
        }

        // else go to freemove if input released and not held long enough
        else if (FlagChargeReleaseInput && TimerChargeUp < minChargeTime)
        {
            FlagChargeReleaseInput= false;
            InitFlagChargeUp = false;
            playerController.ChangeState(e_PlayerControllerStates.FreeMove);
            currentAttack = e_PlayerAttacks.None;
            currentConsecutive = 0;
            Debug.Log("early release");
        }
    }

    void ProcessKick()
    {
        // initialise
        if (!InitFlagKick)
        {
            InitFlagKick = true;
            TimerKickUptime = kickUptime;
            kickDirection = playerController.inputVector.normalized * kickMoveSpeed;
            playerController.inputVector = kickDirection;
        }

        TimerKickUptime -= Time.deltaTime;

        if(TimerKickUptime <= 0)
        {
            InitFlagKick = false;
            playerController.ChangeState(e_PlayerControllerStates.FreeMove);
            currentAttack = e_PlayerAttacks.None;
        }
    }

    public void ProcessHeavySlash()
    {
        // initialise state
        if (!InitFlagHeavySlash)
        {
            InitFlagHeavySlash = true;
            TimerHeavySlashLifetime = 0;
            FlagChargeReleaseInput = false;
            FlagHoldAttackTriggered = false;
            currentConsecutive += 1;
        }

        TimerHeavySlashLifetime += Time.deltaTime;

        // go back to charge up if hold input received
        if(FlagHoldAttackTriggered && TimerHeavySlashLifetime > heavyComboMinTime && currentConsecutive <= heavyMaxConsecutive)
        {
            InitFlagHeavySlash = false;
            currentAttack = e_PlayerAttacks.ChargeUp;
            Debug.Log("consec charge");
        }

        // else go to freemove if we time out
        if(TimerHeavySlashLifetime > heavyComboChainTime)
        {
            InitFlagHeavySlash = false;
            playerController.ChangeState(e_PlayerControllerStates.FreeMove);
            currentAttack = e_PlayerAttacks.None;
            currentConsecutive = 0;
            Debug.Log("heavy slash done");
        }

    }

    */
}
