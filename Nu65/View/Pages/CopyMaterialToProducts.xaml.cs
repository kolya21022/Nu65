using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using NU65.Db;
using NU65.Entities.External;
using NU65.Services;
using NU65.Storages.Mssql;
using NU65.Util;
using NU65.View.Util;

namespace NU65.View.Pages
{
    public partial class CopyMaterialToProducts : IPageable
    {
        // Создаем Delegate который соответствует
        // методу ProgressBar.SetValue
        private delegate void UpdateProgressBarDelegate(DependencyProperty dp, Object value);

        /// <summary>
        /// Локальное хранилище списка для поиска [Из изделия].
        /// (загружается при создании страницы и служит неизменяемым источником данных при фильтрации)
        /// </summary>
        private List<Product> _searchProduct1Storage = new List<Product>();

        /// <summary>
        /// Локальное хранилище списка для поиска [В изделие].
        /// (загружается при создании страницы и служит неизменяемым источником данных при фильтрации)
        /// </summary>
        private List<Product> _searchProduct2Storage = new List<Product>();

        /// <summary>
        /// Локальное хранилище списка выбранных [Продуктов].
        /// </summary>
        private List<Product> _selectedProductList = new List<Product>();

        /// <summary>
        /// Локальное хранилище списка для поиска остатков [Материала].
        /// (загружается при создании страницы и служит неизменяемым источником данных при фильтрации)
        /// </summary>
        private List<Material> _searchMaterialStorage = new List<Material>();

        /// <summary>
        /// Выбранное изделие [Из изделия]
        /// </summary>
        private Product _selectedProduct1;

        /// <summary>
        /// Выбранное изделие [В изделия]
        /// </summary>
        private Product _selectedProduct2;

        /// <summary>
        /// Выбранный материал
        /// </summary>
        private Material _selectedMaterial;


        public CopyMaterialToProducts()
        {
            InitializeComponent();
            AdditionalInitializeComponent();
            VisualInitializeComponent();
            PageUtil.SelectSpecifiedOrFirstDataGridRow<Product>(ProductListDataGrid, null);
        }

        /// <summary>
        /// Загрузка списка объектов из базы данных, их отображение в таблице, указание их кол-ва в Label
        /// </summary>
        /// <inheritdoc />
        public void AdditionalInitializeComponent()
        {
            TitleLabel.Content = "Скопировать материал в изделия";
            MaterialWrapperGrid.IsEnabled = false;
            try
            {
                _searchProduct1Storage = ProductsService.GetAllMssql();
                _searchProduct1Storage.Sort();
                SearchProduct1DataGrid.ItemsSource = _searchProduct1Storage;
                _searchProduct2Storage = ProductsService.GetAllMssql();
                _searchProduct2Storage.Sort();
                SearchProduct2DataGrid.ItemsSource = _searchProduct2Storage;
                _searchMaterialStorage = null;
                _selectedProductList = new List<Product>();
                ProductListDataGrid.ItemsSource = _selectedProductList;
                ShowCountItemsPageDataGrid();
            }
            catch (StorageException ex)
            {
                Common.ShowDetailExceptionMessage(ex);
            }
        }

        /// <summary>
        /// Отображение числа записей в Label над таблицей
        /// </summary>
        private void ShowCountItemsPageDataGrid()
        {
            const string countItemsPattern = PageLiterals.PatternCountItemsTable;
            var message = string.Format(countItemsPattern, ProductListDataGrid.Items.Count);
            CountItemsLabel.Content = message;
        }

        /// <summary>
        /// Визуальная инициализация страницы (цвета и размеры шрифтов контролов)
        /// </summary>
        public void VisualInitializeComponent()
        {
            FontSize = Constants.FontSize;
            WrappedGrid.Background = Constants.LineBorderColor2_Nepal;
            ButtonWrappedGrid.Background = Constants.BackColor5_WaikawaGray;

            // Заголовок страницы
            TitlePageGrid.Background = Constants.BackColor4_BlueBayoux;
            var titleLabels = TitlePageGrid.Children.OfType<Label>();
            foreach (var titleLabel in titleLabels)
            {
                titleLabel.Foreground = Constants.ForeColor2_PapayaWhip;
            }
        }

