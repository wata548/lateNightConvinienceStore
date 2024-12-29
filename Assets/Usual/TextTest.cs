using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class TextTest : MonoBehaviour {

    public TMP_Text text;
    public void Awake() {

        text = GetComponent<TMP_Text>();
        
        TextManager.Instance.StartTyping(text,$"Sample1:{TextManager.Instance.AddEffect(Effect.Flow, TextManager.Instance.AddColor(Color.red, "RedFlow"))}\n"
            + $"Smaple2 {TextManager.Instance.AddEffect(Effect.Shake, TextManager.Instance.AddColor(Color.blue,"BlueShake"))}");
        
    }

    private float time = 0;
    public void Update() {

        time += Time.deltaTime;

        bool check = false;
        if (time > 3 / 60f) {

            time = 0;
            check = true;
        }

        if (!check)
            return;

        TextManager.Instance.EffectProcedure(text);
    }
}