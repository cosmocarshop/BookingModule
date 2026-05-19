using Dnn.BookingModule.BookingModule.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Dnn.BookingModule.BookingModule.Components
{
    internal class BrevoManager
    {
        public static BrevoManager Instance { get; } = new BrevoManager();

        private static readonly string ApiKey = System.Configuration.ConfigurationManager.AppSettings["BrevoApiKey"];
        private const string ApiUrl = "https://api.brevo.com/v3/events";
        private const string DateFormat = "yyyy'.' MM'.' dd '–' HH:mm";

        private HttpClient _httpClient = new HttpClient();

        public class BrevoResponse
        {
            public bool success { get; set; }
            public string message { get; set; }
        }

        public BrevoResponse SendBookingEvent(Booking b)
        {
            if (string.IsNullOrEmpty(ApiKey))
            {
                return new BrevoResponse { success = false, message = "Brevo hiba: Hiányzó API kulcs" };
            }
            var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl);
            request.Headers.Add("api-key", ApiKey);

            var products = BookingManager.Instance.GetAvailableServices().FindAll(s => b.ProductBvins.Contains(s.Bvin)).ToList();

            var product_names = string.Join(", ", products.Select(p => p.ProductName));

            var amount_total = products.Sum(p => (int) p.SitePrice).ToString() + " Ft";

            var eventData = new
            {
                event_name = "booking",
                identifiers = new
                {
                    email_id = b.Email
                },
                event_properties = new
                {
                    start = b.Start.ToString(DateFormat),
                    end = b.End.ToString(DateFormat),
                    name = b.Name,
                    phone = b.PhoneNr,
                    comment = b.Comment,
                    services = product_names,
                    amountTotal = amount_total
                }
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(eventData);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = _httpClient.SendAsync(request).GetAwaiter().GetResult();

                var errorStr = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    return new BrevoResponse { success = true, message = $"Sikeres API hívás" };
                }

                return new BrevoResponse {
                    success = false,
                    message = $"Brevo hiba: {(int)response.StatusCode}. Response: {errorStr}"
                };
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                return new BrevoResponse { success = false, message = $"Brevo hiba: {ex.ToString()}" };
            }


        }

    }
}
