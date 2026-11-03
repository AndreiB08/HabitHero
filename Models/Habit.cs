namespace HabitHero.Models;

public class Habit
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public string Frequency { get; set; } = "Daily";

    public string FrequencyAndCategory => $"{Frequency} · {Category}";
}