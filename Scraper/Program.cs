using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Scraper
{
    class Program
    {
        static void Main(string[] args)
        {
            Root flightsInfo = AccessJson();

            //paths
            var path1 = @"..\Debug\cheapestconnectingflights.csv";
            var path2 = @"..\Debug\allconnectingflights.csv";
            var path3 = @"..\Debug\alldirectflights.csv";
            var path4 = @"..\Debug\cheapestdirectflights.csv";

            //all flights variables
            bool connectingFlight = GetConnectionsNumber();
            var AllPrices = GetAllPrices(flightsInfo);
            var AllTaxes = GetAllTaxes();
            var AllOutboundFlights = OutboundJourneys();
            var AllInboundFlights = InboundJourneys();

            //cheapest flights variables
            var LowestPrice = GetLowestPrice().ToString();
            var LowestTax = GetLowestTax().ToString();
            var CheapOutboundFlights = GetCheapestFlights();
            var CheapInboundFlights = GetCheapestInboundFlights();

            if (AllOutboundFlights.Count > 0)
            {
                if (connectingFlight == true)
                {
                    //All connecting Flights
                    for (int i = 0; i <= AllOutboundFlights.Count / 2 - 1; i++)
                    {
                        AddRecordPriceAndTax(AllPrices[i].ToString(), AllTaxes[i].ToString(), path2);
                        AddOutboundRecord(AllOutboundFlights[0 + i * 2], path2);
                        AddOutboundRecord(AllOutboundFlights[1 + i * 2], path2);
                        AddInboundRecord(AllInboundFlights[0 + i * 2], path2);
                        AddInboundRecord(AllInboundFlights[1 + i * 2], path2);
                        AddEmptyRecord(path2);
                    }

                    //Cheapest connecting flights
                    for (int i = 0; i <= CheapOutboundFlights.Count / 2 - 1; i++)
                    {
                        AddRecordPriceAndTax(LowestPrice.ToString(), LowestTax.ToString(), path1);
                        AddOutboundRecord(CheapOutboundFlights[0 + i * 2], path1);
                        AddOutboundRecord(CheapOutboundFlights[1 + i * 2], path1);
                        AddInboundRecord(CheapInboundFlights[0 + i * 2], path1);
                        AddInboundRecord(CheapInboundFlights[1 + i * 2], path1);
                        AddEmptyRecord(path1);
                    }
                }

                if (connectingFlight == false)
                {
                    //All direct  flights
                    for (int i = 0; i <= AllOutboundFlights.Count - 1; i++)
                    {
                        AddRecordPriceAndTax(AllPrices[i].ToString(), AllTaxes[i].ToString(), path3);
                        AddOutboundRecord(AllOutboundFlights[i], path3);
                        AddInboundRecord(AllInboundFlights[i], path3);
                        AddEmptyRecord(path3);
                    }

                    //Cheapest direct flights
                    for (int i = 0; i <= CheapOutboundFlights.Count - 1; i++)
                    {
                        AddRecordPriceAndTax(LowestPrice.ToString(), LowestTax.ToString(), path4);
                        AddOutboundRecord(AllOutboundFlights[i], path4);
                        AddInboundRecord(AllInboundFlights[i], path4);
                        AddEmptyRecord(path4);
                    }
                }
            }
            else
            {
                Console.WriteLine("Skrydziu pagal parinktus parametrus nera");
            }

            Console.WriteLine("pabaiga");
            Console.ReadLine();
        }

        private static List<double> GetAllPrices(Root flightsInfo)
        {
            var listoutbound = new List<Journey>();
            var listinbound = new List<Journey>();
            var listflight = new List<Flight>();
            int id = GetAllIds(flightsInfo);
            foreach (var item in flightsInfo.body.data.journeys)
            {
                if (item.direction == "I")
                {
                    Journey kelione = new Journey()
                    {
                        recommendationId = item.recommendationId,
                        identity = item.identity,
                        direction = item.direction,
                        importTaxAdl = item.importTaxAdl,
                        flights = item.flights

                    };
                    listoutbound.Add(kelione);
                }
            }

            foreach (var item in flightsInfo.body.data.journeys)
            {
                if (item.direction == "V")
                {
                    Journey kelione = new Journey()
                    {
                        recommendationId = item.recommendationId,
                        identity = item.identity,
                        direction = item.direction,
                        importTaxAdl = item.importTaxAdl,
                        flights = item.flights

                    };
                    listinbound.Add(kelione);
                }
            }

            double price = 0;
            var pricelist = new List<double>();
            var result = listoutbound.Where(x => listinbound.Any(z => z.recommendationId == x.recommendationId && x.identity == z.identity));
            foreach (var item in result)
            {
                if (item.direction == "I")
                {
                    foreach (var cost in flightsInfo.body.data.totalAvailabilities)
                    {
                        if (item.recommendationId == cost.recommendationId)
                        {
                            price = cost.total;
                            pricelist.Add(price);
                        }
                    }
                }
            }

            return pricelist;
        }

        private static double GetLowestPrice()
        {
            Root flightsInfo = AccessJson();
            double lowestPrice = flightsInfo.body.data.totalAvailabilities.Min(c => c.total);
            return lowestPrice;
        }

        private static double GetLowestTax()
        {
            Root flightsInfo = AccessJson();

            double tax1 = 0;
            double tax2 = 0;
            double sum = 0;

            int id = GetCheapestID(flightsInfo);

            foreach (var item in flightsInfo.body.data.journeys)
            {
                if (item.direction.Contains("I") && item.recommendationId == id)
                {
                    tax1 = item.importTaxAdl;
                }
                if (item.direction.Contains("V") && item.recommendationId == id)
                {
                    tax2 = item.importTaxAdl;

                }
                sum = tax1 + tax2;
            }
            return sum;
        }

        private static int GetCheapestID(Root flightsInfo)
        {
            double lowprice = GetLowestPrice();

            int id = 0;

            foreach (var item in flightsInfo.body.data.totalAvailabilities)
            {
                if (item.total == lowprice)
                {
                    id = item.recommendationId;
                }
            }
            return id;
        }

        private static List<Flight> GetCheapestFlights()
        {
            Root flightsInfo = AccessJson();
            var Cheapestid = GetCheapestID(flightsInfo);
            var cheapestprice = GetLowestPrice();
            var list = new List<Flight>();

            var listoutbound = new List<Journey>();
            var listinbound = new List<Journey>();
            var listflight = new List<Flight>();

            foreach (var item in flightsInfo.body.data.journeys)
            {
                if (item.direction == "I")
                {
                    Journey kelione = new Journey()
                    {
                        recommendationId = item.recommendationId,
                        identity = item.identity,
                        direction = item.direction,
                        importTaxAdl = item.importTaxAdl,
                        flights = item.flights

                    };
                    listoutbound.Add(kelione);
                }
            }

            foreach (var item in flightsInfo.body.data.journeys)
            {
                if (item.direction == "V")
                {
                    Journey kelione = new Journey()
                    {
                        recommendationId = item.recommendationId,
                        identity = item.identity,
                        direction = item.direction,
                        importTaxAdl = item.importTaxAdl,
                        flights = item.flights

                    };
                    listinbound.Add(kelione);
                }
            }
            var result = listoutbound.Where(x => listinbound.Any(z => z.recommendationId == x.recommendationId && x.identity == z.identity));

            foreach(var item in result)
            {
                foreach(var flight in item.flights)
                {
                    if (item.recommendationId == Cheapestid)
                    {
                        list.Add(flight);
                    }
                }
            }

            return list;
        }

        private static List<Flight> GetCheapestInboundFlights()
        {
            Root flightsInfo = AccessJson();
            var Cheapestid = GetCheapestID(flightsInfo);
            var cheapestprice = GetLowestPrice();
            var list = new List<Flight>();

            var listoutbound = new List<Journey>();
            var listinbound = new List<Journey>();
            var listflight = new List<Flight>();

            foreach (var item in flightsInfo.body.data.journeys)
            {
                if (item.direction == "I")
                {
                    Journey kelione = new Journey()
                    {
                        recommendationId = item.recommendationId,
                        identity = item.identity,
                        direction = item.direction,
                        importTaxAdl = item.importTaxAdl,
                        flights = item.flights

                    };
                    listoutbound.Add(kelione);
                }
            }

            foreach (var item in flightsInfo.body.data.journeys)
            {
                if (item.direction == "V")
                {
                    Journey kelione = new Journey()
                    {
                        recommendationId = item.recommendationId,
                        identity = item.identity,
                        direction = item.direction,
                        importTaxAdl = item.importTaxAdl,
                        flights = item.flights

                    };
                    listinbound.Add(kelione);
                }
            }
            var result = listinbound.Where(x => listoutbound.Any(z => z.recommendationId == x.recommendationId && x.identity == z.identity));

            foreach (var item in result)
            {
                foreach (var flight in item.flights)
                {
                    if (item.recommendationId == Cheapestid)
                    {
                        list.Add(flight);
                    }
                }
            }

            return list;
        }

        private static int GetAllIds(Root flightsInfo)
        {
            int id = 0;

            foreach (var item in flightsInfo.body.data.totalAvailabilities)
            {
                for (int i = 0; i < flightsInfo.body.data.totalAvailabilities.Count; i++)
                {
                    id = item.recommendationId;
                }
            }
            return id;
        }
        private static List<double> GetAllTaxes()
        {
            Root flightsInfo = AccessJson();

            double tax1 = 0;
            double tax2 = 0;
            double sum = 0;

            var list = new List<double>();

            var listoutbound = new List<Journey>();
            var listinbound = new List<Journey>();
            var listflight = new List<Flight>();
            int id = GetAllIds(flightsInfo);
            
            foreach (var item in flightsInfo.body.data.journeys)
            {
                if (item.direction == "I")
                {
                    Journey kelione = new Journey()
                    {
                        recommendationId = item.recommendationId,
                        identity = item.identity,
                        direction = item.direction,
                        importTaxAdl = item.importTaxAdl,
                        flights = item.flights

                    };
                    listoutbound.Add(kelione);
                }
            }

            foreach (var item in flightsInfo.body.data.journeys)
            {
                if (item.direction == "V")
                {
                    Journey kelione = new Journey()
                    {
                        recommendationId = item.recommendationId,
                        identity = item.identity,
                        direction = item.direction,
                        importTaxAdl = item.importTaxAdl,
                        flights = item.flights

                    };
                    listinbound.Add(kelione);
                }
            }
            var result = listoutbound.Where(x => listinbound.Any(z => z.recommendationId == x.recommendationId && x.identity == z.identity));
            var result2 = listinbound.Where(x => listoutbound.Any(z => z.recommendationId == x.recommendationId && x.identity == z.identity));
            foreach (var item in result)
            {
                if (item.direction.Contains("I"))
                {
                    tax1 = item.importTaxAdl;
                }
            }
            foreach (var item in result2)
            {
                if (item.direction.Contains("V"))
                {
                    tax2 = item.importTaxAdl;

                }
            }

            for (int i = 0; i < listoutbound.Count; i++)
            {
                sum = tax1 + tax2;
                list.Add(sum);
            }
            return list;
        }

        private static bool GetConnectionsNumber()
        {
            Root flightsInfo = AccessJson();
            var numberlist = new List<string>();
            var idlist = new List<int>();
            bool connecting;
            foreach (var item in flightsInfo.body.data.journeys)
            {
                idlist.Add(item.recommendationId);
                if (item.direction == "I" || item.direction == "V")
                {

                    foreach (var flight in item.flights)
                    {
                        numberlist.Add(flight.number);
                    }
                }
            }

            if (numberlist.Count / idlist.Count != 2)
            {
                connecting = false;
            }
            else
            {
                connecting = true;
            }
            return connecting;
        }

        private static List<Flight> OutboundJourneys()
        {
            Root flightsInfo = AccessJson();
            var listoutbound = new List<Journey>();
            var listinbound = new List<Journey>();
            var listflight = new List<Flight>();
            int id = GetAllIds(flightsInfo);
            foreach (var item in flightsInfo.body.data.journeys)
            {
                if (item.direction == "I")
                {
                    Journey kelione = new Journey()
                    {
                        recommendationId = item.recommendationId,
                        identity = item.identity,
                        direction = item.direction,
                        importTaxAdl = item.importTaxAdl,
                        flights = item.flights

                    };
                    listoutbound.Add(kelione);
                }
            }

            foreach (var item in flightsInfo.body.data.journeys)
            {
                if (item.direction == "V")
                {
                    Journey kelione = new Journey()
                    {
                        recommendationId = item.recommendationId,
                        identity = item.identity,
                        direction = item.direction,
                        importTaxAdl = item.importTaxAdl,
                        flights = item.flights

                    };
                    listinbound.Add(kelione);
                }
            }

            //selecting ONLY round flights
            var result = listoutbound.Where(x=> listinbound.Any(z=> z.recommendationId == x.recommendationId && x.identity == z.identity));

            foreach (var item in result)
            {
                //removing flights with more than 1 connection from selection
                if(item.flights.Count <= 2)
                {
                    foreach (var flight in item.flights)
                    {
                        listflight.Add(flight);
                    }
                }
            }
            return listflight;
        }

        private static List<Flight> InboundJourneys()
        {
            Root flightsInfo = AccessJson();
            var listoutbound = new List<Journey>();
            var listinbound = new List<Journey>();
            var listflight = new List<Flight>();
            int id = GetAllIds(flightsInfo);
            foreach (var item in flightsInfo.body.data.journeys)
            {
                if (item.direction == "I")
                {
                    Journey kelione = new Journey()
                    {
                        recommendationId = item.recommendationId,
                        identity = item.identity,
                        direction = item.direction,
                        importTaxAdl = item.importTaxAdl,
                        flights = item.flights

                    };
                    listoutbound.Add(kelione);
                }
            }

            foreach (var item in flightsInfo.body.data.journeys)
            {
                if (item.direction == "V")
                {
                    Journey kelione = new Journey()
                    {
                        recommendationId = item.recommendationId,
                        identity = item.identity,
                        direction = item.direction,
                        importTaxAdl = item.importTaxAdl,
                        flights = item.flights

                    };
                    listinbound.Add(kelione);
                }
            }

            var result = listinbound.Where(x => listoutbound.Any(z => z.recommendationId == x.recommendationId && x.identity == z.identity));

            foreach (var item in result)
            {
                foreach (var flight in item.flights)
                {
                    listflight.Add(flight);
                }
            }
            return listflight;
        }

        private static void AddOutboundRecord(Flight flight, string path)
        {
            try
            {
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.Write(flight.airportDeparture.code + "," + flight.airportArrival.code + " , " + flight.dateDeparture + " , " + flight.dateArrival + " ," + flight.companyCode+flight.number + " , ");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private static void AddInboundRecord(Flight flight, string path)
        {
            try
            {
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.Write(flight.airportDeparture.code + "," + flight.airportArrival.code + " , " + flight.dateDeparture + " , " + flight.dateArrival + " ," + flight.companyCode + flight.number + " , ");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private static void AddEmptyRecord(string path)
        {
            try
            {
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void AddRecordPriceAndTax(string price, string taxes, string path)
        {
            try
            {
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.Write(price + " , " + taxes + " , ");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static Root AccessJson()
        {
            Task<string> html = GetHTML();

            var flightsInfo = JsonConvert.DeserializeObject<Root>(html.Result);
            return flightsInfo;
        }

        private static Task<string> GetHTML()
        {
            var url = "http://homeworktask.infare.lt/search.php?from=MAD&to=FUE&depart=2023-02-09&return=2023-02-16";
            //var url = "http://homeworktask.infare.lt/search.php?from=MAD&to=AUH&depart=2022-06-17&return=2022-12-22";
            //var url = "http://homeworktask.infare.lt/search.php?from=MAD&to=FUE&depart=2023-05-27&return=2025-01-01";
            //var url = "http://homeworktask.infare.lt/search.php?from=MAD&to=AUH&depart=2022-10-13&return=2023-03-24";
            //var url = "http://homeworktask.infare.lt/search.php?from=MAD&to=AUH&depart=2022-06-09&return=2022-06-30";
            //var url = "http://homeworktask.infare.lt/search.php?from=MAD&to=FUE&depart=2022-06-09&return=2022-06-30";
            //var url = "http://homeworktask.infare.lt/search.php?from=MAD&to=FUE&depart=2024-06-09&return=2024-08-30";
            //var url = "http://homeworktask.infare.lt/search.php?from=JFK&to=FUE&depart=2024-06-01&return=2024-12-30";
            //var url = "http://homeworktask.infare.lt/search.php?from=MAD&to=AUH&depart=2025-01-05&return=2025-02-01";
            //var url = "http://homeworktask.infare.lt/search.php?from=MAD&to=FUE&depart=2025-01-05&return=2025-02-01";

            var httpClient = new HttpClient();
            var html = httpClient.GetStringAsync(url);
            return html;
        }

        public class TotalAvailability
        {
            public int recommendationId { get; set; }
            public double total { get; set; }
        }

        public class AirportDeparture
        {
            public string code { get; set; }
            public string description { get; set; }
        }

        public class AirportArrival
        {
            public string code { get; set; }
            public string description { get; set; }
        }

        public class Flight
        {
            public string number { get; set; }
            public AirportDeparture airportDeparture { get; set; }
            public AirportArrival airportArrival { get; set; }
            public string dateDeparture { get; set; }
            public string dateArrival { get; set; }
            public string companyCode { get; set; }

            public Flight()
            {

            }

            public Flight(string Number, AirportDeparture AirportDeparture, AirportArrival AirportArrival, string DateDeparture, string DateArrival, string CompanyCode)
            {
                number = Number;
                airportDeparture = AirportDeparture;
                airportArrival = AirportArrival;
                dateDeparture = DateDeparture;
                dateArrival = DateArrival;
                companyCode = CompanyCode;
            }
        }
        public class Journey
        {
            public int recommendationId { get; set; }
            public int identity { get; set; }
            public string direction { get; set; }
            public double importTaxAdl { get; set; }
            public List<Flight> flights { get; set; }

            public Journey()
            {

            }
            public Journey(int recommendID, int Identity, string Direction, double ImportTax, List<Flight> Flights)
            {
                recommendationId = recommendID;
                identity = Identity;
                direction = Direction;
                importTaxAdl = ImportTax;
                flights = Flights;
            }
        }

        public class AvailabilityFactor
        {
            public string availabilityProviderType { get; set; }
            public string availabilityProviderReasonType { get; set; }
        }

        public class Data
        {
            public List<TotalAvailability> totalAvailabilities { get; set; }
            public List<Journey> journeys { get; set; }
            public AvailabilityFactor availabilityFactor { get; set; }
            public string availabilityZoneType { get; set; }
        }

        public class Body
        {
            public Data data { get; set; }
        }

        public class Root
        {
            public Body body { get; set; }
        }


    }
}
