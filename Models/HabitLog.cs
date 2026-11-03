using SQLite;

namespace HabitHero.Models;

public class HabitLog
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [Indexed]
    public int HabitId { get; set; }
    [Indexed]
    public DateTime LogDate { get; set; }
    public int ExperienceAwarded { get; set; } = 10;
}
