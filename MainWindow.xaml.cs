using beauty_salon.Base;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace beauty_salon
{
        /// <summary>
        /// Логика взаимодействия для MainWindow.xaml
        /// </summary>
        public partial class MainWindow : Window
        {
                private List<Service> allServices;
                private bool isAdminMode = false;
                private static EditServiceWindow _currentEditWindow = null;
                private static BookingWindow _currentBookingWindow = null;
                private static BookingsViewWindow _currentBookingsViewWindow = null;

                public MainWindow()
                {
                        InitializeComponent();
                        LoadServices();
                        InitializeComboBoxes();
                        UpdateAdminUI();
                }

                private void LoadServices()
                {
                        using (var context = Helper.GetContext())
                        {
                                allServices = context.Service.ToList();
                        }
                        ApplyFilters();
                }

                private void InitializeComboBoxes()
                {
                        cmbSorting.ItemsSource = new List<string>
            {
                "По возрастанию цены",
                "По убыванию цены"
            };
                        cmbSorting.SelectedIndex = 0;

                        cmbFilter.ItemsSource = new List<string>
            {
                "Все",
                "0-5%",
                "5-15%",
                "15-30%",
                "30-70%",
                "70-100%"
            };
                        cmbFilter.SelectedIndex = 0;
                }

                private void ApplyFilters()
                {
                        if (allServices == null) return;

                        var filteredServices = allServices.AsEnumerable();

                        if (!string.IsNullOrEmpty(txtSearch.Text))
                        {
                                string searchText = txtSearch.Text.ToLower();
                                filteredServices = filteredServices.Where(s =>
                                    s.Title.ToLower().Contains(searchText) ||
                                    (s.Description != null && s.Description.ToLower().Contains(searchText)));
                        }

                        if (cmbFilter.SelectedItem != null && cmbFilter.SelectedItem.ToString() != "Все")
                        {
                                var filter = cmbFilter.SelectedItem.ToString();
                                double min = 0, max = 0;

                                if (filter == "0-5%")
                                {
                                        min = 0;
                                        max = 0.05;
                                }
                                else if (filter == "5-15%")
                                {
                                        min = 0.05;
                                        max = 0.15;
                                }
                                else if (filter == "15-30%")
                                {
                                        min = 0.15;
                                        max = 0.3;
                                }
                                else if (filter == "30-70%")
                                {
                                        min = 0.3;
                                        max = 0.7;
                                }
                                else if (filter == "70-100%")
                                {
                                        min = 0.7;
                                        max = 1.01; // Добавляем 0.01 чтобы включить 100%
                                }

                                filteredServices = filteredServices.Where(s =>
                                    s.Discount.HasValue && s.Discount >= min && s.Discount < max);
                        }

                        if (cmbSorting.SelectedItem != null)
                        {
                                if (cmbSorting.SelectedItem.ToString() == "По возрастанию цены")
                                        filteredServices = filteredServices.OrderBy(s => s.Cost);
                                else if (cmbSorting.SelectedItem.ToString() == "По убыванию цены")
                                        filteredServices = filteredServices.OrderByDescending(s => s.Cost);
                        }

                        LViewProduct.ItemsSource = filteredServices.ToList();
                        txtResultAmount.Text = filteredServices.Count().ToString();
                        txtAllAmount.Text = allServices.Count.ToString();
                }

                private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
                {
                        ApplyFilters();
                }

                private void CmbSorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
                {
                        ApplyFilters();
                }

                private void CmbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
                {
                        ApplyFilters();
                }

                private void BtnAdmin_Click(object sender, RoutedEventArgs e)
                {
                        var passwordDialog = new PasswordDialog();
                        if (passwordDialog.ShowDialog() == true && passwordDialog.Password == "0000")
                        {
                                isAdminMode = true;
                                btnAdmin.Content = "Режим администратора (активен)";
                                btnAdmin.Background = System.Windows.Media.Brushes.Green;
                                UpdateAdminUI();
                        }
                        else
                        {
                                MessageBox.Show("Неверный код администратора");
                        }
                }

                private void UpdateAdminUI()
                {
                        // Показываем/скрываем элементы интерфейса администратора
                        contextMenu.Visibility = isAdminMode ? Visibility.Visible : Visibility.Collapsed;

                        // Обновляем видимость панели администратора через привязку данных
                        this.DataContext = new { IsAdminMode = isAdminMode };
                }

                private void BtnEditService_Click(object sender, RoutedEventArgs e)
                {
                        if (!isAdminMode) return;

                        if (LViewProduct.SelectedItem is Service selectedService)
                        {
                                if (_currentEditWindow != null)
                                {
                                        MessageBox.Show("Окно редактирования уже открыто.");
                                        return;
                                }

                                _currentEditWindow = new EditServiceWindow(selectedService);
                                _currentEditWindow.Closed += (s, args) => { _currentEditWindow = null; };

                                if (_currentEditWindow.ShowDialog() == true)
                                {
                                        LoadServices();
                                }
                        }
                        else
                        {
                                MessageBox.Show("Выберите услугу для редактирования");
                        }
                }

                private void BtnDeleteService_Click(object sender, RoutedEventArgs e)
                {
                        if (!isAdminMode) return;

                        if (LViewProduct.SelectedItem is Service selectedService)
                        {
                                if (MessageBox.Show("Вы уверены, что хотите удалить эту услугу?",
                                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                                {
                                        using (var context = Helper.GetContext())
                                        {
                                                if (context.ClientService.Any(cs => cs.ServiceID == selectedService.ID))
                                                {
                                                        MessageBox.Show("Нельзя удалить услугу, на которую есть записи.");
                                                        return;
                                                }

                                                var images = context.ServicePhoto.Where(sp => sp.ServiceID == selectedService.ID);
                                                context.ServicePhoto.RemoveRange(images);

                                                var serviceToDelete = context.Service.Find(selectedService.ID);
                                                if (serviceToDelete != null)
                                                {
                                                        context.Service.Remove(serviceToDelete);
                                                        context.SaveChanges();
                                                        MessageBox.Show("Услуга удалена.");
                                                        LoadServices();
                                                }
                                        }
                                }
                        }
                        else
                        {
                                MessageBox.Show("Выберите услугу для удаления");
                        }
                }

                private void BtnBookService_Click(object sender, RoutedEventArgs e)
                {
                        if (!isAdminMode) return;

                        if (LViewProduct.SelectedItem is Service selectedService)
                        {
                                if (_currentBookingWindow != null)
                                {
                                        MessageBox.Show("Окно записи уже открыто.");
                                        return;
                                }

                                _currentBookingWindow = new BookingWindow(selectedService);
                                _currentBookingWindow.Closed += (s, args) => { _currentBookingWindow = null; };
                                _currentBookingWindow.ShowDialog();
                        }
                        else
                        {
                                MessageBox.Show("Выберите услугу для записи клиента");
                        }
                }

                private void BtnAddService_Click(object sender, RoutedEventArgs e)
                {
                        if (!isAdminMode) return;

                        if (_currentEditWindow != null)
                        {
                                MessageBox.Show("Окно редактирования уже открыто.");
                                return;
                        }

                        _currentEditWindow = new EditServiceWindow();
                        _currentEditWindow.Closed += (s, args) => { _currentEditWindow = null; };

                        if (_currentEditWindow.ShowDialog() == true)
                        {
                                LoadServices();
                        }
                }

                private void BtnViewBookings_Click(object sender, RoutedEventArgs e)
                {
                        if (!isAdminMode) return;

                        if (_currentBookingsViewWindow != null)
                        {
                                MessageBox.Show("Окно просмотра записей уже открыто.");
                                return;
                        }

                        _currentBookingsViewWindow = new BookingsViewWindow();
                        _currentBookingsViewWindow.Closed += (s, args) => { _currentBookingsViewWindow = null; };
                        _currentBookingsViewWindow.Show();
                }

                private void LViewProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
                {
                        // Обработка изменения выбора
                }
        }
}