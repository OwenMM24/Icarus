using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RefineUI
{
    public class TimedAction : MonoBehaviour
    {
        [Header("TIMING (SECONDS)")]
        public float timer = 10;
        public bool enableAtStart;

        [Header("END ACTION")]
        public UnityEvent timerAction;

        void Start()
        {
            if(enableAtStart == true)
            {
                StartCoroutine("TimedEvent");
            }
        }

        IEnumerator TimedEvent()
        {
            yield return new WaitForSeconds(timer);
            timerAction.Invoke();
            StopIEnumerator();
        }

        public void StartIEnumerator ()
        {
            StartCoroutine("TimedEvent");
        }

        public void StopIEnumerator ()
        {
            StopCoroutine("TimedEvent");
        }
    }
}
