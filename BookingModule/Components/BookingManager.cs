/*
' Copyright (c) 2026 Hengerfejek
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using Dnn.BookingModule.BookingModule.Models;
using DotNetNuke.Data;
using DotNetNuke.Framework;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dnn.BookingModule.BookingModule.Components
{
    internal interface IBookingManager
    {
        List<Product> GetAvailableServices();
        List<TimeSpan> GetTimeSlots(DateTime? forDay);
        void CreateBooking(Booking b);
        void DeleteBooking(int bookingId, int moduleId);
        void DeleteBooking(Booking b);
        IEnumerable<Booking> GetBookings(int moduleId);
        Booking GetBooking(int bookingId, int moduleId);
        void UpdateBooking(Booking b);
        bool IsTimeSlotAvailable(DateTime targetStart, DateTime targetEnd);
    }

    internal class BookingManager : ServiceLocator<IBookingManager, BookingManager>, IBookingManager
    {
        public List<Product> GetAvailableServices()
        {
            var app = HotcakesApplication.Current;

            var serviceType = app.CatalogServices.ProductTypes.FindAll().Where(pt => pt.ProductTypeName == "service").FirstOrDefault();

            if (serviceType == null)
            {
                return new List<Product>();
            }

            var ptId = serviceType.Bvin;

            var products = app.CatalogServices.Products.FindAllPaged(1, int.MaxValue).FindAll(p => p.ProductTypeId == ptId);

            return products;
        }

        // Returns all time slots for a given day if forDay is null, otherwise checks for availability and returns only available time slots
        public List<TimeSpan> GetTimeSlots(DateTime? forDay)
        {
            var timeSlots = new List<TimeSpan>();

            var startTime = new TimeSpan(9, 0, 0); // 9:00 AM
            var endTime = new TimeSpan(17, 0, 0); // 5:00 PM

            var interval = new TimeSpan(1, 0, 0); // 1 hour interval
            var now = DateTime.Now;

            for (var time = startTime; time < endTime; time += interval)
            {
                if (forDay.HasValue)
                {
                    var targetStart = forDay.Value.Date + time;
                    var targetEnd = targetStart + interval;

                    // Skip time slots that have already passed in real time
                    if (targetStart <= now)
                    {
                        continue;
                    }

                    if (!IsTimeSlotAvailable(targetStart, targetEnd))
                    {
                        continue; // Skip this time slot as it is booked
                    }
                }
                else
                {
                    // If we just want ALL valid slots for the calendar, we still might not want to return past slots for "today" 
                    // But to keep 'AllTimeSlots' intact, we only filter past slots if 'forDay' is provided.
                }

                timeSlots.Add(time);
            }
            return timeSlots;
        }

        public void CreateBooking(Booking b)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Booking>();
                rep.Insert(b);
            }
        }

        public void DeleteBooking(int bookingId, int moduleId)
        {
            var b = GetBooking(bookingId, moduleId);
            DeleteBooking(b);
        }

        public void DeleteBooking(Booking b)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Booking>();
                rep.Delete(b);
            }
        }

        public IEnumerable<Booking> GetBookings(int moduleId)
        {
            IEnumerable<Booking> b;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Booking>();
                b = rep.Get(moduleId);
            }
            return b;
        }

        public Booking GetBooking(int bookingId, int moduleId)
        {
            Booking b;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Booking>();
                b = rep.GetById(bookingId, moduleId);
            }
            return b;
        }

        public void UpdateBooking(Booking b)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Booking>();
                rep.Update(b);
            }
        }

        public bool IsTimeSlotAvailable(DateTime targetStart, DateTime targetEnd)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                // We only need to check bookings that overlap with our target interval.
                // An overlap occurs if the booking starts before the target ends AND the booking ends after the target starts.
                // We format the exact query manually to ensure performance.

                var sql = @"
                    SELECT TOP 1 1 
                    FROM {objectQualifier}BookingModule_Bookings 
                    WHERE [Start] < @1 AND [End] > @0";

                var overlaps = ctx.ExecuteQuery<int>(System.Data.CommandType.Text, sql, targetStart, targetEnd);

                // If the query returns any row, it means the slot overlaps (is NOT available).
                return !overlaps.Any();
            }
        }

        protected override System.Func<IBookingManager> GetFactory()
        {
            return () => new BookingManager();
        }
    }
}