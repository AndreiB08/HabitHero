
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HabitHero.Models;
using HabitHero.Services;

namespace HabitHero.ViewModels;

public class HabitsViewModel : INotifyPropertyChanged
{
    private readonly HabitDatabase _db;
    private readonly SemaphoreSlim _loadLock = new(1, 1);
    private string _search = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<Habit> Habits { get; } = new();

    public string SearchText
    {
        get => _search;
        set
        {
            if (_search == value)
                return;

            _search = value;
            Changed();
            _ = LoadAsync();
        }
    }

    public HabitsViewModel(HabitDatabase db)
    {
        _db = db;
    }

    public async Task LoadAsync()
    {
        await _loadLock.WaitAsync();

        try
        {
            var search = SearchText;
            var items = await _db.GetHabitsAsync();

            Habits.Clear();

            foreach (var item in items)
            {
                if (item.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    item.Category.Contains(search, StringComparison.OrdinalIgnoreCase))
                {
                    Habits.Add(item);
                }
            }
        }
        finally
        {
            _loadLock.Release();
        }
    }

    private void Changed([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
