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
    public partial class AddAppointmentForm : Form
    {

        private MainForm _mainForm;
        public AddAppointmentForm(MainForm mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;

            ///hook up load event
            this.Load += AddAppointmentForm_Load;


            // date + time:
            dtpStart.Format = DateTimePickerFormat.Custom;
            dtpStart.CustomFormat = "MM/dd/yyyy hh:mm tt";

            dtpEnd.Format = DateTimePickerFormat.Custom;
            dtpEnd.CustomFormat = "MM/dd/yyyy hh:mm tt";

            //// TESTING /////////////////////////////////////////////==========
            dtpStart.ShowUpDown = true;
            dtpEnd.ShowUpDown = true;


            //LoadCustomers();
            btnSave.Enabled = false;

            txtType.TextChanged += ValidateInputs;
            dtpStart.ValueChanged += ValidateInputs;
            dtpEnd.ValueChanged += ValidateInputs;
            cmbCustomers.SelectedIndexChanged += ValidateInputs;
        }

        private void AddAppointmentForm_Load(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            //load customers into the combo box - use customer repo!
            var customers = CustomerRepository.GetAllCustomers();
            cmbCustomers.DataSource = customers;
            cmbCustomers.DisplayMember = "CustomerName";
            cmbCustomers.ValueMember = "CustomerId";
        }

        // ================ VALIDATE INPUTS ================ //
        private void ValidateInputs(object sender, EventArgs e)
        {
            btnSave.Enabled =
                 cmbCustomers.SelectedItem != null &&
                 !string.IsNullOrWhiteSpace(txtType.Text) &&
                 dtpEnd.Value > dtpStart.Value;


        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int customerId = (int)cmbCustomers.SelectedValue;
                string type = txtType.Text.Trim();
                DateTime localStart = dtpStart.Value;
                DateTime localEnd = dtpEnd.Value;


                // ============= VALIDATION before save
                if (string.IsNullOrEmpty(type))
                {
                    MessageBox.Show("Please enter a type.");
                    return;
                }

                if (cmbCustomers.SelectedIndex == -1 || cmbCustomers.SelectedValue == null)
                { 
                    MessageBox.Show("Please select a customer.");
                    return;
                }
                if(localEnd <= localStart)
                {
                    MessageBox.Show("End time must be after start time.");
                    return;
                }


                // save already disabled
                //if (localEnd <= localStart)
                //{
                //    MessageBox.Show("End time must be after start time. ");
                //    return;
                //}

                // convert time
                DateTime utcStart = TimeZoneInfo.ConvertTimeToUtc(localStart);
                DateTime utcEnd = TimeZoneInfo.ConvertTimeToUtc(localEnd);

                //  Business hours check
                if (!IsWithinBusinessHours(localStart, localEnd))
                {
                    MessageBox.Show("Appointments must be scheduled during the business hours of 9:00 a.m. to 5:00 p.m., Monday–Friday, eastern standard time.");
                    return;
                }

                // ======= Overlap check
                if (isOverLappingAppointment(1, utcStart, utcEnd))
                {
                    MessageBox.Show("This appointment overlaps with an existing appointment.");
                    return;
                }

                // -------> SQL INSERT
                using (var conn = Database.Connection)
                {

                    conn.Open();
                    string query = @"
                        INSERT into appointment
                        (customerId, userId, title, description, location, contact, type, url, start, end, createDate, createdBy, lastUpdateBy)
                        VALUES
                        (@customerId, @userId, '', '', '', '', @type, '', @start, @end, NOW(), 'test', 'test')";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@customerId", customerId);
                        cmd.Parameters.AddWithValue("@userId", 1);
                        cmd.Parameters.AddWithValue("@type", type);
                        cmd.Parameters.AddWithValue("@start", utcStart);
                        cmd.Parameters.AddWithValue("@end", utcEnd);

                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Appointment added successfully.");
                _mainForm.LoadAppointments();
                this.DialogResult = DialogResult.OK;
                this.Close();

            }
            catch (Exception ex) { 
                MessageBox.Show("Error saving appointment: " + ex.Message);
            }
        }



        // OVERLAPPING METHOD:
        private bool isOverLappingAppointment(int customerId, DateTime newStart, DateTime newEnd)
        {
            using (var conn = Database.Connection)
            {
                conn.Open();


                // removed AND apointtmentId != @appointmentId for now
                string query = @"
                    SELECT * FROM appointment
                    WHERE userId = @userId
                        AND (
                            (@start < end AND @end > start)
                        );
                ";


                using (var cmd = new MySqlCommand(query, conn))
                {
                    //cmd.Parameters.AddWithValue("@customerId", customerId);
                    //cmd.Parameters.AddWithValue("@appointmentId", _appointment.AppointmentId);
                    cmd.Parameters.AddWithValue("@userId", 1);
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
            if (endEST.DayOfWeek == DayOfWeek.Saturday || endEST.DayOfWeek == DayOfWeek.Sunday)
                return false;
            // Business hours check
            TimeSpan startTime = startEST.TimeOfDay;
            TimeSpan endTime = endEST.TimeOfDay;
            TimeSpan openTime = new TimeSpan(9, 0, 0); // 9AM
            TimeSpan closeTime = new TimeSpan(17, 0, 0); // 5PM

            return (startTime >= openTime && endTime <= closeTime);

        }








    }
}


//============== DATETIME PICKER
//https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.datetimepicker.customformat?view=windowsdesktop-9.0
