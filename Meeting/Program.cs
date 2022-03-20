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
    public class Program
    {

        static void Main(string[] args)
        {
            var path = @"meetings.json";
            string file = File.ReadAllText(path);
            var json = JsonConvert.DeserializeObject<List<Meeting>>(file);

            Console.WriteLine("Welcome to meeting app!");
            Console.WriteLine("What would you like to do?\n");

            Console.WriteLine("1. Create meeting\n2. Delete meeting\n3. Add person to a meeting\n4. Remove person from a meeting\n5. Filter all meetings");

            int response = 0;
            bool tryagain = true;
            while (tryagain)
            {
                try
                {
                    response = int.Parse(Console.ReadLine());
                    tryagain = false;

                }
                catch
                {
                    Console.WriteLine("Enter a number, try again");
                }
            }

            while (response < 1 || response > 5)
            {
                    Console.WriteLine("Incorrent input, try again");
                    response = Convert.ToInt32(Console.ReadLine());
            }

                switch (response)
                {
                    case 1:
                    {
                        AddMeeting(path, json);
                        break;
                    }
                case 2:
                    {
                       DeleteMeeting(path, json);
                        break;
                    }
                case 3:
                    {
                        AddPerson(path, json);
                        break;
                    }
                case 4:
                    {
                        RemovePerson(path, json);
                        break;
                    }
                case 5:
                    {
                        FilterBy(json);
                        break;
                    }
            }
            Console.WriteLine("Program has finished working");
            Console.Read();
        }

       public static void FilterBy(List<Meeting> json)
        {
            Console.WriteLine("How would you like to filter?");
            Console.WriteLine("1. By Description");
            Console.WriteLine("2. By Responsible person");
            Console.WriteLine("3. By Category");
            Console.WriteLine("4. By Type");
            Console.WriteLine("5. By Date");
            Console.WriteLine("6. By Atendees");
            int choice = Convert.ToInt32(Console.ReadLine());

            while (choice < 1 || choice > 6)
            {
                Console.WriteLine("Incorrent input, try again");
                choice = Convert.ToInt32(Console.ReadLine());
            }

            switch (choice)
            {
                case 1:
                    {
                        FilterByDescription(json);
                        break;
                    }
                case 2:
                    {
                        FilterByRespPerson(json);
                        break;
                    }
                case 3:
                    {
                        FilterByCategory(json);
                        break;
                    }
                case 4:
                    {
                        FilterByType(json);
                        break;
                    }
                case 5:
                    {
                        FilterByDate(json);
                        break;
                    }
                case 6:
                    {
                        FilterByAtendees(json);
                        break;
                    }
            }
        }

        private static void AddPerson(string path, List<Meeting> json)
        {
            Console.WriteLine("Who would you like to add?");
            string person = Convert.ToString(Console.ReadLine());
            Console.WriteLine("Who is responsible for the meeting?");
            string responsible = Convert.ToString(Console.ReadLine());

            DateTime date = DateTime.Now;
            bool datecheck = true;
            while (datecheck)
                try
                {
                    Console.WriteLine("Enter starting date and time YYYY-MM-DD HH:MM :");
                    date = DateTime.Parse(Console.ReadLine());
                    datecheck = false;
                }
                catch
                {
                    Console.WriteLine("Enter valid date: YYYY-MM-DD HH:MM");
                }

            var allmeets = json.Where(pers => pers.ResponsiblePerson.Contains(responsible) && pers.StartDate == date);
            var warning = json.Where(pers => pers.Persons.Contains(person) && pers.EndDate > date);

            if(!allmeets.Any(pers => pers.ResponsiblePerson.Contains(responsible) && pers.StartDate == date))
            {
                Console.WriteLine("There is no such meeting");
            }
            if (warning.Any())
            {
                Console.WriteLine("Person already has a meeting scheduled during this time");
            }

            foreach (var item in allmeets)
            {
                if (item.Persons.Contains(person))
                {
                    Console.WriteLine("Person is already in this meeting");
                    break;
                }

                item.Persons.Add(person);
                Console.WriteLine("Person has been added");
            }

            var jsonconvert = JsonConvert.SerializeObject(json);
            File.WriteAllText(path, jsonconvert);
        }

        private static void RemovePerson(string path, List<Meeting> json)
        {
            Console.WriteLine("Who is responsible for the meeting?");
            string responsible = Convert.ToString(Console.ReadLine());

            DateTime date = DateTime.Now;
            bool datecheck = true;
            while (datecheck)
                try
                {
                    Console.WriteLine("When does the meeting start?");
                    date = DateTime.Parse(Console.ReadLine());
                    datecheck = false;
                }
                catch
                {
                    Console.WriteLine("Enter valid date: YYYY-MM-DD HH:MM");
                }

            Console.WriteLine("Who would you like to remove");
            string removedPers = Convert.ToString(Console.ReadLine());

            var allmeets = json.Where(pers => pers.ResponsiblePerson.Contains(responsible) && pers.StartDate == date);

            if(!allmeets.Any(pers => pers.ResponsiblePerson.Contains(responsible) && pers.StartDate == date))
            {
                Console.WriteLine("There is no such meeting");
            }

            foreach (var person in allmeets)
            {
                if (removedPers == person.ResponsiblePerson)
                {
                    Console.WriteLine("Cannot remove responsible person");
                }
                else if (!string.Join(",", person.Persons).Contains(removedPers))
                {
                    Console.WriteLine("There is no such person in this meeting");
                }
                else
                {
                    Console.WriteLine("Person has been removed");
                    person.Persons.Remove(removedPers);
                }
            }
            var jsonconvert = JsonConvert.SerializeObject(json);
            File.WriteAllText(path, jsonconvert);
        }

        private static void FilterByAtendees(List<Meeting> json)
        {
            Console.WriteLine("Enter a number of atendees");
            int atendees = 0;
                bool tryagain = true;
            while (tryagain)
            {
                try
                {
                  atendees = int.Parse(Console.ReadLine());
                  tryagain = false;
                }
                catch
                {
                    Console.WriteLine("Enter a number, try again");
                }
            }
            var allmeets = json.Where(pers => pers.Persons.Count > atendees);

            foreach (var item in allmeets)
            {
                Console.WriteLine(item);
            }
        }

        private static void FilterByDate(List<Meeting> json)
        {
            Console.WriteLine("What is the date interval that are you looking for?");
            Console.WriteLine("Format: YYYY-MM-DD");
            DateTime startDate = DateTime.Now;
            bool datecheck = true;
            while (datecheck)
                try
                {
                    Console.WriteLine("Enter starting date and time YYYY-MM-DD HH:MM :");
                    startDate = DateTime.Parse(Console.ReadLine());
                    datecheck = false;
                }
                catch
                {
                    Console.WriteLine("Enter valid date: YYYY-MM-DD HH:MM");
                }

            DateTime endDate = DateTime.Now;
            bool datecheck2 = true;
            while (datecheck2)
                try
                {
                    Console.WriteLine("Enter ending date and time YYYY-MM-DD HH:MM :");
                    endDate = DateTime.Parse(Console.ReadLine());
                    datecheck2 = false;
                }
                catch
                {
                    Console.WriteLine("Enter valid date: YYYY-MM-DD HH:MM");
                }

            var allmeets = json.Where(d => d.StartDate > startDate && d.EndDate < endDate);

            foreach (var item in allmeets)
            {
                Console.WriteLine(item);
            }
        }

        private static void FilterByType(List<Meeting> json)
        {
            Console.WriteLine("What type are you looking for?");
            string type = Convert.ToString(Console.ReadLine());
            var allmeets = json.Where(t => t.Type.Contains(type));

            foreach (var item in allmeets)
            {
                Console.WriteLine(item);
            }
        }

        private static void FilterByCategory(List<Meeting> json)
        {
            Console.WriteLine("What category are you looking for?");
            string categ = Convert.ToString(Console.ReadLine());
            var allmeets = json.Where(cat => cat.Category.Contains(categ));

            foreach (var item in allmeets)
            {
                Console.WriteLine(item);
            }
        }

       private static void FilterByRespPerson(List<Meeting> json)
        {
            Console.WriteLine("Who is the responsible person?");
            string person = Convert.ToString(Console.ReadLine());
            var allmeets = json.Where(pers => pers.ResponsiblePerson.Contains(person));

            foreach (var item in allmeets)
            {
                Console.WriteLine(item);
            }
        }

        private static void FilterByDescription(List<Meeting> json)
        {
            Console.WriteLine("What description are you looking for?");
            string description = Convert.ToString(Console.ReadLine());
            var allmeets = json.Where(desc => desc.Description.Contains(description));

            foreach (var item in allmeets)
            {
                Console.WriteLine(item);
            }
        }

        private static void DeleteMeeting(string path, List<Meeting> json)
        {
            Console.WriteLine("What is your name?");
            string name = Convert.ToString(Console.ReadLine());

            var responsiblepers = json.RemoveAll(resp => resp.ResponsiblePerson == name);

            if (responsiblepers < 1)
            {
                Console.WriteLine("You have no meetings");
            }
            else
            {
                Console.WriteLine("All your meetings have been deleted");
            }

            var jsonconvert = JsonConvert.SerializeObject(json);
            File.WriteAllText(path, jsonconvert);
        }

        private static void AddMeeting(string path, List<Meeting> json)
        {

            Console.WriteLine("Meeting name: ");
            string meetingname = Convert.ToString(Console.ReadLine());
            Console.WriteLine("Meeting description: ");
            string meetingdesc = Convert.ToString(Console.ReadLine());
            Console.WriteLine("Responsible Person: ");
            string respperson = Convert.ToString(Console.ReadLine());
            Console.WriteLine("Choose category: \n1.CodeMonkey\n2.Hub\n3.Short\n4.Teambuilding");
            int meetcat = 0;
            bool tryagain = true;
            while (tryagain)
            {
                try
                {
                    meetcat = int.Parse(Console.ReadLine());
                    tryagain = false;

                }
                catch
                {
                    Console.WriteLine("Enter a number, try again");
                }
            }

            bool tryagain2 = true;
            while (tryagain2)
           {
                    try
                    {
                    while (meetcat < 1 || meetcat > 4)
                    {
                        Console.WriteLine("Incorrent input, try again");
                        meetcat = int.Parse(Console.ReadLine());
                        
                    }
                    tryagain2 = false;
                }
                    catch
                    {
                        Console.WriteLine("Enter a number, try again");
                    }
            }    
                
                string meetcategory = null;
                if (meetcat == 1)
                {
                    meetcategory = "Codemonkey";
                }
                else if (meetcat == 2)
                {
                    meetcategory = "Hub";
                }
                else if (meetcat == 3)
                {
                    meetcategory = "Short";
                }
                else
                {
                    meetcategory = "Teambuilding";
                }
                
            Console.WriteLine("Choose category: \n1.Live\n2.In Person");
            int typecat = 0;
            bool trycat = true;
            while (trycat)
            {
                try
                {
                    typecat = int.Parse(Console.ReadLine());
                    trycat = false;

                }
                catch
                {
                    Console.WriteLine("Enter a number, try again");
                }
            }
            string typecategory = null;
            bool trycat2 = true;
                
                while (trycat2)
                {
                    try
                    {
                        while (typecat < 1 || typecat > 2)
                        {
                            Console.WriteLine("Incorrent input, try again");
                            typecat = int.Parse(Console.ReadLine());
                        }
                        trycat2 = false;
                    }
                    catch
                    {
                        Console.WriteLine("Enter a number, try again");
                    }
                }

            if (typecat == 1)
            {
                typecategory = "Live";
            }
            else
            {
                typecategory = "InPerson";
            }
            Console.WriteLine("Who would you like to add?");
            string name = Convert.ToString(Console.ReadLine());
            List<string> nameslist = new List<string>();
            nameslist.Add(respperson);
            nameslist.Add(name);
            Console.WriteLine("Would you like to add another person? Y/N");
            char answer = 'A'; 
            bool trychar = true;

            //Checking if we want to add people
            while (trychar)
            {
                try
                {
                    answer = char.Parse(Console.ReadLine());
                    while (answer.ToString() != "Y" || answer.ToString() != "N")
                    {
                        if (answer.ToString() == "Y" || answer.ToString() == "N")
                       {
                           trychar = false;
                           break;
                       }
                        Console.WriteLine("Y or N");
                        answer = char.Parse(Console.ReadLine());
                    }
                    if (answer.ToString() == "Y" || answer.ToString() == "N")
                    {
                        trychar = false;
                    }
                }
                catch
                {
                    Console.WriteLine("Enter a letter, try again");
                }
            }

            //Adding more people
               bool trychar2 = true;
               while (trychar2)
               {
                   try
                   {
                       while (answer.ToString() == "Y")
                       {
                           Console.WriteLine("Enter another name");
                           name = Convert.ToString(Console.ReadLine());
                           nameslist.Add(name);
                           Console.WriteLine("Would you like to add another person? Y/N");
                           answer = char.Parse(Console.ReadLine());
                       
                            if (answer.ToString() == "N")
                            {
                               break;
                            }
                       }
                       trychar2 = false;
                   }
                   catch
                   {
                       Console.WriteLine("Enter a letter, try again");
                   }
               }

            DateTime StartDate = DateTime.Now;
            bool datecheck = true;
            while(datecheck)
            try
            {
                Console.WriteLine("Enter starting date and time YYYY-MM-DD HH:MM :");
                StartDate = DateTime.Parse(Console.ReadLine());
                datecheck = false;
            }
            catch
            {
                Console.WriteLine("Enter valid date: YYYY-MM-DD HH:MM");
            }

            DateTime EndDate = DateTime.Now;
            bool datecheck2 = true;
            while (datecheck2)
                try
                {
                    Console.WriteLine("Enter ending date and time YYYY-MM-DD HH:MM :");
                    EndDate = DateTime.Parse(Console.ReadLine());
                    datecheck2 = false;
                }
                catch
                {
                    Console.WriteLine("Enter valid date: YYYY-MM-DD HH:MM");
                }

            bool datecompare = true;
            while (datecompare)
            {
                try
                {
                    while (EndDate < StartDate)
                    {
                        Console.WriteLine("Meeting cannot finish before it started, try again");
                        EndDate = Convert.ToDateTime(Console.ReadLine());
                        if(EndDate > StartDate)
                        {
                            datecompare = false;
                            break;
                        }
                    }
                    datecompare = false;
                }
                catch
                {
                    Console.WriteLine("Enter valid date: YYYY-MM-DD HH:MM");
                }
            }
            Console.WriteLine("Meeting added!");

            var meeting = new Meeting
            {
                MeetingName = meetingname,
                Description = meetingdesc,
                ResponsiblePerson = respperson,
                Category = meetcategory,
                Type = typecategory,
                Persons = nameslist,
                StartDate = StartDate,
                EndDate = EndDate
            };

            json.Add(meeting);
            var jsonconvert = JsonConvert.SerializeObject(json);
            File.WriteAllText(path, jsonconvert);
        }
    }



}
