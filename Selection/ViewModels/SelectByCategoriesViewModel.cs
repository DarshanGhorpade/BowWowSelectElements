using Selection.Revit.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Selection.Revit.ViewModels
{
    public class SelectByCategoriesViewModel : INotifyPropertyChanged
    {
        private double _allowedWidth = 1000;
        private double _windowWidth = 1000;
        private double _minWidth = 300;
        private double _comboBoxWidth = 180;
        private bool _canAddComboBox = true;
        private bool _isFirstLoad = true;
        private static int _acceptableCount = 6;   // default max combos

        public ObservableCollection<CatItem> CatItems { get; set; } = new ObservableCollection<CatItem>();

        // The list of all available categories (filled once by the command)
        public ObservableCollection<SelectedCategory> AvailableCategories { get; set; }
            = new ObservableCollection<SelectedCategory>();

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

        public SelectByCategoriesViewModel()
        {
            // Start with one empty row
            if (CatItems.Count == 0)
                CatItems.Add(new CatItem());

            AddCommand = new RelayCommand(_ => AddItem(), _ => CanAddComboBox);
            ClearCommand = new RelayCommand(_ => ClearItems());
            PostCommand = new RelayCommand(_ => { /* will be set by the Cmd layer */ });

            CheckAcceptableCount();
        }

        public void AddItem()
        {
            CatItems.Add(new CatItem());
            CheckAcceptableCount();
            CalcWidth();
        }

        public void ClearItems()
        {
            CatItems.Clear();
            CatItems.Add(new CatItem());  
            CheckAcceptableCount();
            
            CalcWidth();
        }

        public void CheckAcceptableCount()
        {
            CanAddComboBox = CatItems.Count < _acceptableCount;
        }

        private void CalcWidth()
        {
            if (CatItems.Count == 0) return;

            ComboBoxWidth = Math.Min(200,
                Math.Max(120, _allowedWidth / CatItems.Count - 12));
        }

        private void CalcAcceptableCount()
        {
            if (!_isFirstLoad) return;

            _acceptableCount = (int)Math.Round(_allowedWidth / 190.0);
            if (_acceptableCount < 1) _acceptableCount = 1;
            if (_acceptableCount > 10) _acceptableCount = 10;

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