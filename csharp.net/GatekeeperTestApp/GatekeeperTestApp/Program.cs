using Newtonsoft.Json;
using System;
using System.Management;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace GatekeeperTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // gatekeeper license check URL
            string gatekeeperCheckLicenseUrl = "https://app.ljnath.com/gatekeeper/v3/licenses/license";

            // name and details of this console application
            string applicationName = "GatekeeperTestApp";
            string applicationVersion = "1.0.1";
            string applicationId = "CSHARP_7599d4dd0bdd4c0db81c58ef4974df3a";

            // node id where this application is running
            string nodeId = Hardware.GetId();

            string appToken = "app-token-created-for-this-application";

            Console.WriteLine($"Starting {applicationName} v{applicationVersion}");
            while (true)
            {
                Console.WriteLine("\nWaiting for 3 seconds before checking for license using Gatekeeper...");
                Thread.Sleep(3000);
                using (HttpClient httpClient = new HttpClient())
                {
                    // add authentication header to the http client
                    httpClient.DefaultRequestHeaders.Add("x-app-token", appToken);

                    // create the license check request payload
                    object requestPayload = new
                    {
                        application_id = applicationId,
                        application_name = applicationName,
                        application_version = applicationVersion,
                        node_id = nodeId
                    };

                    string jsonPayload = JsonConvert.SerializeObject(requestPayload);
                    StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");


                    // make POST call and get the response from the server
                    HttpResponseMessage httpResponseMessage = httpClient.PostAsync(gatekeeperCheckLicenseUrl, content).Result;
                    string responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;

                    Console.WriteLine("License response is: " + responseContent);
                }
            }
        }
    }



    class Hardware
    {

        public static string GetId()
        {
            string hardwareId = String.Empty;
            string processorId = GetProcessorId();
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(processorId));
                foreach (byte b in hashValue)
                    hardwareId += $"{b:X2}";
            }
            return hardwareId;
        }

        private static string GetProcessorId()
        {
            string info = string.Empty;
            try
            {
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor");
                ManagementObjectCollection queryCollection = managementObjectSearcher.Get();
                foreach (ManagementObject managementObject in queryCollection)
                {
                    info = managementObject.ToString().Trim();
                    break;
                }
            }
            catch
            {
                info = string.Empty;
            }
            return info;
        }
    }

}