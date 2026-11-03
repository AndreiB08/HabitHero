using HabitHero.ViewModels;

namespace HabitHero.Views;

[QueryProperty(nameof(HabitId), "habitId")]
public partial class EditHabitPage : ContentPage
{
    private readonly HabitFormViewModel _vm;
    private string _habitId = string.Empty;
    public string HabitId
    {
        get => _habitId;
        set { _habitId = value; if (int.TryParse(value, out var id)) _ = _vm.LoadAsync(id); }
    }
    public EditHabitPage() { InitializeComponent(); _vm = new HabitFormViewModel(App.Database); BindingContext = _vm; }
}
