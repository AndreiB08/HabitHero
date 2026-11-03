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
    public StatisticsViewModel(HabitDatabase db) { _db = db; }
    public async Task LoadAsync()
    {
        var xp = await _db.TotalXpAsync();
        var count = await _db.TotalCompletionsAsync();
        var streak = await _db.CurrentStreakAsync();
        var habits = await _db.GetHabitsAsync();
        TotalXp = $"{xp} XP";
        Completed = count.ToString();
        Streak = $"{streak} days";
        CompletionRate = habits.Count == 0 ? "0%" : $"{Math.Min(100, count * 100.0 / Math.Max(1, habits.Count * DateTime.Today.Day)):F0}%";
        LevelProgress = (xp % 100) / 100.0;
        NextLevel = $"{100 - xp % 100} XP until the next level";
    }
    private void Changed([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
