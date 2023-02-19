using Microsoft.Win32;
using ProductStore.DBContext;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProductStore.Pages.Windows
{
    /// <summary>
    /// Interaction logic for UpdateAgentWindow.xaml
    /// </summary>
    public partial class UpdateAgentWindow : Window
    {
        private readonly Agent _agent;

        public bool IsClosed { get; set; } = false;

        public UpdateAgentWindow()
        {
            InitializeComponent();

            TypesCb.ItemsSource = MainWindow.db.AgentType.ToList();
            TypesCb.DisplayMemberPath = "Title";

            AddBtn.Visibility = Visibility.Visible;
            DeleteBtn.Visibility = Visibility.Collapsed;
            UpdateBtn.Visibility = Visibility.Collapsed;
        }

        public UpdateAgentWindow(Agent agent)
        {
            InitializeComponent();
            _agent = MainWindow.db.Agent.Attach(agent);

            TypesCb.ItemsSource = MainWindow.db.AgentType.ToList();
            TypesCb.DisplayMemberPath = "Title";

            TitleTb.Text = agent.Title;
            TypesCb.SelectedItem = agent.AgentType;
            AddressTb.Text = agent.Address;
            InnTb.Text = agent.INN;
            KppTb.Text = agent.KPP;
            DirectorTb.Text = agent.DirectorName;
            PhoneTb.Text = agent.Phone;
            EmailTb.Text = agent.Email;

            AddBtn.Visibility = Visibility.Collapsed;
            DeleteBtn.Visibility = Visibility.Visible;
            UpdateBtn.Visibility = Visibility.Visible;
        }

        private void ImageBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if(TitleTb.Text == string.Empty || AddressTb.Text == string.Empty 
                || InnTb.Text == string.Empty || KppTb.Text == string.Empty 
                || DirectorTb.Text == string.Empty || PhoneTb.Text == string.Empty
                || EmailTb.Text == string.Empty)
            {
                MessageBox.Show("Enter the data");
                return;
            }

            var selectedType = TypesCb.SelectedItem as AgentType;
            if(selectedType == null)
            {
                MessageBox.Show("Choose an agent type");
                return;
            }

            Agent agent = new Agent();
            agent.Title = TitleTb.Text;
            agent.Address = AddressTb.Text;
            agent.INN = InnTb.Text;
            agent.KPP = KppTb.Text;
            agent.DirectorName = DirectorTb.Text;
            agent.Phone = PhoneTb.Text;
            agent.Email = EmailTb.Text;
            agent.AgentTypeID = selectedType.ID;
            agent.Priority = 1;

            try
            {
                MainWindow.db.Agent.Add(agent);
                MainWindow.db.SaveChanges();
            }
            catch
            {
                MessageBox.Show("Error");
            }
            DialogResult = true;
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TitleTb.Text == string.Empty || AddressTb.Text == string.Empty
                || InnTb.Text == string.Empty || KppTb.Text == string.Empty
                || DirectorTb.Text == string.Empty || PhoneTb.Text == string.Empty
                || EmailTb.Text == string.Empty)
            {
                MessageBox.Show("Enter the data");
                return;
            }

            var selectedType = TypesCb.SelectedItem as AgentType;
            if (selectedType == null)
            {
                MessageBox.Show("Choose an agent type");
                return;
            }

            _agent.Title = TitleTb.Text;
            _agent.Address = AddressTb.Text;
            _agent.INN = InnTb.Text;
            _agent.KPP = KppTb.Text;
            _agent.DirectorName = DirectorTb.Text;
            _agent.Phone = PhoneTb.Text;
            _agent.Email = EmailTb.Text;
            _agent.AgentTypeID = selectedType.ID;
            _agent.Priority = 1;

            try
            {
                MainWindow.db.SaveChanges();
            }
            catch
            {
                MessageBox.Show("Error");
            }
            DialogResult = true;
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show($"Удалить?", "Удаление", MessageBoxButton.YesNo);

                if(result == MessageBoxResult.Yes)
                {
                    MainWindow.db.Agent.Remove(_agent);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            DialogResult = true;
        }
    }
}
