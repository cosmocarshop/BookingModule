using Dnn.BookingModule.BookingModule.Components;
using Dnn.BookingModule.BookingModule.Models;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using DotNetNuke.Web.Api.Auth;
using System.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Dnn.BookingModule.BookingModule.Controllers
{
    [DnnAuthorize(AuthTypes = "JWT", StaticRoles = "Administrators")]
    public class BookingsApiController : DnnApiController
    {
        // ------------------
        // To use these REST Endpoints from a client (like Postman):
        // 
        // 1. Enable JWT Auth in DNN Site Settings > Security -> Options -> Enable JWT.
        // 2. Call the built-in login endpoint: POST /DesktopModules/JwtAuth/API/mobile/login 
        //    with JSON {"u": "host", "p": "yourpassword"}.
        // 3. Receive a JSON token `accessToken`.
        // 4. Attach this token to calls to this controller via header:
        //    Authorization: Bearer [your_access_token]
        // ------------------

        [HttpGet]
        public HttpResponseMessage GetAll(int moduleId)
        {
            try
            {
                var bookings = BookingManager.Instance.GetBookings(moduleId);
                return Request.CreateResponse(HttpStatusCode.OK, bookings);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage Get(int moduleId, int bookingId)
        {
            try
            {
                var booking = BookingManager.Instance.GetBooking(bookingId, moduleId);
                if (booking == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                
                return Request.CreateResponse(HttpStatusCode.OK, booking);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage Create(Booking booking)
        {
            try
            {
                BookingManager.Instance.CreateBooking(booking);
                return Request.CreateResponse(HttpStatusCode.Created, booking);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public HttpResponseMessage Update(Booking booking)
        {
            try
            {
                var existing = BookingManager.Instance.GetBooking(booking.BookingId, booking.ModuleId);
                if (existing == null) return Request.CreateResponse(HttpStatusCode.NotFound);

                BookingManager.Instance.UpdateBooking(booking);
                return Request.CreateResponse(HttpStatusCode.OK, booking);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int moduleId, int bookingId)
        {
            try
            {
                BookingManager.Instance.DeleteBooking(bookingId, moduleId);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}