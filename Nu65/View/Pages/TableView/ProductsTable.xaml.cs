using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using NU65.Db;
using NU65.Entities.External;
using NU65.Services;
using NU65.Util;
using NU65.View.Util;

namespace NU65.View.Pages.TableView
{
    /// <summary>
    /// Страница с таблицей [Изделий]
    /// </summary>
    /// <inheritdoc cref="Page" />
    public partial class ProductsTable : IPageable
    {
	    private string _type;
		/// <summary>
		/// Критерии фильтрации главного DataGrid страницы
		/// </summary>
		private readonly FilterCriterias _filterCriterias = new FilterCriterias();
        public ProductsTable(string type)
        {
	        _type = type;
            InitializeComponent();
            AdditionalInitializeComponent();
            VisualInitializeComponent();
            // Указание первой строки выбранным объектом DataGrid
            PageUtil.SelectSpecifiedOrFirstDataGridRow<Product>(PageDataGrid, null);
        }

		/// <summary>
		/// Загрузка списка объектов из базы данных, их отображение в таблице, указание их кол-ва в Label
		/// </summary>
		// ReSharper disable once InheritdocInvalidUsage
		/// <inheritdoc />
		public void AdditionalInitializeComponent()
        {
            FilterBarCoverLabel.Content = PageLiterals.FilterBarCoverLabel; // Сообщение-заглушка панели фильтрации
            try
            {
	            var products = new List<Product>();
				if (_type == "s")
				{
					TitleLabel.Content = " База изделий [MsSQL]";
					products = ProductsService.GetAllMssql();
				}
				else
				{
					TitleLabel.Content = " База изделий [FoxPro]";
					 products = ProductsService.GetAllFoxPro();
				}
				if (products != null && products.Count > 0)
                {
                    // Критерии сортировки указаны в реализации интерфейса IComparable класса
                    products.Sort();
                }
                PageDataGrid.ItemsSource = products;
                ShowCountItemsPageDataGrid();
            }

            catch (StorageException ex)
            {
                Common.ShowDetailExceptionMessage(ex);
            }
        }

        /// <summary>
        /// Визуальная инициализация страницы (цвета и размеры шрифтов контролов)
        /// </summary>
        public void VisualInitializeComponent()
        {
            FontSize = Constants.FontSize;
            PageDataGrid.AlternatingRowBackground = Constants.BackColor1_AthensGray;

            // Заголовок страницы
            TitlePageGrid.Background = Constants.BackColor4_BlueBayoux;
            var titleLabels = TitlePageGrid.Children.OfType<Label>();
            foreach (var titleLabel in titleLabels)
            {
                titleLabel.Foreground = Constants.ForeColor2_PapayaWhip;
            }

            // Панель фильтрации и контекстное меню фильтра главного DataGrid
            var filterBarCoverLabels = FilterBarCoverStackPanel.Children.OfType<Label>();
            foreach (var label in filterBarCoverLabels)
            {
                label.Foreground = Constants.ForeColor1_BigStone;
            }
            FilterBarCoverStackPanel.Background = Constants.BackColor1_AthensGray;
            if (PageDataGrid.ContextMenu != null)
            {
                PageDataGrid.ContextMenu.FontSize = Constants.FontSize;
            }
        }

        /// <summary>
        /// Горячие клавиши текущей страницы
        /// </summary>
        public string PageHotkeys()
        {
            const string filter = PageLiterals.HotkeyLabelFilter;
            const string closeApp = PageLiterals.HotkeyLabelCloseApp;
            const string separator = PageLiterals.HotkeyLabelsSeparator;
            const string displayed = filter + separator + closeApp;
            return displayed;
        }

        /// <summary>
        /// Обработка нажатия клавиш в фокусе всей страницы 
        /// </summary>
        public void Page_OnKeyDown(object senderIsPageOrWindow, KeyEventArgs eventArgs)
        {
            if (eventArgs.Key != Key.Escape)
            {
                return;
            }
            eventArgs.Handled = true;
            PageUtil.ConfirmCloseApplication(); // Если нажат [Esc] - запрос подтверждения выхода у пользователя
        }

        /// <summary>
        /// Отображение числа записей в Label над таблицей
        /// </summary>
        private void ShowCountItemsPageDataGrid()
        {
            const string countItemsPattern = PageLiterals.PatternCountItemsTable;
            var message = string.Format(countItemsPattern, PageDataGrid.Items.Count);
            CountItemsLabel.Content = message;
        }

        /// <summary>
        /// Выставление клавиатурного фокуса ввода на строку DataGrid
        /// </summary>
        private void PageDataGrid_OnLoaded(object senderIsDatagrid, RoutedEventArgs eventArgs)
        {
            PageUtil.SetFocusOnSelectedRowInDataGrid(senderIsDatagrid);
        }

