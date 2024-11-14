using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KunaiTarget : MonoBehaviour
{
    // this script is for things in the env that you can shoot with kunai to make things happen
    // at first this one will just make a noise and add a condition to guards


    public Sound sound;
    public GameObject soundPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void KunaiHit()
    {
        GameObject snd = Instantiate(soundPrefab, transform.position, Quaternion.identity);
        snd.GetComponent<NoiseScript>().soundSO = sound;
    }
}
