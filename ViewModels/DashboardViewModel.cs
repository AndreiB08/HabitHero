using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HabitHero.Models;
using HabitHero.Services;

namespace HabitHero.ViewModels;

public class DashboardViewModel : INotifyPropertyChanged
{
    private readonly HabitDatabase _db;
    private readonly MotivationService _motivation = new();
    private int _completed;
    private int _total;
    private int _xp;
    private int _streak;
    private string _message = "Small steps every day lead to big changes.";
    public event PropertyChangedEventHandler? PropertyChanged;
    public ObservableCollection<Habit> Habits { get; } = new();
    public string Greeting => DateTime.Now.Hour < 12 ? "Good morning! 👋" : DateTime.Now.Hour < 18 ? "Good afternoon! 👋" : "Good evening! 👋";
    public string CurrentDate => DateTime.Now.ToString("dddd, MMMM d");
    public int Completed { get => _completed; set { _completed = value; Changed(); Changed(nameof(ProgressText)); Changed(nameof(ProgressPercentage)); Changed(nameof(Progress)); } }
    public int Total { get => _total; set { _total = value; Changed(); Changed(nameof(ProgressText)); Changed(nameof(ProgressPercentage)); Changed(nameof(Progress)); } }
    public double Progress => Total == 0 ? 0 : (double)Completed / Total;
    public string ProgressText => $"{Completed} of {Total} completed";
    public string ProgressPercentage => $"{Progress:P0}";
    public int Xp { get => _xp; set { _xp = value; Changed(); Changed(nameof(XpText)); Changed(nameof(LevelText)); } }
    public string XpText => $"{Xp} XP";
    public string LevelText => $"Level {Xp / 100 + 1}";
    public int Streak { get => _streak; set { _streak = value; Changed(); Changed(nameof(StreakText)); } }
    public string StreakText => Streak == 1 ? "1 day" : $"{Streak} days";
    public string Message { get => _message; set { _message = value; Changed(); } }

    public DashboardViewModel(HabitDatabase db) { _db = db; }

    public async Task LoadAsync()
    {
        Habits.Clear();
        var habits = await _db.GetHabitsAsync();
        var logs = await _db.GetLogsAsync(DateTime.Today);
        var completedIds = logs.Select(x => x.HabitId).ToHashSet();
        foreach (var habit in habits)
        {
            habit.IsCompletedToday = completedIds.Contains(habit.Id);
            Habits.Add(habit);
        }
        Total = habits.Count;
        Completed = completedIds.Count;
        Xp = await _db.TotalXpAsync();
        Streak = await _db.CurrentStreakAsync();
        Message = await _motivation.GetDailyMessageAsync();
    }

    public async Task ToggleAsync(Habit habit)
    {
        await _db.ToggleAsync(habit.Id, DateTime.Today);
        await LoadAsync();
    }

    private void Changed([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
