using HabitHero.ViewModels;

namespace HabitHero.Views;

public partial class CalendarPage : ContentPage
{
    private readonly CalendarViewModel _vm;
    public CalendarPage() { InitializeComponent(); _vm = new CalendarViewModel(App.Database); BindingContext = _vm; }
    protected override async void OnAppearing() { base.OnAppearing(); await _vm.LoadAsync(); }
}
