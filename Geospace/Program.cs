//using System.Net.Http;
//using System;
//using System.Diagnostics;
//using System.Runtime.InteropServices;

//class Program
//{
//    static void Main(string[] args)
//    {
//        var foregroundProcess = IntPtr.Zero;
//        var previousProcess = IntPtr.Zero;

//        while (true)
//        {
//            foregroundProcess = GetForegroundWindow();

//            if (foregroundProcess != previousProcess)
//            {
//                // Log the new foreground process or application
//                var process = Process.GetProcessById(GetProcessId(foregroundProcess));
//                Console.WriteLine($"Foreground Process: {process.ProcessName}");

//                previousProcess = foregroundProcess;
//            }

//            // Adjust the polling interval
//            System.Threading.Thread.Sleep(1000);
//        }
//    }

//    [DllImport("user32.dll")]
//    private static extern IntPtr GetForegroundWindow();

//    [DllImport("user32.dll")]
//    private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
//}

using IPinfo;
using IPinfo.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
//using System.Device.Location;

class Program
{

    static void Main(string[] args)
    {
        //Mains();
        //DateTime currentdate = DateTime.Now;
       //var check =  currentdate.tostring("h:mm tt");
       // Console.WriteLine(check);

    }
    static async void Mains()
    {
        var foregroundProcess = IntPtr.Zero;
        var previousProcessId = 0;
        var previousProcessStartTime = DateTime.MinValue;
        var localIPAddress = GetIPAddress();

        string token = "e310b1062b370b";
        IPinfoClient client = new IPinfoClient.Builder()
            .AccessToken(token)
            .Build();

        string ip = "216.239.36.21";
        IPResponse ipResponse =  client.IPApi.GetDetails(ip);

        var location = await GetLocationFromIPAddress(localIPAddress, token);


        while (true)
        {
            foregroundProcess = GetForegroundWindow();

            if (foregroundProcess != IntPtr.Zero)
            {
                GetWindowThreadProcessId(foregroundProcess, out var currentProcessId);

                if (currentProcessId != previousProcessId)
                {
                    var currentTime = DateTime.Now;
                    var timeSpent = currentTime - previousProcessStartTime;
                    var timeSpentInMinutes = timeSpent.TotalMinutes;
                    // Log the new foreground process or application
                    var process = Process.GetProcessById(currentProcessId);
                    Console.WriteLine($"Foreground Process: {process.ProcessName}");
                    Console.WriteLine($"Time Spent: {timeSpentInMinutes / 60} Hours : {timeSpentInMinutes % 60} Minutes");

                    Console.WriteLine($"IP Address: {localIPAddress}, {location}");
                    Console.WriteLine();
                    //Console.WriteLine($"IPResponse.IP: {ipResponse.IP}");
                    //Console.WriteLine($"IPResponse.City: {ipResponse.City}");
                    //Console.WriteLine($"IPResponse.Latitude: {ipResponse.Latitude}");
                    //Console.WriteLine($"IPResponse.Longitude: {ipResponse.Longitude}");
                    //Console.WriteLine($"IPResponse.CountryName: {ipResponse.CountryName}");

                    //previousProcessId = currentProcessId;
                }
                previousProcessId = currentProcessId;
                previousProcessStartTime = DateTime.Now;
            }

            // Adjust the polling interval
            Thread.Sleep(1000);
        }
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);








    static async Task<string> GetLocationFromIPAddress(string ipAddress, string apiKey)
    {
        //var ipinfo = new IPinfoClient.Builder().AccessToken(apiKey).Build();
        //var ipDetails = await ipinfo.IPApi.GetDetailsAsync(ipAddress);
        
        try
        {
            var ipDetails = new LocationObject();

            using (var client = new HttpClient())
            {
                var message =  client.GetAsync($"https://ipinfo.io?token={apiKey}").Result;
                var body = await message.Content.ReadAsStringAsync();

                ipDetails = JsonConvert.DeserializeObject<LocationObject>(body);
            }

            // You can access various properties from the ipDetails object
            string country = ipDetails.Country;
            string region = ipDetails.Region;
            string city = ipDetails.City;
            string latitude = ipDetails.Loc.Split(',')[0];
            string longitude = ipDetails.Loc.Split(',')[1];

            // You can format the location information as needed
            string locationInfo = $"{city}, {region}, {country} (Lat: {latitude}, Long: {longitude})";

            return locationInfo;
        }
        catch (Exception ex) 
        {
            throw new Exception(ex.Message);
        }
    }
    private static string GetIPAddress()
    {

        StringBuilder sb = new StringBuilder();
        String strHostName = string.Empty;
        strHostName = Dns.GetHostName();
        //sb.Append("The Local Machine Host Name: " + strHostName);
        //sb.AppendLine();

        IPHostEntry ipHostEntry = Dns.GetHostEntry(strHostName);
        IPAddress[] address = ipHostEntry.AddressList;

        //foreach (IPAddress addr in address)
        //{
        //    sb.Append("The Local IP Address: " + addr.ToString());
        //}
        sb.Append(address[1].ToString());
        sb.AppendLine();

        //return sb.ToString();
        return address[1].ToString();


    }

    public class LocationObject
    {

        public string Ip { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Loc { get; set; }
        public string Org { get; set; }
        public string Timezone { get; set; }
    }

    //private async static string GetLocation()
    //{
    //    string token = "e310b1062b370b";
    //    IPinfoClient client = new IPinfoClient.Builder()
    //        .AccessToken(token)
    //        .Build();

    //    //string ip = "216.239.36.21";

    //    string ip = "216.239.36.21";
    //    IPResponse ipResponse = await client.IPApi.GetDetailsAsync(ip);

    //    return ipResponse;
    //}






}


