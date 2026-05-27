using NewAPP.Services;
using NewAPP2.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;
using static OfficeOpenXml.ExcelErrorValue;

namespace NewAPP.View
{
    /// <summary>
    /// Логика взаимодействия для TestWeightWindow.xaml
    /// </summary>
    public partial class TestWeightWindow : Window
    {
        private readonly IDataBaseService _db;
        DispatcherTimer _timer;
        private readonly IModbusTCPService _tcp;
        private readonly MainViewModel _main;

        string ip = "10.10.2.180";
        int port = 502;
        int WeightUnit = 1;

        private int _currentWeight;
        private int _lastWeight;
        private int _stableCount = 0;
        private bool _isReading = false;

        private int _weight;
        public int Weight 
        {
            get => _weight;
            set
            {
                _weight = value;
                WeightText.Text = value.ToString();
                CheckWeight(value);
            }
        }

        private string _selectedNomenclature;
        public string SelectedNomenclature
        {
            get => _selectedNomenclature;
            set
            {
                _selectedNomenclature = value;
                OnPropertyChanged();
                if(!string.IsNullOrEmpty(value))
                {
                    var product = _db.AllNum().FirstOrDefault(x => x.Name == value);
                    if(product != null)
                    {
                        WeightUnit = product.WeightUnit;
                        WeightTextBox.Text = WeightUnit.ToString();
                    }
                }
            }
        }
        MainViewModel main;

        public TestWeightWindow ( MainViewModel main, IModbusTCPService tcp, IDataBaseService db)
        {
            InitializeComponent();
            _db = db;
            _tcp = tcp;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            ComboName.ItemsSource = _db.AllNum();
            DataContext = this;
            _main = main;
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            await ReadWeightAsync();
        }

        private async Task ReadWeightAsync ()
        {
            try
            {
                WeightUnit = int.Parse(WeightTextBox.Text); // вес который мы будем делить
                int weight = await Task.Run(( ) => _tcp.tcpGmt(ip, port));
                Dispatcher.Invoke(( ) =>
                {
                    Weight = weight / WeightUnit;
                });
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private void CheckWeight(int newWeight)
        {
            if (SelectedNomenclature == null) return;

            if (_lastWeight == 0)  // Если это первый вызов
            {
                _lastWeight = newWeight;  // Просто запоминаем вес
                return;                   // Выходим, не сохраняя в БД
            }

            int difference = Math.Abs(newWeight - _lastWeight);


          

            if (difference >= 5)
            {
                if(newWeight < _lastWeight)
                {
                    _db.DeleteUnit(SelectedNomenclature, difference);
                    var product = _db.AllNum().FirstOrDefault(x => x.Name == SelectedNomenclature);
                    if (product != null)
                    {
                        _db.SaveNomenclature(product.Id, "списание", difference, main.CurrentUser?.Login ?? "Lolkek");//это команда для сохранения номенклатуры

                    }
                }
                if(newWeight > _lastWeight)
                {
                    _db.AddUnit(SelectedNomenclature, difference);
                    var product = _db.AllNum().FirstOrDefault(x => x.Name == SelectedNomenclature);
                    if (product != null)
                    {
                        _db.SaveNomenclature(product.Id, "добавление", difference, main.CurrentUser?.Login ?? "Lolkek");//это команда для сохранения номенклатуры

                    }
                }
            }
            _lastWeight = newWeight;
        }

        public void Delete_Click (object sender, EventArgs e)
        {
            _db.DeleteUnit(SelectedNomenclature, 5);
            var product = _db.AllNum().FirstOrDefault(x => x.Name == SelectedNomenclature);
            if (product != null)
            {
                _db.SaveNomenclature(product.Id, "списание", 5, main.CurrentUser?.Login ?? "LolChek");//это команда для сохранения номенклатуры

            }
        }

        public void Start_Click( object sender, RoutedEventArgs e )
        {
            _timer.Start();
            
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged ( [CallerMemberName] string name = null )
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
