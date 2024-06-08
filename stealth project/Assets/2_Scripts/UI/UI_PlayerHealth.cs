using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UI_PlayerHealth : MonoBehaviour
{

    private PlayerController playerScript;
    private int currentHealth;
    private int maxHealth;
    private int previousHealth;
    public Sprite spr_healthFull;
    public Sprite spr_healthHalf;
    public Sprite spr_healthEmpty;
    private bool healthChanged = false;
    public GameObject[] hearts;
    public float heartSpacing = 1;
    public GameObject heartPrefab;
    public Color lightColor;
    public Color darkColor;
    private bool spritesLit = false;

    // Start is called before the first frame update
    void Start()
    {
        
        InitSprites();
        UpdateLit();
    }

    // Update is called once per frame
    void Update()
    {
        currentHealth = playerScript.currentHP;
        if(currentHealth != previousHealth)
        {
            healthChanged = true;
            UpdateHealth();
        }

        UpdateLit();

        previousHealth = currentHealth;
        healthChanged = false;
    }

    public void UpdateHealth()
    {
        currentHealth = playerScript.currentHP;
        int tmp = currentHealth;
        foreach (GameObject heart in hearts)
        {
            if (tmp > 1)
            {
                heart.GetComponent<SpriteRenderer>().sprite = spr_healthFull;
                tmp -= 2;
            }
            else if (tmp > 0)
            {
                heart.GetComponent<SpriteRenderer>().sprite = spr_healthHalf;
                tmp -= 1;
            }
            else if (tmp <= 0)
            {
                heart.GetComponent<SpriteRenderer>().sprite = spr_healthEmpty;
            }
        }
    }

    private void UpdateLit()
    {
        if(spritesLit != playerScript.lit)
        {
            spritesLit = playerScript.lit;

            Color newColor = lightColor;
            if (!spritesLit) newColor = darkColor;

            foreach(GameObject heart in hearts)
            {
                heart.GetComponent<SpriteRenderer>().color = newColor;
            }

        }
    }

    public void InitSprites()
    {

        // delete existing navPoint objects
        int nbChildren = transform.childCount;
        if (nbChildren > 0)
        {
            for (int i = nbChildren - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        playerScript = GameObject.FindWithTag("Player").
                        GetComponent<PlayerController>();
        maxHealth = playerScript.maxHP;

        if (maxHealth % 2 == 0) hearts = new GameObject[maxHealth/2];
        else hearts = new GameObject[(maxHealth + 1) / 2];

        for (int i = 0; i < hearts.Length; i++)
        {
            Vector3 pos = transform.position;
            pos.x = pos.x + heartSpacing * i;
            hearts[i] = Instantiate(heartPrefab, pos, Quaternion.identity, transform);
        }
    }
}


[CustomEditor(typeof(UI_PlayerHealth))]


public class UI_PlayerHealthEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        UI_PlayerHealth generator = (UI_PlayerHealth)target;


        EditorGUILayout.Space();

        if (GUILayout.Button("Generate"))
        {

            

            generator.InitSprites();
            generator.UpdateHealth();
        }

        EditorGUILayout.Space();

    }

}
