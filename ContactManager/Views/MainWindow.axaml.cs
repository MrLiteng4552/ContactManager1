using Avalonia.Controls;
using System.Threading.Tasks;
using ContactManager.ViewModels;

namespace ContactManager.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        this.FindControl<Button>("DeleteBtn")!.Click += async (s, e) => await ShowDeleteConfirmDialog();
    }

    private async Task ShowDeleteConfirmDialog()
    {
        if (DataContext is not MainWindowViewModel vm) return;

        var dialog = new Window
        {
            Title = "Подтверждение удаления",
            Width = 380,
            Height = 130,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Content = new StackPanel
            {
                Spacing = 15,
                Margin = new Avalonia.Thickness(15),
                Children =
                {
                    new TextBlock 
                    {
                        Text = "Вы действительно хотите удалить этот контакт?", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                    },
                    
                    new StackPanel
                    {
                        Orientation = Avalonia.Layout.Orientation.Horizontal,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        Spacing = 20,
                        Children =
                        {
                            new Button 
                            {
                                Content = "Да", Width = 70, HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center, Background = Avalonia.Media.Brush.Parse("#2E7D32"), Foreground = Avalonia.Media.Brushes.White
                            },
                            
                            new Button 
                            { 
                                Content = "Нет", Width = 70, HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center, Background = Avalonia.Media.Brush.Parse("#D32F2F"), Foreground = Avalonia.Media.Brushes.White 
                            }
                        }
                    }
                }
            }
        };

        var stack = (StackPanel)dialog.Content;
        var btnPanel = (StackPanel)stack.Children[1];
        var yesBtn = (Button)btnPanel.Children[0];
        var noBtn = (Button)btnPanel.Children[1];

        bool isConfirmed = false;
        yesBtn.Click += (s, e) => 
        {
            isConfirmed = true; dialog.Close();
        };
        noBtn.Click += (s, e) => 
        {
            isConfirmed = false; dialog.Close(); 
        };

        await dialog.ShowDialog(this);

        if (isConfirmed)
        {
            await vm.ConfirmDeleteContactAsync();
        }
    }
}
