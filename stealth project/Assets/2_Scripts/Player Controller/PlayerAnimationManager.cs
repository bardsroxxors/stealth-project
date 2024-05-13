using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{

    public bool noiseTrigger = false;
    private PlayerController controller;
    private bool noiseLastFrame = false;

    // Start is called before the first frame update
    void Start()
    {
        controller = transform.parent.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (noiseTrigger && controller != null && !noiseLastFrame)
        {
            controller.f_noiseAnimationTrigger = noiseTrigger;
            //noiseTrigger = false;
            noiseLastFrame = true;
        }
        if (!noiseTrigger) noiseLastFrame = false;
    }
}
