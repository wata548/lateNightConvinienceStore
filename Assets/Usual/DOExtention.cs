using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static  class DOExtention {

    public static Tween DOBlink(this SpriteRenderer target, float appear = 0, float stay = 0, float disappear = 0) {

        if (appear < 0 || stay < 0 || disappear < 0)
            throw new ArgumentOutOfRangeException(" function's parameter should to over 0(because it is time" + 
                                                  $"AppearTime: {appear}, StayTime: {stay}, DisappearTime: {disappear}");
        
        var procedure = DOTween.Sequence()
            .Append(target.DOFade(endValue: 1, duration: appear))
            .AppendInterval(interval: stay)
            .Append(target.DOFade(endValue: 0, duration: disappear));

        return procedure;
    }

    public static Tween DOBlink(this Image target, float appear = 0, float stay = 0, float disappear = 0) {
       
        if (appear < 0 || stay < 0 || disappear < 0)
            throw new ArgumentOutOfRangeException(" function's parameter should to over 0(because it is time" + 
                                                  $"AppearTime: {appear}, StayTime: {stay}, DisappearTime: {disappear}");
        
        var procedure = DOTween.Sequence()
            .Append(target.DOFade(endValue: 1, duration: appear))
            .AppendInterval(interval: stay)
            .Append(target.DOFade(endValue: 0, duration: disappear));
        
        return procedure;
    }
    
    public static Tween DOBlink(this TMP_Text target, float appear = 0, float stay = 0, float disappear = 0) {
           
        if (appear < 0 || stay < 0 || disappear < 0)
            throw new ArgumentOutOfRangeException(" function's parameter should to over 0(because it is time" + 
                                                  $"AppearTime: {appear}, StayTime: {stay}, DisappearTime: {disappear}");
            
        var procedure = DOTween.Sequence()
            .Append(target.DOFade(endValue: 1, duration: appear))
            .AppendInterval(interval: stay)
            .Append(target.DOFade(endValue: 0, duration: disappear));
            
        return procedure;
    }

    public static Tween DOWait(this Tween target, float time) {
        
        var result = DOTween.Sequence() 
            .AppendInterval(time)
            .Append(target);

        return result;
    }
}