using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

using NU65.Util;
using NU65.View.Pages;
using NU65.View.Pages.TableView;
using NU65.View.Util;

namespace NU65.View.Menus
{
    /// <summary>
    /// Логика взаимодействия c боковым меню
    /// </summary>
    /// <inheritdoc cref="UserControl" />
    public partial class SideMenu
    {
        public static SideMenu Instance { get; private set; }
        public SideMenu()
        {
            Instance = this;
            InitializeComponent();
            VisualInitializeComponent();
        }

        /// <summary>
        /// Визуальная инициализация меню (цвета и размеры шрифтов контролов)
        /// </summary>
        private void VisualInitializeComponent()
        {
            // Экспандеры, вложенные в них StackPanel и вложенные в них Buttons
            var expanders = WrapperStackPanel.Children.OfType<Expander>();
            foreach (var expander in expanders)
            {
                expander.Background = Constants.BackColor4_BlueBayoux;
                expander.BorderBrush = Constants.LineBorderColor2_Nepal;
                expander.Foreground = Constants.ForeColor2_PapayaWhip;
                expander.FontSize = Constants.FontSize;

                var stackPanel = expander.Content as StackPanel;
                if (stackPanel == null)
                {
                    continue;
                }

                stackPanel.Background = Constants.BackColor4_BlueBayoux;

                var buttons = new List<Button>();
                buttons.AddRange(stackPanel.Children.OfType<Button>());
                foreach (var button in buttons)
                {
                    button.Foreground = Constants.ForeColor1_BigStone;
                }
            }
        }

        /// <summary>
        /// Выбор пункта меню [Работа с Nu65]
        /// </summary>
        private void WorkingPartNu65Button_OnClick(object sender, RoutedEventArgs e)
        {
            PageSwitcher.Switch(new WorkingPartNu65());
        }

        /// <summary>
        /// Выбор пункта меню "Справочники MsSQL" [Материалы]
        /// </summary>
        private void MssqlMaterialButton_OnClick(object sender, RoutedEventArgs e)
        {
            var typeDb = "s";
            PageSwitcher.Switch(new MaterialsTable(typeDb));
        }

        /// <summary>
        /// Выбор пункта меню "Справочники MsSQL" [Единицы измерения]
        /// </summary>
        private void MssqlMeasureButton_OnClick(object sender, RoutedEventArgs e)
        {
            PageSwitcher.Switch(new MeasuresTable());
        }

        /// <summary>
        /// Выбор пункта меню "Справочники MsSQL" [Nu65]
        /// </summary>
        private void MssqlNu65Button_OnClick(object sender, RoutedEventArgs e)
        {
            PageSwitcher.Switch(new Nu65Table());
        }

        /// <summary>
        /// Выбор пункта меню "Справочники MsSQL" [База изделий]
        /// </summary>
        private void MssqlProductBaseButton_OnClick(object sender, RoutedEventArgs e)
        {
            var typeDb = "s";
            PageSwitcher.Switch(new ProductsTable(typeDb));
        }

        /// <summary>
        /// Выбор пункта меню "Справочники FoxPro" [База изделий]
        /// </summary>
        private void FoxProMaterialButton_OnClick(object sender, RoutedEventArgs e)
        {
            var typeDb = "l";
            PageSwitcher.Switch(new MaterialsTable(typeDb));
        }

        /// <summary>
        /// Выбор пункта меню "Справочники FoxPro" [Материалы]
        /// </summary>
        private void FoxProProductButton_OnClick(object sender, RoutedEventArgs e)
        {
            var typeDb = "l";
            PageSwitcher.Switch(new ProductsTable(typeDb));
        }

        /// <summary>
        /// Выбор пункта меню [Скопировать материал в изделия]
        /// </summary>
	    private void CopyMaterialToProductsButton_OnClick(object sender, RoutedEventArgs e)
        {
            PageSwitcher.Switch(new CopyMaterialToProducts());
        }
    }
}
