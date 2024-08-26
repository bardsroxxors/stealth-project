using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PointBounty : MonoBehaviour
{
    // this is the points bounty thing
    // attach it to stuff that gives you points
    PointsManager pm;
    public int bounty = 50;

    // Start is called before the first frame update
    void Start()
    {
        GameObject pmo = GameObject.Find("Points Manager");
        pm = pmo.GetComponent<PointsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GrantPoints()
    {
        if (pm != null) 
        {
            pm.SendMessage("AddScore", bounty);
        }

    }

    public void AddBounty(int more)
    {
        bounty += more;
    }

    public void SubtractBounty(int less)
    {
        bounty -= less;
    }

    private void OnDrawGizmos()
    {
        Handles.Label(transform.position - new Vector3(0, 1.5f, 0), bounty.ToString());
    }

}
