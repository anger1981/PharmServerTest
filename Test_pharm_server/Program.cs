using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PharmaceuticalInformation.Server;

namespace Test_pharm_server
{
    class Program
    {
        static void Main(string[] args)
        {
            Server srv = new Server();
            srv.StartingOfServer();
            Console.WriteLine("success!!!");
            Console.ReadLine();
        }
    }
}
