using Google.Protobuf.Reflection;
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
    public partial class ModifyCustomerForm : Form
    {

        private Customer originalCustomer;

        public ModifyCustomerForm(Customer customer)
        {
            InitializeComponent();
            originalCustomer = customer;

            // ============= fields filler ============== //
            txtName.Text = customer.CustomerName;
            txtAddress.Text = customer.Address;
            txtPhone.Text = customer.Phone;
            txtCity.Text = customer.City;   
            txtCountry.Text = customer.Country;
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            originalCustomer.CustomerName = txtName.Text;
            originalCustomer.Address = txtAddress.Text;
            originalCustomer.Phone = txtPhone.Text;
            originalCustomer.City = txtCity.Text;
            originalCustomer.Country = txtCountry.Text;


            try
            {
                // My Custom Exceptions <---------------
                if (string.IsNullOrWhiteSpace(txtName.Text))
                    throw new MyCustomExceptionClass("Customer name cannot be empty.");

                if (!isValidPhoneNumber(txtPhone.Text))
                    throw new MyCustomExceptionClass("Invalid phone number. Please use format: 123-456-7890");


                CustomerRepository.UpdateCustomer(originalCustomer);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Error updating customer: {ex.Message}");
            //}
            catch (MyCustomExceptionClass ex) 
            { 
                MessageBox.Show($"Validation error:  {ex.Message}");
            }
        }

      

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }






        private bool isValidPhoneNumber(string text)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(text, @"^\d{3}-\d{3}-\d{4}$");
        }
    }
}
