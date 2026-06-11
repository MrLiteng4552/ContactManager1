using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using ContactManager.Models;
using ContactManager.ViewModels;
using ContactManager.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ContactManager;

public partial class App : Application
{
    public static IServiceProvider? ServiceProvider { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();

        services.AddDbContextFactory<AppDbContext>(options =>
            options.UseSqlite("Data Source=contacts.db"));

        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<MainWindow>();

        ServiceProvider = services.BuildServiceProvider();

        var dbFactory = ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
        using (var context = dbFactory.CreateDbContext())
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }


        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = ServiceProvider.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
/* 
  =========================================
  ГЛОБАЛЬНОЕ ОПИСАНИЕ ФАЙЛА (ДЛЯ ЗАЩИТЫ):
  =========================================
  Этот файл — главная пусковая точка приложения, управляющая его жизненным циклом и конфигурацией.

  ЧТО ОН ДЕЛАЕТ И ЗАЧЕМ НУЖЕН КРАТКО:
  1. ServiceCollection (IoC-контейнер) — инициализирует механизм Dependency Injection (внедрение зависимостей). 
     Сюда регистрируются фабрика контекста бд (IDbContextFactory) и все слои окон и логики (View / ViewModel).
  2. Автосоздание базы — блок context.Database.EnsureCreated() при старте проверяет диск. Если файла SQLite нет, 
     он на лету генерирует contacts.db и строит в ней таблицы на основе C#-моделей.
  3. Связывание слоев — в самом конце метод вытаскивает MainWindow и MainWindowViewModel из контейнера 
     зависимостей, связывает их через DataContext и выводит готовое окно на экран пользователя.
  =========================================
*/