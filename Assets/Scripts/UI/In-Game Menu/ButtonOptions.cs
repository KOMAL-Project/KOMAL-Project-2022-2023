using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOptions : MonoBehaviour
{
    public static Dictionary<string, bool> buttonControls = new Dictionary<string, bool>() {{"FPS", true}, {"Side", false}};
    [SerializeField] private string controlName;
    private GameObject controller;
    private RectTransform controllerTransform, pauseRt;
    private Vector2 pivotLeft = new Vector2(-4.5f, -4.5f), pivotRight = new Vector2(5.5f, -4.5f);
    CameraScript cs;
    DieOverlayController doc;
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("D-Pad");
        cs = Camera.main.gameObject.GetComponentInParent<CameraScript>();
        if (controlName == "Side") doc = GameObject.FindGameObjectWithTag("DieOverlay").GetComponent<DieOverlayController>();

        if (controller)
        {
            pauseRt = controller.transform.parent.GetChild(0).gameObject.GetComponent<RectTransform>();
            controllerTransform = controller.GetComponent<RectTransform>();
        }


        changeProperty(buttonControls[controlName]);
        GetComponent<Toggle>().isOn = buttonControls[controlName];
    }

    public void changeProperty(bool toggleValue)
    {
        buttonControls[controlName] = toggleValue;

        if (controlName == "FPS") 
        {
            Application.targetFrameRate = toggleValue ? 60 : 30;
            Debug.Log(toggleValue ? 60 : 30);
        }
        else if (controlName == "Side") 
        {
            
            if (toggleValue) 
            { //right side
                controllerTransform.anchorMin = controllerTransform.anchorMax = Vector2.right;
                controllerTransform.pivot = pivotRight;
                pauseRt.anchorMin = pauseRt.anchorMax = new Vector2(0, 1);
                pauseRt.anchoredPosition = new Vector2(pauseRt.anchoredPosition.x * -1, pauseRt.anchoredPosition.y);
                cs.SetOffset(Vector3.left);
                doc.SetSide(Vector2.left);
            }
            else 
            { //left side
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
