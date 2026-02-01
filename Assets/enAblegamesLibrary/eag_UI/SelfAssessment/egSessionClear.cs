using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class egSessionClear : MonoBehaviour
{
    [SerializeField] Transform TargetPos_BelowView;
    [SerializeField] Transform TargetPos_InView;

    public bool isSessionClear;

    void Update()
    {
        Transform targetPos;
        if (isSessionClear)
            targetPos = TargetPos_InView;
        else
            targetPos = TargetPos_BelowView;

        transform.position = Vector3.Lerp(this.transform.position, targetPos.position, Time.unscaledDeltaTime * 5);
    }
}
