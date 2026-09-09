using Selection.Revit.Models;
using Selection.Revit.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Windows;

namespace Selection.Revit.Forms
{
    public class SelectByFilterView : StackPanel
    {
        private readonly Window _revitWindow;

        private TextBlock _titleText;
        private Border _border;
        private Button _addButton;
        private ItemsControl _itemsControl;
        private Button _startButton;
        private Button _clearButton;

        public SelectByFilterView(SelectByFilterViewModel viewModel, Window revitWindow)
        {
            DataContext = viewModel;
            _revitWindow = revitWindow;

            BuildUI();
            InitializeBindingsAndEvents();

            Dispatcher.BeginInvoke(new Action(SetMaxWidth), DispatcherPriority.Loaded);
        }

        private void BuildUI()
        {
            Orientation = Orientation.Horizontal;
            Height = 26;
            MaxHeight = 26;
            MinHeight = 26;
            VerticalAlignment = VerticalAlignment.Center;
            Background = System.Windows.Media.Brushes.Transparent;

            _titleText = new TextBlock
            {
                Text = "Select By Filter",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(12, 0, 8, 0),
                FontSize = 11
            };
            Children.Add(_titleText);

            _border = new Border
            {
                Width = 1,
                Background = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(0, 4, 8, 4)
            };
            Children.Add(_border);

            _itemsControl = new ItemsControl
            {
                VerticalAlignment = VerticalAlignment.Center
            };

            // Horizontal layout
            var panelFactory = new FrameworkElementFactory(typeof(StackPanel));
            panelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            _itemsControl.ItemsPanel = new ItemsPanelTemplate(panelFactory);

            Children.Add(_itemsControl);

            _addButton = new Button
            {
                Content = "+",
                Width = 24,
                Height = 18,
                Margin = new Thickness(4, 0, 6, 0),
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Padding = new Thickness(0),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                ToolTip = "Add filter"
            };
            _addButton.Template = CreateFlatButtonTemplate();
            Children.Add(_addButton);

            _startButton = new Button
            {
                Content = "Start Selection",
                Width = 110,
                Height = 18,
                Margin = new Thickness(0, 0, 6, 0),
                FontSize = 11,
                Padding = new Thickness(0),
                VerticalAlignment = VerticalAlignment.Center
            };
            _startButton.Template = CreateFlatButtonTemplate();
            Children.Add(_startButton);

            _clearButton = new Button
            {
                Content = "Clear All",
                Width = 80,
                Height = 18,
                FontSize = 11,
                Padding = new Thickness(0),
                VerticalAlignment = VerticalAlignment.Center
            };
            _clearButton.Template = CreateFlatButtonTemplate();
            Children.Add(_clearButton);
        }

        private void InitializeBindingsAndEvents()
        {
            _itemsControl.ItemTemplate = CreateFilterItemTemplate();

            _itemsControl.SetBinding(ItemsControl.ItemsSourceProperty,
                new Binding(nameof(SelectByFilterViewModel.FilterItems)));

            _addButton.Click += (s, e) =>
            {
                if (DataContext is SelectByFilterViewModel vm)
                    vm.AddItem();
            };

            _startButton.Click += (s, e) =>
            {
                if (DataContext is SelectByFilterViewModel vm)
                    vm.PostCommand?.Execute(null);
            };

            _clearButton.Click += (s, e) =>
            {
                if (DataContext is SelectByFilterViewModel vm)
                    vm.ClearCommand?.Execute(null);
            };

            _itemsControl.SizeChanged += (s, e) =>
            {
                if (DataContext is SelectByFilterViewModel vm)
                {
                    _addButton.Visibility = vm.CanAddComboBox
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            };
        }

        private DataTemplate CreateFilterItemTemplate()
        {
            var comboFactory = new FrameworkElementFactory(typeof(ComboBox));

            comboFactory.SetValue(ComboBox.HeightProperty, 18.0);
            comboFactory.SetValue(ComboBox.MarginProperty, new Thickness(0, 0, 6, 0));
            comboFactory.SetValue(ComboBox.FontSizeProperty, 11.0);
            comboFactory.SetValue(ComboBox.VerticalContentAlignmentProperty, VerticalAlignment.Center);
            comboFactory.SetValue(ComboBox.VerticalAlignmentProperty,VerticalAlignment.Center);
            comboFactory.SetValue(ComboBox.PaddingProperty,new Thickness(4, 0, 4, 0));
            comboFactory.SetValue(ComboBox.DisplayMemberPathProperty, nameof(FilterInfo.Name));

            comboFactory.SetBinding(ComboBox.WidthProperty,
                new Binding("DataContext.ComboBoxWidth")
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(SelectByFilterView), 1)
                });

            comboFactory.SetBinding(ComboBox.ItemsSourceProperty,
                new Binding("DataContext.AvailableFilters")
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(SelectByFilterView), 1)
                });

            comboFactory.SetBinding(ComboBox.SelectedItemProperty,
                new Binding(nameof(FilterItem.SelectedItem))
                {
                    Mode = BindingMode.TwoWay
                });

            return new DataTemplate { VisualTree = comboFactory };
        }

        private ControlTemplate CreateFlatButtonTemplate()
        {
            var border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
            border.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(Button.BorderBrushProperty));
            border.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(Button.BorderThicknessProperty));
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(2));
            border.SetValue(Border.SnapsToDevicePixelsProperty, true);

            var content = new FrameworkElementFactory(typeof(ContentPresenter));
            content.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            content.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            content.SetValue(ContentPresenter.MarginProperty, new Thickness(0));

            border.AppendChild(content);

            return new ControlTemplate(typeof(Button)) { VisualTree = border };
        }

        private void SetMaxWidth()
        {
            if (!(DataContext is SelectByFilterViewModel vm)) return;

            double fixedWidth =
                _titleText.ActualWidth +
                _border.ActualWidth +
                _addButton.ActualWidth +
                _startButton.ActualWidth +
                _clearButton.ActualWidth +
                40;

            vm.MinWidth = fixedWidth;
            vm.WindowWidth = _revitWindow?.ActualWidth ?? 1200;
        }
    }
}
