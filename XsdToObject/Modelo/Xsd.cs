using System.Collections.Generic;

namespace XsdToObject.Modelo
{
    public class Xsd
    {
        public IList<Nodo> Nodos { get; set; }
        public class Nodo
        {
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public IList<Nodo> Nodos { get; set; }
            public IList<Atributo> Atributos { get; set; }
            /// <summary>
            ///   Contiene mas de un elemento
            /// </summary>
            public bool Ilimitado { get; set; }
        }
        public class Atributo
        {
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public bool Requerido { get; set; }
        }
    }
}
