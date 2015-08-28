using System.Collections.Generic;
using System.Xml;
using XsdToObject.Modelo;

namespace XsdToObject
{
    public class ReadXsd
    {
        public Xsd Procesar(XmlDocument documento)
        {
            var xsdModel = new Xsd
            {
                Nodos = new List<Xsd.Nodo>()
            };

            foreach (XmlElement nodoElemento in documento.DocumentElement.ChildNodes)
            {
                if (nodoElemento.LocalName == "element")
                {
                    var readXsd = new ReadXsd();
                    xsdModel.Nodos.Add(readXsd.Nodo(nodoElemento, documento));
                }
            }

            return xsdModel;
        }
        private Xsd.Nodo Nodo(XmlElement elemento, XmlDocument documento, IList<Xsd.Nodo> nodos = null)
        {
            if (elemento.LocalName == "any") return null;

            //sequence [SubNodos]
            if (elemento.LocalName == "sequence")
            {
                foreach (XmlElement nodoElemento in elemento.ChildNodes)
                {
                    var readXsd = new ReadXsd();
                    var nodoResultado = readXsd.Nodo(nodoElemento, documento);

                    if (nodoResultado != null)
                        nodos.Add(nodoResultado);
                }

                return null;
            }

            //[Namespace]
            var manager = new XmlNamespaceManager(documento.NameTable);
            manager.AddNamespace("ns", "http://www.w3.org/2001/XMLSchema");

            var nodoModel = new Xsd.Nodo
            {
                Nombre = elemento.Attributes.GetNamedItem("name").Value,
                Atributos = new List<Xsd.Atributo>(),
                Nodos = new List<Xsd.Nodo>()
            };

            //type [Atributos Externos]
            if (elemento.HasAttribute("type"))
            {
                var complexTypeNodoNodo = ObtenerAtributosPorTipo(documento, elemento.Attributes.GetNamedItem("type").Value, manager);
                foreach (XmlElement complexTypeAtributos in complexTypeNodoNodo.ChildNodes)
                {
                    if (complexTypeAtributos.LocalName == "attribute")
                    {
                        var atributoModel = new Xsd.Atributo { Nombre = complexTypeAtributos.Attributes.GetNamedItem("name").Value };
                        nodoModel.Atributos.Add(atributoModel);
                    }
                }
            }

            //complexType [SubNodos]
            foreach (XmlElement nodoComplexType in elemento.LastChild.ChildNodes)
            {
                //Nodos
                if (nodoComplexType.LocalName == "sequence" || nodoComplexType.LocalName == "choice")
                {
                    foreach (XmlElement nodoElemento in nodoComplexType.ChildNodes)
                    {
                        var readXsd = new ReadXsd();
                        //sequence [SubNodos]
                        if (nodoElemento.LocalName == "sequence")
                        {
                            readXsd.Nodo(nodoElemento, documento, nodoModel.Nodos);
                            continue;
                        }

                        var nodoResultado = readXsd.Nodo(nodoElemento, documento);
                        if (nodoResultado != null)
                            nodoModel.Nodos.Add(nodoResultado);
                    }

                }
                //Atributos del nodo
                if (nodoComplexType.LocalName == "attribute")
                {
                    var atributoModel = new Xsd.Atributo
                    {
                        Nombre = nodoComplexType.Attributes.GetNamedItem("name").Value
                    };
                    nodoModel.Atributos.Add(atributoModel);
                }
            }

            return nodoModel;
        }

        private XmlNode ObtenerAtributosPorTipo(XmlDocument xmlDocumento, string nombre, XmlNamespaceManager manager)
        {
            if (nombre.Contains(":"))
                return xmlDocumento.DocumentElement.SelectNodes("//ns:complexType[@name='" + nombre.Split(':')[1] + "']", manager)[0];

            return xmlDocumento.DocumentElement.SelectNodes("//ns:complexType[@name='" + nombre + "']", manager)[0];
        }
    }
}
