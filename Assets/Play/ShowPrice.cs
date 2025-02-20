using System;
using System.Linq;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using UnityEngine.UI;

public enum ShowState {

    On,
    Stay,
    End
};

public class ShowPrice: MonoBehaviour {

   //==================================================||Property 
   
    public static ShowPrice Instance { get; private set; } = null;
    
    //==================================================||Field 

    [SerializeField] private SpriteRenderer item;
    [SerializeField] private GameObject priceBox = null;
    [SerializeField] private SpriteRenderer eventBox = null;

    private TMP_Text eventInfo = null;
    private List<MatchItemAndCount> purchaseList; 
    private TMP_Text price = null;
    private SpriteRenderer priceBoxBack;
    private bool showing = false;
    private int index = 0;

    //==================================================||Method 

    public int Setting(int day, string characterName) {

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

        var result = purchaseList.Sum(item => {

            int price = ConvertJson.Instance.GetPrice(item.Name);
            
            //Check unevent item
            int count = item.Number * item.Event / (1 + item.Event);
            if (item.Event == 0)
                count = item.Number;

            return count * price;
        });
        index = 0;

        return result;

    }

    public ShowState StartShow(float stay = 1.8f, float power = 1, float appear = 0.9f, float disappear = 0.9f) {

        if (showing)
            return ShowState.Stay;

        if (index >= purchaseList.Count || showing) {

            item.sprite = null;
            return ShowState.End;
        }
        
        
        string purchaseItem = purchaseList[index].Name;
        int count = purchaseList[index].Number;
        int itemPrice = ConvertJson.Instance.GetPrice(purchaseItem);
        int eventCount = purchaseList[index].Event;
        index++;

        item.sprite = ConvertJson.Instance.GetImage(purchaseItem); 
        price.text = $"{purchaseItem}  {itemPrice}원  {count}개";
        eventInfo.text = $"{eventCount} + 1 행사 제품";
        
        showing = true;
        priceBoxBack.DOBlink(appear, stay, disappear, power)
            .OnComplete(() => showing = false);

        if (eventCount != 0) {

            eventBox.DOBlink(appear, stay, disappear, power);
            eventInfo.DOBlink(appear, stay, disappear, power);
        }
        price.DOBlink(appear - 0.1f, stay, disappear - 0.1f, power)
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

        priceBoxBack = priceBox.GetComponent<SpriteRenderer>();

        if (eventBox == null)
            throw new Exception("please set event Box on inspector");
        
        eventInfo = eventBox.GetComponentInChildren<TMP_Text>();
        if (eventInfo == null)
            throw new Exception("wrong object. please check \"eventBox\"component");
    }
}