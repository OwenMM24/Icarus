using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RefineUI
{
    public class LayoutGroupPositionFix : MonoBehaviour
    {
        public LayoutGroup layoutGroup;

        void Start()
        {
            if(layoutGroup == null)
            {
                layoutGroup = gameObject.GetComponent<LayoutGroup>();
            }

            StartCoroutine(ExecuteAfterTime(0.01f));
        }

        IEnumerator ExecuteAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            layoutGroup.enabled = false;
            layoutGroup.enabled = true;
            Destroy(this);
        }
    }
}