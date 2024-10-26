using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public GameObject settingsMenuPanel;

    public Dropdown resolutionDropdown;

    public Toggle fullscreenToggle;
    
    Resolution[] customResolutions = new Resolution[]
    {
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 1280, height = 720 },
        new Resolution { width = 1600, height = 900 }
    };

    void Start ()
    {
        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;
        List<string> options = new List<string>();

        for (int i = 0; i < customResolutions.Length; i++)
        {
            string option = customResolutions[i].width + "x" + customResolutions[i].height;
            options.Add(option);

            if (customResolutions[i].width == Screen.currentResolution.width && customResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(delegate {
            setResolution(resolutionDropdown.value);
        });

        fullscreenToggle.isOn = Screen.fullScreen;

        fullscreenToggle.onValueChanged.AddListener(setFullScreen);
    }

    public void setResolution (int resolutionIndex)
    {
        Resolution resolution = customResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void setFullScreen (bool isFullScreen) 
    {
        Screen.fullScreen = isFullScreen;
            if (!isFullScreen)
            {
                Screen.SetResolution(1280, 720, false);
            }
    }

}
