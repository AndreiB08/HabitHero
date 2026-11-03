using System.Collections.ObjectModel;
using HabitHero.Models;
using HabitHero.Services;

namespace HabitHero.ViewModels;

public class AchievementsViewModel
{
    private readonly HabitDatabase _db;
    public ObservableCollection<Achievement> Items { get; } = new();
    public AchievementsViewModel(HabitDatabase db) { _db = db; }
    public async Task LoadAsync()
    {
        await _db.UpdateAchievementsAsync();
        Items.Clear();
        foreach (var item in await _db.GetAchievementsAsync()) Items.Add(item);
    }
}
