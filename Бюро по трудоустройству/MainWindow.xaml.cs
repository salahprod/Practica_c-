using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Бюро_по_трудоустройству.Models;
using Бюро_по_трудоустройству.Data;
using Бюро_по_трудоустройству.Dialogs;

namespace Бюро_по_трудоустройству
{
    public partial class MainWindow : Window
    {
        private List<Employer> employers;
        private List<JobSeeker> seekers;
        private List<Deal> deals;
        private XmlDataService dataService;

        private List<Employer> filteredEmployers;
        private List<JobSeeker> filteredSeekers;
        private List<Deal> filteredDeals;

        public MainWindow()
        {
            InitializeComponent();
            dataService = new XmlDataService();

            employers = new List<Employer>();
            seekers = new List<JobSeeker>();
            deals = new List<Deal>();

            filteredEmployers = new List<Employer>();
            filteredSeekers = new List<JobSeeker>();
            filteredDeals = new List<Deal>();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                employers = dataService.LoadEmployers();
                seekers = dataService.LoadSeekers();
                deals = dataService.LoadDeals(employers, seekers);

                filteredEmployers = new List<Employer>(employers);
                filteredSeekers = new List<JobSeeker>(seekers);
                filteredDeals = new List<Deal>(deals);

                RefreshEmployersGrid();
                RefreshSeekersGrid();
                RefreshDealsGrid();
                UpdateStatisticsTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveAll()
        {
            try
            {
                dataService.SaveEmployers(employers);
                dataService.SaveSeekers(seekers);
                dataService.SaveDeals(deals);
                UpdateStatisticsTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshEmployersGrid()
        {
            EmployersGrid.ItemsSource = null;
            EmployersGrid.ItemsSource = filteredEmployers;
        }

        private void RefreshSeekersGrid()
        {
            JobSeekersGrid.ItemsSource = null;
            JobSeekersGrid.ItemsSource = filteredSeekers;
        }

        private void RefreshDealsGrid()
        {
            DealsGrid.ItemsSource = null;
            DealsGrid.ItemsSource = filteredDeals;
        }

        private void UpdateStatisticsTab()
        {
            try
            {
                TotalEmployersText.Text = $"🏢 Всего работодателей: {employers.Count}";
                TotalSeekersText.Text = $"👤 Всего соискателей: {seekers.Count}";
                TotalDealsText.Text = $"💰 Всего сделок: {deals.Count}";
                TotalIncomeText.Text = $"💵 Общий доход: {deals.Sum(d => d.Commission):N2} руб.";

                var topEmployers = deals.GroupBy(d => d.Employer?.Name)
                    .Where(g => g.Key != null)
                    .Select(g => new { Name = g.Key, Total = g.Sum(d => d.Commission) })
                    .OrderByDescending(x => x.Total)
                    .Take(5)
                    .Select(x => $"🏆 {x.Name} - {x.Total:N2} руб.")
                    .ToList();

                if (topEmployers.Any())
                    TopEmployersList.ItemsSource = topEmployers;
                else
                    TopEmployersList.ItemsSource = new List<string> { "Нет данных" };

                var recentDeals = deals.OrderByDescending(d => d.Date).Take(5)
                    .Select(d => $"📅 {d.Date:dd.MM.yyyy} | {d.JobSeeker?.FullName} → {d.Employer?.Name} | {d.Position} | {d.Commission:N2} руб.")
                    .ToList();

                if (recentDeals.Any())
                    RecentDealsList.ItemsSource = recentDeals;
                else
                    RecentDealsList.ItemsSource = new List<string> { "Нет сделок" };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка обновления статистики: {ex.Message}");
            }
        }

        // ========== ПОИСК ==========

        private void ApplyEmployerFilter()
        {
            string searchText = EmployerSearchBox.Text?.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(searchText))
                filteredEmployers = new List<Employer>(employers);
            else
                filteredEmployers = employers.Where(e =>
                    (e.Name?.ToLower().Contains(searchText) ?? false) ||
                    (e.ActivityType?.ToLower().Contains(searchText) ?? false) ||
                    (e.Address?.ToLower().Contains(searchText) ?? false) ||
                    (e.Phone?.ToLower().Contains(searchText) ?? false)).ToList();
            RefreshEmployersGrid();
        }

        private void EmployerSearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyEmployerFilter();
        private void ClearEmployerSearchBtn_Click(object sender, RoutedEventArgs e) { EmployerSearchBox.Text = ""; ApplyEmployerFilter(); }

        private void ApplySeekerFilter()
        {
            string searchText = SeekerSearchBox.Text?.Trim().ToLower();
            string filterField = (SeekerSearchFilter.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrWhiteSpace(searchText))
                filteredSeekers = new List<JobSeeker>(seekers);
            else
            {
                switch (filterField)
                {
                    case "Фамилия":
                        filteredSeekers = seekers.Where(s => (s.LastName?.ToLower().Contains(searchText) ?? false)).ToList();
                        break;
                    case "Имя":
                        filteredSeekers = seekers.Where(s => (s.FirstName?.ToLower().Contains(searchText) ?? false)).ToList();
                        break;
                    case "Квалификация":
                        filteredSeekers = seekers.Where(s => (s.Qualification?.ToLower().Contains(searchText) ?? false)).ToList();
                        break;
                    default:
                        filteredSeekers = seekers.Where(s =>
                            (s.LastName?.ToLower().Contains(searchText) ?? false) ||
                            (s.FirstName?.ToLower().Contains(searchText) ?? false) ||
                            (s.Patronymic?.ToLower().Contains(searchText) ?? false) ||
                            (s.Qualification?.ToLower().Contains(searchText) ?? false) ||
                            (s.ActivityType?.ToLower().Contains(searchText) ?? false)).ToList();
                        break;
                }
            }
            RefreshSeekersGrid();
        }

        private void SeekerSearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplySeekerFilter();
        private void SeekerSearchFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplySeekerFilter();
        private void ClearSeekerSearchBtn_Click(object sender, RoutedEventArgs e) { SeekerSearchBox.Text = ""; SeekerSearchFilter.SelectedIndex = 0; ApplySeekerFilter(); }

        private void ApplyDealFilter()
        {
            string searchText = DealSearchBox.Text?.Trim().ToLower();
            string filterField = (DealSearchFilter.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrWhiteSpace(searchText))
                filteredDeals = new List<Deal>(deals);
            else
            {
                switch (filterField)
                {
                    case "Соискатель":
                        filteredDeals = deals.Where(d => (d.JobSeeker?.FullName?.ToLower().Contains(searchText) ?? false)).ToList();
                        break;
                    case "Работодатель":
                        filteredDeals = deals.Where(d => (d.Employer?.Name?.ToLower().Contains(searchText) ?? false)).ToList();
                        break;
                    default:
                        filteredDeals = deals.Where(d =>
                            (d.JobSeeker?.FullName?.ToLower().Contains(searchText) ?? false) ||
                            (d.Employer?.Name?.ToLower().Contains(searchText) ?? false) ||
                            (d.Position?.ToLower().Contains(searchText) ?? false)).ToList();
                        break;
                }
            }
            RefreshDealsGrid();
        }

        private void DealSearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyDealFilter();
        private void DealSearchFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyDealFilter();
        private void ClearDealSearchBtn_Click(object sender, RoutedEventArgs e) { DealSearchBox.Text = ""; DealSearchFilter.SelectedIndex = 0; ApplyDealFilter(); }

        // ========== ДОБАВЛЕНИЕ ==========

        private void AddEmployerBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EmployerDialog();
            dialog.Owner = this;
            if (dialog.ShowDialog() == true)
            {
                int newId = employers.Count > 0 ? employers.Max(emp => emp.Id) + 1 : 1;
                dialog.Employer.Id = newId;
                employers.Add(dialog.Employer);
                ApplyEmployerFilter();
                SaveAll();
                MessageBox.Show("Работодатель добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddSeekerBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new JobSeekerDialog();
            dialog.Owner = this;
            if (dialog.ShowDialog() == true)
            {
                int newId = seekers.Count > 0 ? seekers.Max(s => s.Id) + 1 : 1;
                dialog.JobSeeker.Id = newId;
                seekers.Add(dialog.JobSeeker);
                ApplySeekerFilter();
                SaveAll();
                MessageBox.Show("Соискатель добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddDealBtn_Click(object sender, RoutedEventArgs e)
        {
            if (employers.Count == 0)
            {
                MessageBox.Show("Добавьте работодателей!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (seekers.Count == 0)
            {
                MessageBox.Show("Добавьте соискателей!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new DealDialog(employers, seekers);
            dialog.Owner = this;
            if (dialog.ShowDialog() == true)
            {
                int newId = deals.Count > 0 ? deals.Max(d => d.Id) + 1 : 1;
                dialog.Deal.Id = newId;
                deals.Add(dialog.Deal);
                ApplyDealFilter();
                SaveAll();
                MessageBox.Show("Сделка оформлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ========== ОБЩАЯ КНОПКА ИЗМЕНИТЬ ==========

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            // Определяем какая вкладка активна
            var selectedTab = MainTabControl.SelectedItem as TabItem;
            string tabHeader = selectedTab?.Header?.ToString() ?? "";

            if (tabHeader.Contains("Работодатели"))
            {
                if (EmployersGrid.SelectedItem is Employer selectedEmployer)
                {
                    var dialog = new EmployerDialog(selectedEmployer);
                    dialog.Owner = this;
                    if (dialog.ShowDialog() == true)
                    {
                        selectedEmployer.Name = dialog.Employer.Name;
                        selectedEmployer.ActivityType = dialog.Employer.ActivityType;
                        selectedEmployer.Address = dialog.Employer.Address;
                        selectedEmployer.Phone = dialog.Employer.Phone;
                        ApplyEmployerFilter();
                        SaveAll();
                        MessageBox.Show("Данные работодателя обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Выберите работодателя для редактирования!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (tabHeader.Contains("Соискатели"))
            {
                if (JobSeekersGrid.SelectedItem is JobSeeker selectedSeeker)
                {
                    var dialog = new JobSeekerDialog(selectedSeeker);
                    dialog.Owner = this;
                    if (dialog.ShowDialog() == true)
                    {
                        selectedSeeker.LastName = dialog.JobSeeker.LastName;
                        selectedSeeker.FirstName = dialog.JobSeeker.FirstName;
                        selectedSeeker.Patronymic = dialog.JobSeeker.Patronymic;
                        selectedSeeker.Qualification = dialog.JobSeeker.Qualification;
                        selectedSeeker.ActivityType = dialog.JobSeeker.ActivityType;
                        selectedSeeker.ExpectedSalary = dialog.JobSeeker.ExpectedSalary;
                        selectedSeeker.OtherInfo = dialog.JobSeeker.OtherInfo;
                        ApplySeekerFilter();
                        SaveAll();
                        MessageBox.Show("Данные соискателя обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Выберите соискателя для редактирования!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (tabHeader.Contains("Сделки"))
            {
                if (DealsGrid.SelectedItem is Deal selectedDeal)
                {
                    var dialog = new DealDialog(employers, seekers, selectedDeal);
                    dialog.Owner = this;
                    if (dialog.ShowDialog() == true)
                    {
                        selectedDeal.Employer = dialog.Deal.Employer;
                        selectedDeal.JobSeeker = dialog.Deal.JobSeeker;
                        selectedDeal.Position = dialog.Deal.Position;
                        selectedDeal.Commission = dialog.Deal.Commission;
                        selectedDeal.Date = dialog.Deal.Date;
                        ApplyDealFilter();
                        SaveAll();
                        MessageBox.Show("Сделка обновлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Выберите сделку для редактирования!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        // ========== УДАЛЕНИЕ ==========

        private void DeleteSelectedBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedTab = MainTabControl.SelectedItem as TabItem;
            string tabHeader = selectedTab?.Header?.ToString() ?? "";

            if (tabHeader.Contains("Работодатели"))
            {
                var selected = EmployersGrid.SelectedItems.Cast<Employer>().ToList();
                if (!selected.Any())
                {
                    MessageBox.Show("Выберите работодателей для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show($"Удалить {selected.Count} работодатель(ей)?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    foreach (var item in selected) employers.Remove(item);
                    deals.RemoveAll(d => selected.Select(x => x.Id).Contains(d.Employer.Id));
                    ApplyEmployerFilter();
                    ApplyDealFilter();
                    SaveAll();
                    MessageBox.Show("Удалено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else if (tabHeader.Contains("Соискатели"))
            {
                var selected = JobSeekersGrid.SelectedItems.Cast<JobSeeker>().ToList();
                if (!selected.Any())
                {
                    MessageBox.Show("Выберите соискателей для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show($"Удалить {selected.Count} соискатель(ей)?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    foreach (var item in selected) seekers.Remove(item);
                    deals.RemoveAll(d => selected.Select(x => x.Id).Contains(d.JobSeeker.Id));
                    ApplySeekerFilter();
                    ApplyDealFilter();
                    SaveAll();
                    MessageBox.Show("Удалено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else if (tabHeader.Contains("Сделки"))
            {
                var selected = DealsGrid.SelectedItems.Cast<Deal>().ToList();
                if (!selected.Any())
                {
                    MessageBox.Show("Выберите сделки для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show($"Удалить {selected.Count} сделок?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    foreach (var item in selected) deals.Remove(item);
                    ApplyDealFilter();
                    SaveAll();
                    MessageBox.Show("Удалено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ClearAllBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("ВНИМАНИЕ! Вы действительно хотите удалить ВСЕ данные?\nЭто действие нельзя отменить!",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                employers.Clear();
                seekers.Clear();
                deals.Clear();

                filteredEmployers.Clear();
                filteredSeekers.Clear();
                filteredDeals.Clear();

                RefreshEmployersGrid();
                RefreshSeekersGrid();
                RefreshDealsGrid();
                SaveAll();

                MessageBox.Show("Все данные удалены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ========== ОТЧЁТЫ И СТАТИСТИКА ==========

        private void ReportBtn_Click(object sender, RoutedEventArgs e)
        {
            if (deals.Count == 0)
            {
                MessageBox.Show("Нет сделок для отчёта!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            decimal totalIncome = deals.Sum(d => d.Commission);
            string message = $"📊 ФИНАНСОВЫЙ ОТЧЁТ 📊\n\n" +
                $"━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                $"📈 Общее количество сделок: {deals.Count}\n" +
                $"💰 Общий доход: {totalIncome:N2} руб.\n" +
                $"📊 Средний доход за сделку: {totalIncome / deals.Count:N2} руб.\n" +
                $"━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                $"🏆 Лучший результат: {deals.Max(d => d.Commission):N2} руб.\n" +
                $"📉 Худший результат: {deals.Min(d => d.Commission):N2} руб.";

            MessageBox.Show(message, "Финансовый отчёт", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV файлы (*.csv)|*.csv";
                saveFileDialog.Title = "Экспорт данных";
                saveFileDialog.FileName = $"Отчёт_{DateTime.Now:yyyyMMdd_HHmmss}";

                if (saveFileDialog.ShowDialog() == true)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Тип,ID,Название/ФИО,Дополнительная информация");

                    foreach (var emp in employers)
                    {
                        sb.AppendLine($"Работодатель,{emp.Id},{emp.Name},{emp.ActivityType}");
                    }

                    foreach (var seek in seekers)
                    {
                        sb.AppendLine($"Соискатель,{seek.Id},{seek.FullName},{seek.Qualification}");
                    }

                    foreach (var deal in deals)
                    {
                        sb.AppendLine($"Сделка,{deal.Id},{deal.JobSeeker?.FullName} → {deal.Employer?.Name},{deal.Position} | {deal.Commission:N2} руб.");
                    }

                    System.IO.File.WriteAllText(saveFileDialog.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show($"Данные экспортированы!\nФайл: {saveFileDialog.FileName}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            MessageBox.Show("Данные обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void TopEmployersBtn_Click(object sender, RoutedEventArgs e)
        {
            var topEmployers = deals.GroupBy(d => d.Employer?.Name)
                .Where(g => g.Key != null)
                .Select(g => new { Name = g.Key, Total = g.Sum(d => d.Commission), Count = g.Count() })
                .OrderByDescending(x => x.Total)
                .ToList();

            if (!topEmployers.Any())
            {
                MessageBox.Show("Нет данных для отображения!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string message = "🏆 ТОП РАБОТОДАТЕЛЕЙ 🏆\n\n";
            for (int i = 0; i < Math.Min(5, topEmployers.Count); i++)
            {
                message += $"{i + 1}. {topEmployers[i].Name}\n";
                message += $"   💰 Доход: {topEmployers[i].Total:N2} руб.\n";
                message += $"   📊 Сделок: {topEmployers[i].Count}\n\n";
            }

            MessageBox.Show(message, "Топ работодателей", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void StatsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (deals.Count == 0)
            {
                MessageBox.Show("Нет данных для статистики!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var bestDeal = deals.OrderByDescending(d => d.Commission).FirstOrDefault();
            var worstDeal = deals.OrderBy(d => d.Commission).FirstOrDefault();

            string message = "📊 РАСШИРЕННАЯ СТАТИСТИКА 📊\n\n";
            message += $"━━━━━━━━━━━━━━━━━━━━━━━━━━━\n";
            message += $"📈 Всего сделок: {deals.Count}\n";
            message += $"💰 Общий доход: {deals.Sum(d => d.Commission):N2} руб.\n";
            message += $"📊 Средний доход: {deals.Average(d => d.Commission):N2} руб.\n";
            message += $"━━━━━━━━━━━━━━━━━━━━━━━━━━━\n";
            message += $"🏆 Лучшая сделка:\n";
            message += $"   {bestDeal?.JobSeeker?.FullName} → {bestDeal?.Employer?.Name}\n";
            message += $"   💵 {bestDeal?.Commission:N2} руб.\n";
            message += $"━━━━━━━━━━━━━━━━━━━━━━━━━━━\n";
            message += $"📉 Худшая сделка:\n";
            message += $"   {worstDeal?.JobSeeker?.FullName} → {worstDeal?.Employer?.Name}\n";
            message += $"   💵 {worstDeal?.Commission:N2} руб.\n";
            message += $"━━━━━━━━━━━━━━━━━━━━━━━━━━━\n";
            message += $"👥 Всего работодателей: {employers.Count}\n";
            message += $"👤 Всего соискателей: {seekers.Count}";

            MessageBox.Show(message, "Расширенная статистика", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}