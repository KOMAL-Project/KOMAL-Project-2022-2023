using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ToggleButtonUIController : MonoBehaviour
{
    public bool value = false, isPressed = false;
    [SerializeField] public Sprite on, off, pressed;
    public Image img;
    DirectionalButtonController dbc;
    private void Start()
    {
        dbc = transform.parent.gameObject.GetComponent<DirectionalButtonController>();
        img = gameObject.GetComponent<Image>();
    }

    public void Press()
    {
        isPressed = true;
    }

    public void Release()
    {
        isPressed = false;
    }

    public void SetImage(bool tValue)
    {
        img.sprite = tValue ? on : off;
    }
}
