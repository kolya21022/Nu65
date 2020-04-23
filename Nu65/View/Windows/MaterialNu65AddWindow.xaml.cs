using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using NU65.Db;
using NU65.Entities.External;
using NU65.Util;
using NU65.Services;
using NU65.View.Util;

namespace NU65.View.Windows
{
	/// <summary>
	/// Логика взаимодействия для ConsumptionRateWindow.xaml
	/// </summary>
	public partial class MaterialNu65AddWindow
    {
        /// <summary>
        /// Лист материалов поисковика
        /// </summary>
        private List<Material> _searchMaterials;

        /// <summary>
        /// Хранилище продукта изделия
        /// </summary>
        private readonly Product _editedProduct;
	    
		/// <summary>
		/// Хранилище редактируемого материала изделия
		/// </summary>
		private readonly Material _editedMaterial;

		/// <summary>
		/// Константа для установки значение поля по умолчанию
		/// </summary>
		private const string EmptySubstitutableValue = "-";

		private Material _selectedMaterial;
		private Measure _selectedMeasure;

        /// <summary>
        /// Хранилище ед. изм.
        /// </summary>
        private List<Measure> _measures;

        /// <summary>
        /// Конструктор режима добавления
        /// </summary>
        /// <inheritdoc />
        public MaterialNu65AddWindow(Product editedProduct)
		{
			_editedProduct = editedProduct;
			InitializeComponent();
			VisualInitializeComponent();
			AdditionalInitializeComponent();
		}

		/// <summary>
		/// Конструктор режима редактирования
		/// </summary>
		/// <inheritdoc />
		public MaterialNu65AddWindow(Material editedMaterial, Product editedProduct)
		{
		    _editedMaterial = editedMaterial;
		    _editedProduct = editedProduct;
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
			// Установка даты по умолчанию
			DatePicker.SelectedDate = DateTime.Today;

			//Получение списков кодов материала и ед. измерения
			MeasureComboBox.ItemsSource = null;
			MaterialComboBox.ItemsSource = null;
			try
			{
				_searchMaterials = MaterialsService.GetAllMssql();
				_measures = MeasuresService.GetAll();
			}
			catch (StorageException ex)
			{
				Common.ShowDetailExceptionMessage(ex);
			}
			if ((_searchMaterials != null && _searchMaterials.Count > 0) && (_measures != null && _measures.Count > 0))
			{
			    _searchMaterials.Sort();
				_measures.Sort();
			}
			MaterialComboBox.ItemsSource = _searchMaterials;
			MeasureComboBox.ItemsSource = _measures;

			if (_editedProduct != null)
			{
				ProductIdTextBox.Text = _editedProduct.DisplayCodeString;
				ProductNameTextBox.Text = _editedProduct.Name;
				ProductNameTextBox.ToolTip = _editedProduct.Name;
				ProductMarkTextBox.Text = _editedProduct.Mark;
				ProductMarkTextBox.ToolTip = _editedProduct.Mark;
			}

		    if (_editedMaterial == null)
		    {
		        // Режим добавления
		        EditingTypeLabel.Content = PageLiterals.EditPageTypeAdd;
		        AddMaterialButton.Visibility = Visibility.Visible;
		    }
		    else
		    {
		        // Режим редактирования
		        EditingTypeLabel.Content = PageLiterals.EditPageTypeEdit;
		        AddMaterialButton.Visibility = Visibility.Hidden;
		        var searchMaterial = new Material()
		        {
		            Id = _editedMaterial.Id,
		            CodeMaterial = _editedMaterial.CodeMaterial,
		            Name = _editedMaterial.Name,
		            Profile = _editedMaterial.Profile,
		            Gost = _editedMaterial.Gost,
		        };
		        if (_measures != null)
		        {
		            foreach (var measure in _measures)
		            {
		                if (measure.Id == _editedMaterial.ServiceMappedMeasureId)
		                {
		                    _editedMaterial.Measure = measure;
		                    break;
		                }
		            }
		        }

                MaterialComboBox.SelectedItem = searchMaterial;
				MaterialNameTextBox.Text = _editedMaterial.Name;
                MaterialGostTextBox.Text = _editedMaterial.Gost;
			    MaterialProfileTextBox.Text = _editedMaterial.Profile;
			    MaterialProfileTextBox.ToolTip = _editedMaterial.Profile;
		        FlowRateTextBox.Text = _editedMaterial.FlowRate;
                MeasureComboBox.SelectedItem = _editedMaterial.Measure;
                MeasureShortNameTextBox.Text = _editedMaterial.Measure.ShortName;
				WorkGuildIdLongUpDown.Text = _editedMaterial.WorkGuildId;
				ParcelIdLongUpDown.Text = _editedMaterial.ParcelId;
				SignMaterialLongUpDown.Text = _editedMaterial.SignMaterial;
				NormUnitLongUpDown.Text = _editedMaterial.UnitValidation;
			    AuxiliaryMaterialConsumptionRateTextBox.Text = _editedMaterial.AuxiliaryMaterialConsumptionRate;
				DatePicker.SelectedDate = _editedMaterial.Date;

			    MaterialComboBox.IsEnabled = false;
			}
		  //  WorkGuildIdTextBox.Focus(); // Фокус на поле установки значения Код цеха
		}

