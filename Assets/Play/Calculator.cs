using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Calculator : MonoBehaviour {

    [SerializeField] private GameObject calculator;
    [SerializeField] private TMP_Text calculatorNumber;
    [FormerlySerializedAs("interactCalculator")] [SerializeField] private Sprite[] activeNumber;
    [SerializeField] private Sprite activeDelete;
    [SerializeField] private Sprite activeEnter;
    [SerializeField] private Sprite defaultCalculator;
    [SerializeField] private SpriteRenderer renderer;
    
    public static Calculator Instance { get; private set; } = null;

    private const long MaxValue = (long)1e+10;
    
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

    IEnumerator InteractButton(Sprite sprite, float time = 0.3f) {

        renderer.sprite = sprite;
        yield return new WaitForSeconds(time);

        if (renderer.sprite == sprite) {
            renderer.sprite = defaultCalculator;
        }
    }
    
    public void AddValue(int up) {

        StartCoroutine(InteractButton(activeNumber[up]));
        
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

        else {
            StartCoroutine(InteractButton(activeDelete));
            Estimate /= 10;
        }
    }

    private readonly float waitStartBackSpace = 0.5f;
    private readonly float waitBackSpace = 0.1f;
    private float backSpace = 0;
    private bool startBackSpace = false;
    
    private void Update() {

        if (Input.GetKeyDown(KeyCode.P))
            ShoutingSkill.Instance.StartSkill("hello");
        
        if (!calculator.activeSelf)
            return;
        
        for (int i = 0; i <= 9; i++) {

            if (Input.GetKeyDown(i.ToString())) {

                AddValue(i);
            }
        }

        if (Input.GetKeyDown(KeyCode.C)) {

            Erase(true);
        }
        
        if (Input.GetKey(KeyCode.Backspace)) {
            if (backSpace == 0)
                Erase();
            
            backSpace += Time.deltaTime;

            bool first = !startBackSpace && backSpace > waitStartBackSpace;
            bool after = startBackSpace && backSpace > waitBackSpace;
            
            if (first || after) {
                startBackSpace = true;
                Erase();
                backSpace = 0;
            }
        }
            
        else if (Input.GetKeyUp(KeyCode.Backspace)) {

            backSpace = 0;
            startBackSpace = false;
        }
    }

    private void Awake() {

        if (Instance == null)
            Instance = this;
        
        else if (Instance != this)
            Destroy(this);
    }
}