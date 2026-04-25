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
        public string ProductBvin { get; set; }
        public int UserId { get; set; }
        public string OrderBvin { get; set; }
        public string OrderItemBvin { get; set; }
        public DateTime StartDateTimeUtc { get; set; }
        public DateTime EndDateTimeUtc { get; set; }
        public int Status { get; set; }
        public string ExtendedData { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}