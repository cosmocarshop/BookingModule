using Hotcakes.Commerce.Catalog;
using System;
using System.Collections.Generic;

namespace Dnn.BookingModule.BookingModule.Models
{
    public class BookingWizard
    {
        public int CurrentStep { get; set; } = 1;

        public List<Product> AvailableServices { get; set; }

        public List<string> SelectedServiceBvins { get; set; } = new List<string>();

        public DateTime? SelectedDate { get; set; }
        
        public TimeSpan? SelectedTime { get; set; }
        
        public List<TimeSpan> AvailableTimeSlots { get; set; } = new List<TimeSpan>();
        
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNr { get; set; }
        public string Comment { get; set; }
    }
}