using System.Collections.Generic;

namespace XsdToObject.Modelo
{
    public class Xsd
    {
        public IList<Nodo> Nodos { get; set; }
        public class Nodo
        {
            public string Nombre { get; set; }
            public IList<Nodo> Nodos { get; set; }
            public IList<Atributo> Atributos { get; set; }
        }
        public class Atributo
        {
            public string Nombre { get; set; }
        }
    }
}
