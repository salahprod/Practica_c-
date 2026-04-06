using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Бюро_по_трудоустройству.Models;

namespace Бюро_по_трудоустройству.Dialogs
{
    public partial class JobSeekerDialog : Window
    {
        public JobSeeker JobSeeker { get; private set; }

        public JobSeekerDialog(JobSeeker jobSeeker = null)
        {
            InitializeComponent();

            if (jobSeeker != null)
            {
                JobSeeker = jobSeeker;
                LoadJobSeekerData();
            }
            else
            {
                JobSeeker = new JobSeeker();
            }
        }

        private void LoadJobSeekerData()
        {
            LastNameBox.Text = JobSeeker.LastName;
            FirstNameBox.Text = JobSeeker.FirstName;
            PatronymicBox.Text = JobSeeker.Patronymic;
            QualificationBox.Text = JobSeeker.Qualification;
            ActivityTypeBox.Text = JobSeeker.ActivityType;
            ExpectedSalaryBox.Text = JobSeeker.ExpectedSalary.ToString();
            OtherInfoBox.Text = JobSeeker.OtherInfo;
        }

        // Ограничение ввода только цифр для зарплаты
        private void ExpectedSalaryBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        // Проверка, что строка содержит только буквы и пробелы
        private bool IsOnlyLetters(string text)
        {
            return !string.IsNullOrWhiteSpace(text) && Regex.IsMatch(text, @"^[а-яА-ЯёЁa-zA-Z\s\-]+$");
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка фамилии
            if (string.IsNullOrWhiteSpace(LastNameBox.Text))
            {
                MessageBox.Show("Введите фамилию!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                LastNameBox.Focus();
                return;
            }

            if (!IsOnlyLetters(LastNameBox.Text))
            {
                MessageBox.Show("Фамилия должна содержать только буквы!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                LastNameBox.Focus();
                return;
            }

            // Проверка имени
            if (string.IsNullOrWhiteSpace(FirstNameBox.Text))
            {
                MessageBox.Show("Введите имя!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                FirstNameBox.Focus();
                return;
            }

            if (!IsOnlyLetters(FirstNameBox.Text))
            {
                MessageBox.Show("Имя должно содержать только буквы!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                FirstNameBox.Focus();
                return;
            }

            // Проверка отчества (необязательно, но если введено, то только буквы)
            if (!string.IsNullOrWhiteSpace(PatronymicBox.Text))
            {
                if (!IsOnlyLetters(PatronymicBox.Text))
                {
                    MessageBox.Show("Отчество должно содержать только буквы!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    PatronymicBox.Focus();
                    return;
                }
            }

            // Проверка квалификации
            if (string.IsNullOrWhiteSpace(QualificationBox.Text))
            {
                MessageBox.Show("Введите квалификацию!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                QualificationBox.Focus();
                return;
            }

            // Проверка сферы деятельности
            if (string.IsNullOrWhiteSpace(ActivityTypeBox.Text))
            {
                MessageBox.Show("Введите желаемую сферу деятельности!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                ActivityTypeBox.Focus();
                return;
            }

            // Проверка зарплаты (только цифры)
            if (string.IsNullOrWhiteSpace(ExpectedSalaryBox.Text))
            {
                MessageBox.Show("Введите ожидаемую зарплату!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                ExpectedSalaryBox.Focus();
                return;
            }

            if (!int.TryParse(ExpectedSalaryBox.Text, out int salary) || salary <= 0)
            {
                MessageBox.Show("Введите корректную сумму зарплаты (только цифры)!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                ExpectedSalaryBox.Focus();
                return;
            }

            if (salary > 10000000)
            {
                if (MessageBox.Show($"Зарплата {salary:N0} руб. превышает 10 миллионов. Это корректно?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    ExpectedSalaryBox.Focus();
                    return;
                }
            }

            // Заполнение данных
            JobSeeker.LastName = LastNameBox.Text.Trim();
            JobSeeker.FirstName = FirstNameBox.Text.Trim();
            JobSeeker.Patronymic = PatronymicBox.Text.Trim();
            JobSeeker.Qualification = QualificationBox.Text.Trim();
            JobSeeker.ActivityType = ActivityTypeBox.Text.Trim();
            JobSeeker.ExpectedSalary = salary;
            JobSeeker.OtherInfo = OtherInfoBox.Text.Trim();

            DialogResult = true;
            Close();
        }
    }
}