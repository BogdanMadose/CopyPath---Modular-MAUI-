using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CopyPath___Modular_MAUI_.Helpers;
using CopyPath___Modular_MAUI_.Models;
using System.Collections.ObjectModel;

namespace CopyPath___Modular_MAUI_.ViewModels
{
    public partial class SettingsPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<FileTransferOptions> options;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string source;

        [ObservableProperty]
        private string destination;

        public SettingsPageViewModel()
        {
            Options = [.. XmlHelper.ReadOptions()];
            Console.WriteLine("ViewModel Initialized");
        }

        [RelayCommand]
        private void AddOption()
        {
            var option = new FileTransferOptions
            {
                Name = Name,
                Source = Source,
                Destination = Destination
            };
            Options.Add(option);
            XmlHelper.WriteOptions([.. Options]);
        }

        [RelayCommand]
        private void DeleteOption(FileTransferOptions option)
        {
            Options.Remove(option);
            XmlHelper.WriteOptions([.. Options]);
        }
    }
}
