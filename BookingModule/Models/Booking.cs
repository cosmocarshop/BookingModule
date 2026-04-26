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

using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Web.Caching;

namespace Dnn.BookingModule.BookingModule.Models
{
    [TableName("BookingModule_Bookings")]
    //setup the primary key for table
    [PrimaryKey("BookingId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Bookings", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    [Scope("ModuleId")]
    public class Booking
    {
        public int BookingId { get; set; }
        public int ModuleId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNr { get; set; }
        public string Comment { get; set; }
        public int Status { get; set; }

        [IgnoreColumn]
        public List<string> ProductBvins { get; set; } = new List<string>();

        // This property stores the list as a comma-separated string in the database
        public string SerializedProductBvins
        {
            get
            {
                if (ProductBvins == null || ProductBvins.Count == 0) return string.Empty;
                return string.Join(",", ProductBvins);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    ProductBvins = new List<string>();
                }
                else
                {
                    ProductBvins = new List<string>(value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                }
            }
        }
    }
}