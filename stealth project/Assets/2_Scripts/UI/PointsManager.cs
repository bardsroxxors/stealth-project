using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PointsManager : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    public int score = 0;
    public int multiplier = 1;
    public float multiDecayTime = 5;
    private float t_multiDecayTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        BuildText();
    }

    // Update is called once per frame
    void Update()
    {
        if(multiplier > 1)
        {
            if (t_multiDecayTime > 0) t_multiDecayTime -= Time.deltaTime;
            else
            {
                multiplier -= 1;
                t_multiDecayTime = multiDecayTime;
            }
        }
    }



    void AddScore(int n)
    {
        score += n;
        BuildText();
        
    }

    void BuildText()
    {
        scoreText.text = score.ToString() + "\n" + multiplier + "x";
    }

    
}
