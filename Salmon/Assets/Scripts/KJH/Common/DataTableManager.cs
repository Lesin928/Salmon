using System.Collections.Generic;
using System;
using UnityEngine;
using Singleton.Component;
using System.Linq;

public class DataTableManager : SingletonComponent<DataTableManager>
{
    private const string DATA_PATH = "DataTable";

    #region Singleton
    protected override void AwakeInstance()
    {
        Initialize();
    }

    protected override bool InitInstance()
    {
        LoadAchievementDataTable();
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

    #region ACHIEVEMENT_DATA
    private const string ACHIEVEMENT_DATA_TABLE = "AchievementDataTable";
    private List<AchievementData> AchievementDataTable = new List<AchievementData>();

    public List<AchievementData> GetAchievementDataList()
    {
        return AchievementDataTable;
    }

    private void LoadAchievementDataTable()
    {
        var parsedDataTable = CSVReader.Read($"{DATA_PATH}/{ACHIEVEMENT_DATA_TABLE}");

        foreach (var data in parsedDataTable)
        {
            var achievementData = new AchievementData
            {
                AchievementKey = (AchievementKey)Enum.Parse(typeof(AchievementKey), data["achievement_key"].ToString()),
                AchievementName = data["achievement_name"].ToString(),
                AchievementDescription = data["achievement_description"].ToString(),
                LockedIcon = data["locked_icon"].ToString(),
                AchievedIcon = data["achieved_icon"].ToString(),
                AchievementGoal = Convert.ToInt32(data["achievement_goal"]),
                NotificationFrequency = Convert.ToInt32(data["notification_frequency"]),
                Suffix = data["suffix"].ToString(),
                IsHidden = Convert.ToBoolean(data["is_hidden"])
            };

            AchievementDataTable.Add(achievementData);
        }
    }

    public AchievementData GetAchievementData(AchievementKey achievementKey)
    {
        return AchievementDataTable.Where(item => item.AchievementKey == achievementKey).FirstOrDefault();
    }
    #endregion
}
