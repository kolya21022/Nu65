using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using NU65.Db;
using NU65.Entities.External;
using NU65.Services;
using NU65.Util;
using NU65.View.Util;

namespace NU65.View.Windows
{
    /// <summary>
    /// Окно для копирования из одного изделия в другое
    /// </summary>
    public partial class CopyMaterialWindow
    {
        // Создаем Delegate который соответствует
        // методу ProgressBar.SetValue
        private delegate void UpdateProgressBarDelegate(DependencyProperty dp, Object value);

        /// <summary>
        /// Хранилище продукта в который копируется
        /// </summary>
        private readonly Product _editedProduct;

        /// <summary>
        /// Материалы продукта в который копируется
        /// </summary>
        private static List<Material> _editedMaterials;

        /// <summary>
        /// Хранилище продукта из которого копируется
        /// </summary>
        private Product _selectedProduct;

        /// <summary>
        /// Материалы продукта из которого копируется
        /// </summary>
        private static List<Material> _selectedMaterials;

        /// <summary>
        /// Материалы продукта скопированные
        /// </summary>
        private static List<Material> _resultMaterials;

        private const string EmptySubstitutableValue = "-";

        public CopyMaterialWindow(Product editedProduct, List<Material> editedMaterials)
        {
            _editedProduct = editedProduct;
            _editedMaterials = editedMaterials;

            InitializeComponent();
            VisualInitializeComponent();
            AdditionalInitializeComponent();

        }

        /// <summary>
        /// Инициализация данными полей формы, загрузка соответсвующих списков из БД в локальные хранилища,
        /// заполнение локальных списков и таблиц ящиков/драг.металлов (в случае редактирования)
        /// </summary>
        public void AdditionalInitializeComponent()
        {
            //Получение списка изделий
            var product = new List<Product>();

            FromProductComboBox.ItemsSource = null;
            ToProductComboBox.ItemsSource = null;
            try
            {
                product = ProductsService.GetAllMssql();
            }
            catch (StorageException ex)
            {
                Common.ShowDetailExceptionMessage(ex);
            }

            if (product != null && product.Count > 0)
            {
                product.Sort();
            }

            FromProductComboBox.ItemsSource = product;
            ToProductComboBox.ItemsSource = product;

            if (_editedProduct == null)
            {
                return;
            }

            var searchProduct = new Product()
            {
                Id = _editedProduct.Id,
                CodeProduct = _editedProduct.CodeProduct,
                Name = _editedProduct.Name,
                Mark = _editedProduct.Mark,
            };
            ToProductComboBox.SelectedItem = searchProduct;
            ToProductNameTextBox.Text = searchProduct.Name;
            ToProductComboBox.IsEnabled = false;

            MaterialToProductListBox.ItemsSource = null;
            MaterialToProductListBox.ItemsSource = _editedMaterials;
        }

