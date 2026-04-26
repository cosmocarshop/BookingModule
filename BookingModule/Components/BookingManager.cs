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
using System.Collections.Generic;
using System.Linq;

namespace Dnn.BookingModule.BookingModule.Components
{
    internal interface IBookingManager
    {
        List<Product> GetAvailableServices();
        void CreateBooking(Booking b);
        void DeleteBooking(int bookingId, int moduleId);
        void DeleteBooking(Booking b);
        IEnumerable<Booking> GetBookings(int moduleId);
        Booking GetBooking(int bookingId, int moduleId);
        void UpdateBooking(Booking b);
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

        protected override System.Func<IBookingManager> GetFactory()
        {
            return () => new BookingManager();
        }
    }
}