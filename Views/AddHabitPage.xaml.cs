
using HabitHero.Models;
using HabitHero.ViewModels;

namespace HabitHero.Views;

public partial class AddHabitPage : ContentPage
{
    public AddHabitPage()
    {
        InitializeComponent();
        BindingContext = new HabitFormViewModel(App.Database);
    }

    private async void SaveClicked(object sender, EventArgs e)
    {
        var vm = (HabitFormViewModel)BindingContext;

        if (string.IsNullOrWhiteSpace(vm.Name))
        {
            await DisplayAlert("Missing name", "Please enter a habit name.", "OK");
            return;
        }

        try
        {
            var habit = new Habit
            {
                Name = vm.Name.Trim(),
                Icon = string.IsNullOrWhiteSpace(vm.Icon) ? "✨" : vm.Icon.Trim(),
                Category = vm.Category,
                Frequency = vm.Frequency,
                Description = vm.Description.Trim()
            };

            await App.Database.SaveHabitAsync(habit);

            await DisplayAlert("Success", "Your habit has been saved!", "OK");

            await Shell.Current.GoToAsync("//habits");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void CancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//habits");
    }
}
