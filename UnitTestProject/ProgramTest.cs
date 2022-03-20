using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Meeting;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace UnitTestProject
{
    [TestClass]
    public class ProgramTest
    {
        [TestMethod]
        public void FindCorrectJsonFileInDirectory()
        {
            var path = @"..\..\..\Meeting\bin\Debug\meetings.json";
            File.Exists(path);
        }

        [TestMethod]
        public void AddingCustomNamesToPersonsList()
        {
            List<string> nameslist = new List<string>();
            nameslist.Add("vardas");
            nameslist.Add("pavarde");

            Meeting.Meeting meeting = new Meeting.Meeting()
            {
                Persons = nameslist,
            };

            Program program = new Program();
            Assert.AreEqual(string.Join(",", meeting.Persons), string.Join(",", nameslist));
        }

        [TestMethod]
        public void PersonsListContainsResponsiblePerson()
        {
            List<string> nameslist = new List<string>();
            nameslist.Add("Antanas");

            Meeting.Meeting meeting = new Meeting.Meeting()
            {
                ResponsiblePerson = "Jonas",
                Persons = nameslist
            };
            nameslist.Add(meeting.ResponsiblePerson);

            Assert.IsTrue(meeting.Persons.Contains(meeting.ResponsiblePerson));
        }
    }
}
