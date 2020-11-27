using NorthwindDesktop.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NorthwindDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ListBox listboxCustomer;
        ObservableCollection<Customer> Customers;
        
        public MainWindow()
        {
            InitializeComponent();
            Customers = new ObservableCollection<Customer>();
            
            
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await GetCustomerAsync();
        }

        private async Task<ObservableCollection<Customer>> GetCustomerAsync()
        {
            
           
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://localhost:44388/api/customers/");
            if (response.IsSuccessStatusCode)
            {
                List<Customer> customersList = await response.Content.ReadAsAsync<List<Customer>>();
                Customers = new ObservableCollection<Customer>();
                customersList.ForEach(c=> Customers.Add(c));
                ListView1.ItemsSource = Customers;
            }
            return null;
        }

        private void StackPanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Window window = new Window();
            window.ShowDialog();
        }
    }
}