        /// <summary>
        /// Горячие клавиши текущей страницы
        /// </summary>
        /// <inheritdoc />
        public string PageHotkeys()
        {
            //todo
            const string closeApp = PageLiterals.HotkeyLabelCloseApp;
            const string displayed = closeApp;
            return displayed;
        }

        /// <summary>
        /// Обработка нажатия клавиш в фокусе всей страницы 
        /// </summary>
        /// <inheritdoc />
        public void Page_OnKeyDown(object senderIsPageOrWindow, KeyEventArgs eventArgs)
        {
            if (eventArgs.Key != Key.Escape)
            {
                return;
            }
            eventArgs.Handled = true;
            PageUtil.ConfirmCloseApplication(); // Если нажат [Esc] - запрос подтверждения выхода у пользователя
        }

        private void Reload()
        {
            _selectedProduct2 = null;
            Product2TextBox.Text = string.Empty;
            Product2CodeLabel.Content = string.Empty;
            Product2NameLabel.Content = string.Empty;
            Product2MarkLabel.Content = string.Empty;
        }

        private void SearchFieldWrapperGrid_OnLostFocus(object senderIsGrid, RoutedEventArgs e)
        {
            PageUtil.SearchFieldWrapperGrid_OnLostFocusHideTable(senderIsGrid);
        }

        private void SearchFieldWrapperGrid_OnGotFocus(object senderIsGrid, RoutedEventArgs e)
        {
            PageUtil.SearchFieldWrapperGrid_OnGotFocusShowTable(senderIsGrid);
        }


