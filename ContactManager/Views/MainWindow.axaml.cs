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
                    new TextBlock { Text = "Вы действительно хотите удалить этот контакт?", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center },
                    new StackPanel
                    {
                        Orientation = Avalonia.Layout.Orientation.Horizontal,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        Spacing = 20,
                        Children =
                        {
                            new Button { Content = "Да", Width = 70, HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center, Background = Avalonia.Media.Brush.Parse("#2E7D32"), Foreground = Avalonia.Media.Brushes.White },
                            new Button { Content = "Нет", Width = 70, HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center, Background = Avalonia.Media.Brush.Parse("#D32F2F"), Foreground = Avalonia.Media.Brushes.White }
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
        yesBtn.Click += (s, e) => { isConfirmed = true; dialog.Close(); };
        noBtn.Click += (s, e) => { isConfirmed = false; dialog.Close(); };

        await dialog.ShowDialog(this);

        if (isConfirmed)
        {
            await vm.ConfirmDeleteContactAsync();
        }
    }
}
/* 
  =========================================
  ГЛОБАЛЬНОЕ ОПИСАНИЕ ФАЙЛА (ДЛЯ ЗАЩИТЫ):
  =========================================
  Этот файл представляет собой Code-behind (код поддержки) для главного XAML-окна.
  По канонам MVVM он должен быть максимально пустым, но здесь реализована логика вызова диалоговых окон.

  ЧТО ОН ДЕЛАЕТ И ЗАЧЕМ НУЖЕН КРАТКО:
  1. InitializeComponent() — стандартный метод Avalonia, который парсит XAML-файл разметки и собирает окно.
  2. Подписка на клик (FindControl) — программа находит кнопку "DeleteBtn" в XAML и подписывает её на событие клика.
  3. ShowDeleteConfirmDialog() — динамически собирает из C#-кода всплывающее модальное окно Window (MessageBox) 
     с вопросом «Вы действительно хотите удалить контакт?» и кнопками «Да»/«Нет».
  4. Передача управления — если пользователь нажал «Да», этот файл вызывает асинхронный метод ConfirmDeleteContactAsync() 
     во ViewModel, запуская логику мягкого удаления.
  =========================================
*/