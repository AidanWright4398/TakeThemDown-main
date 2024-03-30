using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OptionScript : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Dropdown resolutionDropDown, qualityDropDown;

    private Resolution[] resolutions;

    // Start is called before the first frame update
    void Start()
    {
        resolutions = Screen.resolutions;

        List<string> options = new List<string>();

        int currentScreenResolutionId = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string res = resolutions[i].width + " x " + resolutions[i].width;
            options.Add(res);

            if (Screen.currentResolution.width == resolutions[i].width
                && Screen.currentResolution.height == resolutions[i].height)
            {
                currentScreenResolutionId = i;
            }
        }

        resolutionDropDown.ClearOptions();
        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = currentScreenResolutionId;

        qualityDropDown.ClearOptions();
        qualityDropDown.AddOptions(QualitySettings.names.ToList());
        qualityDropDown.value = QualitySettings.GetQualityLevel();

        qualityDropDown.onValueChanged.AddListener(SetQuality);
        resolutionDropDown.onValueChanged.AddListener(SetResolution);
    }

    public void SetResolution(int resolutionId)
    {
        Resolution res = resolutions[resolutionId];

        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void SetQuality(int qualityId)
    {
        QualitySettings.SetQualityLevel(qualityId);
    }
}
