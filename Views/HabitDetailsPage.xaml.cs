using HabitHero.Models;

namespace HabitHero.Views;

[QueryProperty(nameof(HabitId), "habitId")]
public partial class HabitDetailsPage : ContentPage
{
    private int _id;
    private Habit? _habit;
    public string HabitId { get => _id.ToString(); set { if (int.TryParse(value, out var id)) { _id = id; _ = LoadAsync(); } } }
    public HabitDetailsPage() { InitializeComponent(); }
    protected override async void OnAppearing() { base.OnAppearing(); await LoadAsync(); }
    private async Task LoadAsync()
    {
        if (_id <= 0) return;
        _habit = await App.Database.GetHabitAsync(_id);
        if (_habit is null) return;
        BindingContext = new DetailsData
        {
            Name = _habit.Name,
            Icon = _habit.Icon,
            FrequencyAndCategory = _habit.FrequencyAndCategory,
            CategoryImage = _habit.CategoryImage,
            HasCategoryImage = _habit.HasCategoryImage,
            Description = string.IsNullOrWhiteSpace(_habit.Description) ? "No description added yet." : _habit.Description,
            CreatedText = $"Created on {_habit.CreatedAt:dd MMM yyyy}"
        };
    }
    private async void CompleteClicked(object sender, EventArgs e)
    {
        if (_habit is null) return;
        await App.Database.ToggleAsync(_habit.Id, DateTime.Today);
        await DisplayAlert("Updated", "Today's completion has been recorded or removed.", "OK");
    }
    private async void EditClicked(object sender, EventArgs e)
    {
        if (_habit is not null) await Shell.Current.GoToAsync($"{nameof(EditHabitPage)}?habitId={_habit.Id}");
    }
    private async void DeleteClicked(object sender, EventArgs e)
    {
        if (_habit is null) return;
        if (await DisplayAlert("Delete habit", "Delete this habit and its history?", "Delete", "Cancel"))
        {
            await App.Database.DeleteHabitAsync(_habit);
            await Shell.Current.GoToAsync("//habits");
        }
    }
    private class DetailsData
    {
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string FrequencyAndCategory { get; set; } = string.Empty;
        public string CategoryImage { get; set; } = string.Empty;
        public bool HasCategoryImage { get; set; }
        public string Description { get; set; } = string.Empty;
        public string CreatedText { get; set; } = string.Empty;
    }
}
