using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public static float volume = 0.5f;
    [SerializeField] private AudioMixer mixer;
    // Start is called before the first frame update
    void Start() {
        GetComponent<UnityEngine.UI.Slider>().value = volume;
    }
    public void setVolume(float sliderValue) {
        mixer.SetFloat("Master Volume", Mathf.Log10(sliderValue) * 20);
        volume = sliderValue;
    }


}
