using NewAPP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NewAPP.View
{
    /// <summary>
    /// Логика взаимодействия для SelectedProduct.xaml
    /// </summary>
    public partial class SelectedProduct : Window
    {
        private ObservableCollection<NomenclatureUnit> _allProduct;
        private ObservableCollection<NomenclatureUnit> _availableProducts;
        private ObservableCollection<NomenclatureUnit> _selectedProducts;

        private List<NomenclatureUnit> _selectedAvailable = new List<NomenclatureUnit>();
        private List<NomenclatureUnit> _selectedSelected = new List<NomenclatureUnit>();

        public List<NomenclatureUnit> Result { get; set; }

        public SelectedProduct ( List<NomenclatureUnit> allProducts, List<NomenclatureUnit> preselected = null )
        {
            InitializeComponent();

            _allProduct = new ObservableCollection<NomenclatureUnit>(allProducts);
            _availableProducts = new ObservableCollection<NomenclatureUnit>(allProducts);
            _selectedProducts = new ObservableCollection<NomenclatureUnit>();

            // ИСПРАВЛЕНО: имена из XAML
            AvailableProductsListBox.ItemsSource = _availableProducts;
            SelectedProductsListBox.ItemsSource = _selectedProducts;

            // Если есть предвыбранные товары
            if (preselected != null)
            {
                foreach (var product in preselected)
                {
                    AddProduct(product);
                }
            }

            UpdateUI();
        }

        public void AddProduct ( NomenclatureUnit nomenclatureUnit )
        {
            if (!_selectedProducts.Contains(nomenclatureUnit) && _availableProducts.Contains(nomenclatureUnit))
            {
                _availableProducts.Remove(nomenclatureUnit);
                _selectedProducts.Add(nomenclatureUnit);
                UpdateUI();
            }
        }

        public void AddProducts ( List<NomenclatureUnit> productsToAdd )
        {
            var toAdd = productsToAdd.Where(p => !_selectedProducts.Contains(p)).ToList();
            foreach (var product in toAdd)
            {
                _availableProducts.Remove(product);
                _selectedProducts.Add(product);
            }
            UpdateUI();
        }

        private void AddAll ( )
        {
            var all = _availableProducts.ToList();
            foreach (var product in all)
            {
                _selectedProducts.Add(product);
            }
            _availableProducts.Clear();
            UpdateUI();
        }

        private void RemoveProducts ( List<NomenclatureUnit> products )
        {
            foreach (var product in products)
            {
                _selectedProducts.Remove(product);
                _availableProducts.Add(product);
            }
            SortAvailable();
            UpdateUI();
        }

        private void RemoveAll ( )
        {
            var all = _selectedProducts.ToList();
            foreach (var product in all)
            {
                _selectedProducts.Remove(product);
                _availableProducts.Add(product);
            }
            SortAvailable();
            UpdateUI();
        }

        private void SortAvailable ( )
        {
            var sorted = _availableProducts.OrderBy(p => p.Name).ToList();
            _availableProducts.Clear();
            foreach (var product in sorted)
            {
                _availableProducts.Add(product);
            }
        }

        private void UpdateUI ( )
        {
            // ИСПРАВЛЕНО: имена из XAML
            AvailableCountText.Text = $"Доступно: {_availableProducts.Count}";
            SelectedCountText.Text = $"Выбрано: {_selectedProducts.Count}";
            Title = $"Выбор товаров - выбрано {_selectedProducts.Count}";

            AddSelectedButton.IsEnabled = _selectedAvailable.Any();
            AddAllButton.IsEnabled = _availableProducts.Any();
            RemoveSelectedButton.IsEnabled = _selectedSelected.Any();
            RemoveAllButton.IsEnabled = _selectedProducts.Any();
        }

        private void SearchTextBox_TextChanged ( object sender, TextChangedEventArgs e )
        {
            string searchText = SearchTextBox.Text?.ToLower() ?? "";

            if (string.IsNullOrWhiteSpace(searchText))
            {
                AvailableProductsListBox.ItemsSource = _availableProducts;
            }
            else
            {
                var filtered = _availableProducts
                    .Where(p => p.Name.ToLower().Contains(searchText))
                    .ToList();
                AvailableProductsListBox.ItemsSource = filtered;
            }
        }

        private void AvailableProductsListBox_SelectionChanged ( object sender, SelectionChangedEventArgs e )
        {
            // ИСПРАВЛЕНО: правильное имя ListBox и тип
            _selectedAvailable = AvailableProductsListBox.SelectedItems.Cast<NomenclatureUnit>().ToList();
            UpdateUI();
        }

        private void SelectedProductsListBox_SelectionChanged ( object sender, SelectionChangedEventArgs e )
        {
            // ИСПРАВЛЕНО: правильное имя ListBox и тип
            _selectedSelected = SelectedProductsListBox.SelectedItems.Cast<NomenclatureUnit>().ToList();
            UpdateUI();
        }

        private void AddSelectedButton_Click ( object sender, RoutedEventArgs e )
        {
            AddProducts(_selectedAvailable);
            _selectedAvailable.Clear();
            AvailableProductsListBox.SelectedItems.Clear();
            UpdateUI();
        }

        private void AddAllButton_Click ( object sender, RoutedEventArgs e )
        {
            AddAll();
            UpdateUI();
        }

        private void RemoveSelectedButton_Click ( object sender, RoutedEventArgs e )
        {
            RemoveProducts(_selectedSelected);
            _selectedSelected.Clear();
            SelectedProductsListBox.SelectedItems.Clear();
            UpdateUI();
        }

        private void RemoveAllButton_Click ( object sender, RoutedEventArgs e )
        {
            RemoveAll();
            UpdateUI();
        }

        private void OkButton_Click ( object sender, RoutedEventArgs e )
        {
            Result = _selectedProducts.ToList();
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click ( object sender, RoutedEventArgs e )
        {
            DialogResult = false;
            Close();
        }
    }
}