public enum AchievementKey
{
    REACH_THE_TOP,
    REACH_THE_TOP_FIVE_TIMES,
    PLAY_FOR_TEN_HOURS,
    SPEEDRUNNER
}

public class AchievementData
{
    public AchievementKey AchievementKey;
    public string AchievementName;
    public string AchievementDescription;
    public string LockedIcon;
    public string AchievedIcon;
    public int AchievementGoal;
    public int NotificationFrequency;
    public string Suffix;
    public bool IsHidden;
}
