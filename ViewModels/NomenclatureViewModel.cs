using Microsoft.Extensions.DependencyInjection;

using NewAPP;
using NewAPP.Models;
using NewAPP.Services;
using NewAPP.View;
using NewAPP2.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace NewAPP2.ViewModels
{
    public class NomenclatureViewModel : INotifyPropertyChanged
    {
        //сервисы
        private readonly IDataBaseService _dataBase;
        private readonly IServiceProvider _serviceProvider;
        public User CurrentUser
        {
            get; set;
        
        }
        private readonly IExcele _excele;

        //свойства
        private ObservableCollection<NomenclatureUnit> _nomenclatureItems; //свойство для NumGride.ItemsSource
        public ObservableCollection<NomenclatureUnit> NomenclatureItems
        {
            get => _nomenclatureItems;
            set { _nomenclatureItems = value; OnPropertyChanged(); }
        }
        //для добавления номенклатуры
        private string _name;
        public string Name //имя для добавления датчика
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(); // ← Уведомляем UI об изменении
            }
        }

        private string _unit;//ед.изм
        public string Unit
        {
            get => _unit;
            set
            {
                _unit = value;
                OnPropertyChanged();
            }
        }

        private string _internalArticle; //внешний номер
        public string InternalArticle
        {
            get => _internalArticle;
            set
            {
                _internalArticle = value;
                OnPropertyChanged();
            }
        }

        private string _externalArticle; //внутренний номер
        public string ExternalArticle
        {
            get => _externalArticle;
            set
            {
                _externalArticle = value;
                OnPropertyChanged();
            }
        }

        private string _characteristic; //характеристика
        public string Characteristic
        {
            get => _characteristic;
            set
            {
                _characteristic = value;
                OnPropertyChanged();
            }
        }

        private string _serialNumber; //серийный номер
        public string SerialNumber
        {
            get => _serialNumber;
            set
            {
                _serialNumber = value;
                OnPropertyChanged();
            }
        }

        private string _adressCell; //адресс ячейки
        public string AdressCell
        {
            get => _adressCell;
            set
            {
                _adressCell = value;
                OnPropertyChanged();
            }
        }

        private int _oldQuantity; //начальное кол-во
        public int OldQuantity
        {
            get => _oldQuantity;
            set
            {
                _oldQuantity = value;
                OnPropertyChanged();
            }
        }

        private int _operationTypeIn; //добавили
        public int OperationTypeIn
        {
            get => _operationTypeIn;
            set
            {
                _operationTypeIn = value;
                OnPropertyChanged();
            }
        }

        private int _operationTypeOut; //убрали
        public int OperationTypeOut
        {
            get => _operationTypeOut;
            set
            {
                _operationTypeOut = value;
                OnPropertyChanged();
            }
        }

        private int _newQuantity; //остаток
        public int NewQuantity
        {
            get => _newQuantity;
            set
            {
                _newQuantity = value;
                OnPropertyChanged();
            }
        }

        private int _unitPrice;
        public int UnitPrice  //свойство для цены за единицу
        {
            get => _unitPrice;
            set
            {
                _unitPrice = value;
                OnPropertyChanged();
            }
        }

        private int _weightUnit;
        public int WeightUnit //вес за ед товара
        {
            get => _weightUnit;
            set
            {
                _weightUnit = value;
                OnPropertyChanged();
            }
        }

        //для удаления номенклатуры 
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
        private string _searchText;
        public string SearchText 
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

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

        //видимость колонок
        private Visibility _showId;//показывать Id
        private Visibility _showName;//показывать имя
        private Visibility _showInNumber;//показывать вне.номер
        private Visibility _showOutNumber;//показывать внутр.номер
        private Visibility _showCharacteristick;//показывать характеристики
        private Visibility _showUnit;//показывать шт
        private Visibility _showAdr;//показывать адрес
        private Visibility _showQuantity;//показывать кол-во
        private Visibility _showUnitPrice;//показывать цену
        private Visibility _showWeightUnit; //вес за единицу

        public Visibility ShowId
        {
            get => _showId;
            set
            {
                _showId = value;
                OnPropertyChanged();
            }
        }
        public Visibility ShowName
        {
            get => _showName;
            set
            {
                _showName = value;
                OnPropertyChanged();
            }
        }
        public Visibility ShowInNumber
        {
            get => _showInNumber;
            set
            {
                _showInNumber = value;
                OnPropertyChanged();
            }
        }
        public Visibility ShowOutNumber
        {
            get => _showOutNumber;
            set
            {
                _showOutNumber = value;
                OnPropertyChanged();
            }
        }
        public Visibility ShowCharacteristick
        {
            get => _showCharacteristick;
            set
            {
                _showCharacteristick = value;
                OnPropertyChanged();
            }
        }
        public Visibility ShowUnit
        {
            get => _showUnit;
            set
            {
                _showUnit = value;
                OnPropertyChanged();
            }
        }
        public Visibility ShowAdr
        {
            get => _showAdr;
            set
            {
                _showAdr = value;
                OnPropertyChanged();
            }
        }
        public Visibility ShowQuantity
        {
            get => _showQuantity;
            set
            {
                _showQuantity = value;
                OnPropertyChanged();
            }
        }
        public Visibility ShowUnitPrice
        {
            get => _showUnitPrice;
            set
            {
                _showUnitPrice = value;
                OnPropertyChanged();
            }
        }

        public Visibility ShowWeightUnit
        {
            get => _showWeightUnit;
            set
            {
                _showWeightUnit = value;
                OnPropertyChanged();
            }
        }
        

        //команды
        public ICommand AddNomenclatureButton { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UpdateButtonNum { get; }
        public ICommand ExceleOtchet { get; }
        public ICommand OpenSettingVisible { get; }
        public ICommand SearchCommand { get; } 

        public NomenclatureViewModel(IDataBaseService dataBaseService, IServiceProvider serviceProvider,  IExcele excele)
        {
            _dataBase = dataBaseService;
            _serviceProvider = serviceProvider;
            //_user = user;
            _excele = excele;
            

            AddNomenclatureButton = new RelayCommand(ExecuteAddNomenclature);
            DeleteCommand = new RelayCommand(ExecuteDeleteCommand);
            OpenSettingVisible = new RelayCommand(ExecuteOpenSettingVisible);
            UpdateButtonNum = new RelayCommand(ExecuteUpdateButtonNum);
            ExceleOtchet = new RelayCommand(ExecuteExceleOtchet);
            SearchCommand = new RelayCommand(ExecuteSearchCommand); 
            LoadAllNomenclature();
        }
        //методы

        private void LoadAllNomenclature ( ) //подгружает номенклатуру 
        {
            try
            {
                var all = _dataBase.AllNum();
                NomenclatureItems = new ObservableCollection<NomenclatureUnit>(all);
                //StaticCount = NomenclatureItems?.Count ?? 0;
            }
            catch (Exception ex)
            {

                //StatusText = $"Ошибка загрузки: {ex.Message}";
            }

        }


        //метод добавления номенклатуры
        private async void ExecuteAddNomenclature ( object param ) //кнопка добавления номенклатуры
        {
            Window mainWindow = Application.Current.MainWindow; //окно которое привязывает код основного окна к дочерним
            var addNomenclatureWindow = _serviceProvider.GetRequiredService<AddNomenclatureWindow>();
            addNomenclatureWindow.Owner = mainWindow; //строка показывает что дочернее окно зависит от основоного
            addNomenclatureWindow.WindowStyle = WindowStyle.SingleBorderWindow; //создаем все кнопки в правом верхнем углу
            addNomenclatureWindow.ResizeMode = ResizeMode.CanResize; //создаем все кнопки в правом верхнем углу

            addNomenclatureWindow.addAction += addActiv;
            addNomenclatureWindow.Show();
            
            
        }

        private void addActiv ( (string val1, string val2, string val3, string val4, string val5, string val6, string val7, int val8, int val9, int val10, int val11, int val12, int val13) data )
        {
            Name = data.val1;
            InternalArticle = data.val2;
            ExternalArticle = data.val3;
            Characteristic = data.val4;
            SerialNumber = data.val5;
            Unit = data.val6;
            AdressCell = data.val7;
            OldQuantity = data.val8;
            OperationTypeIn = data.val9;
            OperationTypeOut = data.val10;
            NewQuantity = data.val11;
            UnitPrice = data.val12;
            WeightUnit = data.val13;
            _dataBase.AddNomenclature(Name, InternalArticle, ExternalArticle, Characteristic, SerialNumber, Unit, AdressCell, OldQuantity, OperationTypeIn, OperationTypeOut, NewQuantity, UnitPrice, WeightUnit);

            var all = _dataBase.AllNum();
            var addProduct = all.FirstOrDefault(x => x.Name == Name);
            if (addProduct != null)
            {
                _dataBase.SaveNomenclature(addProduct.Id, "Добавление", NewQuantity, CurrentUser?.Login);
            }


            MessageBox.Show("позиция добавлена");
        }

        //метод удаления 
        private async void ExecuteDeleteCommand ( object param )
        {
            var deleteNomenclatute = new DeleteNomenclatureWindow();
            deleteNomenclatute.deleteAction += deleteAction;
            deleteNomenclatute.Show();
            //if (deleteNomenclatute.ShowDialog() == true)
            //{
            //    DeleteName = deleteNomenclatute.Name;
            //    _dataBase.DeleteNomenclature(DeleteName);
            //}
        }

        public void deleteAction ( string name )
        {
            var product = _dataBase.AllNum().FirstOrDefault(x => x.Name == name);

            if (product != null)
            {
                _dataBase.SaveNomenclature(product.Id, "удаление", NewQuantity, CurrentUser?.Login);
            }

            DeleteName = name;
            _dataBase.DeleteNomenclature(DeleteName);

        }

        //открытие настроек видимости колонок
        public void ExecuteOpenSettingVisible ( object param )
        {
            var settingVisibleWindow = new SettingVisibleNom();

            settingVisibleWindow.chkId.IsChecked = ShowId == Visibility.Visible;
            settingVisibleWindow.chkName.IsChecked = ShowName == Visibility.Visible;
            settingVisibleWindow.chkInNumber.IsChecked = ShowInNumber == Visibility.Visible;
            settingVisibleWindow.chkOutNumber.IsChecked = ShowOutNumber == Visibility.Visible;
            settingVisibleWindow.chkHaracteristick.IsChecked = ShowCharacteristick == Visibility.Visible;
            settingVisibleWindow.chkUnit.IsChecked = ShowUnit == Visibility.Visible;
            settingVisibleWindow.chkAdr.IsChecked = ShowAdr == Visibility.Visible;
            settingVisibleWindow.chkQuntity.IsChecked = ShowQuantity == Visibility.Visible;
            settingVisibleWindow.chkPrice.IsChecked = ShowUnitPrice == Visibility.Visible;
            settingVisibleWindow.chkWeight.IsChecked = ShowWeightUnit == Visibility.Visible;

            if (settingVisibleWindow.ShowDialog() == true)
            {
                ShowId = settingVisibleWindow.ShowId ? Visibility.Visible : Visibility.Collapsed;
                ShowName = settingVisibleWindow.ShowName ? Visibility.Visible : Visibility.Collapsed;
                ShowInNumber = settingVisibleWindow.ShowInNumber ? Visibility.Visible : Visibility.Collapsed;
                ShowOutNumber = settingVisibleWindow.ShowOutNumber ? Visibility.Visible : Visibility.Collapsed;
                ShowCharacteristick = settingVisibleWindow.ShowCharacteristick ? Visibility.Visible : Visibility.Collapsed;
                ShowUnit = settingVisibleWindow.ShowUnit ? Visibility.Visible : Visibility.Collapsed;
                ShowAdr = settingVisibleWindow.ShowAdr ? Visibility.Visible : Visibility.Collapsed;
                ShowQuantity = settingVisibleWindow.ShowQuntity ? Visibility.Visible : Visibility.Collapsed;
                ShowUnitPrice = settingVisibleWindow.ShowUnitPrice ? Visibility.Visible : Visibility.Collapsed;
                ShowWeightUnit = settingVisibleWindow.ShowWeightUnit ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        //открытие отчета
        private void ExecuteExceleOtchet ( object param )
        {
            Window mainWindow = Application.Current.MainWindow;
            var all = _dataBase.AllNum();
            var reportWindow = new ReportWindow(all, _excele, _dataBase);
            reportWindow.Owner = mainWindow;
            reportWindow.Show();
        }

        //обновление номенклатуры 
        private void ExecuteUpdateButtonNum ( object param )
        {
            var all = _dataBase.AllNum();
            NomenclatureItems = new ObservableCollection<NomenclatureUnit>(all);
        }

        //поиск номенклатуры
        private void ExecuteSearchCommand ( object param )
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                // Если поиск пустой - показываем всю номенклатуру
                LoadAllNomenclature();
               // StatusText = $"Всего записей: {NomenclatureItems?.Count ?? 0}";
                return;
            }

            try
            {
                var results = _dataBase.SearchUnit(SearchText);
                // var results = _dataBase.SearchNomenclature2(SearchText);

                // ПРЕОБРАЗУЕМ NomenclatureItems в NomenclatureUnit для DataGrid
                var convertedResults = new ObservableCollection<NomenclatureUnit>();
                foreach (var item in results)
                {
                    convertedResults.Add(new NomenclatureUnit
                    {
                        Id = item.Id,
                        Name = item.Name,
                        InternalArticle = item.InternalArticle,
                        ExternalArticle = item.ExternalArticle,
                        Characteristic = item.Characteristic,
                        SerialNamber = item.SerialNamber,
                        Unit = item.Unit,
                        AddressCell = item.AddressCell,
                        OldQuantity = item.OldQuantity,
                        OperationTypeIn = item.OperationTypeIn,
                        OperationTypeOut = item.OperationTypeOut,
                        NewQuantity = item.NewQuantity,
                        UnitPrice = item.UnitPrice,
                        OperationDate = null
                    });
                }

                // Обновляем DataGrid результатами поиска
                NomenclatureItems = convertedResults;
                //StatusText = $"Найдено записей: {results.Count}";

                if (results.Count == 0)
                {
                    MessageBox.Show("Ничего не найдено!", "Поиск",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                //StatusText = $"Ошибка поиска: {ex.Message}";
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged ( [CallerMemberName] string name = null )
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
