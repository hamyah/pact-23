using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class XmasButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
    void Start() {
        if (!eventDate()) {
            gameObject.SetActive(false);
        } else {
            StartAnimation();
        }

    }

    Boolean eventDate() {
        int day = DateTime.Today.Day;
        int month = DateTime.Today.Month;

        return month == 12 || (month == 1 && day <= 15);
    }

    void StartAnimation() {
        LeanTween.scale(gameObject, Vector3.one * 1.1f, 0.5f).setLoopPingPong();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        transform.localScale = Vector3.one;
        LeanTween.cancel(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData) {
        StartAnimation();
    }
}
