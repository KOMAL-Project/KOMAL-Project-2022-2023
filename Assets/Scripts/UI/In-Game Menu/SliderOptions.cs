using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderOptions : MonoBehaviour
{
    public static Dictionary<string, float> sliderControls = new Dictionary<string, float>(){{"Transparency", 0.7f}, {"Scale", 0.3f}, {"Side", 0f}};
    [SerializeField] private string controlName;
    private GameObject controller;
    private RectTransform controllerTransform;
    private Image[] imgs;
    

    void Awake()
    {
        //Debug.Log(gameObject.name + " " + transform.parent.gameObject.name);
        controller = GameObject.FindGameObjectWithTag("D-Pad");

        if (controller)
        {
            controllerTransform = controller.GetComponent<RectTransform>();
            imgs = controller.GetComponentsInChildren<Image>();

        }

        changeProperty(sliderControls[controlName]);
        GetComponent<UnityEngine.UI.Slider>().value = sliderControls[controlName];
    }


    public void changeProperty(float sliderValue) 
    {
        //sets the static value to slider value
        sliderControls[controlName] = sliderValue;

        if (controlName == "Transparency") 
        {
            //gets each image of the controls
            Color tempColor = imgs[0].color;
            tempColor.a = sliderValue;
            //changes it's transparency to that slider vlaue
            for (int i = 0; i < imgs.Length; i++) 
            {
                imgs[i].color = tempColor;

            }
        }

        else if (controlName == "Scale") 
        {
            //sets scale to slider value
            controllerTransform.localScale = new Vector3(sliderValue, sliderValue, 1);
        }

        
    }
   

}
