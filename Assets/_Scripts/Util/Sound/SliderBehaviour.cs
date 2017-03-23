﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBehaviour : MonoBehaviour {
    
    private Slider slider;

	// Use this for initialization
	void Start () {
        slider = gameObject.GetComponentInParent<Slider>();
        if (slider.name.Contains("SFX")) {
            slider.value = SoundManager.instance.GetSFXVolume();
        }
        else {
            slider.value = SoundManager.instance.GetMusicVolume();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    /// <summary>
    /// call the sound manager's method
    /// </summary>
    /// <param name="vol"></param>
    public void SetMusicVolume(float vol = 0.8f) {
        SoundManager.instance.SetMusicVolume(vol);
        SetSliderPosition(vol);
    }

    /// <summary>
    /// call the sound manager's method
    /// </summary>
    /// <param name="vol"></param>
    public void SetSFXVolume(float vol = 1f) {
        SoundManager.instance.SetSFXVolume(vol);
        SetSliderPosition(vol);
    }

    /// <summary>
    /// Make the slider show the correct value
    /// </summary>
    public void SetSliderPosition(float pos) {
        slider.value = pos;
    }
}
