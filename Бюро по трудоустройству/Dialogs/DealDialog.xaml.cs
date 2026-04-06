using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Бюро_по_трудоустройству.Models;

namespace Бюро_по_трудоустройству.Dialogs
{
    public partial class DealDialog : Window
    {
        public Deal Deal { get; private set; }

        public DealDialog(List<Employer> employers, List<JobSeeker> seekers, Deal deal = null)
        {
            InitializeComponent();

            EmployerComboBox.ItemsSource = employers;
            SeekerComboBox.ItemsSource = seekers;

            // Ограничение ввода только цифр для комиссионных
            CommissionBox.PreviewTextInput += CommissionBox_PreviewTextInput;

            if (deal != null)
            {
                Deal = deal;
                EmployerComboBox.SelectedItem = deal.Employer;
                SeekerComboBox.SelectedItem = deal.JobSeeker;
                PositionBox.Text = deal.Position;
                CommissionBox.Text = deal.Commission.ToString();
            }
            else
            {
                Deal = new Deal { Date = DateTime.Today };
            }
        }

        // Ограничение ввода только цифр для комиссионных
        private void CommissionBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmployerComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите работодателя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SeekerComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите соискателя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(PositionBox.Text))
            {
                MessageBox.Show("Введите должность!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                PositionBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(CommissionBox.Text))
            {
                MessageBox.Show("Введите сумму комиссионных!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                CommissionBox.Focus();
                return;
            }

            if (!int.TryParse(CommissionBox.Text, out int commission) || commission < 0)
            {
                MessageBox.Show("Введите корректную сумму комиссионных (только цифры)!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                CommissionBox.Focus();
                return;
            }

            Deal.Employer = (Employer)EmployerComboBox.SelectedItem;
            Deal.JobSeeker = (JobSeeker)SeekerComboBox.SelectedItem;
            Deal.Position = PositionBox.Text.Trim();
            Deal.Commission = commission;
            Deal.Date = DateTime.Today;

            DialogResult = true;
            Close();
        }
    }
}