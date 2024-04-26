using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ElectricalStimulationManager : MonoBehaviour
{
    private const int ACK_INTERVAL_MILISECONDS = 500;

    [Header("Switching Circuit")]
    [SerializeField]
    private string switchingCircuitPortName = "COM4";
    [SerializeField]
    private int switchingCircuitBaudRate = 9600;

    [Header("Electrical Stimulator")]
    [SerializeField]
    private string electricalStimulatorPortName = "COM4";
    [SerializeField]
    private int electricalStimulatorBaudRate = 9600;

    private SerialHandler switchingCircuit;
    private SerialHandler electricalStimulator;

    private bool isElectricalStimulationOn = false;
    private DateTime lastACKSentAt = new DateTime();

    int resendCnt = 0;
    int resendGVal = 0;
    int resendPin1 = 0;
    int resendPin2 = 0;
    int resendPin3 = 0;

    void Awake()
    {
        switchingCircuit = new SerialHandler(switchingCircuitPortName, switchingCircuitBaudRate);
        electricalStimulator = new SerialHandler(electricalStimulatorPortName, electricalStimulatorBaudRate);

        switchingCircuit.Open();
        electricalStimulator.Open();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isElectricalStimulationOn)
        {
            DateTime now = DateTime.Now;
            if ((now - lastACKSentAt).TotalMilliseconds > ACK_INTERVAL_MILISECONDS)
            {
                electricalStimulator.Write("a\n");
                lastACKSentAt = now;
            }
        }

        if (resendCnt > 0)
        {
            if (resendCnt % 20 == 0)
            {
                electricalStimulator.Write($"{resendGVal}\n");
            }
            else if (resendCnt % 20 == 5)
            {
                switchingCircuit.Write($"A{resendPin1}\n");
            }
            else if (resendCnt % 20 == 10)
            {
                switchingCircuit.Write($"A{resendPin2}\n");
            }
            else if (resendCnt % 20 == 15)
            {
                switchingCircuit.Write($"A{resendPin3}\n");
            }

            resendCnt--;
        }
    }

    void OnDestroy()
    {
        switchingCircuit.Close();
        electricalStimulator.Close();
    }

    public void StartElectricalStimulation(List<int> activeElectrodes, int gVal)
    {
        foreach (int activeElectrode in activeElectrodes)
        {
            switchingCircuit.Write($"A{activeElectrode}\n");
        }


        electricalStimulator.Write($"a\n");
        // PauseForMiliSeconds(100);
        electricalStimulator.Write($"{gVal}\n");


        isElectricalStimulationOn = true;

        resendGVal = gVal;
        resendPin1 = activeElectrodes[0];
        resendPin2 = activeElectrodes[1];
        resendPin3 = activeElectrodes[2];

        Debug.Log($"{resendPin1}, {resendPin2}, {resendPin3}, {resendGVal}");

        resendCnt = 70;
    }

    public void StopElectricalStimulation()
    {
        for (var i=0; i<3; i++)
        {
            electricalStimulator.Write("0\n");
            PauseForMiliSeconds(10);
        }

        for (var i=0; i<3; i++)
        {
            switchingCircuit.Write("R\n");
            PauseForMiliSeconds(10);
        }

        isElectricalStimulationOn = false;
    }

    private IEnumerator PauseForMiliSeconds(int miliSeconds)
    {
        DateTime startTime = DateTime.Now;

        while ((DateTime.Now - startTime).TotalMilliseconds > miliSeconds)
        {
            yield return null;
        }
    }
}
