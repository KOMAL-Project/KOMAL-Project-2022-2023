using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileControlOptions : MonoBehaviour
{
    public static Dictionary<string, float> controls = new Dictionary<string, float>(){{"Transparency", 0.7f}, {"Scale", 0.3f}, {"Side", 0f}};
    [SerializeField] private string controlName;
    private GameObject controller;
    private RectTransform controllerTransform;
    private Image[] imgs;
    [SerializeField] private Vector2 pivotLeft, pivotRight;
    CameraScript cs;

    void Start()
    {
        
        controller = GameObject.FindGameObjectWithTag("D-Pad");
        cs = Camera.main.gameObject.GetComponentInParent<CameraScript>();
        controllerTransform = controller.GetComponent<RectTransform>();
        imgs = controller.GetComponentsInChildren<Image>();

        changeProperty(controls[controlName]);
        GetComponent<UnityEngine.UI.Slider>().value = controls[controlName];
        cs.SetOffset(controls["Side"] == 1f ? Vector3.left : Vector3.right);
        controllerTransform.position = controls["Side"] == 1 ? controllerTransform.position = Vector2.zero : new Vector2(Screen.width - (900 * controllerTransform.localScale.x), 0);
    }

    public void changeProperty(float sliderValue) {
        controls[controlName] = sliderValue;
        if (controlName == "Transparency") {
            controls["Transparency"] = sliderValue;
            imgs = controller.GetComponentsInChildren<Image>();
            Color tempColor = imgs[0].color;
            tempColor.a = sliderValue;

            for (int i = 0; i < imgs.Length; i++) {
                imgs[i].color = tempColor;

            }
        }

        else if (controlName == "Scale") {
            controllerTransform.localScale = new Vector3(sliderValue, sliderValue, 1);
            controls["Scale"] = sliderValue;
        }

        else if (controlName == "Side") {
            
            if (sliderValue == 1f) { //right side
                controllerTransform.anchorMin = Vector2.right;
                controllerTransform.anchorMax = Vector2.right;
                controllerTransform.pivot = pivotRight;
                controllerTransform.position = new Vector2(Screen.width - (900*controllerTransform.localScale.x), 0); 
                controls["Side"] = sliderValue;
                cs.SetOffset(Vector3.left);
            }
            else { //left side
                controllerTransform.anchorMin = Vector2.zero;
                controllerTransform.anchorMax = Vector2.zero;
                controllerTransform.pivot = pivotLeft;
                controllerTransform.position = Vector2.zero;
                controls["Side"] = sliderValue;
                cs.SetOffset(Vector3.right);

            }
        }
    }

}
