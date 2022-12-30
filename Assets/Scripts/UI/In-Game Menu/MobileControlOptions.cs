using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileControlOptions : MonoBehaviour
{
    public static Dictionary<string, float> controls = new Dictionary<string, float>(){{"Transparency", 0.7f}, {"Scale", 0.3f}, {"Side", 0f}};
    [SerializeField] private string controlName;
    private GameObject controller;
    private RectTransform controllerTransform, pauseRt;
    private Image[] imgs;
    private Vector2 pivotLeft = new Vector2(-4.5f, -4.5f), pivotRight = new Vector2(5.5f, -4.5f);
    CameraScript cs;
    DieOverlayController doc;



    void Awake()
    {
        Debug.Log(gameObject.name + " " + transform.parent.gameObject.name);
        controller = GameObject.FindGameObjectWithTag("D-Pad");
        pauseRt = controller.transform.parent.GetChild(0).gameObject.GetComponent<RectTransform>();
        cs = Camera.main.gameObject.GetComponentInParent<CameraScript>();
        doc = GameObject.FindGameObjectWithTag("DieOverlay").GetComponent<DieOverlayController>();
        controllerTransform = controller.GetComponent<RectTransform>();
        imgs = controller.GetComponentsInChildren<Image>();
        
        changeProperty(controls[controlName]);
        GetComponent<UnityEngine.UI.Slider>().value = controls[controlName];
    }


    public void changeProperty(float sliderValue) {
        //sets the static value to slider value
        controls[controlName] = sliderValue;

        if (controlName == "Transparency") {
            //gets each image of the controls
            imgs = controller.GetComponentsInChildren<Image>();
            Color tempColor = imgs[0].color;
            tempColor.a = sliderValue;
            //changes it's transparency to that slider vlaue
            for (int i = 0; i < imgs.Length; i++) {
                imgs[i].color = tempColor;

            }
        }

        else if (controlName == "Scale") {
            //sets scale to slider value
            controllerTransform.localScale = new Vector3(sliderValue, sliderValue, 1);
        }

        else if (controlName == "Side") {
            
            if (sliderValue == 1f) { //right side
                controllerTransform.anchorMin = controllerTransform.anchorMax = Vector2.right;
                controllerTransform.pivot = pivotRight;
                pauseRt.anchorMin = pauseRt.anchorMax = new Vector2(0, 1);
                pauseRt.anchoredPosition = new Vector2(pauseRt.anchoredPosition.x * -1, pauseRt.anchoredPosition.y);
                cs.SetOffset(Vector3.left);
                doc.SetSide(Vector2.left);
            }
            else { //left side
                controllerTransform.anchorMin = controllerTransform.anchorMax = Vector2.zero;
                controllerTransform.pivot = pivotLeft;
                pauseRt.anchorMin = pauseRt.anchorMax = new Vector2(1, 1);
                pauseRt.anchoredPosition = new Vector2(pauseRt.anchoredPosition.x * -1, pauseRt.anchoredPosition.y);
                cs.SetOffset(Vector3.right);
                doc.SetSide(Vector2.right);
            }
        }
    }
   

}
