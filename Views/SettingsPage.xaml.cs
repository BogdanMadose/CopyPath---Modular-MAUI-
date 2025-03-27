using CopyPath___Modular_MAUI_.Models;
using CopyPath___Modular_MAUI_.ViewModels;

namespace CopyPath___Modular_MAUI_.Views;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        BindingContext = new SettingsPageViewModel();
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

    private void OnItemTapped(object sender, TappedEventArgs e)
    {
        var stackLayout = sender as StackLayout;
        var option = stackLayout.BindingContext as FileTransferOptions;
        ShowContextMenu(option, stackLayout);
    }

    private void ShowContextMenu(FileTransferOptions option, Microsoft.Maui.Controls.View anchorElement)
    {
        if (anchorElement.Handler?.PlatformView is not Microsoft.UI.Xaml.FrameworkElement frameworkElement)
            return;

        var menu = new Microsoft.UI.Xaml.Controls.MenuFlyout();

        var deleteItem = new Microsoft.UI.Xaml.Controls.MenuFlyoutItem
        {
            Text = "Delete",
            Command = ((SettingsPageViewModel)this.BindingContext)?.DeleteOptionCommand,
            CommandParameter = option
        };

        // Set red background
        deleteItem.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
            Windows.UI.Color.FromArgb(255, 255, 0, 0));

        // Set icon (must be in Assets folder)
        deleteItem.Icon = new Microsoft.UI.Xaml.Controls.BitmapIcon
        {
            UriSource = new System.Uri("ms-appx:///Assets/delete_icon.png")
        };

        menu.Items.Add(deleteItem);

        // Show menu relative to the element
        menu.ShowAt(frameworkElement, new Windows.Foundation.Point(0, 0));
    }
}