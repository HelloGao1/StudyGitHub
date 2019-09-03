using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Teams.Integration.Fhir.FhirServer
{
    class Program
    {
        static private IDisposable _fhirServerController;
        static public string _baseAddress;


        static void Main(string[] args)
        {

            bool isProduction = Convert.ToBoolean(ConfigurationManager.AppSettings["Production"]);
            //string host = "127.0.0.1";
            string host = "172.16.51.104";
            int port = 8099;
            IPAddress ip = IPAddress.Parse(host);

            if (isProduction)
            {

                host = ConfigurationManager.AppSettings["Host"];
                var hostEntry = Dns.GetHostEntry(host);
                ip = hostEntry.AddressList[0];
                port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
            }

            using (Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                sock.Bind(new IPEndPoint(ip, port)); // Pass 0 here, it means to go looking for a free port
                port = ((IPEndPoint)sock.LocalEndPoint).Port;
                sock.Close();
            }

            // Now use that randomly located port to start up a local FHIR server
            _baseAddress = "http://" + host + ":" + port + "/";
            _fhirServerController = Microsoft.Owin.Hosting.WebApp.Start<Startup>(_baseAddress);

            // Inititalize the server
            //Console.WriteLine($"Initialize the CDR FHIR Server");
            //Console.WriteLine($"BaseURI: {_baseAddress}");
            Console.WriteLine("Initialize the CDR FHIR Server");
            Console.WriteLine(string.Format("BaseURI: ", _baseAddress));


            // Wait for the console to be Completed
            Console.WriteLine();
            Console.WriteLine("Press any key to end the FHIR server ...");
            Console.ReadKey();

            if (_fhirServerController != null)
                _fhirServerController.Dispose();
        }
    }
}
