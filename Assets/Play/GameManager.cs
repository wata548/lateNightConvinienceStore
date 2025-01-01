using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;

enum ProcedureState {

    Hello,
    ShowPrice,
    EndChoicing,
    CalCulate,
    Result
};

public class GameManager : MonoBehaviour {

    [SerializeField] private TMP_Text customerName;
    [SerializeField] private TMP_Text dialog;
    [SerializeField] private GameObject box;

    private ProcedureState state = ProcedureState.Hello; 
    public int Day { get; private set; } = 1;
    public int CustomerIndex { get; private set; } = 0;

    private int dialogSize;
    private int currentDialog;
    
    private List<string> customers;
    private Dialog commu;

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
    private bool isSettingHello = false;
    private void Update() {

        if (state == ProcedureState.Hello && !isSettingHello) {
            
            customers = new Shuffler<string>(ConvertJson.Instance.PeopleList.Skip(1)).ToList();
            
            commu = ConvertJson.Instance.GetDialog(customers[CustomerIndex], $"Hello{Day}");
            currentDialog = 0;
            dialogSize = commu.Scripts.Count;
            isSettingHello = true; 
            
            Communication();
        }
        
        if ( state == ProcedureState.Hello&& (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))) {

            if (typing) {
                
                typing = false;
            }
            else {
                
                Communication();

                if (!isCommunication) {
                    state++;
                }
            }
        }

        if (state == ProcedureState.ShowPrice) {

            if (!isSettingItem) {
                
                isSettingItem = true;
                ShowPrice.Instance.Setting(Day, customers[CustomerIndex]);
            }

            if (ShowPrice.Instance.StartShow() == ShowState.End) {
                isSettingItem = false;
                state++;
            }
        }
        
    }
}