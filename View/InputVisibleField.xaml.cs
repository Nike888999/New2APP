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

namespace NewAPP.View
{
    /// <summary>
    /// Логика взаимодействия для InputVisibleField.xaml
    /// </summary>
    public partial class InputVisibleField : Window
    {
        //public bool ShowId { get; private set; }
        public bool ShowName { get; private set; }
        public bool ShowInNumber { get; private set; }
        public bool ShowOutNumber { get; private set; }
        public bool ShowCharacteristick { get; private set; }
        public bool ShowSerialNumber { get; private set; }
        public bool ShowPrice { get; private set; }
        public bool ShowWeight { get; private set; }
        public bool ShowUnit { get; private set; }
        public bool ShowAdr { get; private set; }
        public bool ShowOldQuntity { get; private set; }
        public bool ShowOperationIn { get; private set; }
        public bool ShowOperationOut { get; private set; }
        public bool ShowQuantity { get; private set; }

        public event Action<bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool> visibleAction;

        public InputVisibleField ()
        {
            InitializeComponent();
        }

        public void OkButton_Click ( object sender, RoutedEventArgs e )
        {
            ShowName = chkName?.IsChecked == true;
            ShowInNumber = chkInNumber?.IsChecked == true;
            ShowOutNumber = chkOutNumber?.IsChecked == true;
            ShowCharacteristick = chkHaracteristick?.IsChecked == true;
            ShowSerialNumber = chkSerialNumber?.IsChecked == true;
            ShowPrice = chkPrice?.IsChecked == true;
            ShowWeight = chkWeight?.IsChecked == true;
            ShowUnit = chkUnit?.IsChecked == true;
            ShowAdr = chkAdr?.IsChecked == true;
            ShowOldQuntity = chkOldQuantity?.IsChecked == true;
            ShowOperationIn = chkOperationIn?.IsChecked == true;
            ShowOperationOut = chkOperationOut?.IsChecked == true;
            ShowQuantity = chkQuntity?.IsChecked == true;

            // ВЫЗЫВАЕМ событие с 13 параметрами (НЕ кортеж)
            visibleAction?.Invoke(
                ShowName,
                ShowInNumber,
                ShowOutNumber,
                ShowCharacteristick,
                ShowSerialNumber,
                ShowPrice,
                ShowWeight,
                ShowUnit,
                ShowAdr,
                ShowOldQuntity,
                ShowOperationIn,
                ShowOperationOut,
                ShowQuantity
            );

            this.DialogResult = true;
            this.Close();
        }

        private void UpdatePropertiesFromCheckboxes ( )
        {
            // Добавим проверки на null (на всякий случай)
            
            ShowName = chkName?.IsChecked ?? true;
            ShowInNumber = chkInNumber?.IsChecked ?? true;
            ShowOutNumber = chkOutNumber?.IsChecked ?? true;
            ShowCharacteristick = chkHaracteristick?.IsChecked ?? true;
            ShowSerialNumber = chkSerialNumber?.IsChecked ?? true;
            ShowPrice = chkPrice?.IsChecked ?? true;
            ShowWeight = chkUnit?.IsChecked ?? true;
            ShowUnit = chkUnit?.IsChecked ?? true;
            ShowAdr = chkAdr?.IsChecked ?? true;
            ShowOldQuntity = chkOldQuantity?.IsChecked ?? true;
            ShowOperationIn = chkOperationIn?.IsChecked ?? true;
            ShowOperationOut = chkOperationOut?.IsChecked ?? true;
            ShowQuantity = chkQuntity?.IsChecked ?? true;

        }

        private void CheckBox_Changed ( object sender, RoutedEventArgs e )
        {
            // Можно оставить пустым, если не нужна дополнительная логика
            // Или добавить логику, например:
            // var checkBox = sender as CheckBox;
            // if (checkBox.IsChecked == true)
            // {
            //     // Что-то делаем при установке галочки
            // }
        }
    }
}
