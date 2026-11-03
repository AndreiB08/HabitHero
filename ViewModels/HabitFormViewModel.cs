using System.ComponentModel;
using System.Runtime.CompilerServices;
using HabitHero.Models;
using HabitHero.Services;

namespace HabitHero.ViewModels;

public class HabitFormViewModel : INotifyPropertyChanged
{
    private readonly HabitDatabase _db;
    private int _id;
    private string _name = string.Empty;
    private string _icon = "✨";
    private string _category = "Personal";
    private string _frequency = "Daily";
    private string _description = string.Empty;
    public event PropertyChangedEventHandler? PropertyChanged;
    public string Name { get => _name; set { _name = value; Changed(); } }
    public string Icon { get => _icon; set { _icon = value; Changed(); } }
    public string Category { get => _category; set { _category = value; Changed(); } }
    public string Frequency { get => _frequency; set { _frequency = value; Changed(); } }
    public string Description { get => _description; set { _description = value; Changed(); } }
    public Command SaveCommand { get; }
    public Command CancelCommand { get; }
    public HabitFormViewModel(HabitDatabase db)
    {
        _db = db;
        SaveCommand = new Command(async () => await SaveAsync());
        CancelCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
    }
    public async Task LoadAsync(int id)
    {
        var h = await _db.GetHabitAsync(id);
        if (h is null) return;
        _id = h.Id; Name = h.Name; Icon = h.Icon; Category = h.Category; Frequency = h.Frequency; Description = h.Description;
    }
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlert("Missing name", "Please enter a habit name.", "OK");
            return;
        }
        await _db.SaveHabitAsync(new Habit { Id = _id, Name = Name.Trim(), Icon = string.IsNullOrWhiteSpace(Icon) ? "✨" : Icon.Trim(), Category = Category, Frequency = Frequency, Description = Description.Trim() });
        await Shell.Current.GoToAsync("..");
    }
    private void Changed([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
