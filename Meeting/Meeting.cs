using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Meeting
{
   public class Meeting
    {
        [JsonProperty("MeetingName")]
        public string MeetingName { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonProperty("ResponsiblePerson")]
        public string ResponsiblePerson { get; set; }
        [JsonProperty("Category")]
        public string Category { get; set; }
        [JsonProperty("Type")]
        public string Type { get; set; }
        [JsonProperty("Persons")]
        public List<string> Persons { get; set; }
        [JsonProperty("StartDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("EndDate")]
        public DateTime EndDate { get; set; }


        public override string ToString()
        {
            return string.Format("Meetings information:\n\nMeeting Name: {0}\nResponsible Person: {1}\nDescription: {2}\nCategory: {3}\nType: {4}\nPersons: {7}\nStart Date: {5}\nEnd Date: {6}"
                ,MeetingName, ResponsiblePerson, Description, Category, Type,StartDate,EndDate, string.Join(",",Persons));
        }
    }
}
