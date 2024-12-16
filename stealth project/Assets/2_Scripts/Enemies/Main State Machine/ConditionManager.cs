using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum e_EnemyConditions
{
    vigilant,       // Looks around frequently, has been alerted
    blind,
    immobile,
    piqued,         // has become curious
    alerted,        // has become alert
    oblivious,      // has moved past hidden player
    bodySighted,    // has sighted a body
    chimes,
    forceMine
}

public class ConditionManager : MonoBehaviour
{

    public List<e_EnemyConditions> conditions = new List<e_EnemyConditions>();
    public List<float> conditionTimes = new List<float>();

    private EnemyAwareness awareScript;


    private void Start()
    {
        awareScript = GetComponent<EnemyAwareness>();
    }

    private void Update()
    {
        // check if we are freshly piqued
        if (awareScript.currentAwareness == AwarenessLevel.curious)
        {
            AddCondition(e_EnemyConditions.piqued);

        }
        // check if we are freshly piqued
        if (awareScript.currentAwareness == AwarenessLevel.alert)
        {
            AddCondition(e_EnemyConditions.alerted);
        }
    }

    public bool AddCondition(e_EnemyConditions con)
    {
        if (conditions.Contains(con)) return false;

        else
        {
            conditions.Add(con);
            switch (con)
            {
                case e_EnemyConditions.piqued:
                    this.SendMessage("AddBounty", 1);
                    break;
                case e_EnemyConditions.oblivious:
                    this.SendMessage("AddBounty", 1);
                    break;
                case e_EnemyConditions.alerted:
                    this.SendMessage("SubtractBounty", 1);
                    break;
                case e_EnemyConditions.bodySighted:
                    this.SendMessage("AddBounty", 2);
                    break;
                case e_EnemyConditions.chimes:
                    this.SendMessage("AddBounty", 2);
                    break;
                case e_EnemyConditions.forceMine:
                    this.SendMessage("AddBounty", 1);
                    break;
                case e_EnemyConditions.immobile:
                    this.SendMessage("AddBounty", 1);
                    break;
            }
            return true;
        }
    }
}