        private void TextBox_OnPreviewKeyUp(object senderIsTextBox, KeyEventArgs eventArgs)
        {
            // TextBox поиска
            var searchTextBox = senderIsTextBox as TextBox;
            if (searchTextBox == null)
            {
                return;
            }

            // Grid-обёртка DataGrid и TextBox поиска
            var searchWrapperGrid = VisualTreeHelper.GetParent(searchTextBox) as Grid;
            if (searchWrapperGrid == null)
            {
                return;
            }

            // DataGrid поиска сущности
            var searchDataGrid = searchWrapperGrid.Children.OfType<DataGrid>().FirstOrDefault();
            if (searchDataGrid == null)
            {
                return;
            }

            // Если нажата кнопка [Down] - перемещение клавиатурного фокуса на первую строку DataGrid поиска сущности
            if (eventArgs.Key == Key.Down)
            {
                if (searchDataGrid.Items.Count <= 0)
                {
                    return;
                }

                searchDataGrid.SelectedIndex = 0;

                // NOTE: эта копипаста ниже не случайна, нужный функционал срабатывает только со второго раза.
                // Решение в указаном ответе: https://stackoverflow.com/a/27792628 Работает, не трогай
                var row = (DataGridRow)searchDataGrid.ItemContainerGenerator.ContainerFromIndex(0);
                if (row == null)
                {
                    searchDataGrid.UpdateLayout();
                    searchDataGrid.ScrollIntoView(searchDataGrid.Items[0]);
                    row = (DataGridRow)searchDataGrid.ItemContainerGenerator.ContainerFromIndex(0);
                }

                if (row != null)
                {
                    row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
            }
        }

        /// <summary>
        /// Обработка события изменения текста в TextBox поиска [Изделия].
        /// (Перезаполнение DataGrid поиска сущности с учётом введённого текста)
        /// </summary>
        private void Product1TextBox_OnTextChanged(object senderIsTextBox, TextChangedEventArgs e)
        {
            // TextBox поиска
            var searchTextBox = senderIsTextBox as TextBox;
            if (searchTextBox == null)
            {
                return;
            }
            // Grid-обёртка DataGrid и TextBox поиска
            var searchWrapperGrid = VisualTreeHelper.GetParent(searchTextBox) as Grid;
            if (searchWrapperGrid == null)
            {
                return;
            }
            // DataGrid поиска сущности
            var searchDataGrid = searchWrapperGrid.Children.OfType<DataGrid>().FirstOrDefault();
            if (searchDataGrid == null)
            {
                return;
            }

            // Разделение введенного пользователем текста по пробелам на массив слов
            var searchResult = new List<Product>();
            var searchValues = searchTextBox.Text.Trim().Split(null);
            const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
            foreach (var product in _searchProduct1Storage)
            {
                // Поиск совпадений всех значений массива по требуемым полям сущности
                var isCoincided = true;
                var productId = product.CodeProduct.ToString();
                var productName = product.Name;
                foreach (var searchValue in searchValues)
                {
                    isCoincided &= productId.IndexOf(searchValue, comparisonIgnoreCase) >= 0 ||
                                   productName.IndexOf(searchValue, comparisonIgnoreCase) >= 0;
                }
                // Если в полях сущности есть введённые слова, добавляем объект в буферный список
                if (isCoincided)
                {
                    searchResult.Add(product);
                }
            }
            // Перезаполнение DataGrid поиска сущности с учётом найденых значений
            searchDataGrid.ItemsSource = null;
            searchDataGrid.ItemsSource = searchResult;
        }


        /// <summary>
        /// Обработка нажатия клавиш [Enter] и [Up] в DataGrid поиска сущностей
        /// </summary>
        private void SearchDataGrid_OnPreviewKeyUp(object senderIsDataGrid, KeyEventArgs eventArgs)
        {
            const int startOfListIndex = 0;
            // DataGrid поиска сущности
            var searchDataGrid = senderIsDataGrid as DataGrid;
            if (searchDataGrid == null)
            {
                return;
            }
            // Grid-обёртка DataGrid и TextBox поиска
            var searchWrapperGrid = VisualTreeHelper.GetParent(searchDataGrid) as Grid;
            if (searchWrapperGrid == null)
            {
                return;
            }
            // TextBox поиска/добавления
            var searchTextBox = searchWrapperGrid.Children.OfType<TextBox>().FirstOrDefault();
            if (searchTextBox == null)
            {
                return;
            }

            // Если фокус ввода на первой записи DataGrid и нажата [Up] - перевод клавиатурного фокуса ввода к TextBox
            if (startOfListIndex == searchDataGrid.SelectedIndex && eventArgs.Key == Key.Up)
            {
                searchTextBox.Focus();
            }

            // Если записей не 0 и нажат [Enter] - заносим текст объекта в TextBox и переводим фокус к след. контролу
            else if (searchDataGrid.Items.Count > 0 && eventArgs.Key == Key.Enter)
            {
                // Выбранная строка (объект) DataGrid поиска сущности
                var rawSelectedItem = searchDataGrid.SelectedItem;
                if (rawSelectedItem == null)
                {
                    return;
                }
                string displayed;
                var selectedItemType = rawSelectedItem.GetType();
                if (selectedItemType == typeof(Product)) // Если тип найденой сущности: [Изделие]
                {
                    _selectedProduct1 = (Product)rawSelectedItem;
                    Product1CodeLabel.Content = _selectedProduct1.CodeProduct.ToString(CultureInfo.CurrentCulture);
                    Product1NameLabel.Content = _selectedProduct1.Name;
                    Product1MarkLabel.Content = _selectedProduct1.Mark;
                    MaterialWrapperGrid.IsEnabled = true;
                    MaterialCodeLabel.Content = string.Empty;
                    MaterialNameLabel.Content = string.Empty;
                    MaterialMarkLabel.Content = string.Empty;
                    MaterialGostLabel.Content = string.Empty;
                    MeasureLabel.Content = string.Empty;
                    AuxiliaryMaterialConsumptionRateLabel.Content = string.Empty;
                    _searchMaterialStorage = Nu65StorageMssql.GetMaterialsByProductIdWhithMeasure(_selectedProduct1.Id);
                    _searchMaterialStorage.Sort();
                    SearchMaterialDataGrid.ItemsSource = _searchMaterialStorage;
                    displayed = _selectedProduct1.CodeProduct.ToString(CultureInfo.CurrentCulture);
                }
                else if (selectedItemType == typeof(Material)) // Если тип найденой сущности: [Материал]
                {
                    _selectedMaterial = (Material)rawSelectedItem;
                    MaterialCodeLabel.Content = _selectedMaterial.CodeMaterial.ToString(CultureInfo.CurrentCulture);
                    MaterialNameLabel.Content = _selectedMaterial.Name;
                    MaterialMarkLabel.Content = _selectedMaterial.Profile;
                    MaterialGostLabel.Content = _selectedMaterial.Gost;
                    MeasureLabel.Content = _selectedMaterial.Measure.ShortName;
                    WorkGuildLabel.Content = _selectedMaterial.WorkGuildId;
                    RegionLabel.Content = _selectedMaterial.ParcelId;
                    AuxiliaryMaterialConsumptionRateLabel.Content = _selectedMaterial.AuxiliaryMaterialConsumptionRate;
                    displayed = _selectedMaterial.CodeMaterial.ToString(CultureInfo.CurrentCulture);
                }
                else
                {
                    displayed = rawSelectedItem.ToString();
                }
                // Вывод выбраного значения в TextBox поиска/добавления
                searchTextBox.Text = displayed;

                // Перевод фокуса ввода на нижележащий визуальный элемент после [DataGrid] поиска сущности
                var request = new TraversalRequest(FocusNavigationDirection.Down)
                {
                    Wrapped = false
                };
                eventArgs.Handled = true;
                if (searchDataGrid.MoveFocus(request))
                {
                    searchDataGrid.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void SearchDataGrid_OnPreviewKeyDown(object sender, KeyEventArgs eventArgs)
        {
            if (eventArgs.Key == Key.Enter)
            {
                eventArgs.Handled = true;
            }
        }

        /// <summary>
        /// Обработка нажатия мышки на строку DataGrid поиска сущностей
        /// </summary>
        private void SearchDataGrid_OnPreviewMouseDown(object senderIsDataGrid, MouseButtonEventArgs eventArgs)
        {
            // DataGrid поиска сущности
            var searchDataGrid = senderIsDataGrid as DataGrid;
            if (searchDataGrid == null)
            {
                return;
            }
            // Grid-обёртка DataGrid и TextBox поиска
            var searchWrapperGrid = VisualTreeHelper.GetParent(searchDataGrid) as Grid;
            if (searchWrapperGrid == null)
            {
                return;
            }
            // TextBox поиска/добавления
            var searchTextBox = searchWrapperGrid.Children.OfType<TextBox>().FirstOrDefault();
            if (searchTextBox == null)
            {
                return;
            }
            // Выбранная строка (объект) DataGrid поиска сущности
            var rawSelectedItem = searchDataGrid.SelectedItem;
            if (rawSelectedItem == null)
            {
                return;
            }
            string displayed;
            var selectedItemType = rawSelectedItem.GetType();
            if (selectedItemType == typeof(Product)) // Если тип найденой сущности: [Изделие]
            {
                _selectedProduct1 = (Product)rawSelectedItem;
                Product1CodeLabel.Content = _selectedProduct1.CodeProduct.ToString(CultureInfo.CurrentCulture);
                Product1NameLabel.Content = _selectedProduct1.Name;
                Product1MarkLabel.Content = _selectedProduct1.Mark;
                MaterialWrapperGrid.IsEnabled = true;
                MaterialCodeLabel.Content = string.Empty;
                MaterialNameLabel.Content = string.Empty;
                MaterialMarkLabel.Content = string.Empty;
                MaterialGostLabel.Content = string.Empty;
                MeasureLabel.Content = string.Empty;
                AuxiliaryMaterialConsumptionRateLabel.Content = string.Empty;
                _searchMaterialStorage = Nu65StorageMssql.GetMaterialsByProductIdWhithMeasure(_selectedProduct1.Id);
                _searchMaterialStorage.Sort();
                SearchMaterialDataGrid.ItemsSource = _searchMaterialStorage;
                displayed = _selectedProduct1.CodeProduct.ToString(CultureInfo.CurrentCulture);
            }
            else if (selectedItemType == typeof(Material)) // Если тип найденой сущности: [Материал]
            {
                _selectedMaterial = (Material)rawSelectedItem;
                MaterialCodeLabel.Content = _selectedMaterial.CodeMaterial.ToString(CultureInfo.CurrentCulture);
                MaterialNameLabel.Content = _selectedMaterial.Name;
                MaterialMarkLabel.Content = _selectedMaterial.Profile;
                MaterialGostLabel.Content = _selectedMaterial.Gost;
                MeasureLabel.Content = _selectedMaterial.Measure.ShortName;
                WorkGuildLabel.Content = _selectedMaterial.WorkGuildId;
                RegionLabel.Content = _selectedMaterial.ParcelId;
                AuxiliaryMaterialConsumptionRateLabel.Content = _selectedMaterial.AuxiliaryMaterialConsumptionRate;
                displayed = _selectedMaterial.CodeMaterial.ToString(CultureInfo.CurrentCulture);
            }
            else
            {
                displayed = rawSelectedItem.ToString();
            }
            // Вывод выбраного значения в TextBox поиска/добавления
            searchTextBox.Text = displayed;

            // Перевод фокуса ввода на нижележащий визуальный элемент после [DataGrid] поиска сущности
            var request = new TraversalRequest(FocusNavigationDirection.Down)
            {
                Wrapped = false
            };
            eventArgs.Handled = true;
            if (searchDataGrid.MoveFocus(request))
            {
                searchDataGrid.Visibility = Visibility.Collapsed;
            }
        }


        /// <summary>
        /// Обработка события изменения текста в TextBox поиска [Материала].
        /// (Перезаполнение DataGrid поиска сущности с учётом введённого текста)
        /// </summary>
        private void MaterialTextBox_OnTextChanged(object senderIsTextBox, TextChangedEventArgs e)
        {
            // TextBox поиска
            var searchTextBox = senderIsTextBox as TextBox;
            if (searchTextBox == null)
            {
                return;
            }
            // Grid-обёртка DataGrid и TextBox поиска
            var searchWrapperGrid = VisualTreeHelper.GetParent(searchTextBox) as Grid;
            if (searchWrapperGrid == null)
            {
                return;
            }
            // DataGrid поиска сущности
            var searchDataGrid = searchWrapperGrid.Children.OfType<DataGrid>().FirstOrDefault();
            if (searchDataGrid == null)
            {
                return;
            }

            // Разделение введенного пользователем текста по пробелам на массив слов
            var searchResult = new List<Material>();
            var searchValues = searchTextBox.Text.Trim().Split(null);
            const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
            foreach (var material in _searchMaterialStorage)
            {
                // Поиск совпадений всех значений массива по требуемым полям сущности
                var isCoincided = true;
                var materialId = material.CodeMaterial.ToString();
                var materialName = material.Name;
                var materialMark = material.Profile;
                var materialGost = material.Gost;
                foreach (var searchValue in searchValues)
                {
                    isCoincided &= materialId.IndexOf(searchValue, comparisonIgnoreCase) >= 0 ||
                                   materialName.IndexOf(searchValue, comparisonIgnoreCase) >= 0 ||
                                   materialMark.IndexOf(searchValue, comparisonIgnoreCase) >= 0 ||
                                   materialGost.IndexOf(searchValue, comparisonIgnoreCase) >= 0;
                }
                // Если в полях сущности есть введённые слова, добавляем объект в буферный список
                if (isCoincided)
                {
                    searchResult.Add(material);
                }
            }
            // Перезаполнение DataGrid поиска сущности с учётом найденых значений
            searchDataGrid.ItemsSource = null;
            searchDataGrid.ItemsSource = searchResult;
        }

        /// <summary>
        /// Обработка события изменения текста в TextBox поиска [Изделия].
        /// (Перезаполнение DataGrid поиска сущности с учётом введённого текста)
        /// </summary>
        private void Product2TextBox_OnTextChanged(object senderIsTextBox, TextChangedEventArgs e)
        {
            // TextBox поиска
            var searchTextBox = senderIsTextBox as TextBox;
            if (searchTextBox == null)
            {
                return;
            }
            // Grid-обёртка DataGrid и TextBox поиска
            var searchWrapperGrid = VisualTreeHelper.GetParent(searchTextBox) as Grid;
            if (searchWrapperGrid == null)
            {
                return;
            }
            // DataGrid поиска сущности
            var searchDataGrid = searchWrapperGrid.Children.OfType<DataGrid>().FirstOrDefault();
            if (searchDataGrid == null)
            {
                return;
            }

            // Разделение введенного пользователем текста по пробелам на массив слов
            var searchResult = new List<Product>();
            var searchValues = searchTextBox.Text.Trim().Split(null);
            const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
            foreach (var product in _searchProduct2Storage)
            {
                // Поиск совпадений всех значений массива по требуемым полям сущности
                var isCoincided = true;
                var productId = product.CodeProduct.ToString();
                var productName = product.Name;
                foreach (var searchValue in searchValues)
                {
                    isCoincided &= productId.IndexOf(searchValue, comparisonIgnoreCase) >= 0 ||
                                   productName.IndexOf(searchValue, comparisonIgnoreCase) >= 0;
                }
                // Если в полях сущности есть введённые слова, добавляем объект в буферный список
                if (isCoincided)
                {
                    searchResult.Add(product);
                }
            }
            // Перезаполнение DataGrid поиска сущности с учётом найденых значений
            searchDataGrid.ItemsSource = null;
            searchDataGrid.ItemsSource = searchResult;
        }

        /// <summary>
        /// Обработка нажатия клавиш [Enter] и [Up] в DataGrid поиска сущностей
        /// </summary>
        private void SearchProduct2DataGrid_OnPreviewKeyUp(object senderIsDataGrid, KeyEventArgs eventArgs)
        {
            const int startOfListIndex = 0;
            // DataGrid поиска сущности
            var searchDataGrid = senderIsDataGrid as DataGrid;
            if (searchDataGrid == null)
            {
                return;
            }
            // Grid-обёртка DataGrid и TextBox поиска
            var searchWrapperGrid = VisualTreeHelper.GetParent(searchDataGrid) as Grid;
            if (searchWrapperGrid == null)
            {
                return;
            }
            // TextBox поиска/добавления
            var searchTextBox = searchWrapperGrid.Children.OfType<TextBox>().FirstOrDefault();
            if (searchTextBox == null)
            {
                return;
            }

            // Если фокус ввода на первой записи DataGrid и нажата [Up] - перевод клавиатурного фокуса ввода к TextBox
            if (startOfListIndex == searchDataGrid.SelectedIndex && eventArgs.Key == Key.Up)
            {
                searchTextBox.Focus();
            }

            // Если записей не 0 и нажат [Enter] - заносим текст объекта в TextBox и переводим фокус к след. контролу
            else if (searchDataGrid.Items.Count > 0 && eventArgs.Key == Key.Enter)
            {
                // Выбранная строка (объект) DataGrid поиска сущности
                var rawSelectedItem = searchDataGrid.SelectedItem;
                if (rawSelectedItem == null)
                {
                    return;
                }
                string displayed;
                var selectedItemType = rawSelectedItem.GetType();
                if (selectedItemType == typeof(Product)) // Если тип найденой сущности: [Изделие]
                {
                    _selectedProduct2 = (Product)rawSelectedItem;
                    Product2CodeLabel.Content = _selectedProduct2.CodeProduct.ToString(CultureInfo.CurrentCulture);
                    Product2NameLabel.Content = _selectedProduct2.Name;
                    Product2MarkLabel.Content = _selectedProduct2.Mark;
                    AddProductInListButton.IsEnabled = true;
                    displayed = _selectedProduct2.CodeProduct.ToString(CultureInfo.CurrentCulture);
                }
                else
                {
                    displayed = rawSelectedItem.ToString();
                }
                // Вывод выбраного значения в TextBox поиска/добавления
                searchTextBox.Text = displayed;

                // Перевод фокуса ввода на нижележащий визуальный элемент после [DataGrid] поиска сущности
                var request = new TraversalRequest(FocusNavigationDirection.Down)
                {
                    Wrapped = false
                };
                eventArgs.Handled = true;
                if (searchDataGrid.MoveFocus(request))
                {
                    searchDataGrid.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void SearchProduct2DataGrid_OnPreviewMouseDown(object senderIsDataGrid, MouseButtonEventArgs eventArgs)
        {
            // DataGrid поиска сущности
            var searchDataGrid = senderIsDataGrid as DataGrid;
            if (searchDataGrid == null)
            {
                return;
            }
            // Grid-обёртка DataGrid и TextBox поиска
            var searchWrapperGrid = VisualTreeHelper.GetParent(searchDataGrid) as Grid;
            if (searchWrapperGrid == null)
            {
                return;
            }
            // TextBox поиска/добавления
            var searchTextBox = searchWrapperGrid.Children.OfType<TextBox>().FirstOrDefault();
            if (searchTextBox == null)
            {
                return;
            }
            // Выбранная строка (объект) DataGrid поиска сущности
            var rawSelectedItem = searchDataGrid.SelectedItem;
            if (rawSelectedItem == null)
            {
                return;
            }
            string displayed;
            var selectedItemType = rawSelectedItem.GetType();
            if (selectedItemType == typeof(Product)) // Если тип найденой сущности: [Изделие]
            {
                _selectedProduct2 = (Product)rawSelectedItem;
                Product2CodeLabel.Content = _selectedProduct2.CodeProduct.ToString(CultureInfo.CurrentCulture);
                Product2NameLabel.Content = _selectedProduct2.Name;
                Product2MarkLabel.Content = _selectedProduct2.Mark;
                AddProductInListButton.IsEnabled = true;
                displayed = _selectedProduct2.CodeProduct.ToString(CultureInfo.CurrentCulture);
            }
            else
            {
                displayed = rawSelectedItem.ToString();
            }
            // Вывод выбраного значения в TextBox поиска/добавления
            searchTextBox.Text = displayed;

            // Перевод фокуса ввода на нижележащий визуальный элемент после [DataGrid] поиска сущности
            var request = new TraversalRequest(FocusNavigationDirection.Down)
            {
                Wrapped = false
            };
            eventArgs.Handled = true;
            if (searchDataGrid.MoveFocus(request))
            {
                searchDataGrid.Visibility = Visibility.Collapsed;
            }
        }
        /// <summary>
        /// Занесение в список выбранного изделия
        /// </summary>
        private void AddProductButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Получение значений и подписей полей
            var id = _selectedProduct2.CodeProduct;
            var product1Id = _selectedProduct1.CodeProduct;
            var name = _selectedProduct2.Name.Trim();
            var mark = _selectedProduct2.Mark.Trim();

            //Проверка на совпадение выбранных изделий "из изделия" с "в изделия"
            if (id == product1Id)
            {
                const string message = "Нельзя добавить изделие из которого копируется материал!";
                const string header = "Ошибка валидации";
                MessageBox.Show(message, header, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка на дубликат
            foreach (var product in _selectedProductList)
            {
                if (product.CodeProduct == id)
                {
                    const string message = "Изделие с таким кодом уже имеется!";
                    const string header = "Ошибка валидации";
                    MessageBox.Show(message, header, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            try
            {
                _selectedProductList.Add(new Product()
                {
                    Id = _selectedProduct2.Id,
                    CodeProduct = id,
                    Name = name,
                    Mark = mark
                });
                ProductListDataGrid.ItemsSource = _selectedProductList;
                ProductListDataGrid.Items.Refresh();
                ShowCountItemsPageDataGrid();
            }
            catch (StorageException ex)
            {
                Common.ShowDetailExceptionMessage(ex);
            }

            Reload();
        }

        /// <summary>
        /// Нажатие кнопки [УДЛ] главного DataGrid страницы - удаление объекта выделенной строки
        /// </summary>
        private void DeleteProductFromListButton_OnClick(object senderIsButton, RoutedEventArgs e)
        {
            var frameworkElement = senderIsButton as FrameworkElement;
            if (frameworkElement == null)
            {
                return;
            }
            var product = frameworkElement.DataContext as Product;
            if (product == null)
            {
                return;
            }
            _selectedProductList.Remove(_selectedProductList.First(p => p.Id == product.Id && p.CodeProduct == product.CodeProduct && p.Name == product.Name &&
                p.Mark == product.Mark));
            ProductListDataGrid.ItemsSource = _selectedProductList;
            ProductListDataGrid.Items.Refresh();
            ShowCountItemsPageDataGrid();
        }

        /// <summary>
        /// Нажатие кнопки [Копировать]
        /// </summary>
        private void CopyButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_selectedMaterial == null)
            {
                MessageBox.Show($"Вы не выбрали материал для копирования, копирование отменено!",
                    PageLiterals.HeaderInformationOrWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Вы действительно желаете скопировать материал [{_selectedMaterial.CodeMaterial}] на каждое из указанных изделий?",
                PageLiterals.HeaderConfirm, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            if (_selectedProductList.Count > 0)
            {
                //Сохраняем значение ProgressBar
                double value = 0;

                //Создаем новый экземпляр делегата для ProgressBar
                // который показывает на метод ProgressBar.SetValue
                var updatePbDelegate = new UpdateProgressBarDelegate(ProgressBar.SetValue);

                ProgressBarLabel.Content = "Копирование:";
                ProgressBarGrid.Visibility = Visibility.Visible;
                ProgressBar.Maximum = _selectedProductList.Count;
                ProgressBar.Value = value;

                try
                {
                    var productsId = ProductsStorageMssql.GetProductsIdWhereMaterial(_selectedMaterial);
                    foreach (var selectedProduct in _selectedProductList)
                    {
                        var isHave = false;
                        foreach (var productId in productsId)
                        {
                            if (selectedProduct.Id == productId)
                            {
                                isHave = true;
                                break;
                            }
                        }

                        if (!isHave)
                        {
                            Nu65Service.Insert(selectedProduct.Id, selectedProduct.DisplayCodeString,
                                _selectedMaterial.ServiceMappedMeasureId,
                                _selectedMaterial.Measure.DisplayOldDbCodeString, _selectedMaterial.Id, _selectedMaterial.DisplayCodeString,
                                _selectedMaterial.AuxiliaryMaterialConsumptionRate, _selectedMaterial.WorkGuildId, _selectedMaterial.SignMaterial, _selectedMaterial.ParcelId,
                                _selectedMaterial.UnitValidation, DateTime.Today, _selectedMaterial.FlowRate);
                        }

                        value += 1;
                        Dispatcher?.Invoke(updatePbDelegate, DispatcherPriority.Background, ProgressBar.ValueProperty,
                            value);
                    }

                    ProgressBarGrid.Visibility = Visibility.Collapsed;
                    MessageBox.Show($"Копирование успешно произведено!",
                        PageLiterals.HeaderInformationOrWarning, MessageBoxButton.OK, MessageBoxImage.Information);

                    _selectedProductList = new List<Product>();
                    ProductListDataGrid.ItemsSource = _selectedProductList;
                    ProductListDataGrid.Items.Refresh();
                    ShowCountItemsPageDataGrid();
                }
                catch (DbException ex) // DbException - суперкласс для SqlException и OleDbException
                {
                    throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
                }
            }
            else
            {
                MessageBox.Show($"Вы не выбрали куда копировать, копирование отменено!",
                    PageLiterals.HeaderInformationOrWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        /// <summary>
        /// Нажатие кнопки [Отмена]
        /// </summary>
        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            PageSwitcher.Switch(new CopyMaterialToProducts());
        }
    }
}
