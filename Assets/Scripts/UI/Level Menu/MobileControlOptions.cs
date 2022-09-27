using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileControlOptions : MonoBehaviour
{
    public static Dictionary<string, float> controls = new Dictionary<string, float>(){{"Transparency", 0.7f}, {"Scale", 0.6f}, {"Side", 0f}};
    [SerializeField] private string controlName;
    private GameObject controller;
    private RectTransform controllerTransform;
    private Image[] imgs;
    [SerializeField] private Vector2 pivotLeft, pivotRight;

    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("D-Pad");
        controllerTransform = controller.GetComponent<RectTransform>();
        imgs = controller.GetComponentsInChildren<Image>();

        GetComponent<UnityEngine.UI.Slider>().value = controls[controlName];
        changeProperty(controls[controlName]);
        controllerTransform.position = new Vector2(0,0);
    }

    public void changeProperty(float sliderValue) {
        if (controlName == "Transparency") {

            imgs = controller.GetComponentsInChildren<Image>();
            Color tempColor = imgs[0].color;
            tempColor.a = sliderValue;

            for (int i = 0; i < imgs.Length; i++) {
                imgs[i].color = tempColor;

            }
        }

        else if (controlName == "Scale") {
            controllerTransform.localScale = new Vector3(sliderValue, sliderValue, 1);
        }

        else if (controlName == "Side") {

            if (sliderValue == 1f) { //right side
                controllerTransform.anchorMin = Vector2.right;
                controllerTransform.anchorMax = Vector2.right;
                controllerTransform.pivot = pivotRight;
                //some reason this code below moves it -2160 for some reason???? so -2160 + 1860 to compensate???? 
                controllerTransform.position = new Vector2(1860, 0); //hard-coded +300 distance from right side
                
            }
            else { //left side
                controllerTransform.anchorMin = Vector2.zero;
                controllerTransform.anchorMax = Vector2.zero;
                controllerTransform.pivot = pivotLeft;
                controllerTransform.position = Vector3.zero;
            }
        }
    }

}
