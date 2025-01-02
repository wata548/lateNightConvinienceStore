using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class DansoSkill : MonoBehaviour {

    public static DansoSkill Instance { get; private set; } = null;
    private const int SummonCount = 3;

    private List<GameObject> dansos = new();
    [SerializeField] private GameObject dansoPrefab;

    public void StartSkill() {

        for (int i = 0; i < SummonCount; i++) {

            dansos.Add(Instantiate(dansoPrefab));
        }
    }

    public void EndSkill() {

        foreach (var danso in dansos) {

            Destroy(danso);
        }

        dansos = new();
    }

    private void Awake() {

        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
    }
}