using SQLite;

namespace HabitHero.Models;

public class Habit
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = "✨";
    public string Category { get; set; } = "Personal";
    public string Frequency { get; set; } = "Daily";
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Today;
    [Ignore]
    public bool IsCompletedToday { get; set; }
    [Ignore]
    public string FrequencyAndCategory => $"{Frequency} · {Category}";
}