        /// <summary>
		/// Перезаполнение данных главной таблицы с учётом фильтров
		/// </summary>
		private void PageDataGrid_Refilling()
        {
            PageUtil.PageDataGrid_RefillingWithFilters(PageDataGrid, _filterCriterias, MapFilterPredicate);
            ShowCountItemsPageDataGrid();   // Показ нового к-ва записей таблицы
                                            // Установка фокуса нужна для срабатывания Esc для закрытия
            PageUtil.SelectSpecifiedOrFirstDataGridRow<Measure>(PageDataGrid, null);
            PageUtil.SetFocusOnSelectedRowInDataGrid(PageDataGrid);
        }

        /// <summary>
        /// Метод-предикат (булевый) текущей записи коллекции сущностей, который возвращает true или 
        /// false в зависимости от попадания в диапазон фильтра по всем полям фильтрации.
        /// </summary>
        private bool MapFilterPredicate(object rawEntity)
        {
            var product = (Product)rawEntity;
            if (_filterCriterias.IsEmpty)
            {
                return true;
            }
            var result = true;

            // Проверка наличия полей сущности в критериях фильтрации и содержит ли поле искомое значение фильтра
            // Если в фильтре нет поля сущности, поле считается совпадающим по критерию
            string buffer;
            var filter = _filterCriterias;
            result &= !filter.GetValue("DisplayCodeString", out buffer) 
                      || FilterCriterias.ContainsLine(product.DisplayCodeString, buffer);
            result &= !filter.GetValue("Name", out buffer) || FilterCriterias.ContainsLine(product.Name, buffer);
            result &= !filter.GetValue("Mark", out buffer) || FilterCriterias.ContainsLine(product.Mark, buffer);
            return result;
        }

        /// <summary>
        /// Нажатие клавиши в контексном меню - исправление дефекта скрытия фильтра при переключении раскладки ввода
        /// </summary>
        private void PopupFilterContextMenu_OnKeyDown(object senderIsMenuItem, KeyEventArgs eventArgs)
        {
            var key = eventArgs.Key;
            eventArgs.Handled = key == Key.System || key == Key.LeftAlt || key == Key.RightAlt;
        }

        /// <summary>
        /// Обработка нажатия Enter в поле ввода фильтра
        /// </summary>
        private void PopupFilterValue_OnKeyDown(object senderIsTextBox, KeyEventArgs eventArgs)
        {
            if (eventArgs.Key != Key.Return)
            {
                return;
            }
            eventArgs.Handled = true;
            PopupFilterConfirmButton_OnClick(senderIsTextBox, eventArgs);
        }

        /// <summary>
        /// Обработка применения фильтра
        /// </summary>
        private void PopupFilterConfirmButton_OnClick(object senderIsButtonOrTextBox, RoutedEventArgs eventArgs)
        {
            PageUtil.Service_PageDataGridPopupFilterConfirm(senderIsButtonOrTextBox, _filterCriterias);
            FiltersDataGrid_Refilling();
            PageDataGrid_Refilling();
        }

        /// <summary>
        /// Перезаполнение фильтрующего DataGrid и скрытие/отображение соответ. панелей в завис. от критериев фильтра
        /// </summary>
        private void FiltersDataGrid_Refilling()
        {
            PageUtil.RefillingFilterTableAndShowHidePanel(FiltersDataGrid, _filterCriterias,
                FilterBarTableAndButtonGrid, FilterBarCoverStackPanel);
        }

        /// <summary>
        /// Подстановка имени столбца при открытии контексного меню фильтрации DataGrid, 
        /// установка Tag и перемещение фокуса ввода в поле
        /// </summary>
        private void PageDataGrid_OnContextMenuOpening(object senderIsDataGrid, ContextMenuEventArgs eventArgs)
        {
            PageUtil.Service_PageDataGridWithFilterContextMenuOpening(senderIsDataGrid);
        }

        /// <summary>
        /// Нажатие кнопки [Сброс фильтров] панели фильтрации - удаление всех введённых фильтров
        /// </summary>
        private void AllFilterDeleteButton_Click(object senderIsButton, RoutedEventArgs eventArgs)
        {
            _filterCriterias.ClearAll();    // очистка словаря критериев фильтрации
            FiltersDataGrid_Refilling();    // перезаполнение панели фильтров и скрытие/отображение с учётом критериев
            PageDataGrid_Refilling();       // перезаполнение главного DataGrid
        }

        /// <summary>
        /// Нажатие кнопки [УДЛ] DataGrid фильтрации - удаление указанного фильтра
        /// </summary>
        private void FilterDeleteButton_Click(object senderIsButton, RoutedEventArgs eventArgs)
        {
            var pressedButton = senderIsButton as Button;
            if (pressedButton == null)
            {
                return;
            }
            var deletedColumn = pressedButton.Tag as string; // получение столбца фильтра из св-ва Tag кнопки удаления
            if (string.IsNullOrWhiteSpace(deletedColumn))
            {
                return;
            }
            _filterCriterias.RemoveCriteria(deletedColumn); // удаление критерия фильтрации из словаря
            FiltersDataGrid_Refilling();    // перезаполнение панели фильтров и скрытие/отображение с учётом критериев
            PageDataGrid_Refilling();       // перезаполнение главного DataGrid
        }
    }
}
