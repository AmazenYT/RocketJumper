using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider SFXSlider;

    public void setSFXVolume()
    {
        float volume = SFXSlider.value;
        myMixer.SetFloat("SFX", volume);
    }


}


