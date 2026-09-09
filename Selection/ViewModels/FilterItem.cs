using Selection.Revit.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Selection.Revit.ViewModels
{
    public class FilterItem : INotifyPropertyChanged
    {
        private FilterInfo _selectedItem;

        public FilterInfo SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value) return;
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
