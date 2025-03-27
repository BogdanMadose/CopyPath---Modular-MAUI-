using CommunityToolkit.Mvvm.ComponentModel;

namespace CopyPath___Modular_MAUI_.Models
{
    public partial class FileTransferOptions : ObservableObject
    {
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public required string Name { get; set; }
        public required string Source { get; set; }
        public required string Destination { get; set; }
    }
}
