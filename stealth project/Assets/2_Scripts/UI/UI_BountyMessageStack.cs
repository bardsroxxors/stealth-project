using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_BountyMessageStack : MonoBehaviour
{

    public GameObject initialTextObj;
    public int maxMessages = 6;
    public float messageSpacing = 20f;
    private Vector3 topMessagePos;

    private GameObject[] messageObjs;
    private TextMeshProUGUI[] texts; 

    // Start is called before the first frame update
    void Start()
    {
        topMessagePos = initialTextObj.transform.position;


        messageObjs = new GameObject[maxMessages];
        texts = new TextMeshProUGUI[maxMessages];

        messageObjs[0] = initialTextObj;
        texts[0] = initialTextObj.GetComponent<TextMeshProUGUI>();

        for (int i = 0; i< maxMessages; i++)
        {
            if(i != 0)
            {
                messageObjs[i] = Instantiate(initialTextObj, this.transform);
                texts[i] = messageObjs[i].GetComponent<TextMeshProUGUI>();
                messageObjs[i].transform.position = topMessagePos + new Vector3(0, - messageSpacing * i, 0);
                messageObjs[i].SetActive(false);
            }
                
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
