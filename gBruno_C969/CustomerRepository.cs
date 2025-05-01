using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace gBruno_C969
{
    public static class CustomerRepository
    {
        public static List<Customer> GetAllCustomers()
        {
            var customers = new List<Customer>();

            using (var conn = Database.Connection)
            {
                conn.Open();


                string query = @"
                    SELECT  
                        customer.customerId,
                        customer.customerName,
                        address.address,    
                        address.phone,  
                        city.city,  
                        country.country,
                        customer.addressId,
                        address.cityId,
                        city.countryId
                    FROM customer
                    JOIN address ON customer.addressId = address.addressId
                    JOIN city ON address.cityId = city.cityId
                    JOIN country ON city.countryId = country.countryId;
        
                ";


                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customers.Add(new Customer
                        {
                            CustomerId = reader.GetInt32("customerId"),
                            CustomerName = reader.GetString("customerName"),
                            Address = reader.GetString("address"),
                            Phone = reader.GetString("phone"),
                            City = reader.GetString("city"),
                            Country = reader.GetString("country"),
                            AddressId = reader.GetInt32("addressId"),
                            CityId = reader.GetInt32("cityId"),
                            CountryId = reader.GetInt32("countryId")
                        });


                    }
                }
            }

            return customers;
        }


        public static bool CustomerHasAppointments(int customerId)
        {
            using (var conn = Database.Connection)
            {
                conn.Open();
                string query = @"
                    SELECT COUNT(*) 
                    FROM appointment 
                    WHERE customerId = @customerId";
                
                // sql command
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@customerId", customerId);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        public static void DeleteCustomer(int customerId)
        {
            using (var conn = Database.Connection)
            {
                conn.Open();
                string query = @"
                    DELETE FROM customer
                    WHERE customerId = @customerId";
                
                // sql command
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@customerId", customerId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateCustomer(Customer customer)
        {
            using (var conn = Database.Connection) {
                conn.Open();


                string query = @"
                
                    UPDATE customer
                    SET customerName = @name,
                        addressId = @addressId,
                        lastUpdate = CURRENT_TIMESTAMP,
                        lastUpdateBy = 'test'
                    WHERE customerId = @id;

                    UPDATE address
                    SET address = @address,
                        phone = @phone,
                        lastUpdate = CURRENT_TIMESTAMP,
                        lastUpdateBy = 'test'
                    WHERE addressId = @addressId;

                    UPDATE city  
                    SET city = @cityName,       
                        lastUpdate = CURRENT_TIMESTAMP,
                        lastUpdateBy = 'test'
                    WHERE cityId = @cityId;

                    UPDATE country 
                    SET country = @country,
                        lastUpdate = CURRENT_TIMESTAMP,
                        lastUpdateBy = 'test'
                    WHERE countryId = @countryId;   

                ";

                // ======= sql cmds / parameters ======== //
                MySqlCommand cmd = new MySqlCommand(query, conn);   
                cmd.Parameters.AddWithValue("@id", customer.CustomerId);
                cmd.Parameters.AddWithValue("@name", customer.CustomerName);
                cmd.Parameters.AddWithValue("@address", customer.Address);
                cmd.Parameters.AddWithValue("@phone", customer.Phone);
                cmd.Parameters.AddWithValue("@addressId", customer.AddressId);
                cmd.Parameters.AddWithValue("@cityName", customer.City);   // city -> cityName (confusing)
                cmd.Parameters.AddWithValue("@cityId", customer.CityId);
                cmd.Parameters.AddWithValue("@country", customer.Country);
                cmd.Parameters.AddWithValue("@countryId", customer.CountryId);

                cmd.ExecuteNonQuery();
            }

        }
    }
}
