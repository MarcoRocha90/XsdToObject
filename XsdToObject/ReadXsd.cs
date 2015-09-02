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
 
            var ilimitado = elemento.HasAttribute("maxOccurs") && (elemento.Attributes.GetNamedItem("maxOccurs").Value == "unbounded");
            var descripcion = "";
            if (elemento.FirstChild.LocalName == "annotation")
            {
                descripcion = elemento.FirstChild.InnerText;
            }

            var nodoModel = new Xsd.Nodo
            {
                Nombre = elemento.Attributes.GetNamedItem("name").Value,
                Descripcion = descripcion,
                Atributos = new List<Xsd.Atributo>(),
                Nodos = new List<Xsd.Nodo>(),
                Ilimitado = ilimitado
            };

            //type [Atributos Externos]
            if (elemento.HasAttribute("type"))
            {
                var complexTypeNodoNodo = ObtenerAtributosPorTipo(documento, elemento.Attributes.GetNamedItem("type").Value, manager);
                foreach (XmlElement complexTypeAtributos in complexTypeNodoNodo.ChildNodes)
                {
                    var requerido = complexTypeAtributos.HasAttribute("use") && (complexTypeAtributos.Attributes.GetNamedItem("use").Value == "required");
                    var descripcionAttr = "";
                    if (complexTypeAtributos.FirstChild.LocalName == "annotation")
                    {
                        descripcionAttr = complexTypeAtributos.FirstChild.InnerText;
                    }
                    if (complexTypeAtributos.LocalName == "attribute")
                    {
                        var atributoModel = new Xsd.Atributo
                        {
                            Nombre = complexTypeAtributos.Attributes.GetNamedItem("name").Value,
                            Descripcion = descripcionAttr,
                            Requerido = requerido
                        };
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
                    var requerido = nodoComplexType.HasAttribute("use") && (nodoComplexType.Attributes.GetNamedItem("use").Value == "required");
                    var descripcionAttr = "";
                    if (nodoComplexType.FirstChild.LocalName == "annotation")
                    {
                        descripcionAttr = nodoComplexType.FirstChild.InnerText;
                    }
                    var atributoModel = new Xsd.Atributo
                    {
                        Nombre = nodoComplexType.Attributes.GetNamedItem("name").Value,
                        Requerido = requerido,
                        Descripcion = descripcionAttr
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
