using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /*
         * TEST 1
         * ============================================================
         * MIT TESZTELÜNK:
         * A GenerateTimeSlots metódus visszatérési darabszámát.
         * 
         * MIÉRT FONTOS:
         * Ez az alap üzleti logika: 9:00 és 17:00 között óránkénti
         * bontásban kell slotokat generálni.
         * 
         * LOGIKA:
         * 9:00 → 16:00 (17:00 már nincs benne, mert < endTime)
         * 
         * ELVÁRT EREDMÉNY:
         * 8 darab időpont
         * 
         * HIBA, AMIT ELKAP:
         * - rossz ciklus feltétel (<= vs <)
         * - rossz interval
         */

        [Test]
        public void GenerateTimeSlots_ReturnsExactly8Slots()
        {
            var service = new TimeSlotService();

            var result = service.GenerateTimeSlots();

            Assert.That(result.Count, Is.EqualTo(8));
        }

        /*
         * TEST 2
         * ============================================================
         * MIT TESZTELÜNK:
         * A kezdő időpont helyességét (boundary test).
         * 
         * MIÉRT FONTOS:
         * Gyakori hiba, hogy a kezdő érték elcsúszik (pl. 10:00-ről indul).
         * Ez azonnal hibás foglalási rendszert eredményezne.
         * 
         * ELVÁRT:
         * Az első elem pontosan 09:00
         * 
         * HIBA, AMIT ELKAP:
         * - rossz startTime
         * - hibás inicializálás
         */

        [Test]
        public void GenerateTimeSlots_FirstSlotIs9AM()
        {
            var service = new TimeSlotService();

            var result = service.GenerateTimeSlots();

            var first = result.First();

            Assert.That(first, Is.EqualTo(new TimeSpan(9, 0, 0)));
        }

        /*
         * TEST 3
         * ============================================================
         * MIT TESZTELÜNK:
         * A végpont helyességét és az off-by-one hibák kizárását.
         * 
         * MIÉRT FONTOS:
         * Időintervallumoknál az egyik leggyakoribb bug:
         * - belekerül a 17:00 (pedig nem kéne)
         * - vagy kimarad a 16:00
         * 
         * ELVÁRT:
         * - 16:00 benne van
         * - 17:00 nincs benne
         * 
         * HIBA, AMIT ELKAP:
         * - <= használata < helyett
         * - rossz ciklus logika
         */

        [Test]
        public void GenerateTimeSlots_LastSlotIs16_AndDoesNotContain17()
        {
            var service = new TimeSlotService();

            var result = service.GenerateTimeSlots();

            var last = result.Last();

            // Utolsó helyes slot
            Assert.That(last, Is.EqualTo(new TimeSpan(16, 0, 0)));

            // 17:00 NEM lehet benne
            Assert.That(result, Does.Not.Contain(new TimeSpan(17, 0, 0)));
        }

        /*
         * TEST 4
         * ============================================================
         * MIT TESZTELÜNK:
         * Az időpontok közötti különbséget (interval consistency).
         * 
         * MIÉRT FONTOS:
         * Nem elég, hogy jó számú slot van — az intervallumnak is
         * konzisztensnek kell lennie (1 óra).
         * 
         * ELVÁRT:
         * Minden egymást követő slot között 1 óra különbség van.
         */

        [Test]
        public void GenerateTimeSlots_EachSlotIsOneHourApart()
        {
            var service = new TimeSlotService();

            var result = service.GenerateTimeSlots();

            for (int i = 1; i < result.Count; i++)
            {
                var diff = result[i] - result[i - 1];

                Assert.That(diff, Is.EqualTo(TimeSpan.FromHours(1)));
            }
        }
    }
}