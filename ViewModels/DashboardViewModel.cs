using HabitHero.Models;
using System.Collections.ObjectModel;

namespace HabitHero.ViewModels;

public class DashboardViewModel
{
    public ObservableCollection<Habit> Habits { get; set; } = new()
{
    new Habit
    {
        Id = 1,
        Name = "Drink 2L of water",
        Icon = "💧",
        Category = "Health",
        Frequency = "Daily"
    },
    new Habit
    {
        Id = 2,
        Name = "Read 20 pages",
        Icon = "📚",
        Category = "Learning",
        Frequency = "Daily"
    },
    new Habit
    {
        Id = 3,
        Name = "Morning workout",
        Icon = "🏋️",
        Category = "Fitness",
        Frequency = "Daily"
    }
};

    public string Greeting { get; set; } = "Good morning! 👋";
    public string CurrentDate { get; set; } = DateTime.Now.ToString("dddd, MMMM d");
    public int CompletedHabits { get; set; } = 3;
    public int TotalHabits { get; set; } = 5;
    public double DailyProgress => TotalHabits == 0 ? 0 : (double)CompletedHabits / TotalHabits;
    public string ProgressText => $"{CompletedHabits} of {TotalHabits} completed";
    public string ProgressPercentage => $"{DailyProgress:P0}";
    public int CurrentStreak { get; set; } = 7;
    public string CurrentStreakText => $"{CurrentStreak} days";
    public int ExperiencePoints { get; set; } = 240;
    public string ExperiencePointsText => $"{ExperiencePoints} XP";
}