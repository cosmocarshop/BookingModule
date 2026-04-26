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
                // Load time slots based on the selected date. Currently fixed 9:00 to 18:00 hourly times
                wizard.AvailableTimeSlots = new List<TimeSpan>();
                for (int hour = 9; hour < 18; hour++)
                {
                    wizard.AvailableTimeSlots.Add(new TimeSpan(hour, 0, 0));
                }
            }
            else
            {
                wizard.AvailableTimeSlots = new List<TimeSpan>();
            }

            if (action == "next")
            {
                wizard.CurrentStep++;

                // Refresh times for step 2 if we navigate here or stay in it
                if (wizard.CurrentStep == 2 && wizard.SelectedDate.HasValue)
                {
                    wizard.AvailableTimeSlots.Clear();
                    for (int hour = 9; hour < 18; hour++)
                    {
                        wizard.AvailableTimeSlots.Add(new TimeSpan(hour, 0, 0));
                    }
                }
            }
            else if (action == "prev" && wizard.CurrentStep > 1)
            {
                wizard.CurrentStep--;
            } else if (action == "update_date")
            {
                // Just refreshing view (like for Ajax) - will hit the above logic to fill time slots
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