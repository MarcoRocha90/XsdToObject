using System;
using System.Xml;
using XsdToObject;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            const string path = @"C:\ejemplo.xsd";

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(path);

            var resultado = new ReadXsd().Procesar(xmlDoc);

            Console.WriteLine("Press any key to exit");
        }
    }
}
