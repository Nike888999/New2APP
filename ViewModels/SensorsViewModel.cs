using NewAPP;
using NewAPP.Models;
using NewAPP.Services;
using NewAPP.View;
using NewAPP.ViewModels;
using NewAPP2.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace NewAPP2.ViewModels
{
    public class SensorsViewModel : INotifyPropertyChanged
    {
        //Поля
        private DispatcherTimer _timer;
        private Random _random;
        private readonly IModbusTCPService _modbusTCPService;
        private bool _useRealData;
        public  TerminalViewModel TerminalVM { get; set; }
        private readonly ICalibration _calibretion;
        private readonly IDataBaseService _dataBaseService;
        private User _currentUser;
        private Sensor _sensor;
        

        //КОЛЛЕКЦИИ
        public ObservableCollection<Terminal> Terminals { get; set; } //sensor
        public ObservableCollection<Sensor> VisibleSensors { get; set; } //sensor

        //Комбобоксы для открытия датчиков
        public List<int> AvailableCounts { get; } = new List<int> { 16, 24, 32 }; //датчики
        public List<int> AvailableColumns { get; } = new List<int> { 4, 6, 8 }; //колонки
        public List<DisplayConfiguration> DisplayConfigurations { get; } = new List<DisplayConfiguration>()
        {
            new DisplayConfiguration {Name = "4x4", Columns = 4, Sensors = 16,},
            new DisplayConfiguration {Name = "6x4", Columns = 6, Sensors = 24},
            new DisplayConfiguration {Name = "8x4", Columns = 8, Sensors = 32},
        };

        public DisplayConfiguration _display;
        public DisplayConfiguration Display
        {
            get => _display;
            set
            {
                if (_display == value) return;
                _display = value;
                OnPropertyChanged();

                if (value != null)
                {
                    SelectedColumns = value.Columns;
                    SelectedSensorsCount = value.Sensors;
                }
            }
        }
        //свойство датчиков по умолчанию
        private int _selectedSensorsCount = 16; //данные по датчикам по умолчанию
        public int SelectedSensorsCount
        {
            get => _selectedSensorsCount;
            set
            {
                _selectedSensorsCount = value;
                OnPropertyChanged();
                //UpdateVisibleSensors();
            }
        }


        private int _selectedColumns = 4; // данные по колонкам по умолчанию и выбранное значение
        public int SelectedColumns
        {
            get => _selectedColumns;
            set { _selectedColumns = value; OnPropertyChanged(); }
        }

        private int _row = 1;
        private int _cell = 16;
        private int _shelf = 1;

        public int Row
        {
            get => _row;
            set { _row = value; OnPropertyChanged(); }
        }

        public int Cell
        {
            get => _cell;
            set { _cell = value; OnPropertyChanged(); }
        }

        public int Shelf
        {
            get => _shelf;
            set { _shelf = value; OnPropertyChanged(); }
        }
        //свойства для калибровки нуля и веса
        private string _terminalNumber; //добавлено свойство для ввода терминала
        public string TerminalNumber
        {
            get => _terminalNumber;
            set
            {
                _terminalNumber = value;
                OnPropertyChanged();
            }
        }

        private string _etalonWeight = "100"; //эталонный вес
        public string EtalonWeight
        {
            get => _etalonWeight;
            set
            {
                _etalonWeight = value; OnPropertyChanged();
            }
        }
        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAdmin));
                OnPropertyChanged(nameof(IsUser));
                OnPropertyChanged(nameof(UserInfo));
                OnPropertyChanged(nameof(WelcomeMessage));


            }
        }
        //Свойства отображения
        private bool _isRunning;
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StartButtonText));
                OnPropertyChanged(nameof(StartButtonColor));
            }
        }

        private string _statusText = "Готов к работе";
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }

        private string _sensorsCountText = "0/60";
        public string SensorsCountText
        {
            get => _sensorsCountText;
            set { _sensorsCountText = value; OnPropertyChanged(); }
        }

        // ===== ВЫЧИСЛЯЕМЫЕ СВОЙСТВА ДЛЯ UI =====
        public string StartButtonText => IsRunning ? "⏸ ПАУЗА" : "▶ СТАРТ";
        public Brush StartButtonColor => IsRunning ? Brushes.Orange : Brushes.Green;

        public string ModeButtonText => _useRealData ? "📡 MODBUS" : "🎮 ТЕСТ";
        public Brush ModeButtonColor => _useRealData ? Brushes.DodgerBlue : Brushes.Orange;

        public string ModeText => _useRealData ? "MODBUS" : "ТЕСТОВЫЙ";
        public Brush ModeTextColor => _useRealData ? Brushes.DodgerBlue : Brushes.Orange;

        public string UserText => IsAdmin ? "Администратор" : "Пользователь";

        // ===== СВОЙСТВА ДЛЯ НАСТРОЕК =====
        private string _ipAddress; //192.168.0.056" //"10.10.2.180"
        public string IpAddress
        {
            get => _ipAddress;
            set { _ipAddress = value; OnPropertyChanged(); }
        }

        private string _port;// "502";
        public string Port
        {
            get => _port;
            set { _port = value; OnPropertyChanged(); }
        }

        private string _unitId = "1";
        public string UnitId
        {
            get => _unitId;
            set { _unitId = value; OnPropertyChanged(); }
        }
        //команды
        public ICommand StartStopCommand { get; }
        public ICommand SwitchModeCommand { get; }
        public ICommand TestConnectionCommand { get; }
        public ICommand DeleteNomCommand { get; }
        public ICommand SetingSklad { get; }

        //конструктор
        public SensorsViewModel (IDataBaseService dataBaseService, ICalibration calibration, IModbusTCPService modbusTCPService)
        {
            _calibretion = calibration;
            _dataBaseService = dataBaseService;
            _modbusTCPService = modbusTCPService;
            _random = new Random();

            Terminals = new ObservableCollection<Terminal>();
            VisibleSensors = new ObservableCollection<Sensor>();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.1);
            _timer.Tick += Timer_Tick;

            TerminalVM = new TerminalViewModel(_dataBaseService, _modbusTCPService);
            TerminalVM.TerminalsChanged += ( ) =>
            {
                SyncTerminalsFromTerminalVM();
            };
            TerminalVM.PropertyChanged += TerminalVM_PropertyChanged;

            TerminalVM.IpAddress = "10.10.2.180";
            TerminalVM.Port = "502";
            TerminalVM.UnitId = "1";
            TerminalVM.TerminalName = "Терминал 1";

            //Команды
            StartStopCommand = new RelayCommand(ExecuteStartStop);
            SwitchModeCommand = new RelayCommand(ExecuteSwitchMode);
            TestConnectionCommand = new RelayCommand(ExecuteTestConnection);
            //DeleteNomCommand = new RelayCommand(ExecuteStartStop);
            SetingSklad = new RelayCommand(ExecuteSetSklad);

            InitializeTerminals();
            UpdateVisibleSensors();
            AutoCheckConnectionOnStartup();
        }

        //методы

        private void InitializeTerminals ( )
        {
            string terminalIp = TerminalVM.IpAddress;
            string terminalPort = TerminalVM.Port;
            string terminalUnitId = TerminalVM.UnitId;
            string terminalName = TerminalVM.TerminalName;



            var terminal = new Terminal
            {
                Id = 1,
                Name = terminalName,
                IpAddress = terminalIp,
                Port = int.Parse(terminalPort),
                UnitId = byte.Parse(terminalUnitId)
            };

            for (int i = 1; i <= Cell; i++)
            {
                //for (int cellNumber = 1; cellNumber <= Cell; cellNumber++)
                //{
                terminal.Sensors.Add(new Sensor
                {
                    Row = Row,
                    Shelf = Shelf,
                    Cell = i,
                    Name = $"Адрес",//вместо наименования датчика
                    Weight = 20 + i,
                    RegisterAddress = (ushort)(30 + (i - 1) * 2),
                    IsConnected = false,
                    IsFirstRead = true
                });
                // }
            }

            Terminals.Clear();
            Terminals.Add(terminal);
        }


        private void UpdateVisibleSensors ( )
        {
            if (Terminals.Count == 0) return;

            var allSensors = Terminals
                .SelectMany(t => t.Sensors)
                .Take(SelectedSensorsCount)
                .ToList();

            VisibleSensors.Clear();
            foreach (var sensor in allSensors)
                VisibleSensors.Add(sensor);

            //SensorsCountText = $"{VisibleSensors.Count}/60";
        }

        // ===== КОМАНДЫ =====
        private void ExecuteStartStop ( object param )
        {
            if (_timer.IsEnabled)
            {
                _timer.Stop();
                StatusText = "Остановлено";
            }
            else
            {
                _timer.Start();
                StatusText = "Сбор данных...";
            }
            IsRunning = _timer.IsEnabled;
        }

        private void ExecuteSwitchMode ( object param )
        {
            _useRealData = !_useRealData;
            OnPropertyChanged(nameof(ModeButtonText));
            OnPropertyChanged(nameof(ModeButtonColor));
            OnPropertyChanged(nameof(ModeText));
            OnPropertyChanged(nameof(ModeTextColor));

            StatusText = _useRealData ? "Режим MODBUS (реальные данные)" : "Тестовый режим";
        }

        private async void ExecuteTestConnection ( object param )
        {
            if (Terminals.Count == 0) return;

            var terminal = Terminals[0];
            StatusText = "Тестирование подключения...";

            try
            {
                bool connected = await _modbusTCPService.TestConnectionAsync(terminal.IpAddress, terminal.Port);
                StatusText = connected
                    ? $"✅ Подключено к {terminal.IpAddress}:{terminal.Port}"
                    : $"❌ Не удалось подключиться к {terminal.IpAddress}:{terminal.Port}";
                OnPropertyChanged(nameof(TerminalVM.ActivaTerminal));   // ← Обновление активных
                OnPropertyChanged(nameof(TerminalVM.TotalTerminal));    // ← Обновление общего
            }
            catch (Exception ex)
            {
                StatusText = $"❌ Ошибка: {ex.Message}";
            }
        }
        //под вопросом настройки склада
        private void ExecuteSetSklad ( object param )  // настройки склада
        {
            var setSkladWindow = new SetSkladWindow();
            setSkladWindow.actionSklad += ActionSklad;
            setSkladWindow.Show();
        }

        public void ActionSklad ( (int, int, int) data )
        {
            Row = data.Item1;
            Cell = data.Item2;
            Shelf = data.Item3;
            InitializeTerminals(); // ← Переинициализация с новым Row
            UpdateVisibleSensors(); // ← Обновляем отображение
        }

        //настройки калибровки сюда же введем
        private async void ExecuteZeroPoint ( object param )
        {

            try
            {
                await _modbusTCPService.TareAfterExternal(TerminalVM.IpAddress, int.Parse(TerminalVM.Port), byte.Parse(TerminalVM.UnitId));
                //await _calibration.CalibZero("192.168.0.56", 5000, 1);
                MessageBox.Show("Калибровка выполнена успешно");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async void ExecuteEtalonPoint ( object param )
        {
            try
            {
                // Получаем номер терминала из свойства
                if (!byte.TryParse(TerminalNumber, out byte terminalAddress))
                {
                    MessageBox.Show("Введите корректный номер терминала (1-255)", "Ошибка");
                    return;
                }

                //Здесь нужно также добавить свойство для эталонного веса
                int weight = int.Parse(EtalonWeight);

                await _calibretion.CalibWeight(TerminalVM.IpAddress, int.Parse(TerminalVM.Port), byte.Parse(TerminalVM.UnitId), weight);
                MessageBox.Show("Калибровка весом выполнена успешно", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        
        // ===== ТАЙМЕР =====
        private async void Timer_Tick ( object sender, EventArgs e )
        {
            if (_useRealData)
            {
                await ReadRealDataAsync();
            }
            else
            {
                UpdateTestData();
            }

            //UpdateConnectedCount();
        }

        private void UpdateTestData ( ) //расчет тестовых данных на датчик
        {
            foreach (var terminal in Terminals)
            {
                foreach (var sensor in terminal.Sensors)
                {
                    int change = _random.Next(-6, 6);
                    sensor.Weight = Math.Max(0, sensor.Weight + change);
                    sensor.IsConnected = true;
                }
            }
        }


        private async Task ReadRealDataAsync ( )
        {
            if (Terminals.Count == 0) return;
            var terminal = TerminalVM.SelectedTerminal ?? TerminalVM.Terminals.FirstOrDefault();

            var sensors = VisibleSensors.ToList();

            var tasks = sensors.Select(async sensor =>
            {
                try
                {
                    int weightInGrams = await _modbusTCPService.ReadWeightAsync(
                        terminal.IpAddress,
                        terminal.Port,
                        terminal.UnitId,
                        sensor.RegisterAddress);

                    // ===== ГЛАВНОЕ ИЗМЕНЕНИЕ =====
                    // Если есть выбранная номенклатура - переводим в штуки
                    int weightInPieces = weightInGrams;
                    if (!string.IsNullOrEmpty(sensor.SelectedNomenclature))
                    {
                        var product = _dataBaseService.AllNum().FirstOrDefault(x => x.Name == sensor.SelectedNomenclature);
                        if (product != null && product.WeightUnit > 0)
                        {
                            // Проверяем, чтобы не было переполнения
                            if (weightInGrams >= 0 && product.WeightUnit > 0)
                            {
                                weightInPieces = weightInGrams / product.WeightUnit;
                                sensor.WeightUnit = product.WeightUnit;
                            }
                        }
                    }

                    return (sensor, weight: weightInPieces, connected: true);
                }
                catch
                {
                    return (sensor, weight: 0, connected: false);
                }
            });

            var results = await Task.WhenAll(tasks);

            // Обновляем результаты
            foreach (var (sensor, weight, connected) in results)
            {
                int previousWeight = sensor.Weight;  // предыдущее количество в ШТУКАХ
                sensor.Weight = weight;               // новое количество в ШТУКАХ
                sensor.IsConnected = connected;

                bool wasFirstRead = sensor.IsFirstRead;
                if (wasFirstRead)
                {
                    sensor.IsFirstRead = false;
                    continue;
                }

                // ✅ Если предыдущий вес был 0 (и это не первое чтение) — тоже пропускаем
                //    (на случай, если весы обнулились и положили товар)
                if (previousWeight == 0)
                {
                    continue;
                }

                // Проверяем изменение для обновления БД
                if (sensor.IsConnected && !string.IsNullOrEmpty(sensor.SelectedNomenclature) && sensor.WeightUnit > 0)
                {
                    int difference = sensor.Weight - previousWeight;

                    if (difference != 0)
                    {
                        int unitsChanged = Math.Abs(difference);

                        if (difference < 0)  // Уменьшилось - товар убрали
                        {
                            _dataBaseService.DeleteUnit(sensor.SelectedNomenclature, unitsChanged);
                            var product = _dataBaseService.AllNum().FirstOrDefault(x => x.Name == sensor.SelectedNomenclature);
                            if (product != null)
                            {
                                _dataBaseService.SaveNomenclature(product.Id, "Списание", unitsChanged, CurrentUser?.Login ?? "System");
                            }
                        }
                        else if (difference > 0)  // Увеличилось - товар добавили
                        {
                            _dataBaseService.AddUnit(sensor.SelectedNomenclature, unitsChanged);
                            var product = _dataBaseService.AllNum().FirstOrDefault(x => x.Name == sensor.SelectedNomenclature);
                            if (product != null)
                            {
                                _dataBaseService.SaveNomenclature(product.Id, "Добавление", unitsChanged, CurrentUser?.Login ?? "System");
                            }
                        }
                    }
                }
            }
        }

        private async Task AutoCheckConnectionOnStartup ( )
        {
            try
            {
                // Получаем терминал из TerminalVM
                var terminal = TerminalVM?.SelectedTerminal ?? TerminalVM?.Terminals.FirstOrDefault();

                if (terminal == null)
                {
                    System.Diagnostics.Debug.WriteLine("Нет доступных терминалов для проверки");
                    StatusText = "Нет настроенных терминалов. Добавьте терминал в разделе 'Сеть'";
                    return;
                }

                // Проверяем корректность данных
                if (string.IsNullOrWhiteSpace(terminal.IpAddress))
                {
                    System.Diagnostics.Debug.WriteLine("IP адрес терминала не задан");
                    StatusText = "IP адрес терминала не задан. Настройте терминал в разделе 'Сеть'";
                    return;
                }

                if (terminal.Port <= 0 || terminal.Port > 65535)
                {
                    System.Diagnostics.Debug.WriteLine($"Некорректный порт: {terminal.Port}");
                    StatusText = $"Некорректный порт: {terminal.Port}. Проверьте настройки терминала";
                    return;
                }

                StatusText = $"Проверка подключения к терминалу {terminal.Name} ({terminal.IpAddress}:{terminal.Port})...";

                // Выполняем проверку подключения
                bool connected = await _modbusTCPService.TestConnectionAsync(terminal.IpAddress, terminal.Port);

                // Обновляем статус терминала
                terminal.IsConnected = connected;
                terminal.StatusText = connected ? "Online" : "Offline";
                terminal.StatusColor = connected ? "Green" : "Red";

                // Обновляем статистику в TerminalVM
                TerminalVM.OnPropertyChanged(nameof(TerminalVM.ActivaTerminal));
                TerminalVM.OnPropertyChanged(nameof(TerminalVM.TotalTerminal));

                // Обновляем общий статус
                if (connected)
                {
                    StatusText = $"✅ Терминал {terminal.Name} подключен. Готов к работе.";
                    System.Diagnostics.Debug.WriteLine($"Автопроверка: терминал {terminal.IpAddress}:{terminal.Port} - ДОСТУПЕН");
                }
                else
                {
                    StatusText = $"❌ Терминал {terminal.Name} недоступен. Проверьте подключение.";
                    System.Diagnostics.Debug.WriteLine($"Автопроверка: терминал {terminal.IpAddress}:{terminal.Port} - НЕДОСТУПЕН");
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Ошибка при проверке подключения: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Ошибка автопроверки: {ex.Message}");
            }
        }

        //вспомогательный метод проверки
        private void TerminalVM_PropertyChanged ( object sender, PropertyChangedEventArgs e )
        {
            // Когда коллекция терминалов изменилась
            if (e.PropertyName == nameof(TerminalVM.Terminals))
            {
                // Синхронизируем коллекции
                SynchronizeTerminals();
            }
            // Когда выбран другой терминал
            else if (e.PropertyName == nameof(TerminalVM.SelectedTerminal))
            {
                // Обновляем видимые датчики
                UpdateVisibleSensors();
            }
        }
        private void SynchronizeTerminals ( )
        {
            // Очищаем старую коллекцию
            Terminals.Clear();

            // Добавляем все терминалы из TerminalVM
            foreach (var terminal in TerminalVM.Terminals)
            {
                Terminals.Add(terminal);
            }

            // Обновляем отображение датчиков
            UpdateVisibleSensors();
        }

        public bool IsAdmin => CurrentUser?.Role == "Admin";
        public bool IsUser => CurrentUser?.Role == "User";
        public string UserInfo => CurrentUser != null ? $"{CurrentUser.Login} ({CurrentUser.Role})" : "Не авторизован";
        public string WelcomeMessage => CurrentUser != null ? $"Добро пожаловать, {CurrentUser.Login}!" : "Добро пожаловать!";

        public void SyncTerminalsFromTerminalVM ( )
        {
            Terminals.Clear();
            foreach (var terminal in TerminalVM.Terminals)
            {
                Terminals.Add(terminal);
            }
            UpdateVisibleSensors();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged ( [CallerMemberName] string name = null )
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
