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
                AchievementType = (AchievementType)Enum.Parse(typeof(AchievementType), data["achievement_type"].ToString()),
                AchievementName = data["achievement_name"].ToString(),
                AchievementGoal = Convert.ToInt32(data["achievement_goal"]),
                AchievementRewardAmount = Convert.ToInt32(data["achievement_reward_amount"])
            };

            AchievementDataTable.Add(achievementData);
        }
    }

    public AchievementData GetAchievementData(AchievementType achievementType)
    {
        return AchievementDataTable.Where(item => item.AchievementType == achievementType).FirstOrDefault();
    }
    #endregion
}
