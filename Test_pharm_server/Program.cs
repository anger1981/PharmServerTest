using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PharmaceuticalInformation.Server;
using System.Data;
using Test_pharm_server.PharmaceuticalInformation.DataTools;
using System.Reflection;
using Microsoft.CSharp;

namespace Test_pharm_server
{
    class Program
    {
        static void Main(string[] args)
        {
            //Server srv = new Server();
            //srv.StartingOfServer();
            //Console.WriteLine("success!!!");
            //Console.ReadLine();

            PhrmInfTESTEntities db = new PhrmInfTESTEntities("Data Source=СЕМЬЯ-ПК\\SQLEXPRESS;Initial Catalog=PharmaceuticalInformation;Integrated Security=True");
            //var prod_10 = db.Products.Take(10);
            //foreach (Product p in prod_10)
            //    Console.WriteLine("ID {0}, name {1}, group {2}", p.Id_Product, p.Name_full, p.Id_product_group);
            DataTable testprod = new DataTable();
            db.Products.Take(10).AsEnumerable().Fill(ref testprod);
            foreach (dynamic t in testprod.AsEnumerable().Where(p => Convert.ToInt32(p.Id_Product) > 1))//
                Console.WriteLine("ID {0}, name {1}, group {2}", t.Id_Product, t.Name_full, t.Id_product_group);
            //{
            //    PropertyInfo[] pis = t.GetType().GetProperties();
            //    foreach (PropertyInfo p in t.GetType().GetProperties())
            //        Console.Write("{0} {1}", p.Name, p.GetValue(t));
            //    Console.WriteLine();
            //}

            Console.ReadLine();
        }
    }
}
