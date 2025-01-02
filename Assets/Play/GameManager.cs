using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine.Serialization;

[Serializable]
public enum ProcedureState {

    Hello,
    ShowPrice,
    EndOrder,
    Calculate,
    Result
};

public class GameManager : MonoBehaviour {

    [SerializeField] private TMP_Text customerName;
    [SerializeField] private TMP_Text dialog;
    [SerializeField] private GameObject box;

    public ProcedureState State { get; private set; } = ProcedureState.Hello; 
    public int Day { get; private set; } = 1;
    public int CustomerIndex { get; private set; } = 0;

    private int dialogSize;
    private int currentDialog;
    
    private List<string> customers;
    private Dialog commu;

    private int sumPrice = 0;
    
    private const float DefaultPriceShowTime = 1.8f;
    private const float SkillPriceShowTime = 1f;
    private float priceShowTime = DefaultPriceShowTime;
    private const float DefaultPriceShowPower = 1; 
    private const float SkillPriceShowPower = 0.8f;
    private float priceShowPower = DefaultPriceShowPower;
    
    private bool isCommunication = false;
    private bool typing = false;
    IEnumerator Typing(TMP_Text dialog, string context, float interval = 0.1f, int index = 0) {

        if (index == 0) {
            
            typing = true;
            dialog.text = "";
        }
        else if (!typing) {

            dialog.text = context;
            yield break;
        }
        else if (index == context.Length) {

            typing = false;
            yield break;
        }
        dialog.text += context[index];
        yield return new WaitForSeconds(interval);

        StartCoroutine(Typing(dialog, context, interval, index + 1));
    }
    
    private void Communication() {

        if (currentDialog >= dialogSize) {

            box.SetActive(false);
            isCommunication = false;
            return;
        }

        if (!isCommunication) {

            box.SetActive(true);
            isCommunication = true;
        }

        if (commu.Actors[commu.Scripts[currentDialog].Actor] == "Event") {
            
            //TODO: Event procedure
            currentDialog++;
        }
        
        customerName.text = commu.Actors[commu.Scripts[currentDialog].Actor];
        StartCoroutine(Typing(dialog, commu.Scripts[currentDialog].Script));

        currentDialog++;
    }
    
    private void NextDay() {
        
        if (CustomerIndex < 2)
            return;
        CustomerIndex = 0;
        Day++;

        customers = new Shuffler<string>(customers).ToList();
    }

    public void NextCustomer() {
        CustomerIndex++;
    }
    

    private bool isSettingItem = false;
    private bool isSettingCommunication = false;
    private bool isUseSkill = false;
    private void Update() {

        if (State == ProcedureState.Hello && !isSettingCommunication) {
            
            customers = new Shuffler<string>(ConvertJson.Instance.PeopleList.Skip(1)).ToList();
            
            commu = ConvertJson.Instance.GetDialog(customers[CustomerIndex], $"Hello{Day}");
            currentDialog = 0;
            dialogSize = commu.Scripts.Count;
            isSettingCommunication = true; 
            
            Communication();
        }
        
        if ( State == ProcedureState.Hello&& (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))) {
            
            if (typing) {
                
                typing = false;
            }
            else {
                
                Communication();

                if (!isCommunication) {
                    State++;
                    isSettingCommunication = false;
                }
            }
        }

        if (State == ProcedureState.ShowPrice) {

            if (!isUseSkill) {
                isUseSkill = true;

                var customer = customers[CustomerIndex];
                if (customer == "단소 할아버지") {
                    if (Day == 2)
                        DansoSkill.Instance.StartSkill();
                    if (Day == 3) 
                        ShoutingSkill.Instance.StartSkill();
                }
                else if (customer == "부자 아줌마" && Day == 3) {

                    priceShowPower = SkillPriceShowPower;
                }
                else if (customer == "초딩" && Day == 2) {

                    priceShowTime = SkillPriceShowTime;
                }
            }
            
            if (!isSettingItem) {
                
                isSettingItem = true;
                sumPrice = ShowPrice.Instance.Setting(Day, customers[CustomerIndex]);
            }

            if (ShowPrice.Instance.StartShow(priceShowTime, priceShowPower) == ShowState.End) {

                
                
                priceShowPower = DefaultPriceShowPower;
                priceShowTime = DefaultPriceShowTime;
                ShoutingSkill.Instance.EndSkill();
                DansoSkill.Instance.EndSkill();
                
                isSettingItem = false;
                State++;
            }
        }

        if (State == ProcedureState.EndOrder) {

            if (!isSettingCommunication) {
                
                commu = ConvertJson.Instance.GetDialog(customers[CustomerIndex], $"endOrder{Day}");
                currentDialog = 0;
                dialogSize = commu.Scripts.Count;
                isSettingCommunication = true;
                Communication();
            }
            else if(Input.GetKeyDown(KeyCode.Space)) {
                if (typing) {
                    typing = false;
                }
                else {
                    Communication();

                    if (!isCommunication) {
                        State++;
                        isCommunication = false;
                    }
                }
                
            }
        }
        
    }
}