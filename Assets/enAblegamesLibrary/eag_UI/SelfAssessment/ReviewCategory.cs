using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReviewCategory : MonoBehaviour
{
    public egPlayerReview playerReview;

    public List<Image> assessmentIcons;
    [SerializeField] Image bgImage;

    private List<Color> colors;

    public string leftMostValue;
    public string rightMostValue;

    //The current icon selected.
    [SerializeField] int iconsIndex;

    //myPos determines the rank of this object in SelfAssessment's assessmentCategories' heirarchy.
    public int myPos;
    [SerializeField] private int activePos = 0;
    [SerializeField] List<Transform> TargetPos;
    private Transform currentPos;

    private void Awake()
    {
        bgImage = this.GetComponent<Image>();
        bgImage.color = new Color(0.5f, 0.5f, 0.5f);

        colors = playerReview.colors;

        foreach (Image icon in assessmentIcons)
        {
            icon.color = new Color(0.5f, 0.5f, 0.5f);
        }
    }

    void Update()
    {
        transform.position = Vector3.Lerp(this.transform.position, TargetPos[activePos].position, Time.deltaTime * 3);

        if(activePos == 1)
        {
            float placement = Input.GetAxis("Horizontal");

            if (placement < -0.75f)
                iconsIndex = 0;
            else if (placement < -0.25f)
                iconsIndex = 1;
            else if (placement < 0.25f)
                iconsIndex = 2;
            else if (placement < 0.75f)
                iconsIndex = 3;
            else
                iconsIndex = 4;
            UpdateIcons();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(myPos == 0)
                    playerReview.playerHappiness = iconsIndex - 2;
                if (myPos == 1)
                    playerReview.playerExcitement = iconsIndex - 2;
                if (myPos == 2)
                    playerReview.playerControl = iconsIndex - 2;
                playerReview.activePos += 1;
            }
        }

        if (playerReview.activePos == myPos && playerReview.isRunningPlayerReview)
            activePos = 1;
        if (playerReview.activePos != myPos || !playerReview.isRunningPlayerReview)
            activePos = 0;
    }

    private void UpdateIcons()
    {
        for(int i = 0; i < assessmentIcons.Count; i++)
        {
            if (i == iconsIndex)
                assessmentIcons[i].color = Vector4.Lerp(assessmentIcons[i].color, colors[i], Time.deltaTime * 5f);
            else
                assessmentIcons[i].color = Vector4.Lerp(assessmentIcons[i].color, new Color(0.5f, 0.5f, 0.5f), Time.deltaTime * 10f);
        }
    }
}
