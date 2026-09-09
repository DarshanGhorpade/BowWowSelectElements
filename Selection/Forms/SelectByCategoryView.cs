using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Selection.Revit.Models;
using Selection.Revit.ViewModels;   

namespace Selection.Revit.Forms
{
    public class SelectByCategoryView : StackPanel
    {
        private readonly Window _revitWindow;
        private TextBlock _titleText;
        private Border _border;
        private Button _addButton;
        private ItemsControl _itemsControl;
        private Button _startButton;   
        private Button _clearButton;

        public SelectByCategoryView(SelectByCategoriesViewModel viewModel, Window revitWindow)
        {
            DataContext = viewModel;
            _revitWindow = revitWindow;

            BuildUI();
            InitializeBindingsAndEvents();

            // Same trick as the reference: calculate max width after layout
            Dispatcher.BeginInvoke(new Action(SetMaxWidth), DispatcherPriority.Loaded);
        }

        private void BuildUI()
        {
            Orientation = Orientation.Horizontal;
            Height = 26;
            VerticalAlignment = VerticalAlignment.Center;
            Background = System.Windows.Media.Brushes.Transparent; 

            // 1. Title
            _titleText = new TextBlock
            {
                Text = "Select By Categories",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(12, 0, 8, 0),
                FontSize = 11
            };
            Children.Add(_titleText);

            // 2. Border / separator (optional, reference has one)
            _border = new Border
            {
                Width = 1,
                Background = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(0, 4, 8, 4)
            };
            Children.Add(_border);

            // 3. ItemsControl that hosts multiple category combo boxes
            _itemsControl = new ItemsControl
            {
                VerticalAlignment = VerticalAlignment.Center
            };
            _itemsControl.ItemsPanel = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(StackPanel)));
            var panelFactory = new FrameworkElementFactory(typeof(StackPanel));
            panelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            _itemsControl.ItemsPanel = new ItemsPanelTemplate(panelFactory);
            // Binding will be set in XAML style or code later
            Children.Add(_itemsControl);

            // 4. Add button
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
                ToolTip = "Add category"
            };

            // Force a completely flat template that respects the Height
            _addButton.Template = CreateFlatButtonTemplate();
            Children.Add(_addButton);

            // 5. Start / Force button
            _startButton = new Button
            {
                Content = "Start Selection",
                Width = 110,
                Height = 18,
                Margin = new Thickness(0, 0, 6, 0),
                FontSize = 11
            };
            Children.Add(_startButton);

            // 6. Clear button
            _clearButton = new Button
            {
                Content = "Clear All",
                Width = 80,
                Height = 18,
                FontSize = 11
            };
            Children.Add(_clearButton);
        }

        private void InitializeBindingsAndEvents()
        {
            // 1. Create the DataTemplate that contains the ComboBox
            _itemsControl.ItemTemplate = CreateCategoryItemTemplate();

            // 2. Bind the ItemsControl to the ViewModel collection
            _itemsControl.SetBinding(ItemsControl.ItemsSourceProperty,
                new System.Windows.Data.Binding(nameof(SelectByCategoriesViewModel.CatItems)));

            // 3. Button handlers
            _addButton.Click += (s, e) =>
            {
                if (DataContext is SelectByCategoriesViewModel vm)
                    vm.AddItem();
            };

            _startButton.Click += (s, e) =>
            {
                if (DataContext is SelectByCategoriesViewModel vm)
                    vm.PostCommand?.Execute(null);
            };

            _clearButton.Click += (s, e) =>
            {
                if (DataContext is SelectByCategoriesViewModel vm)
                    vm.ClearCommand?.Execute(null);
            };

            // 4. Hide/show Add button according to limit
            _itemsControl.SizeChanged += (s, e) =>
            {
                if (DataContext is SelectByCategoriesViewModel vm)
                {
                    _addButton.Visibility = vm.CanAddComboBox
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            };
        }

        private DataTemplate CreateCategoryItemTemplate()
        {
            // The ComboBox that will appear for every CatItem
            var comboFactory = new FrameworkElementFactory(typeof(ComboBox));

            comboFactory.SetValue(ComboBox.HeightProperty, 18.0);
            comboFactory.SetValue(ComboBox.MarginProperty, new Thickness(0, 0, 6, 0));
            comboFactory.SetValue(ComboBox.FontSizeProperty, 11.0);
            comboFactory.SetValue(ComboBox.VerticalContentAlignmentProperty, VerticalAlignment.Center);
            comboFactory.SetValue(ComboBox.DisplayMemberPathProperty, nameof(SelectedCategory.Name));

            // Width <- ViewModel.ComboBoxWidth
            comboFactory.SetBinding(ComboBox.WidthProperty,
                new System.Windows.Data.Binding("DataContext.ComboBoxWidth")
                {
                    RelativeSource = new RelativeSource(
                        RelativeSourceMode.FindAncestor,
                        typeof(SelectByCategoryView), 1)
                });

            // ItemsSource <- ViewModel.AvailableCategories
            comboFactory.SetBinding(ComboBox.ItemsSourceProperty,
                new System.Windows.Data.Binding("DataContext.AvailableCategories")
                {
                    RelativeSource = new RelativeSource(
                        RelativeSourceMode.FindAncestor,
                        typeof(SelectByCategoryView), 1)
                });

            // SelectedItem <- CatItem.SelectedItem (TwoWay)
            comboFactory.SetBinding(ComboBox.SelectedItemProperty,
                new System.Windows.Data.Binding(nameof(CatItem.SelectedItem))
                {
                    Mode = System.Windows.Data.BindingMode.TwoWay
                });

            return new DataTemplate { VisualTree = comboFactory };
        }

        private void SetMaxWidth()
        {
            if (!(DataContext is SelectByCategoriesViewModel vm)) return;

            // Same calculation idea as the reference
            double fixedWidth =
                _titleText.ActualWidth +
                _border.ActualWidth +
                _addButton.ActualWidth +
                _startButton.ActualWidth +
                _clearButton.ActualWidth +
                40; // margins

            vm.MinWidth = fixedWidth;
            vm.WindowWidth = _revitWindow.ActualWidth;
        }

        private ControlTemplate CreateFlatButtonTemplate()
        {
            // Border that fills the exact size of the button
            var border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
            border.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(Button.BorderBrushProperty));
            border.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(Button.BorderThicknessProperty));
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(2));
            border.SetValue(Border.SnapsToDevicePixelsProperty, true);

            // ContentPresenter centered and with zero margin
            var content = new FrameworkElementFactory(typeof(ContentPresenter));
            content.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            content.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            content.SetValue(ContentPresenter.MarginProperty, new Thickness(0));
            content.SetValue(ContentPresenter.RecognizesAccessKeyProperty, true);

            border.AppendChild(content);

            var template = new ControlTemplate(typeof(Button));
            template.VisualTree = border;
            return template;
        }
    }
}