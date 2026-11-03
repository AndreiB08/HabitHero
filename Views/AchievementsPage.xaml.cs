using HabitHero.ViewModels;

namespace HabitHero.Views;

public partial class AchievementsPage : ContentPage
{
    private readonly AchievementsViewModel _vm;
    public AchievementsPage() { InitializeComponent(); _vm = new AchievementsViewModel(App.Database); BindingContext = _vm; }
    protected override async void OnAppearing() { base.OnAppearing(); await _vm.LoadAsync(); }
}
