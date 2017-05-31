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
            //PhrmInfTESTEntities db = new PhrmInfTESTEntities("Data Source=aqua;Initial Catalog=PharmaceuticalInformationTEST;Integrated Security=True");
            //var prod_10 = db.Products.Take(10);
            //foreach (Product p in prod_10)
            //    Console.WriteLine("ID {0}, name {1}, group {2}", p.Id_Product, p.Name_full, p.Id_product_group);

            //Console.ReadLine();
        }
    }
}
