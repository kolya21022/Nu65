using System.Collections.Generic;
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
using NU65.View.Pages.TableView;

namespace NU65.View.Pages.Edit
{
	/// <summary>
	/// Страница редактирования/добавления [Единиц измерения]
	/// </summary>
	/// <inheritdoc cref="Page" />
	public partial class MeasureEdit : IPageable
	{
        /// <summary>
        /// Список [Единиц измерения]
        /// </summary>
		private List<Measure> _measuresList;

		/// <summary>
		/// Конструктор режима добавления
		/// </summary>
		/// <inheritdoc />
		public MeasureEdit(List<Measure> measuresList)
		{
			_measuresList = measuresList;
			InitializeComponent();
			AdditionalInitializeComponent();
			VisualInitializeComponent();
		}

		/// <summary>
		/// Указание значений полей при редактировании и указание соответсвующих режиму надписей.
		/// </summary>
		/// <inheritdoc />
		public void AdditionalInitializeComponent()
		{
			EditingTypeLabel.Content = PageLiterals.EditPageTypeAdd;
			IdTextBox.Focus();
		}

		/// <summary>
		/// Визуальная инициализация страницы (цвета и размеры шрифтов контролов)
		/// </summary>
		/// <inheritdoc />
		public void VisualInitializeComponent()
		{
			FontSize = Constants.FontSize;
			FieldsWrapperGrid.Background = Constants.BackColor1_AthensGray;

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
			const string jumpNext = PageLiterals.HotkeyLabelJumpNext;
			const string closePageBackToList = PageLiterals.HotkeyLabelClosePageBackToList;
			const string separator = PageLiterals.HotkeyLabelsSeparator;
			const string displayed = jumpNext + separator + closePageBackToList;
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
			ConfirmExitIfDataHasBeenChanged(); // Если нажат [Esc] - проверка изменений полей и запрос подтверждения
		}

		/// <summary>
		/// Перевод клавиатурного фокуса к следующему полю ввода при нажатии Enter.
		/// </summary>
		private void JumpToNextWhenPressEnter_OnKeyDown(object senderIsTextBox, KeyEventArgs eventArgs)
		{
			PageUtil.JumpToNextWhenPressEnter_OnKeyDown(eventArgs);
		}

		/// <summary>
		/// Нажатие кнопки [Сохранить]
		/// </summary>
		private void SaveButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			SaveAndExit();
		}

		/// <summary>
		/// Нажатие кнопки [Отмена (Выйти к списку)]
		/// </summary>
		private void CancelButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			ConfirmExitIfDataHasBeenChanged(); // проверка изменений полей и запрос подтверждения выхода, если изменены
		}

		/// <summary>
		/// Валидация, сохранение и выход к списку сущностей
		/// </summary>
		private void SaveAndExit()
		{
			// Получение значений и подписей полей
			var name = NameTextBox.Text.Trim();
			var shortName = ShortNameTextBox.Text.Trim();
			var fieldShortName = ShortNameLabel.Content.ToString();
			var fieldName = NameLabel.Content.ToString();
			var idMeasure = IdTextBox.Value;

			// Валидация полей
			var isValidFields = IsValidFieldsWithShowMessageOtherwise();
			if (!isValidFields)
			{
				return;
			}
			// Если краткое название ед. изм. содержит некиррилические символы - получение подтверждение пользователя
			if (!Validator.IsCyrillicWithUserConfirmOtherwise(shortName, fieldShortName))
			{
				return;
			}

			// Если название ед. изм. содержит некиррилические символы - получение подтверждение пользователя
			if (!Validator.IsCyrillicWithUserConfirmOtherwise(name, fieldName))
			{
				return;
			}

            // Проверка на дубликат
		    foreach (var measure in _measuresList)
		    {
		        if (measure.OldDbCode == idMeasure)
		        {
		            const string message = "Такой код уже имеется!";
		            const string header = "Ошибка валидации";
		            MessageBox.Show(message, header, MessageBoxButton.OK, MessageBoxImage.Error);
		            return;
		        }
		    }

            try
            {
                // Такое не произайдет из за валидации, но анализатор Resharper ругается
                if (idMeasure == null)
                {
                    return;
                }

                MeasuresService.Insert((long) idMeasure, shortName, name);
            }
			catch (StorageException ex)
			{
				Common.ShowDetailExceptionMessage(ex);
			}

			PageSwitcher.Switch(new MeasuresTable());
		}

		/// <summary>
		/// Валидация (проверка корректности) значений полей страницы, и вывод сообщения при некорректности
		/// </summary>
		private bool IsValidFieldsWithShowMessageOtherwise()
		{
			var name = NameTextBox.Text.Trim();
			var shortName = ShortNameTextBox.Text.Trim();
			var fieldShortName = ShortNameLabel.Content.ToString();
			var fieldName = NameLabel.Content.ToString();
			var fieldIdMeasure = IdLabel.Content.ToString();
			var idMeasure = IdTextBox.Text.Trim();

			var isValid = true;
			var errorMessages = new StringBuilder();
			isValid &= Validator.IsLineNotEmptyAndSizeNoMore(shortName, fieldShortName, 11, errorMessages);
			isValid &= Validator.IsLineNotEmptyAndSizeNoMore(name, fieldName, 20, errorMessages);
			isValid &= Validator.IsLineNotEmptyAndSizeNoMore(idMeasure, fieldIdMeasure, 3, errorMessages);
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
		/// Проверка, изменились ли поля ввода, и запрос подтверждения, если изменились. Далее выход к списку сущностей
		/// </summary>
		private void ConfirmExitIfDataHasBeenChanged()
		{
			var isFieldsNotChanged = true;

				isFieldsNotChanged &= string.IsNullOrWhiteSpace(ShortNameTextBox.Text);
				isFieldsNotChanged &= string.IsNullOrWhiteSpace(NameTextBox.Text);
				isFieldsNotChanged &= string.IsNullOrWhiteSpace(IdTextBox.Text);

			// Если введённые поля изменились - запрос у пользователя подтверждения намерение выхода к списку сущностей
			if (!isFieldsNotChanged && !PageUtil.ConfirmBackToListWhenFieldChanged())
			{
				return;
			}
			PageSwitcher.Switch(new MeasuresTable());
		}
	}
}
