
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HabitHero.Services;

namespace HabitHero.ViewModels;

public class StatisticsViewModel : INotifyPropertyChanged
{
    private readonly HabitDatabase _db;
    private string _xp = "0 XP";
    private string _completed = "0";
    private string _streak = "0 days";
    private string _rate = "0%";
    private double _levelProgress;
    private string _nextLevel = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string TotalXp { get => _xp; set { _xp = value; Changed(); } }
    public string Completed { get => _completed; set { _completed = value; Changed(); } }
    public string Streak { get => _streak; set { _streak = value; Changed(); } }
    public string CompletionRate { get => _rate; set { _rate = value; Changed(); } }
    public double LevelProgress { get => _levelProgress; set { _levelProgress = value; Changed(); } }
    public string NextLevel { get => _nextLevel; set { _nextLevel = value; Changed(); } }

    public ObservableCollection<WeeklyActivityItem> WeeklyActivity { get; } = new();
    public ObservableCollection<CategoryActivityItem> CategoryDistribution { get; } = new();
    public ObservableCollection<XpProgressItem> XpProgress { get; } = new();

    public StatisticsViewModel(HabitDatabase db)
    {
        _db = db;
    }

    public async Task LoadAsync()
    {
        var xp = await _db.TotalXpAsync();
        var count = await _db.TotalCompletionsAsync();
        var streak = await _db.CurrentStreakAsync();
        var habits = await _db.GetHabitsAsync();

        TotalXp = $"{xp} XP";
        Completed = count.ToString();
        Streak = streak == 1 ? "1 day" : $"{streak} days";
        CompletionRate = habits.Count == 0 ? "0%" : $"{Math.Min(100, count * 100.0 / Math.Max(1, habits.Count * DateTime.Today.Day)):F0}%";
        LevelProgress = (xp % 100) / 100.0;
        NextLevel = $"{100 - xp % 100} XP until the next level";

        var weeklyData = await _db.GetWeeklyActivityAsync();
        WeeklyActivity.Clear();

        foreach (var item in weeklyData)
        {
            WeeklyActivity.Add(new WeeklyActivityItem
            {
                Day = item.Day,
                Count = item.Count,
                Progress = weeklyData.Max(x => x.Count) == 0 ? 0 : (double)item.Count / weeklyData.Max(x => x.Count)
            });
        }

        var categoryData = await _db.GetCategoryDistributionAsync();
        CategoryDistribution.Clear();

        foreach (var item in categoryData)
        {
            CategoryDistribution.Add(new CategoryActivityItem
            {
                Category = item.Category,
                Count = item.Count,
                Progress = categoryData.Max(x => x.Count) == 0 ? 0 : (double)item.Count / categoryData.Max(x => x.Count)
            });
        }

        var xpData = await _db.GetXpProgressAsync();
        XpProgress.Clear();

        foreach (var item in xpData)
        {
            XpProgress.Add(new XpProgressItem
            {
                Date = item.Date,
                Xp = item.Xp,
                Progress = xpData.Max(x => x.Xp) == 0 ? 0 : (double)item.Xp / xpData.Max(x => x.Xp)
            });
        }
    }

    private void Changed([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class WeeklyActivityItem
{
    public string Day { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Progress { get; set; }
}

public class CategoryActivityItem
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Progress { get; set; }
}

public class XpProgressItem
{
    public string Date { get; set; } = string.Empty;
    public int Xp { get; set; }
    public double Progress { get; set; }
}
