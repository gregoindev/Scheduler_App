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
    public partial class AddCustomerForm : Form
    {
        public AddCustomerForm()
        {
            InitializeComponent();
            btnSave.Enabled = false;


            // ============= validation section ============== //

            txtName.TextChanged += ValidateInputs;
            txtAddress.TextChanged += ValidateInputs;
            txtPhone.TextChanged += ValidateInputs;
            txtCity.TextChanged += ValidateInputs;
            txtCountry.TextChanged += ValidateInputs;
        }



        private void ValidateInputs(object sender, EventArgs e)
        {
            // Check if all fields are filled
            bool allValid = !string.IsNullOrWhiteSpace(txtName.Text) &&
                           !string.IsNullOrWhiteSpace(txtAddress.Text) &&
                           !string.IsNullOrWhiteSpace(txtPhone.Text) &&
                           !string.IsNullOrWhiteSpace(txtCity.Text) &&
                           !string.IsNullOrWhiteSpace(txtCountry.Text);


            btnSave.Enabled = allValid;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }


        // ============= this section !!  ============== //
        private void btnSave_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string address = txtAddress.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string city = txtCity.Text.Trim();
            string country = txtCountry.Text.Trim();


            // Add validation for phone number format

            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new MyCustomExceptionClass("Customer name cannot be empty.");

                if (!IsValidPhoneNumber(phone))
                    throw new MyCustomExceptionClass("Invalid phone number. Please use format: 123-456-7890");


                using (var conn = Database.Connection)
                {
                    conn.Open();

                    MySqlCommand cmd = conn.CreateCommand();
                    MySqlTransaction transaction = conn.BeginTransaction();
                    cmd.Connection = conn;
                    cmd.Transaction = transaction;

                    try
                    {
                        // insert country

                        cmd.CommandText = @"
                        Insert into country (country, createDate, createdBy, lastUpdateBy)
                        VALUES (@country, NOW(), 'test', 'test')";

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@country", country);
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "SELECT LAST_INSERT_ID()";
                        int countryId = Convert.ToInt32(cmd.ExecuteScalar());


                        // insert city
                        cmd.CommandText = @"
                        Insert into city(city, countryId, createDate, createdBy, lastUpdateBy)
                        VALUES (@city, @countryId, NOW(), 'test', 'test')";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@city", city);
                        cmd.Parameters.AddWithValue("@countryId", countryId);
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "SELECT LAST_INSERT_ID()";

                        int cityId = Convert.ToInt32(cmd.ExecuteScalar());


                        // insert address
                        cmd.CommandText = @"
                        Insert into address(address, address2, cityId, postalCode, phone, createDate, createdBy, lastUpdateBy)
                        VALUES (@address, '', @cityId, '', @phone, NOW(), 'test', 'test')";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@address", address);
                        cmd.Parameters.AddWithValue("@cityId", cityId);
                        cmd.Parameters.AddWithValue("@phone", phone);
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "SELECT LAST_INSERT_ID()";
                        int addressId = Convert.ToInt32(cmd.ExecuteScalar());


                        //insert into customer:
                        cmd.CommandText = @"
                        Insert into customer(customerName, addressId, active, createDate, createdBy, lastUpdateBy)
                        VALUES (@name, @addressId, 1, NOW(), 'test', 'test');
                    ";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@addressId", addressId);
                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                        MessageBox.Show("Customer added successfully!");
                        this.DialogResult = DialogResult.OK;
                        this.Close();


                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Error adding customer. Please try again.\n {ex.Message}");
                    }

                }
            }
            catch (MyCustomExceptionClass ex)
            {
                MessageBox.Show($"Validation Error: {ex.Message}");
            }
           
        }


        private bool IsValidPhoneNumber(string phone)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\d{3}-\d{3}-\d{4}$");
        }


    }
}
