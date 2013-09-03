using System;
using System.Globalization;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Tests.Extensions
{
	public class DateExtensionsTests
	{
		[Test]
		public void GetIsoDateTime()
		{
			var testTime = new DateTime(2013, 11, 17, 13, 6, 1);
			Assert.AreEqual("2013-11-17 13:06:01", testTime.GetIsoDateTime());
			AssertDateTime(testTime, DateTime.Parse(testTime.GetIsoDateTime()));
		}

		private static void AssertDateTime(DateTime expectedDateTime, DateTime actualDateTime)
		{
			Assert.AreEqual(expectedDateTime.GetIsoDateTime(), actualDateTime.GetIsoDateTime());
		}

		[Test]
		public void EmptyStringJustReturnsTheSmallestDate()
		{
			Assert.AreEqual(DateTime.MinValue, DateExtensions.Parse(""));
		}

		[Test]
		public void IncorrectDateStringWillReturnCurrentDateTime()
		{
			AssertDateTime(DateTime.Now, DateExtensions.Parse("2013[08]17"));
			AssertDateTime(DateTime.Now, DateExtensions.Parse("2013[08]17 00"));
		}

		[Test]
		public void ParsePureIsoDate()
		{
			var expectedDate = new DateTime(2013, 8, 21);
			Assert.AreEqual(expectedDate, DateExtensions.Parse(2013 + "-" + 8 + "-" + 21));
		}

		[Test]
		public void CheckIsDateNewer()
		{
			Assert.IsTrue(DateExtensions.IsDateNewer(DateTime.Now, DateTime.Today));
			Assert.IsTrue(DateExtensions.IsDateNewer(DateTime.Today, DateTime.MinValue));
			Assert.IsFalse(DateExtensions.IsDateNewer(DateTime.MinValue, DateTime.MaxValue));
		}

		[Test]
		public void GetDateTimeFromString()
		{
			var isoDateTime = DateExtensions.Parse("2013-08-22 22:37:46");
			var englishDateTime = DateExtensions.Parse("08/22/2013 10:37:46 PM");
			Assert.AreEqual(isoDateTime, englishDateTime);
		}
	}
}