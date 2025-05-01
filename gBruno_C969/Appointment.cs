using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gBruno_C969
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int CustomerId { get; set; }
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string CustomerName { get; set; }


        public override string ToString()
        {
            return $"{CustomerName} - {Type} ({Start.ToShortDateString()})";
        }
    }

}
