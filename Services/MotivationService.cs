using System.Text.Json;

namespace HabitHero.Services;

public class MotivationService
{
    private static readonly string[] Fallback =
    {
        "Small steps every day lead to big changes.",
        "Consistency beats intensity.",
        "You do not have to be perfect. Just keep showing up."
    };

    public async Task<string> GetDailyMessageAsync()
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            var json = await client.GetStringAsync("https://zenquotes.io/api/today");
            using var document = JsonDocument.Parse(json);
            return document.RootElement[0].GetProperty("q").GetString() ?? Fallback[0];
        }
        catch
        {
            return Fallback[Random.Shared.Next(Fallback.Length)];
        }
    }
}
