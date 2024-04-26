using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExperimentTrial
{
    public List<int> activeElectrodes = new List<int>();
    public int gVal;

    private static List<List<int>> activeElectrodesCandidatesForExperiment1 = new List<List<int>>
    {
        //new List<int>() {22, 30, 32},
        //new List<int>() {24, 32, 23},
        //new List<int>() {24, 23, 29}, // 25 => 29
        //new List<int>() {24, 29, 27}, // 25 => 29
        //new List<int>() {24, 27, 39},
        //new List<int>() {24, 39, 41},
        //new List<int>() {24, 41, 43},
        //new List<int>() {26, 43, 38},
        //new List<int>() {26, 38, 40},
        //new List<int>() {26, 40, 42},
        //new List<int>() {26, 42, 44},
        //new List<int>() {22, 44, 30},
        //new List<int>() {22, 34, 36},
        //new List<int>() {24, 36, 31},
        //new List<int>() {24, 31, 33},
        //new List<int>() {24, 33, 35},
        //new List<int>() {24, 35, 45},
        //new List<int>() {24, 45, 47},
        //new List<int>() {24, 47, 49},
        //new List<int>() {26, 49, 46},
        //new List<int>() {26, 46, 48},
        //new List<int>() {26, 48, 50},
        //new List<int>() {26, 50, 52},
        //new List<int>() {22, 52, 34},
        new List<int>() {22, 30, 34},
        new List<int>() {22, 32, 36},
        new List<int>() {24, 23, 31},
        new List<int>() {24, 29, 33}, // 25 => 29
        new List<int>() {24, 27, 35},
        new List<int>() {24, 39, 45},
        new List<int>() {24, 51, 53}, // 41 => 51, 47 => 53
        new List<int>() {26, 43, 49},
        new List<int>() {26, 38, 46},
        new List<int>() {26, 40, 48},
        new List<int>() {26, 42, 50},
        new List<int>() {22, 44, 52}
    };

    private static List<List<int>> activeElectrodesCandidatesForExperiment2 = new List<List<int>>
    {
        new List<int>() {22, 42, 46},
        new List<int>() {22, 46, 50},
        new List<int>() {22, 44, 48},
        new List<int>() {22, 48, 52},
        new List<int>() {22, 30, 34},
        new List<int>() {22, 34, 43},
        new List<int>() {22, 43, 47},
        new List<int>() {22, 47, 51},
        new List<int>() {22, 32, 36},
        new List<int>() {22, 36, 45},
        new List<int>() {22, 45, 49},
        new List<int>() {22, 49, 53},
        new List<int>() {22, 42, 44},
        new List<int>() {22, 46, 48},
        new List<int>() {22, 50, 52},
        new List<int>() {22, 44, 43},
        new List<int>() {22, 48, 47},
        new List<int>() {22, 52, 51},
        new List<int>() {22, 30, 32},
        new List<int>() {22, 34, 36},
        new List<int>() {22, 43, 45},
        new List<int>() {22, 47, 49},
        new List<int>() {22, 51, 53}
    };

    private static List<List<int>> activeElectrodesCandidatesForExperiment3 = new List<List<int>>
    {
        new List<int>() {24, 31, 33},
        new List<int>() {24, 33, 35},
        new List<int>() {24, 23, 29}, // 25 => 29
        new List<int>() {24, 29, 27}, // 25 => 29
        new List<int>() {24, 41, 45},
        new List<int>() {24, 45, 49},
        new List<int>() {24, 49, 53},
        new List<int>() {24, 53, 40},
        new List<int>() {24, 40, 44},
        new List<int>() {24, 39, 43},
        new List<int>() {24, 43, 47},
        new List<int>() {24, 47, 51},
        new List<int>() {24, 51, 38},
        new List<int>() {24, 38, 42},
        new List<int>() {24, 31, 23},
        new List<int>() {24, 33, 29}, // 25 => 29
        new List<int>() {24, 35, 27},
        new List<int>() {24, 23, 41},
        new List<int>() {24, 29, 45}, // 25 => 29
        new List<int>() {24, 27, 49},
        new List<int>() {24, 41, 39},
        new List<int>() {24, 45, 43},
        new List<int>() {24, 49, 47},
        new List<int>() {24, 53, 51},
        new List<int>() {24, 40, 38},
        new List<int>() {24, 44, 42}
    };

    private static List<List<int>> activeElectrodesCandidatesForExperiment4 = new List<List<int>>
    {
        new List<int>() {24, 23, 29}, // 25 => 29
        new List<int>() {24, 27, 31},
        new List<int>() {24, 33, 35},
        new List<int>() {26, 38, 42},
        new List<int>() {26, 42, 46},
        new List<int>() {26, 40, 44},
        new List<int>() {26, 44, 48},
        new List<int>() {26, 39, 41},
        new List<int>() {26, 41, 43},
        new List<int>() {26, 43, 45},
        new List<int>() {26, 45, 47},
        new List<int>() {24, 23, 27},
        new List<int>() {24, 29, 31}, // 25 => 29
        new List<int>() {24, 27, 33},
        new List<int>() {24, 31, 35},
        new List<int>() {26, 33, 42},
        new List<int>() {26, 35, 46},
        new List<int>() {26, 38, 40},
        new List<int>() {26, 42, 44},
        new List<int>() {26, 46, 48},
        new List<int>() {26, 40, 43},
        new List<int>() {26, 44, 45},
        new List<int>() {26, 48, 47}
    };

    private static List<int> gValCandidates = new List<int>
    {
        138, 170 // 2mA++, 2.5mA++
    };

    public ExperimentTrial(List<int> activeElectrodes, int gVal)
    {
        this.activeElectrodes = activeElectrodes;
        this.gVal = gVal;
    }
    
    public static List<ExperimentTrial> GenerateExperimentalTrialList(ExperimentType experimentType, bool isShuffled = false)
    {
        List<ExperimentTrial> experimentTrials = new();

        List<List<int>> activeElectrodesCandidates = new();
        switch (experimentType)
        {
            case ExperimentType.EXPERIMENT_1:
                activeElectrodesCandidates = activeElectrodesCandidatesForExperiment1;
                break;
            case ExperimentType.EXPERIMENT_2:
                activeElectrodesCandidates = activeElectrodesCandidatesForExperiment2;
                break;
            case ExperimentType.EXPERIMENT_3:
                activeElectrodesCandidates = activeElectrodesCandidatesForExperiment3;
                break;
            case ExperimentType.EXPERIMENT_4:
                activeElectrodesCandidates = activeElectrodesCandidatesForExperiment4;
                break;
        }

        foreach (var activeElectrodesCandidate in activeElectrodesCandidates)
        {
            foreach (var gValCandidate in gValCandidates)
            {
                experimentTrials.Add(new ExperimentTrial(activeElectrodesCandidate.Clone(), gValCandidate));
            }
        }

        if (isShuffled)
        {
            experimentTrials = experimentTrials.OrderBy(trial => Random.Range(0f, 1f)).ToList();
        }

        return experimentTrials;
    }
}
