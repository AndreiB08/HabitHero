using SQLite;
using HabitHero.Models;

namespace HabitHero.Services;

public class HabitDatabase
{
    private SQLiteAsyncConnection? _db;
    private readonly SemaphoreSlim _lock = new(1, 1);

    private async Task<SQLiteAsyncConnection> DbAsync()
    {
        if (_db is not null) return _db;
        await _lock.WaitAsync();
        try
        {
            if (_db is null)
            {
                var path = Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "habithero.db3");
                _db = new SQLiteAsyncConnection(path);
                await _db.CreateTableAsync<Habit>();
                await _db.CreateTableAsync<HabitLog>();
                await _db.CreateTableAsync<Achievement>();
                if (await _db.Table<Habit>().CountAsync() == 0)
                {
                    await _db.InsertAllAsync(new[]
                    {
                        new Habit { Name = "Drink 2L of water", Icon = "💧", Category = "Health", Description = "Stay hydrated throughout the day." },
                        new Habit { Name = "Read 20 pages", Icon = "📚", Category = "Learning", Description = "Make time for reading." },
                        new Habit { Name = "Morning workout", Icon = "🏋️", Category = "Fitness", Description = "Move your body and build strength." }
                    });
                }
                if (await _db.Table<Achievement>().CountAsync() == 0)
                {
                    await _db.InsertAllAsync(new[]
                    {
                        new Achievement { Code = "first", Title = "First Step", Description = "Complete your first habit.", Icon = "🌱" },
                        new Achievement { Code = "seven", Title = "Week Warrior", Description = "Complete seven habits.", Icon = "🔥" },
                        new Achievement { Code = "five", Title = "Habit Collector", Description = "Create five habits.", Icon = "🏅" },
                        new Achievement { Code = "xp100", Title = "Century Club", Description = "Earn 100 XP.", Icon = "💎" }
                    });
                }
            }
        }
        finally { _lock.Release(); }
        return _db!;
    }

    public async Task<List<Habit>> GetHabitsAsync() => await (await DbAsync()).Table<Habit>().OrderBy(x => x.Name).ToListAsync();
    public async Task<Habit?> GetHabitAsync(int id) => await (await DbAsync()).Table<Habit>().Where(x => x.Id == id).FirstOrDefaultAsync();
    public async Task SaveHabitAsync(Habit habit)
    {
        var db = await DbAsync();
        if (habit.Id == 0) await db.InsertAsync(habit);
        else await db.UpdateAsync(habit);
        await UpdateAchievementsAsync();
    }
    public async Task DeleteHabitAsync(Habit habit)
    {
        var db = await DbAsync();
        await db.Table<HabitLog>().Where(x => x.HabitId == habit.Id).DeleteAsync();
        await db.DeleteAsync(habit);
    }
    public async Task<List<HabitLog>> GetLogsAsync(DateTime date)
    {
        var db = await DbAsync();
        var start = date.Date;
        var end = start.AddDays(1);

        return await db.Table<HabitLog>()
            .Where(x => x.LogDate >= start && x.LogDate < end)
            .ToListAsync();
    }
    public async Task ToggleAsync(int habitId, DateTime date)
    {
        var db = await DbAsync();
        var start = date.Date;
        var end = start.AddDays(1);

        var item = await db.Table<HabitLog>()
            .Where(x => x.HabitId == habitId && x.LogDate >= start && x.LogDate < end)
            .FirstOrDefaultAsync();

        if (item is null)
        {
            await db.InsertAsync(new HabitLog
            {
                HabitId = habitId,
                LogDate = start
            });
        }
        else
        {
            await db.DeleteAsync(item);
        }

        await UpdateAchievementsAsync();
    }
    public async Task<int> TotalXpAsync() => (await (await DbAsync()).Table<HabitLog>().ToListAsync()).Sum(x => x.ExperienceAwarded);
    public async Task<int> TotalCompletionsAsync() => await (await DbAsync()).Table<HabitLog>().CountAsync();
    public async Task<int> CurrentStreakAsync()
    {
        var logs = await (await DbAsync()).Table<HabitLog>().ToListAsync();
        var dates = logs.Select(x => x.LogDate.Date).Distinct().ToHashSet();
        var day = dates.Contains(DateTime.Today) ? DateTime.Today : DateTime.Today.AddDays(-1);
        var count = 0;
        while (dates.Contains(day)) { count++; day = day.AddDays(-1); }
        return count;
    }
    public async Task<List<Achievement>> GetAchievementsAsync() => await (await DbAsync()).Table<Achievement>().ToListAsync();
    public async Task UpdateAchievementsAsync()
    {
        var db = await DbAsync();
        var completions = await db.Table<HabitLog>().CountAsync();
        var xp = (await db.Table<HabitLog>().ToListAsync()).Sum(x => x.ExperienceAwarded);
        var habits = await db.Table<Habit>().CountAsync();
        var items = await db.Table<Achievement>().ToListAsync();
        foreach (var item in items)
        {
            var unlock = item.Code switch
            {
                "first" => completions >= 1,
                "seven" => completions >= 7,
                "five" => habits >= 5,
                "xp100" => xp >= 100,
                _ => false
            };
            if (unlock && !item.IsUnlocked)
            {
                item.IsUnlocked = true;
                item.UnlockedAt = DateTime.Now;
                await db.UpdateAsync(item);
            }
        }
    }

    public async Task<List<(string Day, int Count)>> GetWeeklyActivityAsync()
    {
        var db = await DbAsync();
        var today = DateTime.Today;
        var start = today.AddDays(-6);
        var end = today.AddDays(1);

        var logs = await db.Table<HabitLog>()
            .Where(x => x.LogDate >= start && x.LogDate < end)
            .ToListAsync();

        var result = new List<(string Day, int Count)>();

        for (int i = 0; i < 7; i++)
        {
            var date = start.AddDays(i);
            var count = logs.Count(x => x.LogDate.Date == date);

            result.Add((date.ToString("ddd"), count));
        }

        return result;
    }


    public async Task<List<(string Category, int Count)>> GetCategoryDistributionAsync()
    {
        var db = await DbAsync();
        var habits = await db.Table<Habit>().ToListAsync();
        var logs = await db.Table<HabitLog>().ToListAsync();

        var result = logs
            .Join(habits,
                log => log.HabitId,
                habit => habit.Id,
                (log, habit) => habit.Category)
            .GroupBy(category => category)
            .Select(group => (group.Key, group.Count()))
            .ToList();

        return result;
    }

    public async Task<List<(string Date, int Xp)>> GetXpProgressAsync()
    {
        var db = await DbAsync();
        var logs = await db.Table<HabitLog>()
            .OrderBy(x => x.LogDate)
            .ToListAsync();

        var result = logs
            .GroupBy(log => log.LogDate.Date)
            .OrderBy(group => group.Key)
            .Select(group => (
                group.Key.ToString("dd/MM"),
                group.Sum(log => log.ExperienceAwarded)))
            .ToList();

        return result;
    }

}
