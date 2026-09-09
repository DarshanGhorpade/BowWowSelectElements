using System.ComponentModel;
using System.Runtime.CompilerServices;
using Selection.Revit.Models;   // SelectedCategory

namespace Selection.Revit.ViewModels
{
    public class CatItem : INotifyPropertyChanged
    {
        private SelectedCategory _selectedItem;

        public SelectedCategory SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value) return;
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        // Optional: you can later add a command to remove/disable this item
        // public ICommand DisableCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}