using HabitHero.Models;
using HabitHero.ViewModels;

namespace HabitHero.Views;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _vm;
    public DashboardPage()
    {
        InitializeComponent();
        _vm = new DashboardViewModel(App.Database);
        BindingContext = _vm;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
    private async void HabitChecked(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox box && box.BindingContext is Habit habit && habit.IsCompletedToday != e.Value)
            await _vm.ToggleAsync(habit);
    }
    private async void ViewAllClicked(object sender, EventArgs e) => await Shell.Current.GoToAsync("//habits");
}
