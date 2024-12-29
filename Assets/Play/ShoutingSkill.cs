using System;
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

    private Queue<GameObject> unactive = new();

    private readonly float startX = -125;
    private readonly float endX = 400;
    private readonly float startY = -370;
    private readonly float endY = -425;

    public void StartSkill(string context) {

        GameObject target;
        if (unactive.Count > 0) {

            target = unactive.Dequeue();
        }
        else {
            target = Instantiate(shout, canvas.transform);
        }
        
        var newPos = new Vector2(Random.Range(startX, endX), Random.Range(startY, endY));
        target.GetComponent<RectTransform>().localPosition = newPos;
        target.GetComponent<Image>().DOBlink(0.5f, 3f, 0.3f, 0.8f)
            .OnComplete(() => unactive.Enqueue(target));

        var text = target.GetComponentInChildren<TMP_Text>();
        text.text = context;
        text.DOBlink(0.4f, 3, 0.2f, 0.8f)
            .DOBeforeWait(0.1f);

    }

    private void Awake() {

        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
    }
}