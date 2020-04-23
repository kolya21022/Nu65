using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using NU65.Db;
using NU65.Entities.External;
using NU65.Services;
using NU65.Util;
using NU65.View.Util;
using NU65.View.Windows;

namespace NU65.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для WorkingPartNu65.xaml
    /// </summary>
    public partial class WorkingPartNu65 : IPageable
    {
        /// <summary>
        /// Лист продуктов поисковика
        /// </summary>
        private List<Product> _searchProducts;

        /// <summary>
        /// Выбранный продукт
        /// </summary>
        private Product _editedProduct;

        /// <summary>
        /// Материалы выбранного продукта
        /// </summary>
        private static List<Material> _materials;

        public WorkingPartNu65()
        {
            InitializeComponent();
            VisualInitializeComponent();
            AdditionalInitializeComponent();
        }

        public void AdditionalInitializeComponent()
        {
            try
            {
                _searchProducts = ProductsService.GetAllMssql();
                _searchProducts.Sort();
                ProductListBox.ItemsSource = null;
                ProductListBox.ItemsSource = _searchProducts;
            }
            catch (StorageException ex)
            {
                Common.ShowDetailExceptionMessage(ex);
            }
        }

        /// <summary>
        /// Визуальная инициализация страницы (цвета и размеры шрифтов контролов)
        /// </summary>
        /// <inheritdoc />
        public void VisualInitializeComponent()
        {
            FontSize = Constants.FontSize + 2D;
            FieldsWrapperGrid.Background = Constants.BackColor1_AthensGray;

            // Заголовок страницы
            TitlePageGrid.Background = Constants.BackColor4_BlueBayoux;
            var titleTextBlocks = TitlePageGrid.Children.OfType<TextBlock>();
            foreach (var titleTextBlock in titleTextBlocks)
            {
                titleTextBlock.Foreground = Constants.ForeColor2_PapayaWhip;
            }
        }

        public string PageHotkeys()
        {
            return "";
        }

        public void Page_OnKeyDown(object senderIsPageOrWindow, KeyEventArgs eventArgs)
        {
            //if (eventArgs.Key != Key.Escape)
            //{
            //    return;
            //}

            //eventArgs.Handled = true;
        }

        private void ChangeProductList(Product product)
        {
            if (product == null)
            {
                return;
            }

            _materials = MaterialsService.GetMaterialsByProductId(product.Id);
            MaterialDataGrid.ItemsSource = null;
            MaterialDataGrid.ItemsSource = _materials;

            IdLabel.Content = product.CodeProduct;
            IdLabel.ToolTip = product.CodeProduct;
            NameLabel.Content = product.Name;
            NameLabel.ToolTip = product.Name;
            DesignationLabel.Content = product.Mark;
            DesignationLabel.ToolTip = product.Mark;
            CopyMaterialButton.IsEnabled = true;
            AddMaterialButton.IsEnabled = true;
        }


        private void SearchTextBox_OnTextChanged(object senderIsTextBox, TextChangedEventArgs e)
        {
            // TextBox поиска
            var searchTextBox = senderIsTextBox as TextBox;
            if (searchTextBox == null)
            {
                return;
            }

            // Разделение введенного пользователем текста по пробелам на массив слов
            var searchResult = new List<Product>();
            var searchValues = searchTextBox.Text.Trim().Split(null);
            const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
            foreach (var item in _searchProducts)
            {
                // Поиск совпадений всех значений массива по требуемым полям сущности
                var isCoincided = true;

                var idString = item.DisplayCodeString.ToString(CultureInfo.InvariantCulture);
                foreach (var searchValue in searchValues)
                {
                    isCoincided &= idString.IndexOf(searchValue, comparisonIgnoreCase) >= 0;
                }

                // Если в полях сущности есть введённые слова, добавляем объект в буферный список
                if (isCoincided)
                {
                    searchResult.Add(item);
                }
            }

            // Перезаполнение DataGrid поиска сущности с учётом найденых значений
            ProductListBox.ItemsSource = null;
            ProductListBox.ItemsSource = searchResult;
        }

        private void ProductListBox_OnSelectionChanged(object senderIsListBox, SelectionChangedEventArgs e)
        {
            var listBox = senderIsListBox as ListBox;
            if (listBox == null)
            {
                return;
            }

            var selectedItems = listBox.SelectedItems;
            if (selectedItems.Count == 0)
            {
                return;
            }

            var product = listBox.SelectedItem as Product;

            _editedProduct = product;
            ChangeProductList(product);
        }

        private void ProductListBox_OnKeyDown(object sender, KeyEventArgs eventArgs)
        {
            if (ReferenceEquals(sender, ProductListBox))
            {
                if (eventArgs.Key == Key.Right || eventArgs.Key == Key.Enter)
                {
                    eventArgs.Handled = true;
                    if (MaterialDataGrid.Items.Count > 0)
                    {
                        Keyboard.Focus(MaterialDataGrid);
                        MaterialDataGrid.SelectedIndex = 0;
                        var row = (DataGridRow) MaterialDataGrid.ItemContainerGenerator.ContainerFromIndex(0);
                        row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    }
                }
            }
        }

        /// <summary>
        /// Обработчик нажатия клавиш MaterialDataGrid
        /// </summary>
        private void MaterialDataGrid_OnPreviewKeyDown(object sender, KeyEventArgs eventArgs)
        {
            if (ReferenceEquals(sender, MaterialDataGrid))
            {
                if (eventArgs.Key == Key.Left)
                {
                    eventArgs.Handled = true;
                    if (ProductListBox.Items.Count > 0)
                    {
                        Keyboard.Focus(ProductListBox);

                        ProductListBox.UpdateLayout(); // Pre-generates item containers 

                        var listBoxItem = (ListBoxItem) ProductListBox
                            .ItemContainerGenerator
                            .ContainerFromItem(ProductListBox.SelectedItem);

                        listBoxItem.Focus();
                    }
                }
            }

            // Если нажата [Delete]
            if (eventArgs.Key == Key.Delete)
            {
                var frameworkElement = sender as FrameworkElement;
                if (frameworkElement == null)
                {
                    return;
                }

                var deleteMaterial = frameworkElement.DataContext as Material;
                if (deleteMaterial == null)
                {
                    return;
                }

                eventArgs.Handled = true;
                DeleteEntityIfConfirmation(deleteMaterial);
            }
        }

        /// <summary>
        /// Нажатие кнопки [Удалить] в MaterialDataGrid
        /// </summary>
        private void DeleteButton_OnClick(object senderIsButton, RoutedEventArgs e)
        {
            var frameworkElement = senderIsButton as FrameworkElement;
            if (frameworkElement == null)
            {
                return;
            }

            var deleteMaterial = frameworkElement.DataContext as Material;
            if (deleteMaterial == null)
            {
                return;
            }

            DeleteEntityIfConfirmation(deleteMaterial); // Удаление сущности, если пользователь подтвердит намерения
        }

        /// <summary>
        /// Подтверждение удаления сущности пользователем, удаление в случае одобрения и перезагрузка страницы
        /// </summary>
        private void DeleteEntityIfConfirmation(Material deleteMaterial)
        {
            const string confirmHeader = PageLiterals.HeaderConfirm;
            const string confirmMessage = PageLiterals.СonfirmDeleteMessage;
            const MessageBoxResult defaultButtonFocus = MessageBoxResult.No;
            const MessageBoxButton buttons = MessageBoxButton.OKCancel;
            const MessageBoxImage type = MessageBoxImage.Question;
            var boxResult = MessageBox.Show(confirmMessage, confirmHeader, buttons, type, defaultButtonFocus);
            if (boxResult != MessageBoxResult.OK)
            {
                return;
            }

            try
            {
                var flag = Nu65Service.Delete(deleteMaterial, _editedProduct);

                if (!flag)
                {
                    return;
                }

                _materials.Remove(deleteMaterial);
                MaterialDataGridUpdate();
            }
            catch (StorageException ex)
            {
                Common.ShowDetailExceptionMessage(ex);
            }
        }

        /// <summary>
        /// Перезаполнение MaterialDataGrid
        /// </summary>
        private void MaterialDataGridUpdate()
        {
            MaterialDataGrid.ItemsSource = null;
            MaterialDataGrid.ItemsSource = _materials;

        }

        private void EditButton_OnClick(object senderIsButton, RoutedEventArgs e)
        {
            var frameworkElement = senderIsButton as FrameworkElement;
            if (frameworkElement == null)
            {
                return;
            }

            var material = frameworkElement.DataContext as Material;
            if (material == null)
            {
                return;
            }

            var materialNu65AddWindow = new MaterialNu65AddWindow(material, _editedProduct)
            {
                Owner = Window.GetWindow(this)
            };
            materialNu65AddWindow.ShowDialog();

            if (!materialNu65AddWindow.DialogResult.HasValue || materialNu65AddWindow.DialogResult != true)
            {
                MaterialDataGrid.SelectedIndex = -1;
                return;
            }

            // Получение введённых пользователем параметров

            var newMaterial = materialNu65AddWindow.SelectedMaterial();
            _materials.Remove(material);
            _materials.Add(newMaterial);
            _materials.Sort();

            ReloadMaterials();
        }

        /// <summary>
        /// Нажатие кнопки [Добавить материал]
        /// </summary>
        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            var materialNu65AddWindow = new MaterialNu65AddWindow(_editedProduct)
            {
                Owner = Window.GetWindow(this)
            };
            materialNu65AddWindow.ShowDialog();

            if (!materialNu65AddWindow.DialogResult.HasValue || materialNu65AddWindow.DialogResult != true)
            {
                return;
            }

            // Получение введённых пользователем параметров

            var material = materialNu65AddWindow.SelectedMaterial();
            _materials.Add(material);

            ReloadMaterials();
        }

        /// <summary>
        /// Нажатие кнопки [Добавить продукт]
        /// </summary>
        private void AddProductButton_OnClick(object sender, RoutedEventArgs e)
        {
            var addProductInMssqlWindow = new AddProductInMssqlWindow()
            {
                Owner = Window.GetWindow(this)
            };
            addProductInMssqlWindow.ShowDialog();

            if (!addProductInMssqlWindow.DialogResult.HasValue || addProductInMssqlWindow.DialogResult != true)
            {
                return;
            }

            // Получение введённых пользователем параметров

            var product = addProductInMssqlWindow.SelectedProduct();
            _searchProducts.Add(product);

            ReloadProducts();
        }

        /// <summary>
        /// Перезагруска списка продуктов
        /// </summary>
        private void ReloadProducts()
        {
            ProductListBox.SelectedIndex = -1;
            ProductListBox.ItemsSource = null;
            ProductListBox.ItemsSource = _searchProducts;
        }

        /// <summary>
        /// Перезагруска DataGrid материалов
        /// </summary>
        private void ReloadMaterials()
        {
            MaterialDataGrid.SelectedIndex = -1;
            MaterialDataGrid.ItemsSource = null;
            MaterialDataGrid.ItemsSource = _materials;
        }

        /// <summary>
        /// Нажатие кнопки [Скопировать материалы]
        /// </summary>
        private void CopyButton_OnClick(object senderIsButton, RoutedEventArgs e)
        {
            var copyMaterialWindow = new CopyMaterialWindow(_editedProduct, _materials)
            {
                Owner = Window.GetWindow(this)
            };
            copyMaterialWindow.ShowDialog();

            if (!copyMaterialWindow.DialogResult.HasValue || copyMaterialWindow.DialogResult != true)
            {
                return;
            }

            // Получение введённых пользователем параметров

            var materials = copyMaterialWindow.SelectedMaterials();
            foreach (var material in materials)
            {
                _materials.Add(material);
            }

            ReloadMaterials();
        }
    }
}
