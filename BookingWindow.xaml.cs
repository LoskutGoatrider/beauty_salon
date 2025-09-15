using beauty_salon.Base;
using System;
using System.Linq;
using System.Windows;

namespace beauty_salon
{
        /// <summary>
        /// Логика взаимодействия для BookingWindow.xaml
        /// </summary>
        public partial class BookingWindow : Window
        {
                private Service _service;

                public BookingWindow(Service service)
                {
                        InitializeComponent();
                        _service = service;
                        txtService.Text = service.Title;
                        LoadClients();
                        dpDate.SelectedDate = DateTime.Today;
                        cmbTime.SelectedIndex = 0;
                }

                private void LoadClients()
                {
                        using (var context = Helper.GetContext())
                        {
                                cmbClient.ItemsSource = context.Client.ToList();
                        }
                        if (cmbClient.Items.Count > 0)
                                cmbClient.SelectedIndex = 0;
                }
                private void BtnSave_Click(object sender, RoutedEventArgs e)
                {
                        if (cmbClient.SelectedItem == null)
                        {
                                MessageBox.Show("Выберите клиента");
                                return;
                        }

                        if (dpDate.SelectedDate == null)
                        {
                                MessageBox.Show("Выберите дату");
                                return;
                        }

                        var client = (Client)cmbClient.SelectedItem;
                        var date = dpDate.SelectedDate.Value;
                        var time = TimeSpan.Parse(((System.Windows.Controls.ComboBoxItem)cmbTime.SelectedItem).Content.ToString());

                        var startTime = date + time;
                        var endTime = startTime.AddSeconds(_service.DurationInSeconds);

                        if (startTime < DateTime.Now)
                        {
                                MessageBox.Show("Нельзя записывать на прошедшее время");
                                return;
                        }

                        using (var context = Helper.GetContext())
                        {
                                bool hasTimeConflict = context.ClientService.Any(cs =>
                                    cs.ServiceID == _service.ID &&
                                    ((cs.StartTime >= startTime && cs.StartTime < endTime) // ||
                                                                                           // (cs.EndTime > startTime && cs.EndTime <= endTime) ||
                                                                                           // (cs.StartTime <= startTime && cs.EndTime >= endTime))
                                        ));

                                if (hasTimeConflict)
                                {
                                        MessageBox.Show("В это время уже есть запись на эту услугу. Выберите другое время.");
                                        return;
                                }

                                bool hasClientConflict = context.ClientService.Any(cs =>
                                    cs.ClientID == client.ID &&
                                    ((cs.StartTime >= startTime && cs.StartTime < endTime) // ||
                                    // (cs.EndTime > startTime && cs.EndTime <= endTime) ||
                                   // (cs.StartTime <= startTime && cs.EndTime >= endTime)                                                       
                                        ));

                                if (hasClientConflict)
                                {
                                        MessageBox.Show("У этого клиента уже есть запись на это время. Выберите другое время.");
                                        return;
                                }

                                var clientService = new ClientService
                                {
                                        ClientID = client.ID,
                                        ServiceID = _service.ID,
                                        StartTime = startTime // ,
                                        // EndTime = endTime
                                };

                                context.ClientService.Add(clientService);
                                context.SaveChanges();

                                MessageBox.Show("Клиент успешно записан на услугу");
                                DialogResult = true;
                                Close();
                        }
                }

                private void BtnCancel_Click(object sender, RoutedEventArgs e)
                {

                }
        }
}