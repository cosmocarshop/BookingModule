using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Dnn.BookingModule.BookingModule.Models;

namespace Dnn.BookingModule.Tests
{
    [TestFixture]
    public class BookingTests
    {
        /*
         * TEST 1
         * ============================================================
         * MIT TESZTELÜNK:
         * A ProductBvins → SerializedProductBvins konverziót (GET).
         * 
         * MIÉRT FONTOS:
         * Az adatbázisban stringként tároljuk a listát.
         * Ha ez hibás:
         * - rossz adatok kerülnek mentésre
         * - később nem lehet visszaolvasni
         * 
         * MIT VÁRUNK:
         * A lista elemei vesszővel elválasztva jelennek meg.
         */

        [Test]
        public void SerializedProductBvins_Get_ConvertsListToCommaSeparatedString()
        {
            var booking = new Booking
            {
                ProductBvins = new List<string> { "A", "B", "C" }
            };

            var result = booking.SerializedProductBvins;

            Assert.That(result, Is.EqualTo("A,B,C"));
        }

        /*
         * TEST 2
         * ============================================================
         * MIT TESZTELÜNK:
         * A SerializedProductBvins → ProductBvins konverziót (SET).
         * 
         * MIÉRT FONTOS:
         * Ez történik adatbázisból való betöltéskor.
         * Ha hibás:
         * - a UI rossz adatot kap
         * - booking szolgáltatások elvesznek
         * 
         * MIT VÁRUNK:
         * A string megfelelően listává alakul.
         */

        [Test]
        public void SerializedProductBvins_Set_ConvertsStringToList()
        {
            var booking = new Booking();

            booking.SerializedProductBvins = "A,B,C";

            Assert.That(booking.ProductBvins.Count, Is.EqualTo(3));
            Assert.That(booking.ProductBvins, Does.Contain("A"));
            Assert.That(booking.ProductBvins, Does.Contain("B"));
            Assert.That(booking.ProductBvins, Does.Contain("C"));
        }

        /*
         * TEST 3
         * ============================================================
         * MIT TESZTELÜNK:
         * Üres érték kezelése a setter-ben.
         * 
         * MIÉRT FONTOS:
         * Az adatbázis gyakran ad vissza:
         * - null
         * - üres string
         * 
         * Ha ezt nem kezeljük:
         * → NullReferenceException
         * 
         * MIT VÁRUNK:
         * Üres lista jön létre
         */

        [Test]
        public void SerializedProductBvins_Set_EmptyString_ResultsInEmptyList()
        {
            var booking = new Booking();

            booking.SerializedProductBvins = "";

            Assert.That(booking.ProductBvins, Is.Not.Null);
            Assert.That(booking.ProductBvins.Count, Is.EqualTo(0));
        }

        /*
         * TEST 4 (edge case)
         * ============================================================
         * MIT TESZTELÜNK:
         * A setter kiszűri az üres elemeket (",,").
         * 
         * MIÉRT FONTOS:
         * Hibás vagy manipulált adat esetén előfordulhat:
         * "A,,B,,C"
         * 
         * MIT VÁRUNK:
         * Az üres elemek eltűnnek
         */

        [Test]
        public void SerializedProductBvins_Set_IgnoresEmptyEntries()
        {
            var booking = new Booking();

            booking.SerializedProductBvins = "A,,B,,C";

            Assert.That(booking.ProductBvins.Count, Is.EqualTo(3));
            Assert.That(booking.ProductBvins, Is.EqualTo(new List<string> { "A", "B", "C" }));
        }

        /*
         * TEST 5 (round-trip teszt – nagyon fontos)
         * ============================================================
         * MIT TESZTELÜNK:
         * GET + SET együtt (round-trip consistency).
         * 
         * MIÉRT FONTOS:
         * Ez a legfontosabb adat integritási teszt:
         * - amit elmentünk
         * - ugyanazt kapjuk vissza
         * 
         * MIT VÁRUNK:
         * Az eredeti lista változatlan marad
         */

        [Test]
        public void SerializedProductBvins_RoundTrip_PreservesData()
        {
            var originalList = new List<string> { "X", "Y", "Z" };

            var booking = new Booking
            {
                ProductBvins = originalList
            };

            var serialized = booking.SerializedProductBvins;

            var newBooking = new Booking
            {
                SerializedProductBvins = serialized
            };

            Assert.That(newBooking.ProductBvins, Is.EqualTo(originalList));
        }
    }
}