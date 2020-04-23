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

namespace NU65.View.Windows
{
    /// <summary>
    /// Окно с таблицей [Изделий] для занесения в MsSql
    /// </summary>
    public partial class AddProductInMssqlWindow
	{
		private List<Product> _products;
	    private Product _selectedProduct;

		/// <summary>
		/// Критерии фильтрации главного DataGrid страницы
		/// </summary>
		private readonly FilterCriterias _filterCriterias = new FilterCriterias();
		public AddProductInMssqlWindow()
		{
			InitializeComponent();
			VisualInitializeComponent();
			AdditionalInitializeComponent();
			// Указание первой строки выбранным объектом DataGrid
			PageUtil.SelectSpecifiedOrFirstDataGridRow<Product>(WindowDataGrid, null);
		}

		/// <summary>
		/// Загрузка списка объектов из базы данных, их отображение в таблице, указание их кол-ва в Label
		/// </summary>
		public void AdditionalInitializeComponent()
		{
			FilterBarCoverLabel.Content = PageLiterals.FilterBarCoverLabel; // Сообщение-заглушка панели фильтрации
			try { 
			_products = ProductsService.GetAllFoxPro();
			if (_products != null && _products.Count > 0)
			{
				// Критерии сортировки указаны в реализации интерфейса IComparable класса
				_products.Sort();
			}

			WindowDataGrid.ItemsSource = _products;
				ShowCountItemsWindowDataGrid();
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
			WindowDataGrid.AlternatingRowBackground = Constants.BackColor1_AthensGray;

			// Заголовок окна
			TitleWindowGrid.Background = Constants.BackColor4_BlueBayoux;
			var titleLabels = TitleWindowGrid.Children.OfType<Label>();
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
			if (WindowDataGrid.ContextMenu != null)
			{
				WindowDataGrid.ContextMenu.FontSize = Constants.FontSize;
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
			const string displayed = filter + separator  + closeApp;
			return displayed;
		}

		/// <summary>
		/// Отображение числа записей в Label над таблицей
		/// </summary>
		private void ShowCountItemsWindowDataGrid()
		{
			const string countItemsPattern = PageLiterals.PatternCountItemsTable;
			var message = string.Format(countItemsPattern, WindowDataGrid.Items.Count);
			CountItemsLabel.Content = message;
		}

		/// <summary>
		/// Обработка нажатия клавиш в фокусе всего окна
		/// </summary>
		private void AddProductInMssqlWindow__OnKeyDown(object senderIsPageOrWindow, KeyEventArgs eventArgs)
		{
			if (eventArgs.Key != Key.Escape)
			{
				return;
			}
			eventArgs.Handled = true;
			Close();
		}

		/// <summary>
		/// Нажатие кнопка [Добавить]
		/// </summary>
		private void AddButton_OnClick(object senderIsButton, RoutedEventArgs e)
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

		    const string confirmHeader = PageLiterals.HeaderConfirm;
		    const MessageBoxResult defaultButtonFocus = MessageBoxResult.No;
		    const MessageBoxButton buttons = MessageBoxButton.YesNo;
		    const MessageBoxImage type = MessageBoxImage.Question;
		    var boxResult = MessageBox.Show("Вы действительно желаете добавить изделие " +
		                                    $"[{product.CodeProduct} {product.Name}] в бд Mssql?",
		        confirmHeader, buttons, type, defaultButtonFocus);

		    if (boxResult != MessageBoxResult.Yes)
		    {
		        return;
		    }

            _selectedProduct = product;
		    AddProductInMssql(); // Добавление продукта в таблицу Mssql, если такого продукта нет

		    Close();
        }

	    private void AddProductInMssql()
	    {
	        var mssqlServer = Properties.Settings.Default.SqlServerNu65;
	        var mssqlDb = Properties.Settings.Default.SqlDbNu65Db;
	        try
	        {
	            using (var connection = DbControl.GetConnection(mssqlServer, mssqlDb))
	            {
	                connection.TryConnectOpen();
	                var flag = ProductsService.IsDublicate(_selectedProduct.CodeProduct, connection);
	                if (flag)
	                {
	                    const MessageBoxImage messageType = MessageBoxImage.Error;
	                    const MessageBoxButton messageButtons = MessageBoxButton.OK;
	                    const string validationHeader = PageLiterals.HeaderValidation;
	                    MessageBox.Show($"Продукт с кодом [{_selectedProduct.CodeProduct}] уже есть в базе MsSql",
	                        validationHeader, messageButtons, messageType);
	                }
	                else
	                {
	                    long id = ProductsService.Insert(_selectedProduct.CodeProduct, _selectedProduct.Name,
	                        _selectedProduct.Mark,
	                        connection);
	                    _selectedProduct.Id = id;
	                    const MessageBoxImage messageType = MessageBoxImage.Information;
	                    const MessageBoxButton messageButtons = MessageBoxButton.OK;
	                    const string header = "Добавление";
	                    MessageBox.Show($"Продукт с кодом [{_selectedProduct.CodeProduct}] добавлен в базу MsSql",
	                        header, messageButtons, messageType);
	                    DialogResult = true;
	                }
	            }
	        }
	        catch (StorageException ex)
	        {
	            Common.ShowDetailExceptionMessage(ex);
	        }
	    }

	    /// <summary>
		/// Подстановка имени столбца при открытии контексного меню фильтрации DataGrid, 
		/// установка Tag и перемещение фокуса ввода в поле
		/// </summary>
		private void WindowDataGrid_OnContextMenuOpening(object senderIsDataGrid, ContextMenuEventArgs eventArgs)
		{
			PageUtil.Service_PageDataGridWithFilterContextMenuOpening(senderIsDataGrid);
		}

		/// <summary>
		/// Перезаполнение данных главной таблицы с учётом фильтров
		/// </summary>
		private void WindowDataGrid_Refilling()
		{
			PageUtil.PageDataGrid_RefillingWithFilters(WindowDataGrid, _filterCriterias, MapFilterPredicate);
			ShowCountItemsWindowDataGrid();   // Показ нового к-ва записей таблицы
			// Установка фокуса нужна для срабатывания Esc для закрытия
			PageUtil.SelectSpecifiedOrFirstDataGridRow<Product>(WindowDataGrid, null);
			PageUtil.SetFocusOnSelectedRowInDataGrid(WindowDataGrid);
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
		/// Перезаполнение фильтрующего DataGrid и скрытие/отображение соответ. панелей в завис. от критериев фильтра
		/// </summary>
		private void FiltersDataGrid_Refilling()
		{
			PageUtil.RefillingFilterTableAndShowHidePanel(FiltersDataGrid, _filterCriterias,
				FilterBarTableAndButtonGrid, FilterBarCoverStackPanel);
		}
		
		/// <summary>
		/// Нажатие кнопки [Сброс фильтров] панели фильтрации - удаление всех введённых фильтров
		/// </summary>
		private void AllFilterDeleteButton_Click(object senderIsButton, RoutedEventArgs eventArgs)
		{

			_filterCriterias.ClearAll();    // очистка словаря критериев фильтрации
			FiltersDataGrid_Refilling();    // перезаполнение панели фильтров и скрытие/отображение с учётом критериев
			WindowDataGrid_Refilling();       // перезаполнение главного DataGrid
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
			WindowDataGrid_Refilling();       // перезаполнение главного DataGrid
		}

		/// <summary>
		/// Обработка применения фильтра
		/// </summary>
		private void PopupFilterConfirmButton_OnClick(object senderIsButtonOrTextBox, RoutedEventArgs eventArgs)
		{
			PageUtil.Service_PageDataGridPopupFilterConfirm(senderIsButtonOrTextBox, _filterCriterias);
			FiltersDataGrid_Refilling();
			WindowDataGrid_Refilling();
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
		/// Нажатие клавиши в контексном меню - исправление дефекта скрытия фильтра при переключении раскладки ввода
		/// </summary>
		private void PopupFilterContextMenu_OnKeyDown(object senderIsMenuItem, KeyEventArgs eventArgs)
		{
			var key = eventArgs.Key;
			eventArgs.Handled = key == Key.System || key == Key.LeftAlt || key == Key.RightAlt;
		}

	    /// <summary>
	    /// Получение выбранного продукта
	    /// </summary>
	    public Product SelectedProduct()
	    {
	        return _selectedProduct;
	    }
    }
}
