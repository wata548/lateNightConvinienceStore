using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class DansoMovement : MonoBehaviour{
    private Rigidbody2D rigidbody2D;
    
    private void Awake() {
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = 0;
    }

    private void Start() {

        CameraShake.Instance.Shake(0.1f, 0.1f);
        disappear = false;
        transform.Rotate(new Vector3(0, 0, Random.Range(0, 360f)));
        transform.localPosition = new Vector3(Random.Range(-6f, 6f), Random.Range(-3f, 1.7f), -1f);
        var degree = transform.rotation.eulerAngles.z;
        
        var direction = new Vector2(Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad));
        if(Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
            direction.x *= -1;
        else
            direction.y *= -1;
        direction = direction.normalized;
        rigidbody2D.linearVelocity = direction;
    }

    private bool disappear = false;

    public float time = 0;
    private void FixedUpdate() {

        time += Time.fixedDeltaTime;
        if (!disappear && time >= 1.1f) {

            time = 0;
            disappear = true;
            
            gameObject.GetComponent<SpriteRenderer>().DOFade(0, 0.15f)
                .OnComplete(() => {
                    Start();
                    GetComponent<SpriteRenderer>().DOFade(1, 0.1f);
                });
        }
        

    }
}