using Selection.Revit.Models;
using Selection.Revit.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Selection.Revit.ViewModels
{
    public class SelectByFilterViewModel : INotifyPropertyChanged
    {
        private double _allowedWidth = 1000;
        private double _windowWidth = 1000;
        private double _minWidth = 300;
        private double _comboBoxWidth = 180;
        private bool _canAddComboBox = true;
        private bool _isFirstLoad = true;
        private int _acceptableCount = 4;

        public ObservableCollection<FilterItem> FilterItems { get; set; }
            = new ObservableCollection<FilterItem>();

        public ObservableCollection<FilterInfo> AvailableFilters { get; set; }
            = new ObservableCollection<FilterInfo>();

        public ICommand AddCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand PostCommand { get; set; }

        public bool CanAddComboBox
        {
            get => _canAddComboBox;
            set
            {
                if (_canAddComboBox == value) return;
                _canAddComboBox = value;
                OnPropertyChanged();
            }
        }

        public double ComboBoxWidth
        {
            get => _comboBoxWidth;
            set
            {
                if (Math.Abs(_comboBoxWidth - value) < 0.1) return;
                _comboBoxWidth = value;
                OnPropertyChanged();
            }
        }

        public double WindowWidth
        {
            get => _windowWidth;
            set
            {
                _windowWidth = value;
                _allowedWidth = value - _minWidth;
                CalcAcceptableCount();
                CalcWidth();
                OnPropertyChanged();
            }
        }

        public double MinWidth
        {
            set => _minWidth = value;
        }

        public int AcceptableCount => _acceptableCount;

        public SelectByFilterViewModel()
        {
            if (FilterItems.Count == 0)
                FilterItems.Add(new FilterItem());

            AddCommand = new RelayCommand(_ => AddItem(), _ => CanAddComboBox);
            ClearCommand = new RelayCommand(_ => ClearItems());
            PostCommand = new RelayCommand(_ => { });

            CheckAcceptableCount();
        }

        public void AddItem()
        {
            FilterItems.Add(new FilterItem());
            CheckAcceptableCount();
            CalcWidth();
        }

        public void ClearItems()
        {
            FilterItems.Clear();
            FilterItems.Add(new FilterItem());
            CheckAcceptableCount();
            CalcWidth();
        }

        public void CheckAcceptableCount()
        {
            CanAddComboBox = FilterItems.Count < _acceptableCount;
        }

        private void CalcWidth()
        {
            if (FilterItems.Count == 0) return;
            ComboBoxWidth = Math.Min(200, Math.Max(120, _allowedWidth / FilterItems.Count - 12));
        }

        private void CalcAcceptableCount()
        {
            if (!_isFirstLoad) return;

            _acceptableCount = (int)Math.Round(_allowedWidth / 190.0);
            if (_acceptableCount < 1) _acceptableCount = 1;
            if (_acceptableCount > 8) _acceptableCount = 8;

            _isFirstLoad = false;
            CheckAcceptableCount();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}