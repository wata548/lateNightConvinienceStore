
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class TextTest : MonoBehaviour {


    public TMP_Text text;

    private float interval = 0.1f;

    void StartTyping(string s) {

        text.text = "";
        StartCoroutine(Typing(s, interval));
    }

    IEnumerator Typing(string s, float interval) {

        bool tag = false;
        
        foreach (var character in s) {

            if (character == '<') {
                tag = true;
            }

            if (character == '>')
                tag = false;
            text.text += character;
            
            if(!tag)
                yield return new WaitForSeconds(interval);

        }
    }

    public MeshRenderer mesh;
    public void Awake() {

        text = GetComponent<TMP_Text>();
        
        StartTyping("slk<color=#ff0000>sdjh</color>skjlsdhf");

        
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
        
        text.ForceMeshUpdate();
        var textInfo = text.textInfo;
        int j = 0;
        foreach (var charInfo in textInfo.characterInfo) {

            if(!charInfo.isVisible) 
                continue;
            
            var vertexes = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            
            var randomVector =new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.5f, 0.5f), 0);
            for (int i = 0; i < 4; i++) {
        
                var origin = vertexes[charInfo.vertexIndex + i];
                //vertexes[charInfo.vertexIndex + i] = origin + (Mathf.Sin(time * 10 +  (float)j /3)) * Vector3.up;
                vertexes[charInfo.vertexIndex + i] = origin+ randomVector / 5f;
            }

            j++;
        }

        foreach (var n in textInfo.meshInfo) {

            n.mesh.vertices = n.vertices;
        }
    }
}