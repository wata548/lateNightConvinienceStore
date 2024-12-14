using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonInteraction: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private TitleButtons buttonType= 0;
    public void SetIndex(TitleButtons type) => buttonType = type;
    
    public void OnPointerEnter(PointerEventData eventData) {

        ButtonManager.Instance.EnterCursor(buttonType);
    }

    public void OnPointerExit(PointerEventData eventData) {

        ButtonManager.Instance.ExitCursor(buttonType);
    }
}