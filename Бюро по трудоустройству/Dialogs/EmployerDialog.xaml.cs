using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Бюро_по_трудоустройству.Models;

namespace Бюро_по_трудоустройству.Dialogs
{
    public partial class EmployerDialog : Window
    {
        public Employer Employer { get; private set; }

        public EmployerDialog(Employer employer = null)
        {
            InitializeComponent();

            if (employer != null)
            {
                Employer = employer;
                LoadEmployerData();
            }
            else
            {
                Employer = new Employer();
            }
        }

        private void LoadEmployerData()
        {
            NameBox.Text = Employer.Name;
            ActivityTypeBox.Text = Employer.ActivityType;
            AddressBox.Text = Employer.Address;
            PhoneBox.Text = Employer.Phone;
        }

        // Ограничение ввода только цифр для телефона
        private void PhoneBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите название организации!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                NameBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(ActivityTypeBox.Text))
            {
                MessageBox.Show("Введите вид деятельности!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                ActivityTypeBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(AddressBox.Text))
            {
                MessageBox.Show("Введите адрес!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                AddressBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneBox.Text))
            {
                MessageBox.Show("Введите телефон!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                PhoneBox.Focus();
                return;
            }

            if (PhoneBox.Text.Length < 10)
            {
                MessageBox.Show("Телефон должен содержать минимум 10 цифр!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                PhoneBox.Focus();
                return;
            }

            Employer.Name = NameBox.Text.Trim();
            Employer.ActivityType = ActivityTypeBox.Text.Trim();
            Employer.Address = AddressBox.Text.Trim();
            Employer.Phone = PhoneBox.Text.Trim();

            DialogResult = true;
            Close();
        }
    }
}