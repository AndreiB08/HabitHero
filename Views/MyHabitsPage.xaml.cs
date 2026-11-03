using HabitHero.Models;
using HabitHero.ViewModels;

namespace HabitHero.Views;

public partial class MyHabitsPage : ContentPage
{
    private readonly HabitsViewModel _vm;
    public MyHabitsPage() { InitializeComponent(); _vm = new HabitsViewModel(App.Database); BindingContext = _vm; }
    protected override async void OnAppearing() { base.OnAppearing(); await _vm.LoadAsync(); }
    private async void HabitSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Habit habit)
        {
            ((CollectionView)sender).SelectedItem = null;
            await Shell.Current.GoToAsync($"{nameof(HabitDetailsPage)}?habitId={habit.Id}");
        }
    }
    private async void AddClicked(object sender, EventArgs e) => await Shell.Current.GoToAsync("//addhabit");
}
