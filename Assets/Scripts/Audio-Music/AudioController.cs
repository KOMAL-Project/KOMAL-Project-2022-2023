using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public static Dictionary<string, float> volume = new Dictionary<string, float>(){{"Music", 0f}, {"SFX", 1f}};
    [SerializeField] private string audioName;
    [SerializeField] private AudioMixer mixer;
    
    void Start() {
        GetComponent<UnityEngine.UI.Slider>().value = volume[audioName];
        SetVolume(volume[audioName]);
    }

    public void SetVolume(float sliderValue) {

        float mixerVolume = sliderValue > 0.01 ? Mathf.Log10(sliderValue) : -80;
        mixer.SetFloat(audioName + " Volume", mixerVolume);
        volume[audioName] = sliderValue;
    }


}
