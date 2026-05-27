using NewAPP.Models;
using NewAPP.Services;
using NewAPP2.Interface;
using NewAPP2.ViewModels;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NewAPP.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       private readonly MainViewModel _viewModel;
        private readonly IDataBaseService _dataBase;


        public MainWindow (MainViewModel viewModel, IDataBaseService dataBase )
        {
            InitializeComponent();

            _viewModel = viewModel;
            _dataBase = dataBase;
            this.DataContext = _viewModel;
            this.Width = 1400;
            this.Height = 800;

            // Подписка на событие Closed (не OnClosing!)
            //this.Closed += ( s, e ) => _viewModel.Cleanup();
            _viewModel.PropertyChanged += OnViewModelPropertyChanger;
        }

        // Только специфичные для окна обработчики
        private void Window_PreviewKeyDown ( object sender, KeyEventArgs e )
        {
            if (e.Key == Key.F5 && _viewModel != null)
            {
                // Можно добавить команду обновления
                e.Handled = true;
            }
        }
        private void DataGrid_MouseDoubleClick ( object sender, MouseButtonEventArgs e )
        {
            var grid = sender as DataGrid;
            var selectedProduct = grid.SelectedItem as NomenclatureUnit;

            if (selectedProduct != null)
            {
                var history = _dataBase.GetOperationHistory(selectedProduct.Id);

                var historyWindow = new HistoryWindow();
                historyWindow.SetData(selectedProduct.Name, history);
                historyWindow.ShowDialog();
            }
        }
        private void OnViewModelPropertyChanger(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(NomenclatureViewModel.ShowId) ||
               e.PropertyName == nameof(NomenclatureViewModel.ShowName) ||
               e.PropertyName == nameof(NomenclatureViewModel.ShowUnit) ||
               e.PropertyName == nameof(NomenclatureViewModel.ShowAdr) ||
               e.PropertyName == nameof(NomenclatureViewModel.ShowInNumber) ||
               e.PropertyName == nameof(NomenclatureViewModel.ShowOutNumber) ||
               e.PropertyName == nameof(NomenclatureViewModel.ShowCharacteristick) ||
               e.PropertyName == nameof(NomenclatureViewModel.ShowQuantity) ||
               e.PropertyName == nameof(NomenclatureViewModel.ShowUnitPrice) ||
               e.PropertyName == nameof(NomenclatureViewModel.ShowWeightUnit))
            {
                UpdateColumnsVisibility();
            }
        }

        private void UpdateColumnsVisibility ( )
        {
            foreach (var column in NumGride.Columns)
            {
                switch (column.Header.ToString())
                {
                    case "Номер":
                        column.Visibility = _viewModel.NomenclatureVM.ShowId;
                        break;
                    case "Наименование":
                        column.Visibility = _viewModel.NomenclatureVM.ShowName;
                        break;
                    case "Вн.номер":
                        column.Visibility = _viewModel.NomenclatureVM.ShowInNumber;
                        break;  
                    case "Внут.номер":
                        column.Visibility = _viewModel.NomenclatureVM.ShowOutNumber;
                        break;
                    case "Характеристика":
                        column.Visibility = _viewModel.NomenclatureVM.ShowCharacteristick;
                        break;
                    case "Ед.изм.":
                        column.Visibility = _viewModel.NomenclatureVM.ShowUnit;
                        break;
                    case "Адрес":
                        column.Visibility = _viewModel.NomenclatureVM.ShowAdr;
                        break;
                    case "Остаток":
                        column.Visibility = _viewModel.NomenclatureVM.ShowQuantity;
                        break;
                    case "Цена за единицу":
                        column.Visibility = _viewModel.NomenclatureVM.ShowUnitPrice;
                        break;
                    case "Вес единицы":
                        column.Visibility = _viewModel.NomenclatureVM.ShowWeightUnit;
                        break;
                }
            }
        }


    }
}
