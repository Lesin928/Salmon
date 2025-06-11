using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : UIBase
{
    [SerializeField] private TMP_Dropdown ResDropDown;
    [SerializeField] private Toggle FullScreenToggle;

    private Resolution[] AllResolutions;
    private bool m_IsFullScreen;
    private int m_SelectedResolution;
    private List<Resolution> SelectedResolutionList = new List<Resolution>();

    private void Start()
    {
        m_IsFullScreen = true;
        AllResolutions = Screen.resolutions;

        List<string> resolutionStringList = new List<string>();
        string newRes;
        foreach (Resolution res in AllResolutions)
        {
            newRes = res.width.ToString() + "x" + res.height.ToString();
            if (!resolutionStringList.Contains(newRes))
            {
                resolutionStringList.Add(newRes);
                SelectedResolutionList.Add(res);
            }
        }

        ResDropDown.AddOptions(resolutionStringList);
    }

    public void SetResolution()
    {
        m_SelectedResolution = ResDropDown.value;
        Screen.SetResolution(SelectedResolutionList[m_SelectedResolution].width,
                            SelectedResolutionList[m_SelectedResolution].height,
                            m_IsFullScreen);
    }

    public void ChangeFullScreen()
    {
        m_IsFullScreen = FullScreenToggle.isOn;
        Screen.fullScreen = m_IsFullScreen;
    }
}
