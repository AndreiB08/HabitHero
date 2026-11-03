using HabitHero.ViewModels;

namespace HabitHero.Views;

public partial class StatisticsPage : ContentPage
{
    private readonly StatisticsViewModel _vm;
    public StatisticsPage() { InitializeComponent(); _vm = new StatisticsViewModel(App.Database); BindingContext = _vm; }
    protected override async void OnAppearing() { base.OnAppearing(); await _vm.LoadAsync(); }
}
