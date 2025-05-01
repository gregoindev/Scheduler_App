using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gBruno_C969
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetupCustomerGrid();
            LoadCustomers();
            LoadAppointments();
            SetupAppointmentGrid();
            CheckUpcomingAppointments();
        }



        // =====> manually populate DGV columns ============== //
        private void SetupCustomerGrid()
        {
            //throw new NotImplementedException();

            dgvCustomers.AutoGenerateColumns = false;
            dgvCustomers.Columns.Clear();

            dgvCustomers.Columns.Add(CreateTextColumn("CustomerId", "ID"));
            dgvCustomers.Columns.Add(CreateTextColumn("CustomerName", "Name"));
            dgvCustomers.Columns.Add(CreateTextColumn("Address", "Address"));
            dgvCustomers.Columns.Add(CreateTextColumn("Phone", "Phone"));
            dgvCustomers.Columns.Add(CreateTextColumn("City", "City"));
            dgvCustomers.Columns.Add(CreateTextColumn("Country", "Country"));

            
            dgvCustomers.AllowUserToAddRows = false; // Disable the ability to add new rows

        }

        // ====================== Create Text UTITLY +++++++++++++++++ //
        private DataGridViewTextBoxColumn CreateTextColumn(string dataProperty, string header)
        {
            return new DataGridViewTextBoxColumn
            {
                DataPropertyName = dataProperty,
                HeaderText = header,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true
            };
        }


        // =====> APPOINTMENT DVG  ============== //
        private void SetupAppointmentGrid()
        {
            dgvAppointments.AutoGenerateColumns = false;
            dgvAppointments.Columns.Clear();

            dgvAppointments.Columns.Add(CreateTextColumn("AppointmentId", "ID"));
            dgvAppointments.Columns.Add(CreateTextColumn("CustomerName", "Customer"));
            dgvAppointments.Columns.Add(CreateTextColumn("Type", "Type"));
            dgvAppointments.Columns.Add(CreateTextColumn("Start", "Start"));
            dgvAppointments.Columns.Add(CreateTextColumn("End", "End"));

            dgvAppointments.AllowUserToAddRows = false; // Disable the ability to add new rows
        }


        // =================== LOAD APPOINTMENTS ================== //
        public void LoadAppointments()
        {
            using (var conn = Database.Connection)
            {
                conn.Open();
                string query = @"
            SELECT
                appointment.appointmentId,
                customer.customerId,
                customer.customerName,
                appointment.type,
                appointment.start,
                appointment.end
            FROM appointment
            JOIN customer ON appointment.customerId = customer.customerId;
        ";

                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    var appointments = new List<Appointment>();

                    while (reader.Read())
                    {
                        appointments.Add(new Appointment
                        {
                            AppointmentId = reader.GetInt32("appointmentId"),
                            CustomerId = reader.GetInt32("customerId"),
                            CustomerName = reader.GetString("customerName"),
                            Type = reader.GetString("type"),
                            Start = reader.GetDateTime("start"),
                            End = reader.GetDateTime("end")
                        });
                    }

                    dgvAppointments.DataSource = new BindingList<Appointment>(appointments);
                }
            }
        }

        private void LoadCustomers()
        {
            //throw new NotImplementedException();
            //using (var conn = Database.Connection)
            //{
            //    conn.Open();

            //    string query = @"
            //        SELECT
            //            customer.customerId AS 'ID',
            //            customer.customerName AS 'Name',
            //            address.address AS 'Address',
            //            address.phone AS 'Phone',
            //            city.city AS 'City',
            //            country.country AS 'Country'

            //        FROM customer
            //        JOIN address ON customer.addressId = address.addressId
            //        JOIN city ON address.cityId = city.cityId
            //        JOIN country ON city.countryId = country.countryId;
            //    ";


            //    // ======================== DATA ADAPTER ========================== //
            //    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
            //    DataTable dt = new DataTable();
            //    adapter.Fill(dt);
            //    dgvCustomers.DataSource = dt;
            //}


            // ---> Re-use Customer repo class
            dgvCustomers.DataSource = new BindingList<Customer>(CustomerRepository.GetAllCustomers());

        }

        private void btnModifyCustomer_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow?.DataBoundItem is Customer selectedCustomer)    // check if row can be selected
            {
                var form = new ModifyCustomerForm(selectedCustomer);
                var result = form.ShowDialog();

                if (result == DialogResult.OK)
                {
                    LoadCustomers();
                }
            }
            else
            {
                MessageBox.Show("Please select a customer to modefy.");
            }
        }

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            AddCustomerForm form = new AddCustomerForm();
            var result = form.ShowDialog();

            if (result == DialogResult.OK)
            {
                LoadCustomers();
            }
        }

        private void btnDeleteCustomer_Click(object sender, EventArgs e)
        {


            if (dgvCustomers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a customer to delete.");
                return;
            }


            var selectedCustomer = dgvCustomers.CurrentRow?.DataBoundItem as Customer;


            if (selectedCustomer == null)
            {
                MessageBox.Show("Please select a customer to delete.");
                return;
            }

            // check if the customer has any appointments
            if (CustomerRepository.CustomerHasAppointments(selectedCustomer.CustomerId))
            {
                MessageBox.Show("Cannot delete customer with appointments.");
                return;
            }

            DialogResult result = MessageBox.Show($"Are you sure you want to delete {selectedCustomer.CustomerName}?",
                "Delete Customer", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);


            if (result == DialogResult.Yes)
            {
                CustomerRepository.DeleteCustomer(selectedCustomer.CustomerId);
                LoadCustomers();
            }
        }

        private void btnAddAppointment_Click(object sender, EventArgs e)
        {
            var form = new AddAppointmentForm(this);
            var result = form.ShowDialog();

            if (result == DialogResult.OK)
            {
                LoadAppointments();
            }
        }


        /// ========== APPOINTMENT SECTION ============ ///
        private void btnModifyAppointment_Click(object sender, EventArgs e)
        {
            if (dgvAppointments.CurrentRow?.DataBoundItem is Appointment selectedAppointment)
            {
                var form = new ModifyAppointmentForm(this, selectedAppointment);
                var result = form.ShowDialog();

                if (result == DialogResult.OK)
                {
                    LoadAppointments();
                }
            }

            else
            {
                MessageBox.Show("Please select an appointment to modify....");
            }
        }

        private void btnDeleteAppointment_Click(object sender, EventArgs e)
        {
            if (dgvAppointments.CurrentRow?.DataBoundItem is Appointment selectedAppointment)
            {
                DialogResult result = MessageBox.Show($"Are you sure you want to delete appointment {selectedAppointment.AppointmentId}?",
                    "Delete Appointment", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);


                if (result == DialogResult.Yes)
                {
                    try
                    {
                        using (var conn = Database.Connection)
                        {
                            conn.Open();
                            string query = "DELETE FROM appointment WHERE appointmentId = @appointmentId";
                            using (var cmd = new MySqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@appointmentId", selectedAppointment.AppointmentId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        MessageBox.Show("Appiontment deleted successfully.");
                        LoadAppointments();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting appointment: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Please select an appointment to be deleted.");
                }
            }
        }

        // ================== 15 min alert rubric ================== //
        private void CheckUpcomingAppointments()
        {
            try
            {
                DateTime now = DateTime.Now;
                DateTime fifteenMinutesFromNow = now.AddMinutes(15);

                using (var conn = Database.Connection) { 
                    conn.Open();

                    string query = @"
                        SELECT appointment.appointmentId, customer.customerName, appointment.start
                        FROM appointment
                        JOIN customer ON appointment.customerId = customer.customerId
                        WHERE appointment.start BETWEEN @now AND @fifteenMinutesFromNow;        
                    ";
                    using (var cmd = new MySqlCommand(query, conn))
                    {

                        cmd.Parameters.AddWithValue("@now", now.ToUniversalTime());
                        cmd.Parameters.AddWithValue("@fifteenMinutesFromNow", fifteenMinutesFromNow.ToUniversalTime());


                        using (var reader = cmd.ExecuteReader()) {

                            if (reader.Read())
                            {
                                string customerName = reader.GetString("customerName");
                                DateTime startTime = reader.GetDateTime("start").ToLocalTime();
                                MessageBox.Show($"Upcoming appointment for {customerName} at {startTime.ToShortTimeString()}. ");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking upcoming appointments: {ex.Message}");
            }
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            ReportsForm reportsForm = new ReportsForm();
            reportsForm.ShowDialog();
        }

        /* TODO: 
            . appointment DGV - done
            . add customer button dfone
            . add customer form = done


            delete  ops -done
            btn save logic - customer form...  - done
            continue UDEMY SQL with winforms - done
            -->ADDCUSTOMERFORM - seperate insert into, selet etc - done
            save button logic - in progress (no customer id + name being added) -  I DID IT!!!!!
            MODIFY, DELETE appointment - in prgoress
        =============
            *appointments can not overlap - refactor <userId>
            *15 min alert- done
            *validate add and modify forms- done
            ==========================================
            *login history - HERE <--- done
            *arraylist?
            *appointmentforms : iswithinbusiness hours? -> done
            *bug modify cust form : Cant Select - 
            *EXCEPTION class rubric - done
            *reports - appointment by type and month - reports dgv  - done
            *lambda with inline comment Rubirc - done
            *custom report - done
        */
    }
}
