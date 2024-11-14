using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseScript : MonoBehaviour
{

    //public float lifetime = 1;
    private float t_lifetime = 0;
    //public float scaleAtDeath = 1.2f;
    //public float initialScale = 2.5f;
    public float awarenessIncrease;

    public Sound soundSO;

    private SpriteRenderer sprite;
    private float initialAlpha = 0;

    public bool directCondition = false;
    public e_EnemyConditions condition;

    // Start is called before the first frame update
    void Start()
    {
        t_lifetime = soundSO.lifetime;
        transform.localScale = new Vector3(soundSO.initialScale, soundSO.initialScale, 1);
        directCondition = soundSO.directCondition;
        if(directCondition)
            condition = soundSO.condition;
        awarenessIncrease = soundSO.awarenessIncrease;
        sprite = GetComponent<SpriteRenderer>();
        if (sprite) initialAlpha = sprite.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        if (awarenessIncrease == null) awarenessIncrease = soundSO.awarenessIncrease;

        if (t_lifetime >= 0) t_lifetime -= Time.deltaTime;
        else
        {
            Destroy(this.gameObject);
        }

        float scale = Mathf.Lerp(soundSO.scaleAtDeath, soundSO.initialScale, (t_lifetime / soundSO.lifetime));
        transform.localScale = new Vector3(scale, scale, 1);

        float alpha = Mathf.Lerp(0, initialAlpha, (t_lifetime / soundSO.lifetime));
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
        

    }

    // call with send message once instantiated to set the scriptable object
    void SetProfile(Sound sound)
    {
        soundSO = sound;

    }
}
