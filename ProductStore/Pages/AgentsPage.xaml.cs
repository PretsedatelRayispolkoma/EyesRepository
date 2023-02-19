using ProductStore.DBContext;
using ProductStore.Pages.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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

namespace ProductStore.Pages
{
    /// <summary>
    /// Interaction logic for AgentsPage.xaml
    /// </summary>
    public partial class AgentsPage : Page
    {
        private List<AgentStat> _agentStats = new List<AgentStat>();
        private List<AgentStat> _sortedStats = new List<AgentStat>();

        private Dictionary<string, List<ProductSale>> _salesDict = new Dictionary<string, List<ProductSale>>();
        private Dictionary<string, List<ProductSale>> _sortedSalesDict = new Dictionary<string, List<ProductSale>>();

        public AgentsPage()
        {
            InitializeComponent();

            var filterByList = MainWindow.db.AgentType.ToList();
            filterByList.Add(new AgentType { ID = 0, Title = "Все типы" });
            FilterByDateCb.ItemsSource = filterByList;
            FilterByDateCb.DisplayMemberPath = "Title";
            FilterByDateCb.SelectedItem = filterByList.Where(i => i.ID == 0).FirstOrDefault();

            SortByDateCb.ItemsSource = new List<string>
            {
                "По умолчанию",
                "Наименование (от а до я)",
                "Наименование (от я до а)",
                "Размер скидки (по возрастанию)",
                "Размер скидки (по убыванию)"
            };

            SortByDateCb.SelectedItem = "По умолчанию";

            List<ProductSale> productsales = MainWindow.db.ProductSale.ToList();

            foreach(var item in productsales)
            {
                string key = $"{item.Agent.Title}|{item.Agent.Phone}|{item.Agent.AgentType.Title}|{item.AgentID}|/Images/{item.Agent.Logo}";
                if(_salesDict.ContainsKey(key))
                {
                    _salesDict.TryGetValue(key, out var list);
                    list.Add(item);
                }
                else
                {
                    var list = new List<ProductSale>
                    {
                        item
                    };
                    _salesDict.Add(key, list);
                }
            }

            UpdateList();
        }

         private void UpdateList()
        {
            _sortedSalesDict.Clear();
            _agentStats.Clear();

            foreach(var item in _salesDict)
            {
                var list = item.Value.Where(i => i.SaleDate.Year == DateTime.Now.Year).ToList();
                _sortedSalesDict.Add(item.Key, list);
            }

            foreach(var item in _sortedSalesDict)
            {
                string[] values = item.Key.Split('|');

                int sales = 0;

                foreach(var sale in item.Value)
                {
                    sales += sale.ProductCount;
                }

                double discount = 0;

                if (sales > 10000 && sales < 50000)
                    discount = 0.05;
                if (sales > 50000 && sales < 150000)
                    discount = 0.1;
                if (sales > 150000 && sales < 500000)
                    discount = 0.2;
                if (sales > 500000)
                    discount = 0.25;

                _agentStats.Add(new AgentStat(values[0], sales, discount, values[1], values[2], values[4], Convert.ToInt32(values[3])));
            }

            switch(SortByDateCb.SelectedItem)
            {
                case "Наименование (от а до я)":
                    _sortedStats = _agentStats.OrderBy(p => p.AgentName).ToList();
                    break;
                case "Наименование (от я до а)":
                    _sortedStats = _agentStats.OrderByDescending(p => p.AgentName).ToList();
                    break;
                case "Размер скидки (по возрастанию)":
                    _sortedStats = _agentStats.OrderBy(p => p.Discount).ToList();
                    break;
                case "Размер скидки (по убыванию)":
                    _sortedStats = _agentStats.OrderByDescending(p => p.Discount).ToList();
                    break;
                default:
                    _sortedStats = _agentStats.ToList();
                    break;
            }

            var selectedType = FilterByDateCb.SelectedItem as AgentType;
            if (selectedType.ID != 0)
                _sortedStats = _sortedStats.Where(p => p.TypeOfAgent == selectedType.Title).ToList();

            _sortedStats = _sortedStats.Where(p => p.AgentName.ToLower().Contains(SearchTb.Text.ToLower())).ToList();

            WorkloadLv.ItemsSource = _sortedStats.ToList();
            WorkloadLv.Items.Refresh();

        }

        private class AgentStat
        {
            public int Id { get; set; }
            public string AgentName { get; set; }
            public int CountOfSales { get; set; }
            public double Discount { get; set; }
            public string Phone { get; set; }
            public string TypeOfAgent { get; set; }

            public string ImagesSource { get; set; }
            public string DiscountColor { get; set; }

            public AgentStat(string agentName, int sales, double discount,
                string phone, string typeOfAgent, string source, int id)
            {
                AgentName = agentName;
                CountOfSales = sales;
                Discount = discount;
                Phone = phone;
                TypeOfAgent = typeOfAgent;
                if (source == "/Images/")
                    ImagesSource = "/Images/picture.png";
                else
                    ImagesSource = source;
                Id = id;

                if (discount >= 0.25)
                    DiscountColor = "LightGreen";
            }
        }

        private void SearchTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateList();
        }

        private void FilterByDateCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateList();
        }

        private void SortByDateCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateList();
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            var agent = (sender as Button).DataContext as AgentStat;

            if(agent != null)
            {
                var agn = MainWindow.db.Agent.Where(a => a.ID == agent.Id).FirstOrDefault();
                if(agn != null)
                {
                    UpdateAgentWindow updateAgent = new UpdateAgentWindow(agn);
                    if(updateAgent.ShowDialog() == true)
                    {
                        UpdateList();
                    }
                }
            }
        }

        private void AddAgentBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateAgentWindow updateAgent = new UpdateAgentWindow();
            if(updateAgent.ShowDialog() == true)
            {
                UpdateList();
            }
        }
    }
}