        /// <summary>
        /// Визуальная инициализация страницы (цвета и размеры шрифтов контролов)
        /// </summary>
        public void VisualInitializeComponent()
        {
            FontSize = Constants.FontSize;
            FieldsWrapperGrid.Background = Constants.LineBorderColor2_Nepal;
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
        /// Нажатие кнопка [Копировать]
        /// </summary>
        private void CopyButton_OnClick(object sender, RoutedEventArgs e)
        {
            CopyAndExit();
        }

        /// <summary>
        /// Нажатие кнопка [Отмена]
        /// </summary>
        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Сервисный метод раскрытия выпадающего списка для события PreviewMouseDown для поискового ComboBox.
        /// </summary>
        private void SearchComboBox_OnPreviewMouseDown(object senderIsComboBox, MouseButtonEventArgs eventArgs)
        {
            OnPreviewMouseDown(eventArgs);
            PageUtil.Service_SearchComboBox_OnPreviewMouseDown(senderIsComboBox);
        }

        /// <summary>
        /// Сервисный метод раскрытия выпадающего списка для события PreviewMouseUp для поискового ComboBox.
        /// </summary>
        private void SearchComboBox_OnPreviewMouseUp(object senderIsComboBox, MouseButtonEventArgs eventArgs)
        {
            OnPreviewMouseUp(eventArgs);
            PageUtil.Service_SearchComboBox_OnPreviewMouseUp(senderIsComboBox);
        }

        /// <summary>
        /// Сервисный метод обработчик для события PreviewMouseDown для поискового ComboBox.
        /// Нажатии [Enter] клавиатурный фокус перемещается на нижележащее поле, иначе раскрытие выпадающего списка.
        /// Нижележащее поле определяется методом FocusNavigationDirection.Down
        /// </summary>
        private void SearchComboBox_OnPreviewKeyUp(object senderIsComboBox, KeyEventArgs eventArgs)
        {
            OnPreviewKeyDown(eventArgs);
            PageUtil.Service_SearchComboBox_OnPreviewKeyDown(senderIsComboBox, eventArgs);

        }

        /// <summary>
        /// Сервисный метод раскрытия выпадающего списка для события GotFocus для поискового ComboBox.
        /// </summary>
        private void SearchComboBox_OnGotFocus(object senderIsComboBox, RoutedEventArgs eventArgs)
        {
            OnGotFocus(eventArgs);
            PageUtil.Service_SearchComboBox_OnGotFocus(senderIsComboBox);
        }

        /// <summary>
        /// Обработка нажатия клавиш в фокусе всей страницы 
        /// </summary>
        private void Window_OnKeyDown(object sender, KeyEventArgs eventArgs)
        {
            if (eventArgs.Key != Key.Escape)
            {
                return;
            }

            eventArgs.Handled = true;
            Close(); // Если нажат [Esc] 
        }

        /// <summary>
        /// Обработчик выбора продукта из которого копируется
        /// </summary>
        private void FromProductComboBox_OnSelectionChanged(object senderIsComboBox, SelectionChangedEventArgs e)
        {
            FromProductNameTextBox.Text = EmptySubstitutableValue;
            // ReSharper disable once UsePatternMatching
            var comboBox = senderIsComboBox as ComboBox;
            if (comboBox == null)
            {
                return;
            }

            _selectedProduct = comboBox.SelectedItem as Product;
            if (_selectedProduct == null)
            {
                return;
            }

            FromProductNameTextBox.Text = _selectedProduct.Name;
            ChangeProductList(_selectedProduct);

        }

        private void ChangeProductList(Product product)
        {
            if (product == null)
            {
                return;
            }

            _selectedMaterials = MaterialsService.GetMaterialsByProductIdWhithMeasure(product.Id);
            MaterialFromProductListBox.ItemsSource = null;
            MaterialFromProductListBox.ItemsSource = _selectedMaterials;
        }

        /// <summary>
        /// Валидация, сохранение и выход к списку сущностей
        /// </summary>
        private void CopyAndExit()
        {
            _resultMaterials = new List<Material>();

            ProgressBarLabel.Content = "Подготовка:";
            ProgressBar.Maximum = _selectedMaterials.Count;
            ProgressBar.Value = 0;

            //Сохраняем значение ProgressBar
            double value = 0;

            //Создаем новый экземпляр делегата для ProgressBar
            // который показывает на метод ProgressBar.SetValue
            var updatePbDelegate = new UpdateProgressBarDelegate(ProgressBar.SetValue);


            foreach (var selected in _selectedMaterials)
            {
                value += 1;

                bool flag = false;
                foreach (var edited in _editedMaterials)
                {
                    if (selected.CodeMaterial == edited.CodeMaterial
                        && selected.ParcelId == edited.ParcelId
                        && selected.WorkGuildId == edited.WorkGuildId)
                    {
                        flag = true;
                        break;
                    }
                }

                if (!flag)
                {
                    _resultMaterials.Add(selected);
                }

                Dispatcher?.Invoke(updatePbDelegate, DispatcherPriority.Background, ProgressBar.ValueProperty,
                    value);
            }

            ProgressBarLabel.Content = "Копирование:";
            ProgressBar.Maximum = _resultMaterials.Count;
            value = 0;
            ProgressBar.Value = value;

            try
            {
                foreach (var item in _resultMaterials)
                {
                    value += 1;

                    var id = Nu65Service.Insert(_editedProduct.Id, _editedProduct.DisplayCodeString,
                        item.ServiceMappedMeasureId,
                        item.Measure.DisplayOldDbCodeString, item.Id, item.DisplayCodeString,
                        item.AuxiliaryMaterialConsumptionRate, item.WorkGuildId, item.SignMaterial, item.ParcelId,
                        item.UnitValidation, DateTime.Today, item.FlowRate);
                    item.Nu65TableId = id != 0 ? id : item.Nu65TableId;

                    Dispatcher?.Invoke(updatePbDelegate, DispatcherPriority.Background, ProgressBar.ValueProperty,
                        value);
                }
            }
            catch (DbException ex) // DbException - суперкласс для SqlException и OleDbException
            {
                throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
            }

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Получение скопированных материалов
        /// </summary>
        public List<Material> SelectedMaterials()
        {
            return _resultMaterials;
        }
    }
}
