using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TitleButtons {
    
    None,
    Start,
    Exit
}

public class ButtonInfo {
    
    public readonly GameObject Button;
    public readonly TMP_Text Text;
    public readonly Vector3 DefaultSize;

    public ButtonInfo(GameObject button) {

        Button = button;
        Text = button.GetComponentInChildren<TMP_Text>();
        DefaultSize = button.transform.localScale;
    }
}
public class ButtonManager : MonoBehaviour {
    public static ButtonManager Instance { get; private set; } = null;
    
    //==================================================||Field 
    
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject exitButton;

    private Dictionary<TitleButtons, ButtonInfo> match = new(); 
    
    private TitleButtons estimateSelect = TitleButtons.None;
    private TitleButtons beforeEstimate = TitleButtons.None;
    private bool animationStart = false;

    private Tween beforeAnimation = null;

    private const float Increase = 1.3f;
    private const float AnimationDuraction = 0.3f;

    private readonly static Color ActiveColor = Color.red;
    private readonly static Color DefaultColor = Color.black;
    
    //==================================================||Method 

    public void EnterCursor(TitleButtons type) {

        beforeEstimate = estimateSelect;
        estimateSelect = type;
        
        animationStart = true;
    }

    public void ExitCursor(TitleButtons type) {

        if (estimateSelect != type)
            return;

        beforeEstimate = estimateSelect;
        estimateSelect = TitleButtons.None;
        animationStart = true;

    }

    private Tween DecereseAnimation(TitleButtons type) {
        if (type == TitleButtons.None)
            return null;
        
        var target = match[type];
        
        var button = target.Button;
        var size = target.DefaultSize;
        var text = target.Text;

        text.color = DefaultColor;
        return button.transform.DOScale(size, 0);
    }
    private Tween IncreaseAnimation(TitleButtons type) {

        if (type == TitleButtons.None)
            return null;
        
        var target = match[type];

        var button = target.Button;
        var size = target.DefaultSize;
        var text = target.Text;
        
        text.color = ActiveColor;
        return button.transform.DOScale(size * Increase, AnimationDuraction);
    }
    
    public void ButtonInteraction() {

        if (!animationStart)
            return;

        if (estimateSelect == beforeEstimate) {

            animationStart = false;
            return;
        }
        
        if (beforeAnimation != null)
            beforeAnimation.Kill();

        DecereseAnimation(beforeEstimate);
        beforeAnimation = IncreaseAnimation(estimateSelect);
    }
   
    //==================================================||Unity Logic 
    void Awake() {

        Instance ??= this;

        var startText = startButton.GetComponentInChildren<TextMeshPro>();
        var exitText = exitButton.GetComponentInChildren<TextMeshPro>();
        match.Add(TitleButtons.Start, new(startButton));
        match.Add(TitleButtons.Exit, new(exitButton));
        
        startButton.AddComponent<ButtonInteraction>().SetIndex(TitleButtons.Start);
        exitButton.AddComponent<ButtonInteraction>().SetIndex(TitleButtons.Exit);
    }

    void Update() {
        ButtonInteraction();
    }
}