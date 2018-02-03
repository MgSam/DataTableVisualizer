using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DataTableViewer.Tests
{
    [TestFixture]
    public class IndexerODataFilterLanguageTests
    {
        private NoThrowDictionary<string, object> _dict = new NoThrowDictionary<String, Object>(new Dictionary<String, Object>(StringComparer.OrdinalIgnoreCase)
        {
            {"Name", "George"},
            {"Age", 25},
            {"Weight", 145.5},
            {"DOB", new DateTime(1990, 01, 05)},
            {"IsJedi", false},
            {"Address", "145" },
            {"Penne", "Pasta" },
            {"MyMinute", 5 }
        });

        [TestCase(01, true, "Name eq 'George'")]
        [TestCase(02, true, "name eq 'George'")]
        [TestCase(03, true, "name eq 'george'")]
        [TestCase(04, true, "age eq 25")]
        [TestCase(05, true, "weight eq 145.50")]
        [TestCase(06, true, "dob eq '1990-01-05'")]
        [TestCase(07, true, "IsJedi eq false")]
        [TestCase(08, false, "IsJedi eq true")]
        [TestCase(09, true, "isjedi eq false")]

        [TestCase(10, true, "Name ne 'Dave'")]
        [TestCase(11, false, "Name ne 'George'")]
        [TestCase(12, true, "Age ne 35")]
        [TestCase(13, true, "Weight ne 145.51")]
        [TestCase(14, true, "DOB ne '1990-01-06'")]
        [TestCase(15, true, "IsJedi ne true")]

        [TestCase(16, true, "Name gt 'Fred'")]
        [TestCase(17, true, "Age gt 24")]
        [TestCase(18, true, "Weight gt 145.0")]
        [TestCase(19, true, "DOB gt '1990-01-01'")]

        [TestCase(20, true, "Name ge 'Fred'")]
        [TestCase(21, true, "Name ge 'George'")]
        [TestCase(22, true, "Age ge 24")]
        [TestCase(23, true, "Age ge 25")]
        [TestCase(24, true, "Weight ge 145.0")]
        [TestCase(25, true, "Weight ge 145.5")]
        [TestCase(26, true, "DOB ge '1990-01-01'")]
        [TestCase(27, true, "DOB ge '1990-01-05'")]

        [TestCase(28, true, "Name lt 'Harry'")]
        [TestCase(29, true, "Age lt 26")]
        [TestCase(30, true, "Weight lt 147.0")]
        [TestCase(31, true, "DOB lt '1990-01-08'")]

        [TestCase(32, true, "Name le 'Harry'")]
        [TestCase(33, true, "Name le 'George'")]
        [TestCase(34, true, "Age le 26")]
        [TestCase(35, true, "Age le 25")]
        [TestCase(36, true, "Weight le 146.0")]
        [TestCase(37, true, "Weight le 145.5")]
        [TestCase(38, true, "DOB le '1990-01-08'")]
        [TestCase(39, true, "DOB le '1990-01-05'")]

        [TestCase(40, true, "Name eq 'Fred' or DOB eq '1990-01-05'")]
        [TestCase(41, true, "Name eq 'George' and DOB eq '1990-01-05'")]
        [TestCase(42, true, "not (Name eq 'Fred')")]
        [TestCase(43, true, "DOB ne '1990-01-05' or not (Name eq 'Fred')")]
        [TestCase(44, true, "Weight eq 145.5 and Age eq 25")]

        [TestCase(45, true , "Address eq '145'")]
        [TestCase(46, true , "Name ct 'eo'")]
        [TestCase(47, false, "Name ct 'f'")]
        [TestCase(48, true , "DOB ct '1990'")]
        [TestCase(49, false, "DOB ct '1492'")]
        [TestCase(50, true , "Name rx 'e$'")]
        [TestCase(51, true , "IsJedi rx '^fa'")]
        [TestCase(52, false, "IsJedi rx 'tr'")]
        [TestCase(53, true , "Address rx '1.5'")]

        //Field names that contain operators or functions
        [TestCase(54, true , "penne ct 'asta'")]
        [TestCase(55, true, "myminute eq 5")]
        [TestCase(56, true, "address rx '5$'")]
        public void ParseTest(int testNum, bool expected, String oDataExpression)
        {
            var func = new IndexerODataFilterLanguage().Parse<NoThrowDictionary<string, object>>(oDataExpression).Compile();

            var result = func.Invoke(_dict);

            Assert.AreEqual(expected, result);
        }
    }
}
