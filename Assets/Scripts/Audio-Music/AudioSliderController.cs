using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSliderController : MonoBehaviour
{
    public static Dictionary<string, float> volume = new Dictionary<string, float>(){{"Music", 0.3f}, {"SFX", 0.5f}};
    [SerializeField] private string audioName;
    [SerializeField] private AudioMixer mixer;
    
    void Start() {
        GetComponent<UnityEngine.UI.Slider>().value = volume[audioName];
        SetVolume(volume[audioName]);
    }

    public void SetVolume(float sliderValue) {

        float mixerVolume = sliderValue > 0.01 ? Remap(Mathf.Log10(sliderValue), -2, 0, -50, -10): -80;
        mixer.SetFloat(audioName + " Volume", mixerVolume);
        volume[audioName] = sliderValue;
    }


    public static float Remap(float from, float fromMin, float fromMax, float toMin,  float toMax) //I stole it from the internet
    {
        var fromAbs  =  from - fromMin;
        var fromMaxAbs = fromMax - fromMin;      
       
        var normal = fromAbs / fromMaxAbs;
 
        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;
 
        var to = toAbs + toMin;
       
        return to;
    }

}
