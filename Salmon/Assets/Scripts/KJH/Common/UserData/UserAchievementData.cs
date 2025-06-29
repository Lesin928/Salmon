using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Storesinformation related to a single achievement
/// </summary>
[System.Serializable]
public struct AchievementInformation
{
    [Tooltip("Name used to unlock/set achievement progress")]
    [SerializeField] public string Key;
    [Tooltip("The display name for an achievement. Shown to the user on the UI.")]
    [SerializeField] public string DisplayName;
    [Tooltip("Description for an achievement. Shown to the user on the UI.")]
    [SerializeField] public string Description;
    [Tooltip("The icon which will be displayed when the achievement is locked")]
    [SerializeField] public Sprite LockedIcon;
    [Tooltip("The icon which will be displayed when the achievement is  Achieved")]
    [SerializeField] public Sprite AchievedIcon;
    [Tooltip("Treat the achievement as a spoiler for the game. Hidden from player until unlocked.")]
    [SerializeField] public bool Spoiler;
    [Tooltip("The goal which must be reached for the achievement to unlock.")]
    [SerializeField] public float ProgressGoal;
    [Tooltip("The rate that progress updates will be displayed on the screen e.g. Progress goal = 100 and Notification Frequency = 25. In this example, the progress will be displayed at 25,50,75 and 100.")]
    [SerializeField] public float NotificationFrequency;
    [Tooltip("A string which will be displayed with a progress achievement e.g. $, KM, Miles etc")]
    [SerializeField] public string ProgressSuffix;
}

/// <summary>
/// Stores the current progress and achieved state
/// </summary>
[System.Serializable]
public class AchievementState
{
    public AchievementState(float NewProgress, bool NewAchieved)
    {
        Progress = NewProgress;
        Achieved = NewAchieved;
    }
    public AchievementState() { }

    [SerializeField] public float Progress;             //Progress towards goal
    [SerializeField] public int LastProgressUpdate = 0; //Last achievement notification bracket
    [SerializeField] public bool Achieved = false;      //Is the achievement unlocked
}

public class UserAchievementData : IUserData
{
    public List<AchievementState> States { get; set; } = new List<AchievementState>();                       //List of achievement states (achieved, progress and last notification)
    public List<AchievementInformation> AchievementList { get; set; } = new List<AchievementInformation>();  //List of all available achievements

    private const string ACHIEVEMENT_PATH = "Achievement";

    public void SetDefaultData()
    {
        AchievementList.Clear();
        AchievementList = DataTableManager.Instance.GetAchievementDataList().Select(x => new AchievementInformation
        {
            Key = x.AchievementKey.ToString(),
            DisplayName = x.AchievementName,
            Description = x.AchievementDescription,
            LockedIcon = Resources.Load<Sprite>($"KJH_Resources/{ACHIEVEMENT_PATH}/{x.LockedIcon}"),
            AchievedIcon = Resources.Load<Sprite>($"KJH_Resources/{ACHIEVEMENT_PATH}/{x.AchievedIcon}"),
            Spoiler = x.IsHidden,
            ProgressGoal = x.AchievementGoal,
            NotificationFrequency = x.NotificationFrequency,
            ProgressSuffix = x.Suffix
        }).ToList();

        States.Clear();
        for (int i = 0; i < AchievementList.Count; i++)
        {
            PlayerPrefs.DeleteKey("AchievementState_" + i);
            States.Add(new AchievementState());
        }
    }

    public bool LoadData()
    {
        bool result = false;

        try
        {
            AchievementState NewState;
            States.Clear();

            for (int i = 0; i < AchievementList.Count; i++)
            {
                //Ensure that new project get default values
                if (PlayerPrefs.HasKey("AchievementState_" + i))
                {
                    NewState = JsonUtility.FromJson<AchievementState>(PlayerPrefs.GetString("AchievementState_" + i));
                    States.Add(NewState);
                }
                else
                {
                    States.Add(new AchievementState());
                }
            }

            result = true;
        }
        catch (Exception e)
        {
            Debug.Log($"Load failed. (" + e.Message + ")");
        }

        return result;
    }

    public bool SaveData()
    {
        bool result = false;

        try
        {
            for (int i = 0; i < States.Count; i++)
            {
                PlayerPrefs.SetString("AchievementState_" + i, JsonUtility.ToJson(States[i]));
            }
            PlayerPrefs.Save();

            result = true;
        }
        catch (Exception e)
        {
            Debug.Log($"Save failed. (" + e.Message + ")");
        }

        return result;
    }
}
