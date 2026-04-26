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

using Dnn.BookingModule.BookingModule.Components;
using Dnn.BookingModule.BookingModule.Models;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using Hotcakes.Commerce.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Dnn.BookingModule.BookingModule.Controllers
{
    [DnnHandleError]
    public class BookingController : DnnController
    {
        public ActionResult Index()
        {
            var wizard = new BookingWizard();

            wizard.AvailableServices = BookingManager.Instance.GetAvailableServices();

            return View(wizard);
        }

        [HttpPost]
        public ActionResult Index(BookingWizard wizard, string action)
        {
            if (wizard.SelectedServiceBvins == null)
            {
                wizard.SelectedServiceBvins = new List<string>();
            }

            wizard.AvailableServices = BookingManager.Instance.GetAvailableServices();

            if (wizard.CurrentStep == 2 && wizard.SelectedDate.HasValue)
            {
                wizard.AllTimeSlots = BookingManager.Instance.GetTimeSlots(null);
                wizard.AvailableTimeSlots = BookingManager.Instance.GetTimeSlots(wizard.SelectedDate.Value);
            }
            else
            {
                wizard.AllTimeSlots = new List<TimeSpan>();
            }

            if (action == "next")
            {
                wizard.CurrentStep++;

                // Refresh times for step 2 if we navigate here or stay in it
                if (wizard.CurrentStep == 2)
                {
                    if (!wizard.SelectedDate.HasValue)
                    {
                        wizard.SelectedDate = DateTime.Today;
                    }

                    wizard.AllTimeSlots = BookingManager.Instance.GetTimeSlots(null);
                    wizard.AvailableTimeSlots = BookingManager.Instance.GetTimeSlots(wizard.SelectedDate.Value);

                    // If today has no available slots, automatically push them to tomorrow
                    if (wizard.SelectedDate.Value.Date == DateTime.Today && !wizard.AvailableTimeSlots.Any())
                    {
                        wizard.SelectedDate = DateTime.Today.AddDays(1);
                        wizard.AvailableTimeSlots = BookingManager.Instance.GetTimeSlots(wizard.SelectedDate.Value);
                        ViewBag.TodayIsFull = true;
                    }
                }
            }
            else if (action == "prev" && wizard.CurrentStep > 1)
            {
                wizard.CurrentStep--;
            } else if (action == "update_date")
            {
                // Just refreshing view (like for Ajax) - will hit the logic above to fill time slots

                // Keep the TodayIsFull flag if today has indeed passed/filled up, 
                // so calendar scripts don't suddenly re-enable today after navigating dates
                if (wizard.CurrentStep == 2)
                {
                    wizard.SelectedTime = null; // Always clear when clicking around the calendar to prevent illegal selections on restricted days.

                    var todaySlots = BookingManager.Instance.GetTimeSlots(DateTime.Today);
                    if (!todaySlots.Any())
                    {
                        ViewBag.TodayIsFull = true;

                        // Failsafe in case they manipulate the payload or calendar bugs out
                        if (wizard.SelectedDate.HasValue && wizard.SelectedDate.Value.Date == DateTime.Today)
                        {
                            wizard.SelectedDate = DateTime.Today.AddDays(1);
                        }
                    }
                    if (wizard.SelectedDate.HasValue)
                    {
                        wizard.AllTimeSlots = BookingManager.Instance.GetTimeSlots(null);
                        wizard.AvailableTimeSlots = BookingManager.Instance.GetTimeSlots(wizard.SelectedDate.Value);
                    }
                }
            } else if (action == "finish")
            {
                // Server-side validation before finalizing booking
                if (wizard.SelectedServiceBvins == null || !wizard.SelectedServiceBvins.Any() ||
                    !wizard.SelectedDate.HasValue || !wizard.SelectedTime.HasValue ||
                    string.IsNullOrWhiteSpace(wizard.Name) ||
                    string.IsNullOrWhiteSpace(wizard.Email) ||
                    string.IsNullOrWhiteSpace(wizard.PhoneNr))
                {
                    wizard.CurrentStep = 1;
                    ViewBag.ErrorMessage = "Hiba történt a foglalás során. Kérem ellenőrizze, hogy minden kötelező mezőt (Szolgáltatások, Időpont, Név, E-mail, Telefonszám) megfelelően kitöltött-e.";
                    ModelState.Clear();
                    return View(wizard);
                }

                // To do: Save booking to DB here

                var startDate = wizard.SelectedDate.Value.Date + wizard.SelectedTime.Value;
                var endDate = wizard.SelectedDate.Value.Date + wizard.SelectedTime.Value.Add(new TimeSpan(1, 0, 0)); // Assuming 1 hour duration; add duration customization logic later

                // Security check to guarantee slot wasn't snatched by another session concurrently 
                if (!BookingManager.Instance.IsTimeSlotAvailable(startDate, endDate))
                {
                    wizard.CurrentStep = 1;
                    ViewBag.ErrorMessage = "Sajnáljuk, időközben ez az időpont már lefoglalásra került. Kérem, válasszon egy másik időpontot.";
                    ModelState.Clear();
                    return View(wizard);
                }

                BookingManager.Instance.CreateBooking(new Booking
                {
                    Start = startDate,
                    End = endDate,
                    Name = wizard.Name,
                    Email = wizard.Email,
                    PhoneNr = wizard.PhoneNr,
                    Comment = wizard.Comment,
                    ProductBvins = wizard.SelectedServiceBvins,
                });

                wizard.CurrentStep = 4;
            } else if (action == "home")
            {
                // Redirect user to the DNN site's Home page
                return Redirect(DotNetNuke.Common.Globals.NavigateURL(DotNetNuke.Entities.Portals.PortalSettings.Current.HomeTabId));
            }

            ModelState.Clear();

            return View(wizard);
        }
    }
}