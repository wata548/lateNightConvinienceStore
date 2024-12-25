using System;
using TMPro;
using UnityEngine;

public class Calculator : MonoBehaviour {
   
    [SerializeField] private GameObject calculator;
    [SerializeField] private TMP_Text calculatorNumber;

    private const long MaxValue = (long)1e+16;
    
    public void TurnOn() => calculator.SetActive(true);
    public void TurnOff() => calculator.SetActive(false);

    [Obsolete("Never touch this value, you must touch by Estimate", false)]
    private long estimate;

    public long Estimate {

        get => estimate;

        private set {

            estimate = value;
            calculatorNumber.text = estimate.ToString();
        }
    }

    public void AddValue(int up) {

        long testValue = Estimate * 10 + up;
        if (testValue >= MaxValue)
            //TODO: Show error mesege(digit limit is 15)
            return;

        Estimate = testValue;

    }

    public void MultipleValue(int up) {
        
        long testValue = Estimate * up;
        if (testValue >= MaxValue)
            //TODO: Show error mesege(digit limit is 15)
            return;
        
        Estimate = testValue;
    }

    public void Erase(bool all = false) {

        if (all)
            Estimate = 0;

        else
            Estimate /= 10;
    }
    
}