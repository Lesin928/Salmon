using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Storesinformation related to a single achievement
/// </summary>
[System.Serializable]
public struct AchievementInfromation
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
    public List<AchievementState> States = new List<AchievementState>();                       //List of achievement states (achieved, progress and last notification)
    public List<AchievementInfromation> AchievementList = new List<AchievementInfromation>();  //List of all available achievements

    public void SetDefaultData()
    {
        States.Clear();
        for (int i = 0; i < AchievementList.Count; i++)
        {
            PlayerPrefs.DeleteKey("AchievementState_" + i);
            States.Add(new AchievementState());
        }
        SaveData();
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

    # region Miscellaneous
    /// <summary>
    /// Does an achievement exist in the list
    /// </summary>
    /// <param name="Key">The Key of the achievement to test</param>
    /// <returns>true : if exists. false : does not exist</returns>
    public bool AchievementExists(string Key)
    {
        return AchievementExists(AchievementList.FindIndex(x => x.Key.Equals(Key)));
    }
    /// <summary>
    /// Does an achievement exist in the list
    /// </summary>
    /// <param name="Index">The index of the achievement to test</param>
    /// <returns>true : if exists. false : does not exist</returns>
    public bool AchievementExists(int Index)
    {
        return Index <= AchievementList.Count && Index >= 0;
    }
    /// <summary>
    /// Returns the total number of achievements which have been unlocked.
    /// </summary>
    public int GetAchievedCount()
    {
        int Count = (from AchievementState i in States
                     where i.Achieved == true
                     select i).Count();
        return Count;
    }
    /// <summary>
    /// Returns the current percentage of unlocked achievements.
    /// </summary>
    public float GetAchievedPercentage()
    {
        if (States.Count == 0)
        {
            return 0;
        }
        return (float)GetAchievedCount() / States.Count * 100;
    }
    #endregion

    #region Progress
    /// <summary>
    /// Set the progress of an achievement to a specific value. 
    /// </summary>
    /// <param name="Key">The Key of the achievement</param>
    /// <param name="Progress">Set progress to this value</param>
    public void SetAchievementProgress(string Key, float Progress)
    {
        SetAchievementProgress(FindAchievementIndex(Key), Progress);
    }
    /// <summary>
    /// Set the progress of an achievement to a specific value. 
    /// </summary>
    /// <param name="Index">The index of the achievement</param>
    /// <param name="Progress">Set progress to this value</param>
    public void SetAchievementProgress(int Index, float Progress)
    {
        States[Index].Progress = Progress;
    }
    /// <summary>
    /// Adds the input amount of progress to an achievement. Clamps achievement progress to its max value.
    /// </summary>
    /// <param name="Key">The Key of the achievement</param>
    /// <param name="Progress">Add this number to progress</param>
    public void AddAchievementProgress(string Key, float Progress)
    {
        AddAchievementProgress(FindAchievementIndex(Key), Progress);
    }
    /// <summary>
    /// Adds the input amount of progress to an achievement. Clamps achievement progress to its max value.
    /// </summary>
    /// <param name="Index">The index of the achievement</param>
    /// <param name="Progress">Add this number to progress</param>
    public void AddAchievementProgress(int Index, float Progress)
    {
        States[Index].Progress += Progress;
    }
    #endregion

    /// <summary>
    /// Find the index of an achievement with a cetain key
    /// </summary>
    /// <param name="Key">Key of achievevment</param>
    private int FindAchievementIndex(string Key)
    {
        return AchievementList.FindIndex(x => x.Key.Equals(Key));
    }
}
