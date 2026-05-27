using System;
using System.Collections.Generic;
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
using NewAPP.Models;
using NewAPP.Services;
using NewAPP.View;
using NewAPP.ViewModels;

namespace NewAPP.View
{
    /// <summary>
    /// Логика взаимодействия для SetSkladWindow.xaml
    /// </summary>
    public partial class SetSkladWindow : Window
    {
        public  int Shelf { get; set; } //стелаж
        public int Cell { get; set; } // ячейка
        public int Row { get; set; }         //ряд
            
        public SetSkladWindow()
        {
            //SkladCommand = new RelayCommand(ExecuteSklad, CanExecuteSklad);
            InitializeComponent();
            this.DataContext = this;
            SkladCommand = new RelayCommand(ExecuteSklad, CanExecuteSklad);

            RowBox.TextChanged += ( s, e ) => RowPreview.Text = RowBox.Text;
            ShelfBox.TextChanged += ( s, e ) => ShelfPreview.Text = ShelfBox.Text;
            CellBox.TextChanged += ( s, e ) => CellPreview.Text = CellBox.Text;

        }

        public event Action<(int, int, int)> actionSklad;
        public ICommand SkladCommand { get; }

        private bool CanExecuteSklad ( object param ) //проверка есть ли данные в кнопке
        {
            return !string.IsNullOrWhiteSpace(ShelfBox?.Text) &&
                   !string.IsNullOrWhiteSpace(RowBox?.Text) &&
                   !string.IsNullOrWhiteSpace(CellBox?.Text);
        }


        public void ExecuteSklad ( object param)
        {
            Shelf = int.Parse(ShelfBox.Text);
            Cell = int.Parse(CellBox.Text);
            Row = int.Parse(RowBox.Text);

            actionSklad((Shelf, Cell, Row));
        }
    }
}
