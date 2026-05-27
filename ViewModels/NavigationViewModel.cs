using NewAPP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NewAPP2.ViewModels
{
    public class NavigationViewModel : INotifyPropertyChanged
    {
        //свойства видимости
        private bool _isWelcomeVisible = true;
        public bool IsWelcomeVisible
        {
            get => _isWelcomeVisible;
            set { _isWelcomeVisible = value; OnPropertyChanged(); }
        }

        private bool _isSensorsVisible;
        public bool IsSensorsVisible
        {
            get => _isSensorsVisible;
            set { _isSensorsVisible = value; OnPropertyChanged(); }
        }

        private bool _isSettingsVisible;
        public bool IsSettingsVisible
        {
            get => _isSettingsVisible;
            set { _isSettingsVisible = value; OnPropertyChanged(); }
        }

        private bool _isTcpSettingsVisible;
        public bool IsTcpSettingsVisible
        {
            get => _isTcpSettingsVisible;
            set { _isTcpSettingsVisible = value; OnPropertyChanged(); }
        }

        private bool _isTopControlsVisible;
        public bool IsTopControlsVisible
        {
            get => _isTopControlsVisible;
            set { _isTopControlsVisible = value; OnPropertyChanged(); }
        }

        private bool _isRightCellsVisible; // правые кнопки 
        public bool IsRightCellsVisible
        {
            get => _isRightCellsVisible;
            set { _isRightCellsVisible = value; OnPropertyChanged(); }
        }
        private bool _isAddUsersVisible; // надо сделать отдельную видимость для кнопки, при этом должна исчезать видимость области за которую отвечает кнопка
        public bool IsAddUsersVisible
        {
            get => _isAddUsersVisible;
            set { _isAddUsersVisible = value; OnPropertyChanged(); }
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
            get => _isNomenclatureVisible;
            set { _isNomenclatureVisible = value; OnPropertyChanged(); }
        }

        private bool _isDataBaseButtonVisible; //свойство видимости кнопки номенклатуры
        public bool IsDataBaseButtonVisible
        {
            get => _isDataBaseButtonVisible;
            set { _isDataBaseButtonVisible = value; OnPropertyChanged(); }
        }

        private bool _isAdmin;
        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                if (_isAdmin == value) return;
                _isAdmin = value;
                OnPropertyChanged();

                // Автоматически обновляем видимость кнопок при изменении роли
                VisibleAdminButton();
                VisibleAdmin();
            }
        }

        private bool _isHotkeysVisible; //горячие клавиши
        public bool IsHotkeysVisible
        {
            get => _isHotkeysVisible;
            set
            {
                _isHotkeysVisible = value;
                OnPropertyChanged();
            }
        }


        
        //команды для видимости
        public ICommand ShowSensorsCommand { get; }
        public ICommand ShowSettingsCommand { get; }
        public ICommand ShowNetworkCommand { get; }
        public ICommand ShowNomenclatureCommand { get; }
        public ICommand AddUserCommand { get; }
        public ICommand ApplyHotkeysCommand { get; }
        public ICommand ShowBindCommand { get; }

        public NavigationViewModel ( )
        {
            ShowSensorsCommand = new RelayCommand(ExecuteShowSensors);
            ShowSettingsCommand = new RelayCommand(ExecuteShowSettings);
            ShowNetworkCommand = new RelayCommand(ExecuteShowNetwork);
            ShowNomenclatureCommand = new RelayCommand(ExecuteShowNomenclature);
            AddUserCommand = new RelayCommand(ExecuteAddUsers);
            ShowBindCommand = new RelayCommand(ExecuteShowHotkeys);
            //IsAddButtonUsersVisible = true;
        }

        //методы видимости
        private void ExecuteShowSensors ( object param )
        {
            
            // Сбрасываем всё
            IsWelcomeVisible = false;
            IsSettingsVisible = false;
            IsTcpSettingsVisible = false;
            IsNomenclatureVisible = false;
            IsAddUsersVisible = false;
            IsHotkeysVisible = false;
            IsRightCellsVisible = true;

            // Включаем нужное
            IsSensorsVisible = true;
            IsTopControlsVisible = true;
        }

        private void ExecuteShowSettings ( object param )
        {
            
            IsSettingsVisible = true;
            IsWelcomeVisible = false;
            IsSensorsVisible = false;
            IsTcpSettingsVisible = false;
            IsNomenclatureVisible = false;
            IsAddUsersVisible = false;
            IsHotkeysVisible = false;

            
            
        }

        private void ExecuteShowNetwork ( object param )
        {
            IsWelcomeVisible = false;
            IsSensorsVisible = false;
            IsSettingsVisible = false;
            IsNomenclatureVisible = false;
            IsAddUsersVisible = false;
            IsHotkeysVisible = false;

            IsTcpSettingsVisible = true;
        }

        private void ExecuteShowNomenclature ( object param )
        {
            IsWelcomeVisible = false;
            IsSensorsVisible = false;
            IsSettingsVisible = false;
            IsTcpSettingsVisible = false;
            IsAddUsersVisible = false;
            IsHotkeysVisible = false;

            IsNomenclatureVisible = true;
        }

        private void ExecuteAddUsers ( object param )
        {
            IsAddUsersVisible = true;
            IsWelcomeVisible = false;
            IsSensorsVisible = false;
            IsSettingsVisible = false;
            IsTcpSettingsVisible = false;
            IsNomenclatureVisible = false;
            IsHotkeysVisible = false;

            
        }

        private void ExecuteShowHotkeys ( object param )
        {
            
            IsWelcomeVisible = false;
            IsSensorsVisible = false;
            IsSettingsVisible = false;
            IsTcpSettingsVisible = false;
            IsNomenclatureVisible = false;
            IsAddUsersVisible = false;
            IsHotkeysVisible = true;


        }
        private void VisibleAdmin ( )
        {
            IsAddUsersVisible = IsAdmin;

        }
        private void VisibleAdminButton ( ) // метод для видимости кнопки добавления и кнопки склада
        {
            IsAddButtonUsersVisible = IsAdmin;
            IsVisibleSklad = IsAdmin;
            IsDataBaseButtonVisible = IsAdmin;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged ( [CallerMemberName] string name = null )
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

