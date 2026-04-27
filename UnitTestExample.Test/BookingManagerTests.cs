using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace UnitTestExample.Test
{
    // ----------------------------
    // SERVICE (produkciós kód)
    // ----------------------------
    public class TimeSlotService
    {

        public List<TimeSpan> GenerateTimeSlots()
        {
            var timeSlots = new List<TimeSpan>();

            var startTime = new TimeSpan(9, 0, 0);
            var endTime = new TimeSpan(17, 0, 0);
            var interval = new TimeSpan(1, 0, 0);

            for (var time = startTime; time < endTime; time += interval)
            {
                timeSlots.Add(time);
            }

            return timeSlots;
        }
    }

    // ----------------------------
    // TESZTEK
    // ----------------------------
    [TestFixture]
    public class TimeSlotServiceTests
    {
        [Test]
        public void GenerateTimeSlots_Returns8Slots()
        {
            var service = new TimeSlotService();

            var result = service.GenerateTimeSlots();

            Assert.That(result.Count, Is.EqualTo(8));
        }

        [Test]
        public void GenerateTimeSlots_Contains9AM()
        {
            var service = new TimeSlotService();

            var result = service.GenerateTimeSlots();

            Assert.That(result, Does.Contain(new TimeSpan(9, 0, 0)));
        }

        [Test]
        public void GenerateTimeSlots_DoesNotContain17()
        {
            var service = new TimeSlotService();

            var result = service.GenerateTimeSlots();

            Assert.That(result.Contains(new TimeSpan(17, 0, 0)), Is.False);
        }
    }
}