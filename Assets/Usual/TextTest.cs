using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class TextTest : MonoBehaviour {

    [Serializable]
    enum Effect {

        Flow,
        Shake,
        None
    };
    
    

    public TMP_Text text;

    private float interval = 0.1f;

    void StartTyping(string s) {

        text.text = "";
        StartCoroutine(Typing(s, interval));
    }

    IEnumerator Typing(string context, float interval) {

        bool tag = false;

        foreach (var character in context) {

            if (character == '<' || character == '[') {
                tag = true;
            }

            if (character == '>' || character == ':')
                tag = false;
            text.text += character;

            if (!tag) {
                MoveText(text);
                yield return new WaitForSeconds(interval);
            }

        }
    }

    public MeshRenderer mesh;
    public void Awake() {

        text = GetComponent<TMP_Text>();
        
        StartTyping($"slk{AddEffect(Effect.Flow)}{AddColor(Color.red)}sdjh{ExtractColor()}{ExtractEffect()}skjlsdhf {AddEffect(Effect.Shake)}shakeshake{ExtractEffect()}");
        
    }

    private float time = 0;

    private Dictionary<Effect, Func<float, Vector3>> match = new() {
        { Effect.Flow, index => new Vector3(0, Mathf.Sin(index * 3f + Time.time) / 4f)},
        { Effect.Shake, index => new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.5f, 0.5f), 0) / 5f },

    };
    public void MoveText(TMP_Text text) {


        bool isEffectTypeTyping = false;
        string effectType = "";
        Effect currentEffect = Effect.None;
        
        text.ForceMeshUpdate();
        var textInfo = text.textInfo;

        int index = 0;
        bool effect = false;
        
        foreach (var character in textInfo.characterInfo) {

            if(!character.isVisible) 
                continue;

            if (isEffectTypeTyping) {
                if (character.character == ':') {
                    isEffectTypeTyping = false;

                    currentEffect = (Effect)Enum.Parse(typeof(Effect),  effectType);
                    continue;
                }

                effectType += character.character;
                continue;
            }    

            if (character.character == '[') {
                effectType = "";
                effect = true;
                isEffectTypeTyping= true;
                continue;
            }

            if (character.character == ']' && effect) {
                currentEffect = Effect.None;
                effect = false;
            }
            
            if (!effect) {
                continue;
            }
            var vertices = textInfo.meshInfo[character.materialReferenceIndex].vertices;

            var move = match[currentEffect](index / 2f + Time.time);
            for (int vertex = 0; vertex < 4; vertex++) {

                var tempIndex = character.vertexIndex + vertex;
                
                var origin = vertices[tempIndex];
                vertices[tempIndex] = origin + move;
            }
            index++;
        }

        foreach (var mesh in textInfo.meshInfo) {

            mesh.mesh.vertices = mesh.vertices;
        }

    }

    string AddColor(Color color) {

        Debug.Log(color.ToString());
        return $"<color=#{ToHex(color.r)}{ToHex(color.g)}{ToHex(color.b)}{ToHex(color.a)}>";

        string ToHex(float factor) {

            int value = (int)(factor * 255);
            return Convert.ToString(value, 16).PadLeft(2, '0');
        }
    }

    /// <summary>
    /// Color format example: "#ff00ff" or "#ffffffdd"
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    string AddColor(string color) {

        return $"<color={color}>";
    }

    string ExtractColor() {
        return "</color>";
    }

    string AddEffect(Effect effect) {

        return $"<size=0%>[{effect.ToString()}:</size>";
    }

    string ExtractEffect() {
        return "<size=0%>]</size>";
    }
    
    public void Update() {

        if (Input.GetKeyDown(KeyCode.T)) {

            int length = text.text.Length;
            int invisible = text.textInfo.characterInfo
                .Count(character => !character.isVisible);

            Debug.Log(length - invisible);
        }
        
        time += Time.deltaTime;

        bool check = false;
        if (time > 3 / 60f) {

            time = 0;
            check = true;
        }

        if (!check)
            return;

        MoveText(text);
    }
}