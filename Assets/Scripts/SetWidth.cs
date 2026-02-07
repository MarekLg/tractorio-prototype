using System;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SetWidth : MonoBehaviour
{
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Set(float width)
    {
        rectTransform.localScale = new Vector3(width, 1f, 1f);
    }
}