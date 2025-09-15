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
        /// Логика взаимодействия для PasswordDialog.xaml
        /// </summary>
        public partial class PasswordDialog : Window
        {
                public string Password { get; private set; }

                public PasswordDialog()
                {
                        InitializeComponent();
                }

                private void OkButton_Click(object sender, RoutedEventArgs e)
                {
                        Password = PasswordBox.Password;
                        DialogResult = true;
                }

                private void CancelButton_Click(object sender, RoutedEventArgs e)
                {
                        DialogResult = false;
                }
        }
}