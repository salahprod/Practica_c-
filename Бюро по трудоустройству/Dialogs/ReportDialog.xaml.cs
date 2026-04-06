using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Бюро_по_трудоустройству.Models;

namespace Бюро_по_трудоустройству.Dialogs
{
    public partial class ReportDialog : Window
    {
        public ReportDialog(List<Deal> deals, List<JobSeeker> seekers)
        {
            InitializeComponent();
            GenerateReport(deals, seekers);
        }

        private void GenerateReport(List<Deal> deals, List<JobSeeker> seekers)
        {
            if (deals.Any())
            {
                int totalDeals = deals.Count;
                decimal totalIncome = deals.Sum(d => d.Commission);
                decimal averageIncome = totalIncome / totalDeals;

                TotalDealsText.Text = totalDeals.ToString();
                TotalIncomeText.Text = $"{totalIncome:N2} руб.";
                AverageIncomeText.Text = $"{averageIncome:N2} руб.";

                if (seekers.Any())
                {
                    decimal averageSalary = seekers.Average(s => s.ExpectedSalary);
                    AverageSalaryText.Text = $"{averageSalary:N2} руб.";
                }
                else
                {
                    AverageSalaryText.Text = "Нет данных";
                }

                var employerStats = deals.GroupBy(d => d.Employer.Name)
                    .Select(g => new EmployerStatistic
                    {
                        EmployerName = g.Key,
                        DealCount = g.Count(),
                        TotalIncome = g.Sum(d => d.Commission)
                    })
                    .OrderByDescending(x => x.TotalIncome)
                    .ToList();

                EmployerIncomeGrid.ItemsSource = employerStats;
            }
            else
            {
                TotalDealsText.Text = "0";
                TotalIncomeText.Text = "0 руб.";
                AverageIncomeText.Text = "0 руб.";
                AverageSalaryText.Text = seekers.Any()
                    ? $"{seekers.Average(s => s.ExpectedSalary):N2} руб."
                    : "Нет данных";

                EmployerIncomeGrid.ItemsSource = new List<EmployerStatistic>();
            }
        }
    }

    public class EmployerStatistic
    {
        public string EmployerName { get; set; }
        public int DealCount { get; set; }
        public decimal TotalIncome { get; set; }
    }
}