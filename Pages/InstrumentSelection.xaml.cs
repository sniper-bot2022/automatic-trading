using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Tinkoff.Trading.OpenApi.Models;

namespace TradeBot
{
    /// <summary>
    ///     Логика взаимодействия для StockSelection.xaml
    /// </summary>
    public partial class InstrumentSelection : UserControl
    {
        private readonly TabItem parent;
        private List<string> instrumentsLabels;
        private MarketInstrumentList instruments;

        public InstrumentSelection(TabItem parent)
        {
            InitializeComponent();

            this.parent = parent; 

            Dispatcher.Invoke(() => StockRadioButton.IsChecked = true);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var activeInstrument =
                    instruments.Instruments[instrumentsLabels.FindIndex(v => v == TickerComboBox.Text)];

                parent.Header = activeInstrument.Name;

                if (RealTimeRadioButton.IsChecked == true)
                {
                    parent.Header += " (Real-Time)";
                    parent.Content = new RealTimeTrading(activeInstrument);
                }
                else if (TestingRadioButton.IsChecked == true)
                {
                    parent.Header += " (Testing)";
                    parent.Content = new TestingTrading(activeInstrument);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TickerComboBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (TickerComboBox.ItemsSource == null)
                return;

            var tb = (TextBox)e.OriginalSource;
            if (tb.SelectionStart != 0)
                TickerComboBox.SelectedItem = null;

            SelectButton.IsEnabled = TickerComboBox.SelectedItem != null;

            if (TickerComboBox.SelectedItem != null) return;
            var cv = (CollectionView)CollectionViewSource.GetDefaultView(TickerComboBox.ItemsSource);
            cv.Filter = s =>
                ((string)s).IndexOf(TickerComboBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;

            TickerComboBox.IsDropDownOpen = cv.Count > 0;
            tb.SelectionLength = 0;
            tb.SelectionStart = tb.Text.Length;
        }

        private async void EtfRadioButton_OnChecked(object sender, RoutedEventArgs e)
        {
            instruments = await TinkoffInterface.Context.MarketEtfsAsync();
            if (instruments == null)
            {
                MessageBox.Show("Failed to get list of ETFs");
                return;
            }
            instrumentsLabels = instruments.Instruments.ConvertAll(v => $"{v.Ticker} - {v.Name}");
            TickerComboBox.ItemsSource = instrumentsLabels;

        }

        private async void StockRadioButton_OnChecked(object sender, RoutedEventArgs e)
        {
            instruments = await TinkoffInterface.Context.MarketStocksAsync();
            if (instruments == null)
            {
                MessageBox.Show("Failed to get list of stocks");
                return;
            }
            instrumentsLabels = instruments.Instruments.ConvertAll(v => $"{v.Ticker} - {v.Name}");
            TickerComboBox.ItemsSource = instrumentsLabels;
        }

        private async void CurrencyRadioButton_OnChecked(object sender, RoutedEventArgs e)
        {
            instruments = await TinkoffInterface.Context.MarketCurrenciesAsync();
            if (instruments == null)
            {
                MessageBox.Show("Failed to get list of currencies");
                return;
            }
            instrumentsLabels = instruments.Instruments.ConvertAll(v => $"{v.Ticker} - {v.Name}");
            TickerComboBox.ItemsSource = instrumentsLabels;
        }

        private void TickerComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TickerComboBox.ItemsSource == null)
                return;
            var cv = (CollectionView)CollectionViewSource.GetDefaultView(TickerComboBox.ItemsSource);
            TickerComboBox.IsDropDownOpen = cv.Count > 0;
        }

        private void RandomizeImage_Click(object sender, RoutedEventArgs e)
        {
            if (TickerComboBox.ItemsSource == null)
                return;
            var rnd = new Random();
            TickerComboBox.SelectedIndex = rnd.Next(TickerComboBox.Items.Count);
        }
    }
}