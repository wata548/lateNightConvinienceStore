using System;
using UnityEngine;

public class Customer : MonoBehaviour {

    [SerializeField] private SpriteRenderer customer;
    [SerializeField] private Sprite dansoDefault;
    [SerializeField] private Sprite chodingDefault;
    [SerializeField] private Sprite chodingSurprise;
    [SerializeField] private Sprite richDefault;
    [SerializeField] private Sprite richAngry;
    [SerializeField] private Sprite richHappy;

    public static Customer Instance { get; private set; } = null;
    
    public void SetCustomer(string state) {

        customer.sprite = state switch {
            "단소 할아버지" => dansoDefault,
            "Danso" => dansoDefault,
            "초딩" => chodingDefault,
            "Choding" => chodingDefault,
            "ChodingSurprise" => chodingSurprise,
            "부자 아줌마" => richDefault,
            "Rich" => richDefault,
            "RichHappy" => richHappy,
            "RichAngry" => richAngry,
            _ => null
        };
    }

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
    }
}