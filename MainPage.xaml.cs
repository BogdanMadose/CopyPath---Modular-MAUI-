using CopyPath___Modular_MAUI_.Helpers;
using CopyPath___Modular_MAUI_.Models;
using CopyPath___Modular_MAUI_.Services;
using System.Collections.ObjectModel;

namespace CopyPath___Modular_MAUI_
{
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
#pragma warning disable CS8602 // Dereference of a possibly null reference.
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "<Pending>")]
    public partial class MainPage : ContentPage
    {
        private readonly FileTransferService _fileTransferService;

        private DateTime _transferStartTime;
        private int _totalFiles;
        private int _processedFiles;
        private ViewCell? _lastSelectedCell;
        public ObservableCollection<FileTransferOptions> Options { get; set; }

        public MainPage(FileTransferService fileTransferService)
        {
            InitializeComponent();
            _fileTransferService = fileTransferService;
            Options = [.. XmlHelper.ReadOptions()];
            BindingContext = this;
            this.Appearing += OnPageAppearing;
        }

        private async void OnStartTransferClicked(object sender, EventArgs e)
        {
            if (OptionsListView.SelectedItem == null)
            {
                await DisplayAlert("Selection Required", "Please select a transfer option first.", "OK");
                return;
            }

            ProgressBar.IsVisible = true;
            ProgressBar.Progress = 0;
            ProgressLabel.IsVisible = true;
            ProgressLabel.Text = "Preparing transfer...";
            var selectedOption = OptionsListView.SelectedItem as FileTransferOptions;
            var cancellationTokenSource = new CancellationTokenSource();
            _transferStartTime = DateTime.Now;
            _processedFiles = 0;
            var progress = new Progress<(double Percentage, int Processed, int Total)>(update =>
            {
                // Real-time UI updates (automatically runs on UI thread)
                ProgressBar.Progress = update.Percentage;

                // Enhanced progress text
                var elapsed = DateTime.Now - _transferStartTime;
                var speed = elapsed.TotalSeconds > 0 ?
                           update.Processed / elapsed.TotalSeconds : 0;

                ProgressLabel.Text = string.Format(
                    "{0}/{1} files ({2:P0}) • {3:0.0} files/sec • {4:hh\\:mm\\:ss}",
                    update.Processed,
                    update.Total,
                    update.Percentage,
                    speed,
                    elapsed);
            });

            try
            {
                await FileTransferService.TransferFilesAsync(selectedOption, cancellationTokenSource.Token, progress);
                ProgressLabel.Text = "Transfer complete!";
                await DisplayAlert("Success", "File transfer completed successfully!", "OK");
            }
            catch (OperationCanceledException)
            {
                ProgressLabel.Text = "Transfer cancelled";
                await DisplayAlert("Cancelled", "File transfer was cancelled.", "OK");
                LoggingHelper.LogInfo("File transfer operation was canceled.");
            }
            catch (Exception ex)
            {
                ProgressLabel.Text = "Transfer failed";
                await DisplayAlert("Error", $"Transfer failed: {ex.Message}", "OK");
                LoggingHelper.LogError("An error occurred during file transfer.", ex);
            }
            finally
            {
                await Task.Delay(2000);
                ProgressBar.IsVisible = false;
                ProgressLabel.IsVisible = false;
            }
        }

        private void OnPageAppearing(object sender, EventArgs e)
        {
            Options.Clear();
            var updatedOptions = XmlHelper.ReadOptions();
            foreach (var option in updatedOptions)
            {
                Options.Add(option);
            }
        }

        private void OnPointerEntered(object sender, PointerEventArgs e)
        {
            var stackLayout = sender as StackLayout;
            stackLayout.BackgroundColor = (Color)Application.Current.Resources["Gray600"];
        }

        private void OnPointerExited(object sender, PointerEventArgs e)
        {
            var stackLayout = sender as StackLayout;
            stackLayout.BackgroundColor = (Color)Application.Current.Resources["Gray900"];
        }

        private void OnButtonPointerEntered(object sender, PointerEventArgs e)
        {
            var button = sender as Button;
            button.BackgroundColor = Colors.LimeGreen;
        }

        private void OnButtonPointerExited(object sender, PointerEventArgs e)
        {
            var button = sender as Button;
            button.BackgroundColor = Color.FromArgb("#28a745");
        }

        private void OptionsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.PreviousSelection.OfType<FileTransferOptions>())
                item.IsSelected = false;

            foreach (var item in e.CurrentSelection.OfType<FileTransferOptions>())
                item.IsSelected = true;
        }
    }
}
