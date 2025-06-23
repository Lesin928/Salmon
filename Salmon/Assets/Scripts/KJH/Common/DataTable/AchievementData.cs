public enum AchievementType
{
    CollectGold,
    ClearChapter1,
    ClearChapter2,
    ClearChapter3,
}

public class AchievementData
{
    public AchievementType AchievementType;
    public string AchievementName;
    public string AchievementDescription;
    public string LockedIcon;
    public string AchievedIcon;
    public int AchievementGoal;
    public int NotificationFrequency;
    public string Suffix;
    public bool IsHidden;
}
