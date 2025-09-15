using System;
using System.Collections.Generic;
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

namespace beauty_salon
{
        /// <summary>
        /// Логика взаимодействия для BookingsViewWindow.xaml
        /// </summary>
        public partial class BookingsViewWindow : Window
        {
                public BookingsViewWindow()
                {
                        InitializeComponent();
                        LoadBookings();
                }

                private void LoadBookings()
                {
                        using (var context = Helper.GetContext())
                        {
                                dgBookings.ItemsSource = context.ClientService
                                    .Include("Client")
                                    .Include("Service")
                                    .ToList();
                        }
                }
        }
}
