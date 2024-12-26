using System;
using System.Linq;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum ShowState {

    On,
    Off,
    End
};

public class ShowPrice: MonoBehaviour {

   //==================================================||Property 
   
    public static ShowPrice Instance { get; private set; } = null;
    
    //==================================================||Field 
    
    [SerializeField] private GameObject priceBox = null;
    
    private List<MatchItemAndCount> purchaseList; 
    private TMP_Text price;
    private Image priceBoxBack;
    private bool showing = false;
    private int index = 0;

    public int result = 0;
    
    //==================================================||Method 

    private void Setting(int day, string characterName) {

        var character = ConvertJson.Instance.GetCharacter(characterName);

        MatchItemAndCount[] tempPurchaseList = day switch {

            1 => character.PurchaseList1,
            2 => character.PurchaseList2,
            3 => character.PurchaseList3,
            _ => throw new Exception("Game is end, but customer visit")
        };
        
        Shuffler<MatchItemAndCount> shuffle = new(tempPurchaseList);
        purchaseList = shuffle
            .ToList();

        result = purchaseList.Sum(item => {

            int price = ConvertJson.Instance.GetPrice(item.Name);
            
            //Check unevent item
            int count = item.Number * item.Event / (1 + item.Event);
            if (item.Event == 0)
                count = item.Number;

            return count * price;
        });
             
    }

    public ShowState StartShow(float appear = 0.9f, float stay = 1.8f, float disappear = 0.9f, float power = 0) {

        if (showing)
            return ShowState.Off;

        if (index >= purchaseList.Count || showing)
            return ShowState.End;

        string puchaseItem = purchaseList[index].Name;
        int count = purchaseList[index].Number;
        int itemPrice = ConvertJson.Instance.GetPrice(puchaseItem);
        index++;
        
        price.text = $"{puchaseItem}  {itemPrice}원  {count}개";

        showing = true;
        priceBoxBack.DOBlink(appear, stay, disappear)
            .OnComplete(() => showing = false);
        price.DOBlink(appear - 0.1f, stay, disappear - 0.1f)
            .DOBeforeWait(0.1f);

        return ShowState.On;
    }
    
    //==================================================||Unity Logic 

    private void Awake() {

        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
    }

    private void Start() {
        if (priceBox == null)
            throw new Exception("You didn't setting price box");

        price = priceBox.GetComponentInChildren<TMP_Text>();
        if (price == null)
            throw new Exception("This price box is not correct");

        priceBoxBack = priceBox.GetComponent<Image>();

        Setting(3,"초딩");
    }
    
    //Test
    private void Update() {

        if (Input.GetKeyDown(KeyCode.Space)) {
            StartShow();
            Debug.Log(ConvertJson.Instance.GetScript("단소 할아버지", "firstCommunication", 0));
        }
    }
}