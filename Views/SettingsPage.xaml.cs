using CopyPath___Modular_MAUI_.Helpers;
using System.Collections.ObjectModel;

namespace CopyPath___Modular_MAUI_.Views;

public partial class SettingsPage : ContentPage
{
    public ObservableCollection<FileTransferOptions> Options { get; set; }

    public SettingsPage()
    {
        InitializeComponent();
        Options = new ObservableCollection<FileTransferOptions>(XmlHelper.ReadOptions());
        BindingContext = this;
    }

    private void OnAddOptionClicked(object sender, EventArgs e)
    {
        var options = new FileTransferOptions
        {
            Name = NameEntry.Text,
            Source = SourceEntry.Text,
            Destination = DestinationEntry.Text
        };
        Options.Add(options);
        XmlHelper.WriteOptions(Options.ToList());
    }

    private void OnDeleteOptionClicked(object sender, EventArgs e)
    {
        var menuItem = sender as MenuItem;
        var option = menuItem.CommandParameter as FileTransferOptions;
        Options.Remove(option);
        XmlHelper.WriteOptions(Options.ToList());
    }

    private void OnPointerEntered(object sender, PointerEventArgs e)
    {
        var stackLayout = sender as StackLayout;
        stackLayout.BackgroundColor = (Color)Application.Current.Resources["Gray500"];
    }

    private void OnPointerExited(object sender, PointerEventArgs e)
    {
        var stackLayout = sender as StackLayout;
        stackLayout.BackgroundColor = (Color)Application.Current.Resources["Gray900"];
    }

    private void OnButtonPointerEntered(object sender, PointerEventArgs e)
    {
        var button = sender as Button;
        button.BackgroundColor = Color.FromArgb("#0099ff");
    }

    private void OnButtonPointerExited(object sender, PointerEventArgs e)
    {
        var button = sender as Button;
        button.BackgroundColor = Color.FromArgb("#007acc");
    }
}