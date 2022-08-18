using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ToggleButtonUIController : MonoBehaviour
{
    bool value = false;
    [SerializeField] Sprite on, off, pressed;
    Image img;

    private void Start()
    {
        img = gameObject.GetComponent<Image>();
    }

    public void Press()
    {
        img.sprite = pressed;
    }

    public void Release()
    {
        value = !value;
        img.sprite = value ? on : off;
    }
}
