using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gBruno_C969
{
    public partial class ReportsForm : Form
    {
        public ReportsForm()
        {
            InitializeComponent();
        }
      
        private void ReportsForm_Load(object sender, EventArgs e)
        {
            LoadAppointmentsdByTypeAndMonth();
        }

        private void LoadAppointmentsdByTypeAndMonth()
        {
            List<AppointmentTypeCount> reportData = new List<AppointmentTypeCount>();

            using (var conn = Database.Connection)
            {
                conn.Open();

                string query = "SELECT type, start FROM appointment";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        List<(string Type, DateTime Start)> rawData = new List<(string, DateTime)>();
                        while (reader.Read()) {
                            string type = reader.GetString("type");
                            DateTime start = reader.GetDateTime("start");
                            rawData.Add((type, start));
                        }

                        var grouped = rawData
                            .GroupBy(x => new { x.Type, Month = x.Start.ToString("MMMM") })
                            .Select(g => new AppointmentTypeCount
                            {
                                Type = g.Key.Type,
                                Month = g.Key.Month,
                                Count = g.Count()
                            })
                            .ToList();
                        reportData = grouped;
                    }
                }
            }

            dgvReports.DataSource = reportData;

        }



        // method to load user schedule
        private void LoadUserSchedule()
        {
            List<UserSchedule> schedule = new List<UserSchedule>();

            using (var connn = Database.Connection)
            {
                connn.Open();
                
                string query = "SELECT user.userName, customer.customerName, appointment.type, appointment.start, appointment.end " +
                               "FROM appointment " +
                               "JOIN user ON appointment.userId = user.userId " +
                               "JOIN customer ON appointment.customerId = customer.customerId";

                using (var cmd = new MySqlCommand(query, connn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string userName = reader.GetString("userName");
                            string customerName = reader.GetString("customerName");
                            string type = reader.GetString("type");
                            DateTime start = reader.GetDateTime("start").ToLocalTime();
                            DateTime end = reader.GetDateTime("end").ToLocalTime();

                            schedule.Add(new UserSchedule()
                            {
                                Username = userName,
                                CustomerName = customerName,
                                Type = type,
                                Start = start,
                                End = end
                            });
                        }
                    }
                }
            }

            dgvReports.DataSource = schedule;
        }



        private void btnUserSchedule_Click(object sender, EventArgs e)
        {
            LoadUserSchedule();
        }



        // Custom Report for the rubric - METHOD
        private void LoadCustomersWithoutAppointments()
        {
            List<CustomerWithoutAppointments> customers = new List<CustomerWithoutAppointments>();

            using (var conn = Database.Connection)

            {
                conn.Open();
                string query = @"
                    SELECT customerName FROM customer WHERE customerId NOT IN (SELECT DISTINCT customerId FROM appointment);
            ";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            customers.Add(new CustomerWithoutAppointments()
                            {
                                CustomerName = reader.GetString("customerName")
                            });
                        }

                    }

                }

            }
            dgvReports.DataSource = customers;

        }





        public class AppointmentTypeCount
        {
            public string Month { get; set; }
            public int Count { get; set; }
            public string Type { get; set; }
        }

        public class UserSchedule 
        {
            public string Username { get; set; }
            public string CustomerName { get; set; }
            public string Type { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }

        // Custom Report for the rubric
        public class CustomerWithoutAppointments
        {
            public string CustomerName { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadCustomersWithoutAppointments();
        }
    }
}
