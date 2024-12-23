using System;
using System.Linq;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ShowPrice: MonoBehaviour {

    //==================================================||Field 
    [SerializeField] private GameObject priceBox = null;
    private TMP_Text price;
    private Image priceBoxBack;
    [SerializeField] private string characterName;
    private int day = 1;
    private bool showing = false;
    
    [SerializeField] private List<MatchItemAndCount> purchaseList; 
    
    //==================================================||Method 
    private void Setting() {

        var character = ConvertJson.Instance.GetCharacter(characterName);

        MatchItemAndCount[] tempPurchaseList = day switch {

            1 => character.PurchaseList1,
            2 => character.PurchaseList2,
            3 => character.PurchaseList3,
            _ => throw new Exception("Game is end, but customer visit")
        };
        
        Shuffler<MatchItemAndCount> shuffle = new(tempPurchaseList);
        purchaseList = shuffle.ToList();
    }

    private int index = 0;
    public void StartShow() {

        if (index >= purchaseList.Count || showing)
            return;
        
        price.text = purchaseList[index++].Name;

        showing = true;
        priceBoxBack.DOBlink(0.8f, 1.5f, 0.8f)
            .OnComplete(() => showing = false);
        price.DOBlink(0.7f, 1.4f, 0.7f)
            .DOWait(0.1f);
    }
    
    //==================================================||Unity Logic 
    
    private void Start() {
        if (priceBox == null)
            throw new Exception("You didn't setting price box");

        price = priceBox.GetComponentInChildren<TMP_Text>();
        if (price == null)
            throw new Exception("This price box is not correct");

        priceBoxBack = priceBox.GetComponent<Image>();

        Setting();
    }
    
    //Test
    private void Update() {

        if (Input.GetKeyDown(KeyCode.Space)) {
            StartShow();
        }
    }
}