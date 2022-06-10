using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Session6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {

        public readonly DateTime CurrentDate = new DateTime(2017,10,20,0,0,0);

        public readonly DateTime StartDate;

        public List<Customer> TopCustomerList { get; set; } 

        public List<Office> TopOfficeList { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            StartDate = CurrentDate.AddMonths(-1);

            var Timer = Stopwatch.StartNew();

            


            var entities = new Session6Entities();


            List<Tickets> LastMonthTickets = entities.Tickets
                .Where(t => t.Schedules.Date > StartDate && t.Schedules.Date <= CurrentDate)
                .ToList();
        

            //Flights
            NumberConfirmedTextBox.Text = LastMonthTickets.Where(t => t.Confirmed == true).Count().ToString();
            NumberCancelledTextBox.Text = LastMonthTickets.Where(t => t.Confirmed == false).Count().ToString();
          
            AverageFlightTimeTextBox.Text = entities.Schedules
                .Where(s => s.Date > StartDate && s.Date <= CurrentDate)
                .GroupBy(s => s.Date.Day)
                .Select(g=> g.Select(s=>s.Routes.FlightTime)
                .Sum())
                .Average()
                .ToString("0.00") + " minutes";


            //Number of passengers flying
            var TicketsByDate = LastMonthTickets
                .GroupBy(t => t.Schedules.Date)
                .Select(g => new { Date = g.Key, FlightAmount = g.Count() });

            var BusiestDay = TicketsByDate
                .Where(g => g.FlightAmount == TicketsByDate.Select(gr => gr.FlightAmount)
                .Max())
                .First();
            BusiestDayTextBlock.Text = $"{BusiestDay.Date:dd/MM} with {BusiestDay.FlightAmount} flying";

            var QuitestDay = TicketsByDate.Where(g => g.FlightAmount == TicketsByDate.Select(gr => gr.FlightAmount)
                .Min())
                .First();
            QuietestDayTextBlock.Text = $"{QuitestDay.Date:dd/MM} with {QuitestDay.FlightAmount} flying";


            //Top Customers

            TopCustomerList = LastMonthTickets
                .Where(t=>t.Confirmed == true)
                .GroupBy(t => t.PassportNumber).Select(g => new Customer
                {
                    Firstname = g.Select(t => t.Firstname).First(),
                    Lastname = g.Select(t => t.Lastname).First(),
                    TicketCount = g.Count()
                })
                .OrderByDescending(x => x.TicketCount)
                .Take(3)
                .ToList();

            //Top Offices
            TopOfficeList = LastMonthTickets
                .Where(t => t.Confirmed == true)
                .GroupBy(t => t.Schedules.Routes.Airports.Name)
                .Select(g => new Office
                {
                    Name = g.Key,
                    Revenue = g.Select(t => t.Schedules.EconomyPrice + (t.CabinTypes.Name == "Economy" ? 0 :
                              t.CabinTypes.Name == "Business" ? t.Schedules.EconomyPrice / 100 * 35 :
                              t.CabinTypes.Name == "First Class" ? (t.Schedules.EconomyPrice / 100 * 35) + ((t.Schedules.EconomyPrice / 100 * 35) / 100 * 30) :
                              0)).Sum()
                })
                .OrderByDescending(o=>o.Revenue)
                .Take(3)
                .ToList();

            //Empty seats
            DateTime ModifiedCurrentDate = CurrentDate.AddDays(-1).Add(new TimeSpan(12, 0, 0));
            DateTime WeekStartDate = ModifiedCurrentDate.WeekStart(DayOfWeek.Monday);
            DateTime LastWeek = WeekStartDate.AddDays(-7);
            DateTime TwoWeekBefore = LastWeek.AddDays(-7);

            var ThisWeekSeats = entities.Schedules
                .Where(s => s.Date >= WeekStartDate && s.Date < ModifiedCurrentDate)
                .Select(s => new
                {
                     TotalSeats = s.Aircrafts.TotalSeats,
                     TakenSeats = s.Tickets.Where(t=>t.Confirmed == true).Count()
                });       
            double ThisWeekEmptySeats =((double)ThisWeekSeats.Sum(es => es.TakenSeats) / (double)ThisWeekSeats.Sum(es => es.TotalSeats)) * 100;
            ThisWeekSeatsTextBlock.Text = $"This week: [ {ThisWeekEmptySeats:N2} ] %";

            var LastWeekSeats = entities.Schedules
                .Where(s => s.Date >= LastWeek && s.Date < WeekStartDate)
                .Select(s => new
                {
                    TotalSeats = s.Aircrafts.TotalSeats,
                    TakenSeats = s.Tickets.Where(t => t.Confirmed == true).Count()
                });
            double LastWeekEmptySeats = ((double)LastWeekSeats.Sum(es => es.TakenSeats) / (double)LastWeekSeats.Sum(es => es.TotalSeats)) * 100;
            LastWeekSeatsTextBlock.Text = $"Last week: [ {LastWeekEmptySeats:N2} ] %";

            var TwoWeekBeforeSeats = entities.Schedules
                .Where(s => s.Date >= TwoWeekBefore && s.Date < LastWeek)
                .Select(s => new
                {
                    TotalSeats = s.Aircrafts.TotalSeats,
                    TakenSeats = s.Tickets.Where(t => t.Confirmed == true).Count()
                });
            double TwoWeekBeforeEmptySeats = ((double)TwoWeekBeforeSeats.Sum(es => es.TakenSeats) / (double)TwoWeekBeforeSeats.Sum(es => es.TotalSeats)) * 100;
            TwoWeekBeforeSeatsTextBlock.Text = $"This week: [ {TwoWeekBeforeEmptySeats:N2} ] %";


            //Ticket Revenue
            var RevenueList = LastMonthTickets
                .Where(t => t.Confirmed == true)
                .GroupBy(t => t.Schedules.Date)
                .Select(g => new 
                {
                    Date = g.Key,
                    Revenue = g.Select(t => t.Schedules.EconomyPrice + (t.CabinTypes.Name == "Economy" ? 0 :
                              t.CabinTypes.Name == "Business" ? t.Schedules.EconomyPrice / 100 * 35 :
                              t.CabinTypes.Name == "First Class" ? (t.Schedules.EconomyPrice / 100 * 35) + ((t.Schedules.EconomyPrice / 100 * 35) / 100 * 30) :
                              0)).Sum()
                })
                .OrderByDescending(o => o.Revenue)                
                .ToList();


            YesterdayRevenueTextBlock.Text = "Yesterday: $" + RevenueList.Where(r => r.Date == CurrentDate.AddDays(-1))?.Single().Revenue.ToString("N2");
            TwoDaysAgoRevenueTextBlock.Text = "Two days ago: $" + RevenueList.Where(r => r.Date == CurrentDate.AddDays(-2))?.Single().Revenue.ToString("N2");
            ThreeDaysAgoRevenueTextBlock.Text = "Three days ago: $" + RevenueList.Where(r => r.Date == CurrentDate.AddDays(-3))?.Single().Revenue.ToString("N2");


            Timer.Stop();
            LoadTimeTextBox.Text = $"Report generated in {Timer.Elapsed:ss':'fff} seconds";

           
        }

       

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public class Customer
        {
            public string Firstname { get; set; }

            public string Lastname { get; set; }

            public int TicketCount { get; set; }
        }

        public class Office
        {

            public string Name { get; set; }

            public decimal Revenue { get; set; }


        }

        

    }


    public static class Extensions
    {
        public static DateTime WeekStart(this DateTime dateTime, DayOfWeek startDayName)
        {
            int difference = ((dateTime.DayOfWeek - startDayName) + 7) % 7;
            DateTime StartDate = dateTime.AddDays(-1 * difference).Date;
            return StartDate;
        }

    }

}
