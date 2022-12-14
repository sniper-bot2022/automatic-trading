using System.Windows;
using System.Windows.Controls;

namespace TradeBot
{
    /// <summary>
    ///     Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            AddInstrumentSelectionTab();
        }

        private void AddInstrumentSelectionTab()
        {
            var newItem = new TabItem();
            newItem.Content = new InstrumentSelection(newItem);
            newItem.Header = "Instrument Selection";
            newItem.IsSelected = true;

            TabControl.Items.Insert(TabControl.Items.Count, newItem);
        }

        private void AddTabButton_Selected(object sender, RoutedEventArgs e)
        {
            AddInstrumentSelectionTab();
        }

        private void CloseTabButton_Selected(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (TabControl.Items.Count <= 1)
                AddInstrumentSelectionTab();

            TabControl.Items.Remove(btn.TemplatedParent);
        }
    }
}