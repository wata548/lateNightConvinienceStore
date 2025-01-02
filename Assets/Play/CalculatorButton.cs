using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CalculatorButton: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] private char value;
    private bool on = false;
    public void OnPointerEnter(PointerEventData eventData) {

        on = true;
    }

    public void OnPointerExit(PointerEventData eventData) {

        on = false;
    }

    private void Update() {

        if (on && Input.GetMouseButtonDown(0)) {
            
            if (value == 'b')
                Calculator.Instance.Erase();
            else if (value == 'c')
                Calculator.Instance.Erase(true);
            else if (value == 'd')
                Calculator.Instance.MultipleValue(100);
            else if (value == 's') {
                
                
            }
            else {
                Calculator.Instance.AddValue((int)value - 48);
            }
        }
    }
}