using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HabitHero.Services;

namespace HabitHero.ViewModels;

public class CalendarViewModel : INotifyPropertyChanged
{
    private readonly HabitDatabase _db;
    private DateTime _date = DateTime.Today;
    private string _summary = string.Empty;
    public event PropertyChangedEventHandler? PropertyChanged;
    public DateTime SelectedDate { get => _date; set { _date = value; Changed(); _ = LoadAsync(); } }
    public string Summary { get => _summary; set { _summary = value; Changed(); } }
    public ObservableCollection<string> Entries { get; } = new();
    public CalendarViewModel(HabitDatabase db) { _db = db; }
    public async Task LoadAsync()
    {
        Entries.Clear();
        var logs = await _db.GetLogsAsync(SelectedDate);
        var habits = await _db.GetHabitsAsync();
        foreach (var log in logs)
        {
            var habit = habits.FirstOrDefault(x => x.Id == log.HabitId);
            if (habit is not null) Entries.Add($"{habit.Icon}  {habit.Name}");
        }
        Summary = $"{Entries.Count} habits completed on {SelectedDate:dd MMM yyyy}";
    }
    private void Changed([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
