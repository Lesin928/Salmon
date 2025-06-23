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
    public int AchievementGoal;
    public int AchievementRewardAmount;
}
