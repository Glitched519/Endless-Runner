﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {

    public Slider volumeSlider;
    public Dropdown resolutionDropdown;

    //public Toggle toggle = GameObject.FindGameObjectsWithTag("CameraShakeToggle").GetComponent<Toggle>();
    //public Toggle toggle;
    public static GameObject gameobject; 
    public Toggle toggle; 

    Resolution[] resolutions;

    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
            string option =resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        toggle = GameObject.FindGameObjectWithTag("ShakeToggle").GetComponent<Toggle>();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetVolume()
    {
        AudioListener.volume = volumeSlider.value * 1;
    }

    public void SetCameraShake(bool isShaking)
    {
        CameraShake.active = isShaking;
        //toggle.isOn = isShaking;
        
    }

}
