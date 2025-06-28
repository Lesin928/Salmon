using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Singleton.Component;

/// <summary>
/// Place where an achievement will be displayed on the screen
/// </summary>
public enum AchievementStackLocation
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}

/// <summary>
/// Controls interactions with the Achievement System
/// </summary>
[System.Serializable]
public class AchievementManager : SingletonComponent<AchievementManager>
{
    [Tooltip("The number of seconds an achievement will stay on the screen after being unlocked or progress is made.")]
    public float DisplayTime = 3;
    [Tooltip("The total number of achievements which can be on the screen at any one time.")]
    public int NumberOnScreen = 3;
    [Tooltip("If true, progress notifications will display their exact progress. If false it will show the closest bracket.")]
    public bool ShowExactProgress = false;
    [Tooltip("If true, achievement unlocks/progress update notifications will be displayed on the player's screen.")]
    public bool DisplayAchievements;
    [Tooltip("The location on the screen where achievement notifications should be displayed.")]
    public AchievementStackLocation StackLocation;
    [Tooltip("If true, the state of all achievements will be saved without any call to the manual save function (Recommended = true)")]
    public bool AutoSave;
    [Tooltip("The message which will be displayed on the UI if an achievement is marked as a spoiler.")]
    public string SpoilerAchievementMessage = "Hidden";

    public List<AchievementState> States = new List<AchievementState>();                       //List of achievement states (achieved, progress and last notification)
    public List<AchievementInformation> AchievementList = new List<AchievementInformation>();  //List of all available achievements

    [Tooltip("If true, one achievement will be automatically unlocked once all others have been completed")]
    public bool UseFinalAchievement = false;
    [Tooltip("The key of the final achievement")]
    public string FinalAchievementKey;

    public AchievementStack Stack;

    #region Singleton
    protected override void AwakeInstance()
    {
        Initialize();
    }

    protected override bool InitInstance()
    {
        Stack = GetComponentInChildren<AchievementStack>();
        LoadAchievementState();
        return true;
    }

    protected override void ReleaseInstance()
    {
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        if (Instance != this)
            Destroy(gameObject);
    }
    #endregion

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

    #region Unlock and Progress
    /// <summary>
    /// Fully unlocks a progression or goal achievement.
    /// </summary>
    /// <param name="Key">The Key of the achievement to be unlocked</param>
    public void Unlock(string Key)
    {
        Unlock(FindAchievementIndex(Key));
    }
    /// <summary>
    /// Fully unlocks a progression or goal achievement.
    /// </summary>
    /// <param name="Index">The index of the achievement to be unlocked</param>
    public void Unlock(int Index)
    {
        if (!States[Index].Achieved)
        {
            States[Index].Progress = AchievementList[Index].ProgressGoal;
            States[Index].Achieved = true;
            DisplayUnlock(Index);
            AutoSaveStates();

            if (UseFinalAchievement)
            {
                int Find = States.FindIndex(x => !x.Achieved);
                bool CompletedAll = (Find == -1 || AchievementList[Find].Key.Equals(FinalAchievementKey));
                if (CompletedAll)
                {
                    Unlock(FinalAchievementKey);
                }
            }
        }
    }
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
        if (States[Index].Progress >= AchievementList[Index].ProgressGoal)
        {
            Unlock(Index);
        }
        else
        {
            States[Index].Progress = Progress;
            DisplayUnlock(Index);
            AutoSaveStates();
        }
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
        if (States[Index].Progress + Progress >= AchievementList[Index].ProgressGoal)
        {
            Unlock(Index);
        }
        else
        {
            States[Index].Progress += Progress;
            DisplayUnlock(Index);
            AutoSaveStates();
        }
    }
    #endregion

    #region Saving and Loading
    /// <summary>
    /// Saves progress and achieved states to player prefs. Used to allow reload of data between game loads. This function is automatically called if the Auto Save setting is set to true.
    /// </summary>
    public void SaveAchievementState()
    {
        var userAchievementData = UserDataManager.Instance.GetUserData<UserAchievementData>();

        if (userAchievementData != null)
        {
            userAchievementData.States = States;
            userAchievementData.AchievementList = AchievementList;
            userAchievementData.SaveData();
        }
    }
    /// <summary>
    /// Loads all progress and achievement states from player prefs. This function is automatically called if the Auto Load setting is set to true.
    /// </summary>
    public void LoadAchievementState()
    {
        var userAchievementData = UserDataManager.Instance.GetUserData<UserAchievementData>();

        if (userAchievementData != null)
        {
            States = userAchievementData.States;
            AchievementList = userAchievementData.AchievementList;
        }

        Debug.Log($"AchievementList Count: {AchievementList.Count}");
    }
    /// <summary>
    /// Clears all saved progress and achieved states.
    /// </summary>
    public void ResetAchievementState()
    {
        var userAchievementData = UserDataManager.Instance.GetUserData<UserAchievementData>();

        if (userAchievementData != null)
        {
            userAchievementData.SetDefaultData();
            userAchievementData.SaveData();
            States = userAchievementData.States;
            AchievementList = userAchievementData.AchievementList;
        }
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
    /// <summary>
    /// Test if AutoSave is valid. If true, save list
    /// </summary>
    private void AutoSaveStates()
    {
        if (AutoSave)
        {
            SaveAchievementState();
        }
    }
    /// <summary>
    /// Display achievements progress to screen  
    /// </summary>
    /// <param name="Index">Index of achievement to display</param>
    private void DisplayUnlock(int Index)
    {
        if (DisplayAchievements && !AchievementList[Index].Spoiler || States[Index].Achieved)
        {
            //If not achieved
            if (States[Index].Progress < AchievementList[Index].ProgressGoal)
            {
                int Steps = (int)AchievementList[Index].ProgressGoal / (int)AchievementList[Index].NotificationFrequency;

                //Loop through all notification point backwards from last possible option
                for (int i = Steps; i > States[Index].LastProgressUpdate; i--)
                {
                    //When it finds the largest valid notification point
                    if (States[Index].Progress >= AchievementList[Index].NotificationFrequency * i)
                    {
                        AudioManager.Instance.PlaySFX("ProgressSound");
                        States[Index].LastProgressUpdate = i;
                        Stack.ScheduleAchievementDisplay(Index);
                        return;
                    }
                }
            }
            else
            {
                AudioManager.Instance.PlaySFX("UnlockSound");
                Stack.ScheduleAchievementDisplay(Index);
            }
        }
    }
}
