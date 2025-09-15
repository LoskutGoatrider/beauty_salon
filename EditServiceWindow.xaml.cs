using beauty_salon.Base;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace beauty_salon
{
        /// <summary>
        /// Логика взаимодействия для EditServiceWindow.xaml
        /// </summary>
        public partial class EditServiceWindow : Window
        {
                private Service _currentService;
                private string _imagePath;
                private bool _isEditMode;

                public EditServiceWindow(Service service = null)
                {
                        InitializeComponent();

                        _currentService = service;
                        _isEditMode = service != null;

                        if (_isEditMode)
                        {
                                Title = "Редактирование услуги";
                                LoadServiceData();
                        }
                        else
                        {
                                Title = "Добавление услуги";
                                txtId.Text = "Автоматически";
                        }
                }

                private void LoadServiceData()
                {
                        txtId.Text = _currentService.ID.ToString();
                        txtTitle.Text = _currentService.Title;
                        txtCost.Text = _currentService.Cost.ToString();
                        txtDuration.Text = (TimeSpan.FromSeconds(_currentService.DurationInSeconds).TotalMinutes).ToString();
                        txtDescription.Text = _currentService.Description;
                        txtDiscount.Text = _currentService.Discount?.ToString() ?? "0";

                        if (!string.IsNullOrEmpty(_currentService.MainImagePath))
                        {
                                _imagePath = _currentService.MainImagePath;
                                txtImagePath.Text = Path.GetFileName(_imagePath);
                                imgPreview.Source = new BitmapImage(new Uri(_imagePath));
                        }
                }

                private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
                {
                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

                        if (openFileDialog.ShowDialog() == true)
                        {
                                _imagePath = openFileDialog.FileName;
                                txtImagePath.Text = Path.GetFileName(_imagePath);
                                imgPreview.Source = new BitmapImage(new Uri(_imagePath));
                        }
                }

                private void BtnSave_Click(object sender, RoutedEventArgs e)
                {
                        // Валидация данных
                        if (string.IsNullOrWhiteSpace(txtTitle.Text))
                        {
                                MessageBox.Show("Введите название услуги.");
                                return;
                        }

                        if (!decimal.TryParse(txtCost.Text, out decimal cost) || cost < 0)
                        {
                                MessageBox.Show("Введите корректную стоимость.");
                                return;
                        }

                        if (!int.TryParse(txtDuration.Text, out int durationMinutes) || durationMinutes <= 0 || durationMinutes > 240)
                        {
                                MessageBox.Show("Длительность должна быть от 1 до 240 минут.");
                                return;
                        }

                        if (!double.TryParse(txtDiscount.Text, out double discount) || discount < 0 || discount > 1)
                        {
                                MessageBox.Show("Скидка должна быть в диапазоне от 0 до 1.");
                                return;
                        }

                        using (var context = Helper.GetContext())
                        {
                                if (_isEditMode)
                                {
                                        if (context.Service.Any(s => s.Title == txtTitle.Text && s.ID != _currentService.ID))
                                        {
                                                MessageBox.Show("Услуга с таким названием уже существует.");
                                                return;
                                        }
                                }
                                else
                                {
                                        if (context.Service.Any(s => s.Title == txtTitle.Text))
                                        {
                                                MessageBox.Show("Услуга с таким названием уже существует.");
                                                return;
                                        }
                                }

                                string imagePathToSave = null;
                                if (!string.IsNullOrEmpty(_imagePath))
                                {
                                        string projectPath = AppDomain.CurrentDomain.BaseDirectory;
                                        string imagesDirectory = Path.Combine(projectPath, "Images");

                                        if (!Directory.Exists(imagesDirectory))
                                        {
                                                Directory.CreateDirectory(imagesDirectory);
                                        }

                                        string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(_imagePath);
                                        string destinationPath = Path.Combine(imagesDirectory, newFileName);

                                        File.Copy(_imagePath, destinationPath, true);
                                        imagePathToSave = destinationPath;
                                }

                                if (_isEditMode)
                                {
                                        var service = context.Service.Find(_currentService.ID);
                                        service.Title = txtTitle.Text;
                                        service.Cost = cost;
                                        service.DurationInSeconds = durationMinutes * 60;
                                        service.Description = txtDescription.Text;
                                        service.Discount = discount;

                                        if (!string.IsNullOrEmpty(imagePathToSave))
                                        {
                                                service.MainImagePath = imagePathToSave;
                                        }
                                }
                                else
                                {
                                        Service newService = new Service
                                        {
                                                Title = txtTitle.Text,
                                                Cost = cost,
                                                DurationInSeconds = durationMinutes * 60,
                                                Description = txtDescription.Text,
                                                Discount = discount,
                                                MainImagePath = imagePathToSave
                                        };

                                        context.Service.Add(newService);
                                }

                                context.SaveChanges();
                                MessageBox.Show("Данные сохранены успешно.");
                                DialogResult = true;
                                Close();
                        }
                }

                private void BtnCancel_Click(object sender, RoutedEventArgs e)
                {
                        DialogResult = false;
                        Close();
                }
        }
}