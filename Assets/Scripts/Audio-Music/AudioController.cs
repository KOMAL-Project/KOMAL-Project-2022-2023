using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public static Dictionary<string, float> volume = new Dictionary<string, float>(){{"Music", 1f}, {"SFX", 1f}};
    [SerializeField] private string audioName;
    [SerializeField] private AudioMixer mixer;
    
    void Start() {
        GetComponent<UnityEngine.UI.Slider>().value = volume[audioName];
        setVolume(volume[audioName]);
    }

    public void setVolume(float sliderValue) {
        mixer.SetFloat(audioName + " Volume", Mathf.Log10(sliderValue) * 30);
        volume[audioName] = sliderValue;
    }


}
