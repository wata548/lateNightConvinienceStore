using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ShoutingSkill : MonoBehaviour {

    public static ShoutingSkill Instance { get; private set; } = null;

    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject shout;

    private Queue<GameObject> disactive = new();

    private const float startX = -125;
    private const float endX = 400;
    private const float startY = -370;
    private const float endY = -425;
    private const float showTime = 2f;
    private const float appearTime = 0.5f;
    private bool isSkill = false;
    
    private IEnumerator MakeShout(string context) {

        GameObject target;
        
        if (disactive.Count > 0) {

            target = disactive.Dequeue();
        }
        else {
            target = Instantiate(shout, canvas.transform);
        }
        
        var newPos = new Vector2(Random.Range(startX, endX), Random.Range(startY, endY));
        target.GetComponent<RectTransform>().localPosition = newPos;
        target.GetComponent<Image>().DOBlink(appearTime, showTime, appearTime, 0.8f)
            .OnComplete(() => disactive.Enqueue(target));

        var text = target.GetComponentInChildren<TMP_Text>();
        text.text = context;
        text.DOBlink(appearTime - 0.1f, showTime, appearTime - 0.1f, 0.8f)
            .DOBeforeWait(0.1f);

        yield return new WaitForSeconds(showTime + 2 * appearTime);

        if (isSkill) {
            
            StartCoroutine(MakeShout(context));
        }
    }

    public void StartSkill() {

        isSkill = true;
        StartCoroutine(MakeShout("context"));
    }

    public void EndSkill() {
        isSkill = false;
    }

    private void Awake() {

        if (Instance == null)
            Instance = this;
            
        else if (Instance != this)
            Destroy(this);
    }
}