using System;
using DG.Tweening;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    public static CameraShake Instance { get; private set; } = null;

    public void Shake(float power, float duraction) {

        transform.position = new(0, 0, -10);
        transform.DOShakePosition(duraction, power)
            .OnComplete(() => transform.position = new(0, 0, -10));
    }

private void Awake() {

        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
    }
}