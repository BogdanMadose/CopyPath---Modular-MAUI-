using CopyPath___Modular_MAUI_.Helpers;

namespace CopyPath___Modular_MAUI_.Views;

public partial class ConflictDialog : ContentPage
{
    private TaskCompletionSource<string> _taskCompletionSource;
    private readonly List<string> _conflictingFiles;

    public ConflictDialog(List<string> conflictingFiles)
    {
        InitializeComponent();
        _conflictingFiles = conflictingFiles;
        UpdateConflictMessage();
    }

    private void UpdateConflictMessage()
    {
        if (_conflictingFiles.Count == 1)
        {
            ConflictMessage.Text = $"File '{Path.GetFileName(_conflictingFiles[0])}' already exists.";
        }
        else
        {
            var fileList = string.Join("\n", _conflictingFiles.Select(f => $"• {Path.GetFileName(f)}"));
            ConflictMessage.Text = $"The following files already exist:\n{fileList}";
        }

        SubtitleLabel.Text = "What would you like to do?";
    }

    public Task<string> ShowAsync()
    {
        _taskCompletionSource = new TaskCompletionSource<string>();
        return _taskCompletionSource.Task;
    }

    private void OnOverwriteClicked(object sender, EventArgs e) => ProcessResult("OverwriteAll");
    private void OnSkipClicked(object sender, EventArgs e) => ProcessResult("SkipAll");
    private void OnCancelClicked(object sender, EventArgs e) => ProcessResult("Cancel");

    private async void ProcessResult(string result)
    {
        try
        {
            if (_taskCompletionSource?.Task.IsCompleted == false)
            {
                _taskCompletionSource.TrySetResult(result);
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    // 3. Use while-loop for reliable closure
                    while (Navigation.ModalStack.Contains(this))
                    {
                        await Navigation.PopModalAsync();
                    }
                });
            }
        }
        catch (Exception ex)
        {
            LoggingHelper.LogError($"Dialog error: {ex.Message}");
        }
    }

    protected override void OnDisappearing()
    {
        // Only cancel if not already completed
        if (_taskCompletionSource?.Task.IsCompleted == false)
        {
            _taskCompletionSource.TrySetCanceled();
        }
        base.OnDisappearing();
    }

    private void OnPointerEntered(object sender, PointerEventArgs e)
    {
        var button = sender as Button;
        button.BackgroundColor = Color.FromArgb("#0099ff");
    }

    private void OnPointerExited(object sender, PointerEventArgs e)
    {
        var button = sender as Button;
        button.BackgroundColor = Color.FromArgb("#007acc");
    }
}