		/// <summary>
		/// Визуальная инициализация страницы (цвета и размеры шрифтов контролов)
		/// </summary>
		public void VisualInitializeComponent()
		{
			FontSize = Constants.FontSize;
			MaterialWrappedGrid.Background = Constants.BackColor5_WaikawaGray;
		    FlowRateWrappedGrid.Background = Constants.LineBorderColor2_Nepal;
            ButtonWrappedGrid.Background = Constants.BackColor5_WaikawaGray;
			WrappedGrid.Background = Constants.BackColor3_SanJuan;

			// Заголовок страницы
			TitlePageGrid.Background = Constants.BackColor4_BlueBayoux;
			var titleLabels = TitlePageGrid.Children.OfType<Label>();
			foreach (var titleLabel in titleLabels)
			{
				titleLabel.Foreground = Constants.ForeColor2_PapayaWhip;
			}
		}

		/// <summary>
		/// Перевод клавиатурного фокуса к следующему полю ввода при нажатии Enter.
		/// </summary>
		private void JumpToNextWhenPressEnter_OnKeyDown(object senderIsTextBox, KeyEventArgs eventArgs)
		{
			PageUtil.JumpToNextWhenPressEnter_OnKeyDown(eventArgs);
		}

		/// <summary>
		/// Обработка нажатия клавиш в фокусе всей страницы 
		/// </summary>
		private void ConsumptionRateWindow__OnKeyDown(object senderIsPageOrWindow, KeyEventArgs eventArgs)
		{
			if (eventArgs.Key != Key.Escape)
			{
				return;
			}
			eventArgs.Handled = true;
			ConfirmExitIfDataHasBeenChanged(); // Если нажат [Esc] - проверка изменений полей и запрос подтверждения
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
		/// Нажатие кнопки [Отмена]
		/// </summary>
		private void CancelButton_OnClick(object sender, RoutedEventArgs e)
		{
			ConfirmExitIfDataHasBeenChanged();
		}

        /// <summary>
        /// Валидация, сохранение и выход к списку сущностей
        /// </summary>
        private void SaveAndExit()
        {
            // Получение значений и подписей полей

            var workGuildId = WorkGuildIdLongUpDown.Value.ToString();
            var parcelId = ParcelIdLongUpDown.Value.ToString();
            var signMaterial = SignMaterialLongUpDown.Value.ToString();
            var unitValidation = NormUnitLongUpDown.Value.ToString();
            var auxiliaryMaterialConsumptionRate = AuxiliaryMaterialConsumptionRateTextBox.Text.Trim();
            var flowRate = FlowRateTextBox.Text.Trim();

            // Валидация полей
            var isValidFields = IsValidFieldsWithShowMessageOtherwise();
            if (!isValidFields)
            {
                return;
            }

            try
            {
                // нормализация параметров       
                var realWorkGuildId = workGuildId;
                while (realWorkGuildId.Length < 2)
                {
                    realWorkGuildId = "0" + realWorkGuildId;
                }

                var realParcelId = parcelId;
                while (realParcelId.Length < 2)
                {
                    realParcelId = "0" + realParcelId;
                }

                var realUnitValidation = unitValidation;
                while (realUnitValidation.Length < 4)
                {
                    realUnitValidation = "0" + realUnitValidation;
                }

                var realOldWorkGuildId = string.Empty;
                var realOldParcelId = string.Empty;

                // Если режим редактирования
                if (_editedMaterial != null)
                {
                    realOldWorkGuildId = _editedMaterial.WorkGuildId;
                    while (realOldWorkGuildId.Length < 2)
                    {
                        realOldWorkGuildId = "0" + realOldWorkGuildId;
                    }

                    realOldParcelId = _editedMaterial.ParcelId;
                    while (realOldParcelId.Length < 2)
                    {
                        realOldParcelId = "0" + realOldParcelId;
                    }

                }

                if (_editedMaterial == null || (realOldWorkGuildId != realWorkGuildId 
                                                || realOldParcelId != realParcelId))
                {
                    // Проверка на уникальность
                    var flag = Nu65Service.IsDublicate(_editedProduct.Id, _selectedMaterial.Id, realWorkGuildId,
                        realParcelId);
                    if (flag)
                    {
                        const string dublicatePattern = "Материал [{0}] с кодом цеха [{1}] и кодом участка [{2}] " +
                                                        "уже имеется у изделия [{3}]";
                        const string header = PageLiterals.HeaderInformationOrWarning;
                        var message = string.Format(dublicatePattern, _selectedMaterial.CodeMaterial, realWorkGuildId,
                            realParcelId, _editedProduct.DisplayCodeString);
                        const MessageBoxButton buttons = MessageBoxButton.OK;
                        const MessageBoxImage messageBoxType = MessageBoxImage.Exclamation;
                        MessageBox.Show(message, header, buttons, messageBoxType);
                        return;
                    }
                }

                var id = _editedMaterial == null
                    ? Nu65Service.Insert(_editedProduct.Id, _editedProduct.DisplayCodeString, _selectedMeasure.Id,
                        _selectedMeasure.DisplayOldDbCodeString, _selectedMaterial.Id,
                        _selectedMaterial.DisplayCodeString, auxiliaryMaterialConsumptionRate, realWorkGuildId,
                        signMaterial, realParcelId, realUnitValidation, DateTime.Today, flowRate)
                    : Nu65Service.Update(_editedMaterial.Nu65TableId, _editedProduct.Id, _editedProduct.DisplayCodeString,
                        _selectedMeasure.Id, _selectedMeasure.DisplayOldDbCodeString, _selectedMaterial.Id,
                        _selectedMaterial.DisplayCodeString, auxiliaryMaterialConsumptionRate, realWorkGuildId, signMaterial,
                        realParcelId, realUnitValidation, DateTime.Today, flowRate, realOldWorkGuildId,
                        realOldParcelId);

                _selectedMaterial.Nu65TableId = id != 0 ? id : _selectedMaterial.Nu65TableId;
                _selectedMaterial.AuxiliaryMaterialConsumptionRate = auxiliaryMaterialConsumptionRate;
                _selectedMaterial.WorkGuildId = realWorkGuildId;
                _selectedMaterial.SignMaterial = signMaterial;
                _selectedMaterial.ParcelId = realParcelId;
                _selectedMaterial.UnitValidation = realUnitValidation;
                _selectedMaterial.Date = DateTime.Today;
                _selectedMaterial.FlowRate = flowRate;
                _selectedMaterial.ServiceMappedMeasureId = _selectedMeasure.Id;
            }
            catch (StorageException ex)
            {
                Common.ShowDetailExceptionMessage(ex);
            }

            DialogResult = true;
            Close();
        }

        /// <summary>
		/// Валидация (проверка корректности) значений полей страницы, и вывод сообщения при некорректности
		/// </summary>
		private bool IsValidFieldsWithShowMessageOtherwise()
		{
		    var fieldWorkGuildId = WorkGuildIdLabel.Content.ToString();
		    var workGuildId = WorkGuildIdLongUpDown.Value.ToString();
		    var fieldParcelId = ParcelIdLabel.Content.ToString();
		    var parcelId = ParcelIdLongUpDown.Value.ToString();
		    var fieldSignMaterial = SignMaterialLabel.Content.ToString();
		    var signMaterial = SignMaterialLongUpDown.Value.ToString();
			var fieldUnitValidation = NormUnitLabel.Content.ToString();
			var unitValidation = NormUnitLongUpDown.Value.ToString();
			var fieldAuxiliaryMaterialConsumptionRate = AuxiliaryMaterialConsumptionRateLabel.Content.ToString();
		    var auxiliaryMaterialConsumptionRate = AuxiliaryMaterialConsumptionRateTextBox.Text.Trim();
            var fieldMaterial = MaterialLabel.Content.ToString();
            var fieldMeasure = MeasureIdlabel.Content.ToString();
		    var fieldFlowRate = FlowRatelabel.Content.ToString();
		    var flowRate = FlowRateTextBox.Text.Trim();


            var isValid = true;
			var errorMessages = new StringBuilder();
			isValid &= Validator.IsLineNotEmptyAndSizeNoMore(workGuildId, fieldWorkGuildId, 2, errorMessages);
			isValid &= Validator.IsLineNotEmptyAndSizeNoMore(parcelId, fieldParcelId, 2, errorMessages);
			isValid &= Validator.IsLineNotEmptyAndSizeNoMore(signMaterial, fieldSignMaterial, 1, errorMessages);
			isValid &= Validator.IsLineNotEmptyAndSizeNoMore(unitValidation, fieldUnitValidation, 4, errorMessages);
			isValid &= Validator.IsLineNotEmptyAndSizeNoMore(auxiliaryMaterialConsumptionRate, 
			    fieldAuxiliaryMaterialConsumptionRate, 11, errorMessages);
		    isValid &= Validator.IsNotNullSelectedObject(_selectedMaterial, fieldMaterial, errorMessages);
		    isValid &= Validator.IsNotNullSelectedObject(_selectedMeasure, fieldMeasure, errorMessages);
		    isValid &= Validator.IsLineMightEmptyAndSizeNoMore(flowRate, fieldFlowRate, 21, errorMessages);
            if (isValid)
			{
				return true;
			}
			const MessageBoxImage messageType = MessageBoxImage.Error;
			const MessageBoxButton messageButtons = MessageBoxButton.OK;
			const string validationHeader = PageLiterals.HeaderValidation;
			MessageBox.Show(errorMessages.ToString(), validationHeader, messageButtons, messageType);

			return false;
		}


		/// <summary>
		/// Нажатие кнопки [Сохранить]
		/// </summary>
		private void SaveButton_OnClick(object sender, RoutedEventArgs e)
		{
			SaveAndExit();
		}

		/// <summary>
		/// Событие изменения выбранной цены изделия/продукции - изменение связанных отображаемых полей на экране
		/// </summary>
		public void MaterialComboBox_OnSelectionChanged(object senderIsComboBox, SelectionChangedEventArgs e)
		{
            // Очистка (установка значение по-умолчанию) значения полей имени, ГОСТа и марки материала
            MaterialNameTextBox.Text = EmptySubstitutableValue;
            MaterialGostTextBox.Text = EmptySubstitutableValue;
		    MaterialProfileTextBox.Text = EmptySubstitutableValue;

		    // ReSharper disable once UsePatternMatching
            var comboBox = senderIsComboBox as ComboBox;
            if (comboBox == null)
            {
                return;
            }

            _selectedMaterial = comboBox.SelectedItem as Material;
            if (_selectedMaterial == null)
            {
                return;
            }

            // Установка значений имени, ГОСТа и марки материала
            MaterialNameTextBox.Text = _selectedMaterial.Name.ToString(CultureInfo.InvariantCulture);
            MaterialGostTextBox.Text = _selectedMaterial.Gost.ToString(CultureInfo.InvariantCulture);
            MaterialProfileTextBox.Text = _selectedMaterial.Profile.ToString(CultureInfo.InvariantCulture);
		}

		private void MeasureComboBox_OnSelectionChanged(object senderIsComboBox, SelectionChangedEventArgs e)
		{
			// Очистка (установка значение по-умолчанию) поля краткого наименования ед. измерения
			MeasureShortNameTextBox.Text = EmptySubstitutableValue;

		    // ReSharper disable once UsePatternMatching
			var comboBox = senderIsComboBox as ComboBox;
			if (comboBox == null)
			{
				return;
			}

			_selectedMeasure = comboBox.SelectedItem as Measure;
			if (_selectedMeasure == null)
			{
				return;
			}

			// Установка краткого наименования ед. измерения
			MeasureShortNameTextBox.Text = _selectedMeasure.ShortName.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Проверка, изменились ли поля ввода, и запрос подтверждения, если изменились. Далее выход к списку сущностей
		/// </summary>
		private void ConfirmExitIfDataHasBeenChanged()
		{
			var isFieldsNotChanged = true;

			const int defaultSelectedIndexComboBox = -1;

			if (_editedMaterial == null) // Если сущность добавляется
			{
				isFieldsNotChanged &= MaterialComboBox.SelectedIndex == defaultSelectedIndexComboBox;
				isFieldsNotChanged &= MeasureComboBox.SelectedIndex == defaultSelectedIndexComboBox;
				isFieldsNotChanged &= string.IsNullOrWhiteSpace(MaterialNameTextBox.Text);
				isFieldsNotChanged &= string.IsNullOrWhiteSpace(MaterialGostTextBox.Text);
				isFieldsNotChanged &= string.IsNullOrWhiteSpace(MaterialProfileTextBox.Text);
			    isFieldsNotChanged &= WorkGuildIdLongUpDown.Value == 0;
			    isFieldsNotChanged &= ParcelIdLongUpDown.Value == 0;
			    isFieldsNotChanged &= SignMaterialLongUpDown.Value == 0;
			    isFieldsNotChanged &= NormUnitLongUpDown.Value == 0;
                isFieldsNotChanged &= string.IsNullOrWhiteSpace(AuxiliaryMaterialConsumptionRateTextBox.Text);
				isFieldsNotChanged &= string.IsNullOrWhiteSpace(MeasureShortNameTextBox.Text);
			    isFieldsNotChanged &= string.IsNullOrWhiteSpace(FlowRateTextBox.Text);

                // проверка изменений внутренних полей класса
                isFieldsNotChanged &= _selectedMaterial == null;
			}
			else // Если сущность редактируется
			{
			    isFieldsNotChanged &= Equals(MeasureComboBox.SelectedItem as Measure, _editedMaterial.Measure);
			    isFieldsNotChanged &= Equals(WorkGuildIdLongUpDown.Text, _editedMaterial.WorkGuildId);
			    isFieldsNotChanged &= Equals(ParcelIdLongUpDown.Text, _editedMaterial.ParcelId);
			    isFieldsNotChanged &= Equals(SignMaterialLongUpDown.Text, _editedMaterial.SignMaterial);
			    isFieldsNotChanged &= Equals(NormUnitLongUpDown.Text, _editedMaterial.UnitValidation);
			    isFieldsNotChanged &= Equals(AuxiliaryMaterialConsumptionRateTextBox.Text.Trim(), _editedMaterial.AuxiliaryMaterialConsumptionRate);
			    isFieldsNotChanged &= Equals(FlowRateTextBox.Text.Trim(), _editedMaterial.FlowRate);

            }
            // Если введённые поля изменились - запрос у пользователя подтверждения намерение выхода к списку сущностей
            if (!isFieldsNotChanged && !PageUtil.ConfirmBackToListWhenFieldChanged())
			{
				return;
			}
			Close();
		}

        /// <summary>
        /// Получение выбранного материала
        /// </summary>
        public Material SelectedMaterial()
        {
            return _selectedMaterial;
        }

	    private void AddMaterialButton_OnClick(object sender, RoutedEventArgs e)
	    {
		    var addMaterialWindow = new AddMaterialWindow()
		    {
			    Owner = Window.GetWindow(this)
		    };
		    addMaterialWindow.ShowDialog();

            if (!addMaterialWindow.DialogResult.HasValue || addMaterialWindow.DialogResult != true)
            {
                return;
            }

            // Получение введённых пользователем параметров
            var material = addMaterialWindow.SelectedMaterial();
	        _searchMaterials.Add(material);

            ReloadMaterials();
        }

        /// <summary>
        /// Перезагруска списка продуктов
        /// </summary>
        private void ReloadMaterials()
        {
            MaterialComboBox.ItemsSource = null;
            MaterialComboBox.ItemsSource = _searchMaterials;
        }
    }	
}
