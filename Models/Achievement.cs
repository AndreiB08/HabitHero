using SQLite;

namespace HabitHero.Models;

public class Achievement
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [Unique]
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = "🏆";
    public bool IsUnlocked { get; set; }
    public DateTime? UnlockedAt { get; set; }
    [Ignore]
    public string StatusText => IsUnlocked ? $"Unlocked {UnlockedAt:dd MMM yyyy}" : "Locked · keep going";
}
