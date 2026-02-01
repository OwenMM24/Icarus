using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class egGameResults : MonoBehaviour
{
    [SerializeField] Transform TargetPos_BelowView;
    [SerializeField] Transform TargetPos_InView;

    public bool showGameResults;

    void Update()
    {
        Transform targetPos;
        if (showGameResults)
            targetPos = TargetPos_InView;
        else
            targetPos = TargetPos_BelowView;

            transform.position = Vector3.Lerp(this.transform.position, targetPos.position, Time.deltaTime * 5);
    }
}
