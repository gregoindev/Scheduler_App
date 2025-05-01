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
    public partial class ModifyAppointmentForm : Form
    {

        private Appointment _appointment;
        private MainForm _mainForm;
        public ModifyAppointmentForm(MainForm mainForm, Appointment appointment)
        {
            InitializeComponent();
            _mainForm = mainForm;
            _appointment = appointment;

            this.Load += ModifyAppointmentForm_Load;
        }

     

        private void ModifyAppointmentForm_Load(object sender, EventArgs e)
        {
            LoadCustomers();

            dtpStart.Format = DateTimePickerFormat.Custom;
            dtpStart.CustomFormat = "MM/dd/yyyy hh:mm tt";
            dtpStart.ShowUpDown = true;

            dtpEnd.Format = DateTimePickerFormat.Custom;
            dtpEnd.CustomFormat = "MM/dd/yyyy hh:mm tt";
            dtpEnd.ShowUpDown = true;



            cmbCustomers.SelectedValue = _appointment.CustomerId;
            txtType.Text = _appointment.Type;
            dtpStart.Value = _appointment.Start.ToLocalTime();
            dtpEnd.Value = _appointment.End.ToLocalTime();
        }

        private void LoadCustomers()
        {
            try
            {
                using (var conn = Database.Connection)
                {
                    conn.Open();
                    string query = "SELECT customerId, customerName FROM customer";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable customersTable = new DataTable();


                            adapter.Fill(customersTable);
                            cmbCustomers.DataSource = customersTable;
                            cmbCustomers.DisplayMember = "customerName";
                            cmbCustomers.ValueMember = "customerId";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}");
            }

        }
        private void btnSave_Click(object sender, EventArgs e)
        {

            try
            {
                int customerId = (int)cmbCustomers.SelectedValue;
                string type = txtType.Text.Trim();
                DateTime localStart = dtpStart.Value;
                DateTime localEnd = dtpEnd.Value;


                if (localEnd <= localStart)
                {
                    MessageBox.Show("End time must be after start time.");
                    return;
                }

                // Validate before save
                if (string.IsNullOrWhiteSpace(txtType.Text))
                {
                    MessageBox.Show("Please enter a type for the appointment.");
                    return;
                }

                if(cmbCustomers.SelectedIndex == -1 ||cmbCustomers.SelectedValue == null)
                {
                    MessageBox.Show("Please select a customer.");
                    return;
                }

                DateTime utcStart = TimeZoneInfo.ConvertTimeToUtc(localStart);
                DateTime utcEnd = TimeZoneInfo.ConvertTimeToUtc(localEnd);

                //  Business hours check
                if (!IsWithinBusinessHours(localStart, localEnd)){ MessageBox.Show("Appointments must be scheduled during the business hours of 9:00 a.m. to 5:00 p.m., Monday–Friday, eastern standard time.");
                    return;
                }


                // Validate if Overlapping ============================//
                if (isOverLappingAppointment(1, utcStart, utcEnd))
                {
                    MessageBox.Show("This appointment overlaps with another appointment for the same customer.");
                    return;
                }


                using (var conn = Database.Connection)
                {
                    conn.Open();

                    string query = @"
                        UPDATE appointment
                        SET customerId = @customerId,
                            type = @type,
                            start = @start,
                            end = @end,
                            lastUpdate = CURRENT_TIMESTAMP,
                            lastUpdateBy = 'test'
                        WHERE appointmentId = @appointmentId;
                    ";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@appointmentId", _appointment.AppointmentId);
                        cmd.Parameters.AddWithValue("@customerId", customerId);
                        cmd.Parameters.AddWithValue("@type", type);
                        cmd.Parameters.AddWithValue("@start", utcStart);
                        cmd.Parameters.AddWithValue("@end", utcEnd);

                        cmd.ExecuteNonQuery();
                    }
                }

                // refresh
                _mainForm.LoadAppointments();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating appointment: {ex.Message}");
            }
        }



        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }



        // OVERLAPPING APPOINTMENTS:
        private bool isOverLappingAppointment(int customerId, DateTime newStart, DateTime newEnd)
        {
            using (var conn = Database.Connection)
            {
                conn.Open();

                string query = @"
                    SELECT * FROM appointment
                    WHERE userId = @userId
                        AND appointmentId != @appointmentId
                        AND (
                            (@start < end AND @end > start)
                        );
                ";


                using (var cmd = new MySqlCommand(query, conn))
                {
                    //cmd.Parameters.AddWithValue("@customerId", customerId);
                    cmd.Parameters.AddWithValue("@userId", 1);
                    cmd.Parameters.AddWithValue("@appointmentId", _appointment.AppointmentId);
                    cmd.Parameters.AddWithValue("@start", newStart);
                    cmd.Parameters.AddWithValue("@end", newEnd);

                    using (var reader = cmd.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }
        }


        // BUSINESS HOURS rubric:
        private bool IsWithinBusinessHours(DateTime localStart, DateTime localEnd)
        {

            // comment -> maybe this will work too?
            //TimeSpan businessStart = new TimeSpan(8, 0, 0); // 8:00 AM
            //TimeSpan businessEnd = new TimeSpan(22, 0, 0); // 10:00 PM
            //return start.TimeOfDay >= businessStart && end.TimeOfDay <= businessEnd;

            TimeZoneInfo estTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime startEST = TimeZoneInfo.ConvertTime(localStart, estTimeZone);
            DateTime endEST = TimeZoneInfo.ConvertTime(localEnd, estTimeZone);

            // Saturday and Sunday check
            if (startEST.DayOfWeek == DayOfWeek.Saturday || startEST.DayOfWeek == DayOfWeek.Sunday)            
                return false;
            if(endEST.DayOfWeek == DayOfWeek.Saturday || endEST.DayOfWeek == DayOfWeek.Sunday)
                return false;
            // Business hours check
            TimeSpan startTime = startEST.TimeOfDay;
            TimeSpan endTime = endEST.TimeOfDay;
            TimeSpan openTime = new TimeSpan(9, 0, 0); // 9AM
            TimeSpan closeTime = new TimeSpan(17, 0, 0); // 5PM

            return (startTime >= openTime && endTime <= closeTime);

        }





        // END.............
    }
}
