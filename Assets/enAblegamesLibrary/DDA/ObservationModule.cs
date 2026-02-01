using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObservationModule : MonoBehaviour
{
    public string name;
    public List<string> performances;
    [Tooltip("Number of Difficulty Levels")]
    public int nol;
    [SerializeField] private DDAModule DDA;

    private void Start()
    {
    }

    public void Initialize()
    {
        DDA.Initialize(nol);
    }

    public int GetLevel()
    {
        return DDA.SuggestLevel();
    }

    public void Observe()
    {
        float totalScore = 0;
        foreach (var p in performances)
        {
            DDAManager.PerformanceElement performanceElement = DDAManager.Instance.PerformanceMapping[p];
            float pValue = (float)DDAManager.Instance.PerformanceData[p];
            if (performanceElement.PerfectThreshold > performanceElement.WorseThreshold)
            {
                if (pValue > performanceElement.PerfectThreshold)
                {
                    totalScore += 2;
                }
                else if (pValue > performanceElement.GoodThreshold)
                {
                    totalScore += 1;
                }
                else if (pValue < performanceElement.WorseThreshold)
                {
                    totalScore -= 2;
                }
                else if (pValue < performanceElement.BadThreshold)
                {
                    totalScore -= 1;
                }
            }
            else
            {
                if (pValue < performanceElement.PerfectThreshold)
                {
                    totalScore += 2;
                }
                else if (pValue < performanceElement.GoodThreshold)
                {
                    totalScore += 1;
                }
                else if (pValue > performanceElement.WorseThreshold)
                {
                    totalScore -= 2;
                }
                else if (pValue > performanceElement.BadThreshold)
                {
                    totalScore -= 1;
                }
            }
        }
        DDA.UpdateBeliefVector(Convert.ToInt32(totalScore / performances.Count));
        DDAManager.Instance.DifficultyValues[name] = DDA.SuggestLevel();
    }

    public void Observe(int value)
    {
        DDA.UpdateBeliefVector(value);
        DDAManager.Instance.DifficultyValues[name] = DDA.SuggestLevel();
    }
}
