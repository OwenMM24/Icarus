using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class NBackDataGenerator : MonoBehaviour
{
    Data myData;
    [SerializeField]
    private string filePath;

    [SerializeField]
    private string fileName;

    [SerializeField] Sample graphMaster;


    public void Awake()
    {
        filePath = Application.streamingAssetsPath + "/" + fileName;
    }


    public void Main()
    {
        myData = ReadNBackFile();
        print("scores: " + myData.Scores);
    }



    Data ReadNBackFile()
    {
        var data = new Data();

        using (var file = new StreamReader(filePath))
        {
            int nTrials = 4; //number of times each n-back test is done
            int nTests = 3; //number of different kinds of n-back tests (0-back, 1-back, 2-back)

            int[,] totalScore = new int[nTests, nTrials];
            
            //Note that in the original MATLAB code, the reactionTimes variable is a cell array, which can hold data of different types and sizes.
            //Here it is represented as an array of List<float>, where each element corresponds to a test (0-back, 1-back, or 2-back) and holds a list of reaction times.
            List<float>[] reactionTimes = new List<float>[nTests];
            for (int i = 0; i < nTests; i++)
            {
                reactionTimes[i] = new List<float>();
            }

            double reactionThreshold = 0.2; //discard if less than .2 seconds (indicates probably holding down button)

            int[,] wrongPresses = new int[nTests, nTrials];
            int[,] wrongNonPresses = new int[nTests, nTrials];

            //read file
            for (int i = 0; i < nTrials; i++)
            {
                for (int j = 0; j < nTests; j++)
                {
                    file.ReadLine(); //title, discard
                    file.ReadLine(); //displayed sequence, discard

                    int[] scores = file.ReadLine().Split(' ').Select(int.Parse).ToArray(); //read in scores (1 = correct click or non click, 0 = wrong click, 2 = wrong non-click)

                    totalScore[j, i] = scores.Count(score => score == 1);
                    wrongPresses[j, i] = scores.Count(score => score == 0);
                    wrongNonPresses[j, i] = scores.Count(score => score == 2);

                    float[] reactions = file.ReadLine().Split(' ').Select(float.Parse).ToArray();
                    reactionTimes[j].AddRange(reactions.Where(reaction => reaction > reactionThreshold));

                    graphMaster.OnButton(new Vector2(1, totalScore[j, i]));
                    print("i: " + i + " j:" + j + " " + totalScore[j, i]);

                    file.ReadLine(); //discard empty line
                    file.ReadLine(); //discard empty line
                }
            }

            print("totalScore: " + totalScore[0,0]);

            //store in returned struct
            data.Scores = totalScore;
            data.NetScores = Enumerable.Range(0, nTests).Select(i => totalScore[i, 0] + totalScore[i, 1] + totalScore[i, 2] + totalScore[i, 3]).Select(score => (double)score / 40).ToArray();
            data.TotalScore = data.NetScores.Average();
            data.RxnTimes = reactionTimes;
            data.WrongPresses = wrongPresses;
            data.WrongNonPresses = wrongNonPresses;
        }

        return data;
    }
}

class Data
{
    public int[,] Scores { get; set; }
    public double[] NetScores { get; set; }
    public double TotalScore { get; set; }
    public List<float>[] RxnTimes { get; set; }
    public int[,] WrongPresses { get; set; }
    public int[,] WrongNonPresses { get; set; }
}
