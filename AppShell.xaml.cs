namespace HabitHero;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(Views.HabitDetailsPage), typeof(Views.HabitDetailsPage));
        Routing.RegisterRoute(nameof(Views.EditHabitPage), typeof(Views.EditHabitPage));
    }
}
