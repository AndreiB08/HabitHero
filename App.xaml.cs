namespace HabitHero;

public partial class App : Application
{
    public static Services.HabitDatabase Database { get; } = new();

    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }
}
