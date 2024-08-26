using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BountyDisplay : MonoBehaviour
{

    PointBounty pb;
    TextMeshPro text;
    int knownScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        pb = GetComponentInParent<PointBounty>();
        text = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        if (text != null && knownScore != pb.bounty)
        {
            knownScore = pb.bounty;
            string score = "";

            for(int i = 0; i < knownScore; i++)
            {
                score += "•";
            }

            text.text = score;
        }
    }
}
