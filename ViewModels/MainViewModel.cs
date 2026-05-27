using Microsoft.Extensions.DependencyInjection;
using NewAPP.Models;
using NewAPP.Services;
using NewAPP.View;
using NewAPP.ViewModels;
using NewAPP2.Interface;
using NewAPP2.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Velopack;
using Velopack.Sources;


namespace NewAPP
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // ===== ПОЛЯ =====
        private DispatcherTimer _timer; //Sensor
        private Random _random; //Sensor
        private readonly IModbusTCPService _modbusService;
        private bool _useRealData;
        private readonly ICalibration _calibration;
        private readonly IDataBaseService _dataBase;
        private readonly IExcele _excele;
       
        private List<NomenclatureUnit> allItems;
        public TerminalViewModel TerminalVM { get; set; }
        public NavigationViewModel NavigationVM { get; set; }
        public NomenclatureViewModel NomenclatureVM { get; set; }
        public SensorsViewModel SensorsVM { get; set; }

        public ObservableCollection<Terminal> Terminals { get; set; }

        //список действий, которые будут открываться в биндинге
        private ObservableCollection<ActionItem> _availableActions;
        private ActionItem _selectedAction;

        public ObservableCollection<ActionItem> AvailableActions
        {
            get => _availableActions;
            set
            {
                _availableActions = value;
                OnPropertyChanged();
            }
        }

        public ActionItem SelectedAction
        {
            get => _selectedAction;
            set
            {
                _selectedAction = value;
                OnPropertyChanged();
            }
        }

        private ActionItem _hotkey1Action;

        // Это свойство обязательно нужно создать!
        public ActionItem Hotkey1Action
        {
            get => _hotkey1Action;
            set
            {
                if (_hotkey1Action != value)
                {
                    _hotkey1Action = value;
                    OnPropertyChanged();

                    if(_hotkey1Action != null)
                    {
                        ButtonContent = value.Name;
                        OpenComand = value.Command;
                    }
                    
                }
            }
        }
        private string _buttonContent;
        public string ButtonContent 
        {
            get => _buttonContent;
            set
            {
                _buttonContent = value;
                OnPropertyChanged();
            }
        }

        private ICommand _openComand;
        public ICommand OpenComand 
        {
            get => _openComand;
            set
            {
                _openComand = value;
                OnPropertyChanged();
            }
        }

        // ===== СВОЙСТВА ДЛЯ ВИДИМОСТИ ОКОН =====
        private bool _isWelcomeVisible = true;
        public bool IsWelcomeVisible
        {
            get => _isWelcomeVisible;
            set { _isWelcomeVisible = value; OnPropertyChanged(); }
        }

        private bool _isSensorsVisible;
        public bool IsSensorsVisible
        {
            get => NavigationVM.IsSensorsVisible;
            set { NavigationVM.IsSensorsVisible = value;  }
        }

        private bool _isSettingsVisible;
        public bool IsSettingsVisible
        {
            get => NavigationVM.IsSettingsVisible;
            set { NavigationVM.IsSettingsVisible = value; OnPropertyChanged(); }
        }

        private string _searchText; //свойство поиска
        public string SearchText
        {
            get => _searchText;

            set 
            {
                var trimmer = value.Trim();
                if (_searchText == trimmer) return;

                _searchText = trimmer; 
                OnPropertyChanged();
                
            }
        }


        private bool _isTcpSettingsVisible;
        public bool IsTcpSettingsVisible
        {
            get => NavigationVM.IsTcpSettingsVisible;
            set { NavigationVM.IsTcpSettingsVisible = value; OnPropertyChanged(); }
        }

       
        private bool _isAddUsersVisible; // надо сделать отдельную видимость для кнопки, при этом должна исчезать видимость области за которую отвечает кнопка
        public bool IsAddUsersVisible
        {
            get => NavigationVM.IsAddUsersVisible;
            set { NavigationVM.IsAddUsersVisible = value; OnPropertyChanged(); }
        }

        private string _isAddUsersLogin; //новый логин
        public string IsAddUsersLogin
        {
            get => _isAddUsersLogin;
            set { _isAddUsersLogin = value; OnPropertyChanged(); }
        }

        private string _isAddUsersPass; //новый пароль
        public string IsAddUsersPass
        {
            get => _isAddUsersPass;
            set { _isAddUsersPass = value; OnPropertyChanged(); }
        }

        private string _userRole = "Пользователь"; // новая роль
        public string UserRole
        {
            get => _userRole;
            set { _userRole = value; OnPropertyChanged(); }
        }
        private bool _isAddButtonUsersVisible; //видимость для кнопки
        public bool IsAddButtonUsersVisible
        {
            get => _isAddButtonUsersVisible;
            set { _isAddButtonUsersVisible = value; OnPropertyChanged(); }
        }

        private bool _isVisibleSklad; //видимость для кнопки склада
        public bool IsVisibleSklad
        {
            get => _isVisibleSklad;
            set { _isVisibleSklad = value; OnPropertyChanged(); }
        }

        

        private bool _isNomenclatureVisible; //свойство видимости номенклатуры
        public bool IsNomenclatureVisible
        {
            get => NavigationVM.IsNomenclatureVisible;
            set => NavigationVM.IsNomenclatureVisible = value;
        }

        private bool _isDataBaseButtonVisible; //свойство видимости кнопки номенклатуры
        public bool IsDataBaseButtonVisible
        {
            get => _isDataBaseButtonVisible;
            set { _isDataBaseButtonVisible = value; OnPropertyChanged(); }
        }

        public List<string> AvailableRoles { get; } = new List<string> //список ролей
        { 
            "Пользователь",
            "Администратор"
        };

      

        private int _staticCount; //свойство для статики
        public int StaticCount
        {
            get => _staticCount;
            set
            {
                _staticCount = value;
                OnPropertyChanged();
            }
        }



        private string _deleteName; //удалить имя
        public string DeleteName
        {
            get => _deleteName;
            set
            {
                _deleteName = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<NomenclatureUnit> _loadlNum;
        public ObservableCollection<NomenclatureUnit> LoadlNum
        {
            get => _loadlNum;
            set
            {
                _loadlNum = value;
                OnPropertyChanged();
            }
        }
        public static List<NomenclatureUnit> AllNomenclatureUnits { get; private set; }


        private readonly IServiceProvider _serviceProvider;

        // ===== КОМАНДЫ =====
        public ICommand ZeroPointCommand { get; }
        public ICommand EtalonPointCommand { get; }
        public ICommand AddUserCommand => NavigationVM.AddUserCommand;
        public ICommand SetingSklad { get; } //кнопка открытия настроек склада
        public ICommand AddButtonUsers { get; private set; } // кнопка добавления пользователя
        public ICommand ButtonDataBase { get; } //кнопка открытия базы данных
        public ICommand DeleteNomCommand { get; } //команда убирания из номенклатуры чего то
        public ICommand OpenTestWindow {  get; } //открытие окна тестового
        public ICommand ApplyHotkeysCommand { get; }
        public ICommand UpgradeCommand { get; } //обновление




        // ===== КОНСТРУКТОР =====
        public MainViewModel ( IModbusTCPService modbusService, IDataBaseService dataBase, ICalibration calibration, IServiceProvider serviceProvider, IExcele excele )
        {
            try
            {
                // Инициализация сервисов
                
                _random = new Random();
                _modbusService = modbusService;
                _calibration = calibration;
                _dataBase = dataBase; // создание экземпляра базы данных
                _serviceProvider = serviceProvider;
                _excele = excele;

                NavigationVM = new NavigationViewModel();
                NavigationVM.PropertyChanged += ( s, e ) => OnPropertyChanged(e.PropertyName);
                NomenclatureVM = new NomenclatureViewModel(_dataBase, serviceProvider, excele);
                NomenclatureVM.CurrentUser = CurrentUser;
                NomenclatureVM.PropertyChanged += ( s, e ) => OnPropertyChanged(e.PropertyName);
                SensorsVM = new SensorsViewModel(_dataBase, _calibration, _modbusService);
                SensorsVM.PropertyChanged += ( s, e ) => OnPropertyChanged(e.PropertyName);

                TerminalVM = new TerminalViewModel(_dataBase, _modbusService);
                TerminalVM.PropertyChanged += TerminalVM_PropertyChanged;


                LoadlNum = new ObservableCollection<NomenclatureUnit>(_dataBase.AllNum());
                AllNomenclatureUnits = _dataBase.AllNum();

                // Инициализация команд
                ZeroPointCommand = new RelayCommand(ExecuteZeroPoint); //sensor
                AddButtonUsers = new RelayCommand(ExecuteAddUser); //добавление при помощи кнопки
                DeleteNomCommand = new RelayCommand(ExecuteDeleteUnitCommand);
                UpgradeCommand = new RelayCommand(ExecuteUpgradeCommand);
                 OpenTestWindow = new RelayCommand(ExecuteOpenTestWindow); //реализация команды открытия тестового окна
                ApplyHotkeysCommand = new RelayCommand(ExecuteApplyHotkeys);

        //        AvailableActions = new ObservableCollection<ActionItem>
        //{
        //    new ActionItem { Name = "Добавить номенклатуру", Command = AddNomenclatureButton },
        //    new ActionItem { Name = "Отчет", Command = ExceleOtchet },
        //    new ActionItem { Name = "Удалить номенклатуру", Command = DeleteCommand },
        //    new ActionItem { Name = "Настройка склада", Command = SetingSklad },
        //    new ActionItem { Name = "Отчет", Command = ExceleOtchet },
        //    new ActionItem { Name = "Отчет", Command = ExceleOtchet }

        //};
                
            }
            catch (Exception ex)
            {
                //StatusText = $"Ошибка инициализации: {ex.Message}";
            }
        }

        private void ExecuteDeleteUnitCommand ( object param ) //sensors
        {
            if (param is Sensor sensor)
            {
                // Теперь у нас есть доступ к конкретному датчику
                string selectedNomenclature = sensor.SelectedNomenclature;
                int weight = sensor.Weight;

                if(selectedNomenclature == null)
                {
                    MessageBox.Show("ничего не выбрано");
                }
                else
                {
                    var product = _dataBase.AllNum().FirstOrDefault(x => x.Name == selectedNomenclature);

                    _dataBase.DeleteUnit(selectedNomenclature, weight);

                    if(product != null)
                    {
                        _dataBase.SaveNomenclature(product.Id, "списание", weight, CurrentUser?.Login);//это команда для сохранения номенклатуры
                        
                    }
                    
                }

            }

        }

        private async void ExecuteUpgradeCommand(object param )
        {
            try
            {
                // Создаём менеджер обновлений, указывая на GitHub репозиторий
                var updateManager = new UpdateManager(
                    new GithubSource(
                        repoUrl: "https://github.com/Nike888999/New2APP",
                        accessToken: null,        // для публичного репозитория токен не нужен
                        prerelease: false
                    )
                );

                // Проверяем наличие новой версии
                var newVersion = await updateManager.CheckForUpdatesAsync();

                if (newVersion == null)
                {
                    MessageBox.Show("У Вас последняя версия", "Обновление",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Спрашиваем пользователя
                var result = MessageBox.Show(
                    $"Доступна новая версия {newVersion.TargetFullRelease.Version}!\n\n" +
                    $"Текущая версия: {updateManager.CurrentVersion?.ToString() ?? "Неизвестно"}\n\n" +
                    "Обновить сейчас?",
                    "Доступно обновление",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Показываем прогресс (опционально)
                    //StatusText = "Загрузка обновления...";

                    // Скачиваем обновление
                    await updateManager.DownloadUpdatesAsync(newVersion);

                    // Применяем и перезапускаем
                    updateManager.ApplyUpdatesAndRestart(newVersion);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке обновлений: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteApplyHotkeys ( object param )
        {
            if (Hotkey1Action != null && Hotkey1Action.Command != null)
            {
                OpenComand = Hotkey1Action.Command;
                ButtonContent = Hotkey1Action.Name;
            }
            else
            {
                // Если ничего не выбрано или команда отсутствует
                MessageBox.Show("Пожалуйста, выберите действие из списка");
            }

        }

        private async void ExecuteZeroPoint ( object param )
        {
           
                try
                {
                await _modbusService.TareAfterExternal(TerminalVM.IpAddress,int.Parse(TerminalVM.Port), byte.Parse(TerminalVM.UnitId));
                    //await _calibration.CalibZero("192.168.0.56", 5000, 1);
                    MessageBox.Show("Калибровка выполнена успешно");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
        }

        private async void ExecuteAddUser ( object param ) //добавление пользователя
        {
            string role = UserRole;
            string dbRole = role switch
            {
                "Администратор" => "Admin",
                "Пользователь" => "User",
                _ => "User"
            }; 

            bool result = _dataBase.AddUser(IsAddUsersLogin, IsAddUsersPass, dbRole);

            if( result )
            {
                MessageBox.Show("Пользователь добавлен успешно");
                IsAddUsersLogin = "";
                IsAddUsersPass = "";
                
            }
            else
            {
                MessageBox.Show("Пользователь не добавлен");
            }
        }


        public void ExecuteOpenTestWindow( object param )
        {
            Window mainWindow = Application.Current.MainWindow;

            var testWeightWindow = new TestWeightWindow(this, _modbusService, _dataBase);
            testWeightWindow.Owner = mainWindow;
            testWeightWindow.Show();
        }

        // ===== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ =====


        // Очистка ресурсов
        public void Cleanup ( )
        {
            _timer?.Stop();
        }

        private void TerminalVM_PropertyChanged ( object sender, PropertyChangedEventArgs e )
        {
            // Когда коллекция терминалов изменилась
            if (e.PropertyName == nameof(TerminalVM.Terminals))
            {
                // Синхронизируем коллекции
                //SynchronizeTerminals();
            }
            // Когда выбран другой терминал
            else if (e.PropertyName == nameof(TerminalVM.SelectedTerminal))
            {
                // Обновляем видимые датчики
                //UpdateVisibleSensors();
            }
        }

        //private void SynchronizeTerminals ( )
        //{
        //    // Очищаем старую коллекцию
        //    Terminals.Clear();

        //    // Добавляем все терминалы из TerminalVM
        //    foreach (var terminal in TerminalVM.Terminals)
        //    {
        //        Terminals.Add(terminal);
        //    }

        //    // Обновляем отображение датчиков
        //    //UpdateVisibleSensors();
        //}


        // ===== INotifyPropertyChanged =====
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged ( [CallerMemberName] string name = null )
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        // Добавь в класс MainViewModel (в любое место, например после других полей)

        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();


                if (NomenclatureVM != null)
                {
                    NomenclatureVM.CurrentUser = value;
                }

                if (SensorsVM != null)
                {
                    SensorsVM.CurrentUser = value;
                }

                if (NavigationVM != null)
                {
                    NavigationVM.IsAdmin = IsAdmin;  // Теперь NavigationVM узнает о роли
                }
                //VisibleAdmin();
                //VisibleAdminButton();
            }
        }

        public bool IsAdmin => CurrentUser?.Role == "Admin";
        public bool IsUser => CurrentUser?.Role == "User";
        
    }
}

