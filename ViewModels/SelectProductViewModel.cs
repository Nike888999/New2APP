using NewAPP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace NewAPP.ViewModels
{
    public class SelectProductViewModel // INotifyCollectionChanged
    {

        //private readonly ObservableCollection<NomenclatureUnit> _allProducts;
        //private ObservableCollection<NomenclatureUnit> _availableProducts;
        //private ObservableCollection<NomenclatureUnit> _selectedProducts;

        //private string _searchText;
        //private ICollectionView _availableCollectionView;

        //public event PropertyChangedEventHandler PropertyChanged; //хз зачем

        //public SelectProductViewModel(ObservableCollection<NomenclatureUnit> allProducts)
        //{
        //    _allProducts = allProducts ?? throw new ArgumentNullException(nameof(allProducts));

        //    AvailableProducts = new ObservableCollection<NomenclatureUnit>(allProducts);
        //    SelectedProducts = new ObservableCollection<NomenclatureUnit>();

        //    AddSelectedCommand = new RelayCommand(_ => AddSelected(), _ => CanAddSelected);
        //    AddAllCommand = new RelayCommand(_ => AddAll());
        //    RemoveSelectedCommand = new RelayCommand(_ => RemoveSelected, _ => CanRemoveSelected());
        //    RemoveAllCommand = new RelayCommand(_ => RemoveAll());
        //    SearchCommand = new RelayCommand(_ => _searchText());

        //    SetupAvailableProductView();
        //}
    }
}
