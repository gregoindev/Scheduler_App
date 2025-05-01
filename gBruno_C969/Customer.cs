using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace gBruno_C969
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }


        public string Address { get; set; }
        public string Phone { get; set; }


        public string City { get; set; }
        public string Country { get; set; } //  --> string country!
        public int AddressId { get; set; }
        public int CityId { get; set; }
        public int CountryId { get; set; }


        // ============== override String ================
        public override string ToString()
        {
            return $"{CustomerName} ({CustomerId})";
        }


        // Constructor


    }
}
