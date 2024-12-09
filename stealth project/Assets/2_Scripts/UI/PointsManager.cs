using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class PointsManager : MonoBehaviour
{
    public GameObject scoreObj;
    public GameObject multiplierObj;
    public GameObject progressBarObj;

    public float barScore = 0;              // bar score increases in order to raise the bar and multiplier
    public float initialBarScoreThreshold = 10;    // initial barscore threshold for mulit increase
    public float initialTierDiff = 3;
    public float tierDiffDelta = 0.5f;
    //public float multiTierScaling = 0;      // exponent used to scale the barscore needed for each multiplier
    public float pipValue = 3;              // barScore value per pip 

    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI multiText;
    private Image progBarImage;
    public int score = 0;
    public int multiplier = 1;
    public int multiMax = 6;
    public float multiDecayDelay = 5; // delay before bar decays
    private float t_multiDecayDelay = 0;
    public float barDecay = 1;          // barscore decay per second

    public float[] barScoreThresholds;

    private float barPercent = 0;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = scoreObj.GetComponent<TextMeshProUGUI>();
        multiText = multiplierObj.GetComponent<TextMeshProUGUI>();
        progBarImage = progressBarObj.GetComponent<Image>();

        SetupThresholds();

        BuildText();
    }

    // Update is called once per frame
    void Update()
    {
        multiplier = GetMulti();
        BuildText();


        if (t_multiDecayDelay > 0) t_multiDecayDelay -= Time.deltaTime;
        else if(barScore > 0)
        {
            barScore -= barDecay * Time.deltaTime;
            SetBarPercent();
                
        }

        if (barScore < 0)
            barScore = 0;
        
    }



    void AddScore(int n)
    {
        score += n*50 * multiplier;
        t_multiDecayDelay = multiDecayDelay;
        BuildText();
        
    }

    void AddMulti(int pips)
    {
        barScore += pips * pipValue;
        SetBarPercent();
        t_multiDecayDelay = multiDecayDelay;
        multiplier = GetMulti();
        BuildText();
    }

    void BuildText()
    {
        scoreText.text = score.ToString();
        multiText.text = multiplier.ToString();
    }

    // this function converts barscore to a multiplier
    public int GetMulti() {
        int m = 0;


        for (int i = 0; i < multiMax; i++)
        {
            if(barScore > barScoreThresholds[i])
            {
                m = i+2;
            }
        }
        if (m > multiMax)
            return multiMax;
        else if (m > 0)
            return m;
        else
            return 1;
    
    }

    private void SetupThresholds()
    {

        //Debug.Log("threshold setup");

        barScoreThresholds = new float[multiMax];

        float init = initialBarScoreThreshold;

        for(int i = 0; i< multiMax; i++)
        {
            barScoreThresholds[i] = init;
            //Debug.Log(init);
            init = init + (tierDiffDelta * (i+1));
        }
    }

    private void SetBarPercent()
    {
        barPercent = GetBarPercent();
        progBarImage.fillAmount = barPercent;
    }

    // this function converts barscore to bar percent
    private float GetBarPercent()
    {

        // barscore
        // current tier threshold (current multiplier)

        // barscore - thresholds[multi]

        // return 1 if at max multi

        if (multiplier == multiMax)
            return 1;

        float lower = 0;

        float upper = barScoreThresholds[0];

        // get the lower bound
        for (int i = 0; i< barScoreThresholds.Length; i++)
        {
            if(barScoreThresholds[i] < barScore)
            {
                lower = barScoreThresholds[i];
                upper = barScoreThresholds[i+1];
            }
        }

        // get the upper bound
        







        float dif = barScore - lower;
        float nextMax = upper - lower;

        float percent = dif / nextMax;

        //Debug.Log("lower: " + lower + "upper: " + upper);

        //Debug.Log(percent);

        return percent;
    }

    void OnAddPoints(InputValue value)
    {
        AddMulti(1);
    }


}
