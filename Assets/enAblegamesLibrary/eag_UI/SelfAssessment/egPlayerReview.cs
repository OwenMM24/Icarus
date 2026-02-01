using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class egPlayerReview : MonoBehaviour
{
    [SerializeField] List<GameObject> assessmentCategories;
    [SerializeField] Text textObject;
    [SerializeField] Text leftmostVaule;
    [SerializeField] Text rightmostVaule;

    public List<Color> colors;

    public bool isRunningPlayerReview;

    [SerializeField] Transform movingText;
    [SerializeField] Image bgImage;
    public egUIManager uiManager;

    public int activePos = 0;

    public int playerHappiness;
    public int playerExcitement;
    public int playerControl;
    public int playerPain;


    private void Awake()
    {
        for(int i = 0; i < assessmentCategories.Count; i++)
        {
            assessmentCategories[i].GetComponent<ReviewCategory>().myPos = i;
        }
    }

    void Update()
    {

        if (isRunningPlayerReview)
        {
            if (activePos < assessmentCategories.Count)
            {
                movingText.position = assessmentCategories[activePos].transform.position;
                leftmostVaule.text = assessmentCategories[activePos].GetComponent<ReviewCategory>().leftMostValue;
                rightmostVaule.text = assessmentCategories[activePos].GetComponent<ReviewCategory>().rightMostValue;
            }
            textObject.color = Vector4.Lerp(textObject.color, Color.white, Time.deltaTime * 1.5f);
            leftmostVaule.color = Vector4.Lerp(leftmostVaule.color, Color.white, Time.deltaTime * 1.5f);
            rightmostVaule.color = Vector4.Lerp(rightmostVaule.color, Color.white, Time.deltaTime * 1.5f);
        }
        else
        {
            textObject.color = Vector4.Lerp(textObject.color, new Color(1, 1, 1, 0), Time.deltaTime * 35);
            leftmostVaule.color = Vector4.Lerp(leftmostVaule.color, new Color(1, 1, 1, 0), Time.deltaTime * 35);
            rightmostVaule.color = Vector4.Lerp(rightmostVaule.color, new Color(1, 1, 1, 0), Time.deltaTime * 35);
        }
        if (activePos == 3)
        {
            uiManager.playerReviewOn = false;
            activePos = 0;
        }
    }
}
