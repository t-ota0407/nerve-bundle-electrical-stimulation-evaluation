using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExperimentManager : MonoBehaviour
{
    [Header("実験条件")]
    [SerializeField] private string userID;
    [SerializeField] private ExperimentType experimentType;

    [Space(30)]
    [Header("References")]
    [SerializeField] private KeyCode loadNextTrialKey;
    [SerializeField] private KeyCode startElectricalStimulationKey;
    [SerializeField] private KeyCode loadNextDrawingTaskKey;
    [SerializeField] private KeyCode loadPreviousDrawingTaskKey;
    [SerializeField] private KeyCode forcedFinishKey;
    [SerializeField] private KeyCode clearPaintKey;

    [SerializeField] private ElectricalStimulationManager electricalStimulationManager;
    [SerializeField] private PaintController paintController;

    [SerializeField] private GameObject leftEventTrigger;
    [SerializeField] private GameObject rightEventTrigger;

    [SerializeField] private Text phaseText;
    [SerializeField] private Text trialText;

    [SerializeField] private Image footGeneralImage;
    [SerializeField] private Image footHorizontalImage;
    [SerializeField] private Image footVerticalImage;

    [SerializeField] private int electricalStimulationDurationMiliSeconds;

    private List<ExperimentTrial> experimentTrials;
    private int currentTrialIndex = -1;
    private ExperimentTrial currentTrial;

    private ExperimentalPhase experimentalPhase = ExperimentalPhase.Initializing;
    public ExperimentalPhase ExperimentalPhase
    {
        set
        {
            experimentalPhase = value;
            
            switch (experimentalPhase)
            {
                case ExperimentalPhase.Initializing:
                    phaseText.text = "";
                    break;
                case ExperimentalPhase.Waiting:
                    phaseText.text = "待機中です。準備ができたら[1]を押して電気刺激を開始してください。";
                    break;
                case ExperimentalPhase.ApplyingElectricalStimulation:
                    phaseText.text = "電気刺激中です。もし刺激を中止したい場合は[Space]を押してください。";
                    break;
                case ExperimentalPhase.DrawingGeneral:
                    phaseText.text = "電気刺激による力覚が生起された範囲をマウスを使って描画してください。";
                    break;
                case ExperimentalPhase.DrawingHorizontal:
                    phaseText.text = "電気刺激による力覚が最も強く生起された位置を右クリックしてください。";
                    break;
                case ExperimentalPhase.DrawingVertical:
                    phaseText.text = "電気刺激による力覚が最も強く生起された位置を右クリックしてください。";
                    break;
                case ExperimentalPhase.Finished:
                    phaseText.text = "実験終了";
                    break;
            }
        }
    }

    private DateTime electricalStimulationStartedAt;
    
    // Start is called before the first frame update
    void Start()
    {
        string folderPath = Application.dataPath + "/Data/" + userID;
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        experimentTrials = ExperimentTrial.GenerateExperimentalTrialList(experimentType, isShuffled: true);
        LoadNextTrial(isInitialTrial: true);
        ExperimentalPhase = ExperimentalPhase.Waiting;
    }

    // Update is called once per frame
    void Update()
    {
        switch (experimentalPhase)
        {
            case ExperimentalPhase.Waiting:

                if (Input.GetKeyUp(startElectricalStimulationKey))
                {
                    StartElectricalStimulation();
                }
                
                if (currentTrialIndex != 0 && Input.GetKeyUp(loadPreviousDrawingTaskKey))
                {
                    BackToPreviousDrawing(DrawingTasks.VERTICAL);
                    ChangeBackgroundImageTo(DrawingTasks.VERTICAL);
                }

                break;
            
            case ExperimentalPhase.ApplyingElectricalStimulation:

                if (Input.GetKeyUp(forcedFinishKey))
                {
                    StopElectricalStimulation();
                    LoadNextTrial(isInitialTrial: false, fileSuffix: "abandoned");
                }

                if ((DateTime.Now - electricalStimulationStartedAt).TotalMilliseconds > electricalStimulationDurationMiliSeconds)
                {
                    StopElectricalStimulation();
                }

                break;
            
            case ExperimentalPhase.DrawingGeneral:

                if (Input.GetKeyUp(startElectricalStimulationKey))
                {
                    StartElectricalStimulation();
                }

                if (Input.GetKeyUp(loadNextDrawingTaskKey))
                {
                    FinishDrawingGeneral();
                    ChangeBackgroundImageTo(DrawingTasks.HORIZONTAL);
                }

                if (Input.GetKeyUp(clearPaintKey))
                {
                    paintController.WhitenImage();
                }

                break;

            case ExperimentalPhase.DrawingHorizontal:

                if (Input.GetKeyUp(loadNextDrawingTaskKey))
                {
                    FinishDrawingHorizontal();
                    ChangeBackgroundImageTo(DrawingTasks.VERTICAL);
                }

                if (Input.GetKeyUp(loadPreviousDrawingTaskKey))
                {
                    BackToPreviousDrawing(DrawingTasks.GENERAL);
                    ChangeBackgroundImageTo(DrawingTasks.GENERAL);
                }

                if (Input.GetKeyUp(clearPaintKey))
                {
                    paintController.WhitenImage();
                }

                break;

            case ExperimentalPhase.DrawingVertical:

                if (Input.GetKeyUp(loadNextTrialKey))
                {
                    Debug.Log(DrawingTasksConverter.ToString(DrawingTasks.VERTICAL));
                    LoadNextTrial(isInitialTrial: false, fileSuffix: DrawingTasksConverter.ToString(DrawingTasks.VERTICAL));
                    ChangeBackgroundImageTo(DrawingTasks.GENERAL);
                }

                if (Input.GetKeyUp(loadPreviousDrawingTaskKey))
                {
                    BackToPreviousDrawing(DrawingTasks.HORIZONTAL);
                    ChangeBackgroundImageTo(DrawingTasks.HORIZONTAL);
                }

                if (Input.GetKeyUp(clearPaintKey))
                {
                    paintController.WhitenImage();
                }

                break;
        }
    }

    private void LoadNextTrial(bool isInitialTrial, string fileSuffix = "")
    {
        if (!isInitialTrial)
        {
            SaveCurrentImageAndReload(fileSuffix);
        }

        leftEventTrigger.SetActive(false);
        rightEventTrigger.SetActive(false);
        paintController.isConstraind = false;

        currentTrialIndex++;
        trialText.text = $"{currentTrialIndex} / {experimentTrials.Count}";

        if (currentTrialIndex < experimentTrials.Count)
        {
            currentTrial = experimentTrials[currentTrialIndex];
            ExperimentalPhase = ExperimentalPhase.Waiting;
        }
        else
        {
            ExperimentalPhase = ExperimentalPhase.Finished;
        }
    }

    private void StartElectricalStimulation()
    {
        electricalStimulationStartedAt = DateTime.Now;

        List<int> activeElectrodes = currentTrial.activeElectrodes;
        int gVal = currentTrial.gVal;
        electricalStimulationManager.StartElectricalStimulation(activeElectrodes, gVal);

        ExperimentalPhase = ExperimentalPhase.ApplyingElectricalStimulation;
    }

    private void StopElectricalStimulation()
    {
        leftEventTrigger.SetActive(true);
        electricalStimulationManager.StopElectricalStimulation();
        ExperimentalPhase = ExperimentalPhase.DrawingGeneral;
    }

    private void FinishDrawingGeneral()
    {
        leftEventTrigger.SetActive(false);
        rightEventTrigger.SetActive(true);
        SaveCurrentImageAndReload(DrawingTasksConverter.ToString(DrawingTasks.GENERAL));
        ExperimentalPhase = ExperimentalPhase.DrawingHorizontal;
    }

    private void FinishDrawingHorizontal()
    {
        SaveCurrentImageAndReload(DrawingTasksConverter.ToString(DrawingTasks.HORIZONTAL));
        ExperimentalPhase = ExperimentalPhase.DrawingVertical;
        paintController.isConstraind = true;
    }

    private void BackToPreviousDrawing(DrawingTasks targetTask)
    {
        if (currentTrialIndex >= 0)
        {
            if (targetTask == DrawingTasks.VERTICAL) // 試行IDをまたぐ場合
            {
                currentTrialIndex--;
                currentTrial = experimentTrials[currentTrialIndex];
                trialText.text = $"{currentTrialIndex} / {experimentTrials.Count}";
            }

            switch (targetTask)
            {
                case DrawingTasks.GENERAL:
                    ExperimentalPhase = ExperimentalPhase.DrawingGeneral;
                    rightEventTrigger.SetActive(false);
                    leftEventTrigger.SetActive(true);
                    break;
                case DrawingTasks.HORIZONTAL:
                    ExperimentalPhase = ExperimentalPhase.DrawingHorizontal;
                    paintController.isConstraind = false;
                    break;
                case DrawingTasks.VERTICAL:
                    ExperimentalPhase = ExperimentalPhase.DrawingVertical;
                    rightEventTrigger.SetActive(true);
                    paintController.isConstraind = true;
                    break;
            }

            string searchFilePattern = string.Join(",", currentTrial.activeElectrodes.ToArray()) + "_" + currentTrial.gVal + "_" + DrawingTasksConverter.ToString(targetTask);
            LoadPreviousImage(searchFilePattern);
        }
    }

    private void SaveCurrentImageAndReload(string suffix)
    {
        string filePath = Application.dataPath + "/Data/" + userID + "/" + string.Join(",", currentTrial.activeElectrodes.ToArray()) + "_" + currentTrial.gVal + "_" + suffix + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
        paintController.SaveRawImage(filePath);
        paintController.WhitenImage();
    }

    private void LoadPreviousImage(string searchFilePattern)
    {
        string searchDirectoryName = Application.dataPath + "/Data/" + userID;
        
        string[] searchedFiles = Directory.GetFiles(searchDirectoryName);
        
        foreach (string searchFile in searchedFiles)
        {
            Debug.Log(searchFile);
            Debug.Log(searchFilePattern);
            if (searchFile.Contains(searchFilePattern) && searchFile.EndsWith(".png"))
            {
                Debug.Log(searchFile);
                paintController.ReadRawImage(searchFile);
            }
        }
    }

    private void ChangeBackgroundImageTo(DrawingTasks drawingTask)
    {
        switch (drawingTask)
        {
            case DrawingTasks.GENERAL:
                footGeneralImage.gameObject.SetActive(true);
                footHorizontalImage.gameObject.SetActive(false);
                footVerticalImage.gameObject.SetActive(false);
                break;

            case DrawingTasks.HORIZONTAL:
                footGeneralImage.gameObject.SetActive(false);
                footHorizontalImage.gameObject.SetActive(true);
                footVerticalImage.gameObject.SetActive(false);
                break;

            case DrawingTasks.VERTICAL:
                footGeneralImage.gameObject.SetActive(false);
                footHorizontalImage.gameObject.SetActive(false);
                footVerticalImage.gameObject.SetActive(true);
                break;
        }
    }
}
