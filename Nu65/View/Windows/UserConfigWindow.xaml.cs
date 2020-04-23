using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using NU65.Util;
using NU65.View.Util;

namespace NU65.View.Windows
{
	/// <summary>
	/// Окно пользовательских настроек.
	/// </summary>
	/// <inheritdoc cref="Window" />
	public partial class UserConfigWindow 
	{
		public UserConfigWindow()
		{
			InitializeComponent();
			AdditionalInitializeComponent();
			VisualInitializeComponent();
		}

		/// <summary>
		/// Визуальная инициализация окна (цвета и размеры шрифтов контролов)
		/// </summary>
		private void VisualInitializeComponent()
		{
			FontSize = Constants.FontSize;
			Background = Constants.BackColor5_WaikawaGray;
			Foreground = Constants.ForeColor2_PapayaWhip;

			// Цвета Labels и TextBlocks
			var mainLabels = FieldsWrapperGrid.Children.OfType<Label>();
			foreach (var label in mainLabels)
			{
				label.Foreground = Constants.ForeColor2_PapayaWhip;
			}
			FontSizeLabel.Foreground = Constants.ForeColor2_PapayaWhip;
			IsRunFullscreenTextBlock.Foreground = Constants.ForeColor2_PapayaWhip;

			// Фоны
			BackgroundRectangle.Fill = Constants.BackColor3_SanJuan;
			HotkeysStackPanel.Background = Constants.BackColor4_BlueBayoux;

			// Панель хоткеев
			var helpLabels = HotkeysStackPanel.Children.OfType<Label>();
			foreach (var helpLabel in helpLabels)
			{
				helpLabel.Foreground = Constants.ForeColor2_PapayaWhip;
			}
		}

		/// <summary>
		/// Получение и отображение значений пользовательских параметров в нужные поля ввода и надписей хоткеев
		/// </summary>
		private void AdditionalInitializeComponent()
		{
			const string closeWindowHotkey = PageLiterals.HotkeyLabelCloseWindow;
			HotkeysTextBlock.Text = closeWindowHotkey;

			var sqlServerNu65= Properties.Settings.Default.SqlServerNu65;
			var foxproDbFolderSkl = Properties.Settings.Default.FoxproDbFolder_Skl;
			var foxproDbFoldeFox60ArmBase = Properties.Settings.Default.FoxproDbFolder_Fox60_Arm_Base;

			var isRunInFullscreen = Properties.Settings.Default.IsRunInFullscreen;
			var fontSize = Properties.Settings.Default.FontSize;

		    SqlServerNu65TextBox.Text = sqlServerNu65;
			FoxproDbFolderSklTextBox.Text = foxproDbFolderSkl;
			FoxproDbFolderFox60ArmBaseTextBox.Text = foxproDbFoldeFox60ArmBase;
			IsRunFullscreenCheckBox.IsChecked = isRunInFullscreen;

			FontSizeDoubleUpDown.Minimum = 8D;
			FontSizeDoubleUpDown.Value = fontSize;
			FontSizeDoubleUpDown.Maximum = 30D;

		}
		private void Window_OnPreviewEscapeKeyDownCloseWindow(object senderIsWindow, KeyEventArgs eventArgs)
		{
			if (eventArgs.Key != Key.Escape)
			{
				return;
			}
			eventArgs.Handled = true;
			Close();
		}

		/// <summary>
		/// Нажатие кнопки [Отмена (Закрыть окно)]
		/// </summary>
		private void CloseButton_OnClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void SaveButton_OnClick(object sender, RoutedEventArgs e)
		{
			// Получение названий полей и значений
			var sqlServerNu65Label = SqlServerNu65Label.Content.ToString();
			var sqlServerNu65 = SqlServerNu65TextBox.Text.Trim();
			var foxproDbFolderSklLabel = FoxproDbFolderSklLabel.Content.ToString();
			var foxproDbFolderSkl = FoxproDbFolderSklTextBox.Text.Trim();
			var foxproDbFolderFox60ArmBaseLabel = FoxproDbFolderFox60ArmBaseLabel.Content.ToString();
			var foxproDbFolderFox60ArmBase = FoxproDbFolderFox60ArmBaseTextBox.Text.Trim();


			var isRunInFullscreen = IsRunFullscreenCheckBox.IsChecked ?? false;

			var fontSizeLabel = FontSizeLabel.Content.ToString();
			var nullableFontSize = FontSizeDoubleUpDown.Value;

			// Валидация на пустоту
			var isValid = true;
			var errorMessage = string.Empty;
			var messagePattern = "Поле [{0}] пустое / не указано" + Environment.NewLine;

			isValid &= !string.IsNullOrWhiteSpace(sqlServerNu65);
			errorMessage += string.IsNullOrWhiteSpace(sqlServerNu65)
				? string.Format(messagePattern, sqlServerNu65Label)
				: string.Empty;
			isValid &= !string.IsNullOrWhiteSpace(foxproDbFolderSkl);
			errorMessage += string.IsNullOrWhiteSpace(foxproDbFolderSkl)
				? string.Format(messagePattern, foxproDbFolderSklLabel)
				: string.Empty;

			isValid &= !string.IsNullOrWhiteSpace(foxproDbFolderFox60ArmBase);
			errorMessage += string.IsNullOrWhiteSpace(foxproDbFolderFox60ArmBase)
				? string.Format(messagePattern, foxproDbFolderFox60ArmBaseLabel)
				: string.Empty;

			isValid &= nullableFontSize != null;
			errorMessage += nullableFontSize == null ? string.Format(messagePattern, fontSizeLabel) : string.Empty;

			if (!isValid) // Если какое-то из полей не указано
			{
				const MessageBoxImage messageBoxType = MessageBoxImage.Error;
				const MessageBoxButton messageBoxButtons = MessageBoxButton.OK;
				MessageBox.Show(errorMessage, PageLiterals.HeaderValidation, messageBoxButtons, messageBoxType);
				return;
			}

			// Сохранение параметров в пользовательский config-файл этой версии приложения и закрытие окна
			// Ориентировочный путь: [ c:\Users\Username\AppData\Local\OJSC_GZSU\ProductStockManager_Url... ]

			var fontSize = (double)nullableFontSize;

			Properties.Settings.Default.SqlServerNu65 = sqlServerNu65;
			Properties.Settings.Default.FoxproDbFolder_Skl = foxproDbFolderSkl;
			Properties.Settings.Default.FoxproDbFolder_Fox60_Arm_Base = foxproDbFolderFox60ArmBase;
			Properties.Settings.Default.IsRunInFullscreen = isRunInFullscreen;
			Properties.Settings.Default.FontSize = fontSize;

			Properties.Settings.Default.Save();
			Close();
		}
	}
}
