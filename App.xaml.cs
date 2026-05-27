using System.Windows;
using NewAPP.Views;
using NewAPP.ViewModels;
using NewAPP.View;
using NewAPP.Services;
using NewAPP2.Interface;
using Microsoft.Extensions.DependencyInjection;
using NewAPP2.ViewModels;
using Velopack;


namespace NewAPP
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;


        protected override void OnStartup ( StartupEventArgs e )
        {
            VelopackApp.Build().Run();

            base.OnStartup(e);
           

            // ========== 1. НАСТРАИВАЕМ DI КОНТЕЙНЕР ==========
            var services = new ServiceCollection();

            // Регистрируем сервисы (Singleton - один экземпляр на всё приложение)
            services.AddSingleton<IDataBaseService, DatabaseService>();
            services.AddSingleton<IModbusTCPService, ModbusTCP>();
            services.AddSingleton<ICalibration, Calibration>();
            services.AddSingleton<IExcele, Excele>();
            // services.AddSingleton<IDialogService, DialogService>(); // если создадите

            // Регистрируем ViewModel (Transient - новый экземпляр каждый раз)
            services.AddTransient<MainViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<TerminalViewModel>();
            services.AddTransient<NomenclatureViewModel>();
            services.AddTransient<SensorsViewModel>();

            // Регистрируем окна (Transient)
            services.AddTransient<MainWindow>();
            services.AddTransient<LoginWindow>();
            services.AddTransient<ReportWindow>();
            services.AddTransient<AddNomenclatureWindow>();
            services.AddTransient<DeleteNomenclatureWindow>();
            services.AddTransient<HistoryWindow>();
            services.AddTransient<SetSkladWindow>();
            services.AddTransient<SelectedProduct>();
            services.AddTransient<TestWeightWindow>();
            services.AddTransient<SettingVisibleNom>();

            // Регистрируем IServiceProvider (чтобы можно было Inject в MainViewModel)
            services.AddSingleton<IServiceProvider>(sp => sp);

            _serviceProvider = services.BuildServiceProvider();

            // ========== 2. СОЗДАЁМ ОКНА ЧЕРЕЗ КОНТЕЙНЕР ==========
            // Отключаем автоматическое закрытие
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // Создаём окно авторизации через контейнер
            var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();

            if (loginWindow.ShowDialog() == true)
            {
                // Получаем данные пользователя из LoginViewModel
                var loginVm = loginWindow.DataContext as LoginViewModel;

                // Создаём главное окно через контейнер
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();

                // Передаём пользователя в MainViewModel
                if (mainWindow.DataContext is MainViewModel mainVm && loginVm != null)
                {
                    mainVm.CurrentUser = loginVm.CurrentUser;
                }

                // Включаем автозакрытие обратно
                Current.ShutdownMode = ShutdownMode.OnLastWindowClose;

                // Показываем главное окно
                mainWindow.Show();
            }
            else
            {
                // Закрываем приложение
                Current.Shutdown();
            }
        }
    }
}
