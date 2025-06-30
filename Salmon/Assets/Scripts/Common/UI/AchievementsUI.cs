using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsUI : UIBase
{
    [SerializeField] private GameObject scrollContent;
    [SerializeField] private GameObject prefab;
    [SerializeField] private TMP_Dropdown Filter;
    [SerializeField] private TextMeshProUGUI CountText;
    [SerializeField] private TextMeshProUGUI CompleteText;
    [SerializeField] private Scrollbar Scrollbar;

    public override void SetInfo(UIBaseData uiData)
    {
        base.SetInfo(uiData);
        AddAchievements("All");
    }

    /// <summary>
    /// Adds all achievements to the UI based on a filter
    /// </summary>
    /// <param name="Filter">Filter to use (All, Achieved or Unachieved)</param>
    private void AddAchievements(string Filter)
    {
        foreach (Transform child in scrollContent.transform)
        {
            Destroy(child.gameObject);
        }
        AchievementManager AM = AchievementManager.Instance;
        int AchievedCount = AM.GetAchievedCount();

        CountText.text = "" + AchievedCount + " / " + AM.States.Count;
        CompleteText.text = "Complete (" + AM.GetAchievedPercentage() + "%)";

        for (int i = 0; i < AM.AchievementList.Count; i++)
        {
            if ((Filter.Equals("All")) || (Filter.Equals("Achieved") && AM.States[i].Achieved) || (Filter.Equals("Unachieved") && !AM.States[i].Achieved))
            {
                AddAchievementToUI(AM.AchievementList[i], AM.States[i]);
            }
        }
        Scrollbar.value = 1;
    }

    public void AddAchievementToUI(AchievementInformation Achievement, AchievementState State)
    {
        AchievementItem UIAchievement = Instantiate(prefab, new Vector3(0f, 0f, 0f), Quaternion.identity).GetComponent<AchievementItem>();
        UIAchievement.Set(Achievement, State);
        UIAchievement.transform.SetParent(scrollContent.transform);
    }
    /// <summary>
    /// Filter out a set of locked or unlocked achievements
    /// </summary>
    public void ChangeFilter()
    {
        AddAchievements(Filter.options[Filter.value].text);
    }
}
