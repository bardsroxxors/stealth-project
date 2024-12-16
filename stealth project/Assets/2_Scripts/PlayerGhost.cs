using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGhost : MonoBehaviour
{

    void Start()
    {
        transform.position = Vector3.zero;
    }

    public void SetPos(Vector3 pos)
    {
        transform.position = pos;
    }


}
