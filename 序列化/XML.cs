using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;//XmlNormalUserCase使用

namespace ALDI.Core
{
    /// <summary>
    /// 序列化、反序列化XML文件
    /// copyright:  Zac (suoxd123@126.com)
    /// 转自fish li的思路
    /// </summary>
    public static class XMLHelper
    {
        /// <summary>
        /// 将对象按XML格式，序列化到数据流中
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="o"></param>
        /// <param name="coder"></param>
        private static void XmlSerializeStream(Stream stream, object o, Encoding coder)
        {
            if (o == null)
                throw new ArgumentNullException("o");
            if (coder == null)
                throw new ArgumentNullException("coder");

            XmlSerializer serializer = new XmlSerializer(o.GetType());

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = coder;
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";

            //settings.OmitXmlDeclaration = true;  //加这个会去掉XML中的头：<?xml version="1.0" encoding="utf-8"?>
            //XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            //namespaces.Add(string.Empty, string.Empty);

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, o);
                //serializer.Serialize(writer, o, namespaces); //加空命名空间会去掉根节点的属性：xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
                writer.Close();
            }
        }

        /// <summary>
        /// 将对象序列化为XML字符串
        /// </summary>
        /// <param name="o"></param>
        /// <param name="coder"></param>
        /// <returns></returns>
        public static string XmlSerialize(object o, Encoding coder)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializeStream(stream, o, coder);
                stream.Position = 0;

                StreamReader reader = new StreamReader(stream, coder);
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// 将XML字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <param name="coder"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(string strXml, Encoding coder)
        {
            if (string.IsNullOrEmpty(strXml))
                throw new ArgumentNullException("strXml");
            if (coder == null)
                throw new ArgumentNullException("coder");

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (MemoryStream stream = new MemoryStream(coder.GetBytes(strXml)))
            {
                StreamReader reader = new StreamReader(stream, coder);
                return (T)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// 直接将XML文件内容反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="coder"></param>
        /// <returns></returns>
        public static T XmlDeserializeFromFile<T>(string path, Encoding coder)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            if (coder == null)
                throw new ArgumentNullException("coder");

            string strXml = File.ReadAllText(path, coder);
            return XmlDeserialize<T>(strXml, coder);
        }
    }

    /// <summary>
    /// 对上面XMLHelper的使用示例
    /// </summary>
    public class XmlUserCase
    {
        #region 实体类定义
        [XmlType("instance")]   //若无instance，则【节点名】默认为类名xmlObject
        public class xmlObject
        {
            [XmlIgnore]         // 这个属性将不会参与序列化
            public int ignore { get; set; }

            [XmlAttribute("id")]//若无id，则【属性名】默认为属性名attribute
            public int attribute { get; set; }

            [XmlElement("name")] //若无name，则【节点名】默认为属性名element
            public string element { get; set; }

            [XmlText]           //标识为内容
            public string content { get; set; }
        }

        public class ListObject
        {
            public xmlObject XmlObj { get; set; }

            //[XmlElement("RemoveListName")] //用这个替代下面两个属性（不可同时存在）后，会去掉listNodeName这一层
            [XmlArrayItem("element")]//若无element，则【节点名】默认为泛型类型名xmlObject
            [XmlArray("listNodeName")]//若无listNodeName，则【节点名】默认为属性名List
            public List<xmlObject> List { get; set; }
        }

        #endregion

        #region 序列化示例
        /// <summary>
        /// 类对象
        /// </summary>
        public void ClassSerialize()
        {
            xmlObject o1 = new xmlObject {ignore=11, attribute = 1, element = "one", content = "Test First;" };

            string strXml = XMLHelper.XmlSerialize(o1, Encoding.UTF8);

            Console.WriteLine(strXml);
            //<?xml version="1.0" encoding="utf-8"?>
            //<instance xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" id="1">
            //<name>one</name>Test First;</instance>
        }

        /// <summary>
        /// 单纯列表、数组
        /// </summary>
        public void ArraySerialize()
        {
            xmlObject o1 = new xmlObject { ignore = 11, attribute = 1, element = "one", content = "Test First;" };
            xmlObject o2 = new xmlObject { ignore = 22, attribute = 2, element = "two", content = "Test Second;" };
            List<xmlObject> o = new List<xmlObject> { o1, o2 };
            xmlObject[] oo = new xmlObject[] {o1,o2 };

            string strXml = XMLHelper.XmlSerialize(o, Encoding.UTF8);
            string strXml2 = XMLHelper.XmlSerialize(oo, Encoding.UTF8);

            Console.WriteLine(strXml);
            Console.WriteLine(strXml2);//两个的结果一模一样

            //<?xml version="1.0" encoding="utf-8"?>
            //<ArrayOfInstance xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
            //<instance id="1">
            //    <name>one</name>Test First;</instance>
            //<instance id="2">
            //    <name>two</name>Test Second;</instance>
            //</ArrayOfInstance>
        }

        /// <summary>
        /// 列表和类对象一起
        /// </summary>
        public void ListSerialize()
        {
            xmlObject o1 = new xmlObject { ignore = 11, attribute = 1, element = "one", content = "Test First;" };
            xmlObject o2 = new xmlObject { ignore = 22, attribute = 2, element = "two", content = "Test Second;" };
            xmlObject o3 = new xmlObject { ignore = 33, attribute = 3, element = "three", content = "Test Third;" };
            ListObject o = new ListObject() { XmlObj = o1, List = new List<xmlObject> { o2, o3 } };

            string strXml = XMLHelper.XmlSerialize(o, Encoding.UTF8);

            Console.WriteLine(strXml);
            //<?xml version="1.0" encoding="utf-8"?>
            //<ListObject xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
            //        <XmlObj id="1">
            //                <name>one</name>Test First;</XmlObj>
            //        <listNodeName>
            //                <element id="2">
            //                        <name>two</name>Test Second;</element>
            //                <element id="3">
            //                        <name>three</name>Test Third;</element>
            //        </listNodeName>
            //</ListObject>

            //ListObject类中用XmlElement代替另外两个时候的输出
            //============================================================================
            //<?xml version="1.0" encoding="utf-8"?>
            //<ListObject xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
            //        <XmlObj id="1">
            //                <name>one</name>Test First;</XmlObj>
            //        <RemoveListName id="2">
            //                <name>two</name>Test Second;</RemoveListName>
            //        <RemoveListName id="3">
            //                <name>three</name>Test Third;</RemoveListName>
            //</ListObject>
        }

        #endregion

        #region 反序列化示例

        /// <summary>
        /// 类对象
        /// </summary>
        public void ClassDeserialize()
        {
            xmlObject o1 = new xmlObject { ignore = 11, attribute = 1, element = "one", content = "Test First;" };

            string strXml = XMLHelper.XmlSerialize(o1, Encoding.UTF8);

            xmlObject xo = XMLHelper.XmlDeserialize<xmlObject>(strXml, Encoding.UTF8);
            Console.WriteLine("attribute:{0}\telement:{1}\tconent:{2}\tignore:{3}", xo.attribute, xo.element, xo.content,xo.ignore);
            //attribute:1     element:one     conent:Test First;      ignore:0
        }

        /// <summary>
        /// 单纯列表、数组
        /// </summary>
        public void ArrayDeserialize()
        {
            xmlObject o1 = new xmlObject { ignore = 11, attribute = 1, element = "one", content = "Test First;" };
            xmlObject o2 = new xmlObject { ignore = 22, attribute = 2, element = "two", content = "Test Second;" };
            List<xmlObject> o = new List<xmlObject> { o1, o2 };
            xmlObject[] oo = new xmlObject[] { o1, o2 };

            string strXml = XMLHelper.XmlSerialize(o, Encoding.UTF8);
            string strXml2 = XMLHelper.XmlSerialize(oo, Encoding.UTF8);

            //xmlObject[]直接替换List<xmlObject>则完全一样的结果
            List<xmlObject> lo1 = XMLHelper.XmlDeserialize<List<xmlObject>>(strXml, Encoding.UTF8);
            foreach (xmlObject xo1 in lo1)
            {
                Console.WriteLine("attribute:{0}\telement:{1}\tconent:{2}\tignore:{3}", xo1.attribute, xo1.element, xo1.content, xo1.ignore);
            }
            //attribute:1     element:one     conent:Test First;      ignore:0
            //attribute:2     element:two     conent:Test Second;     ignore:0
        }

        /// <summary>
        /// 列表和类对象一起
        /// </summary>
        public void ListDeserialize()
        {
            xmlObject o1 = new xmlObject { ignore = 11, attribute = 1, element = "one", content = "Test First;" };
            xmlObject o2 = new xmlObject { ignore = 22, attribute = 2, element = "two", content = "Test Second;" };
            xmlObject o3 = new xmlObject { ignore = 33, attribute = 3, element = "three", content = "Test Third;" };
            ListObject o = new ListObject() { XmlObj = o1, List = new List<xmlObject> { o2, o3 } };

            string strXml = XMLHelper.XmlSerialize(o, Encoding.UTF8);

            ListObject xo = XMLHelper.XmlDeserialize<ListObject>(strXml, Encoding.UTF8);
            foreach (xmlObject item in xo.List)
            {
                Console.WriteLine("attribute:{0}\telement:{1}\tconent:{2}\tignore:{3}", item.attribute, item.element, item.content, item.ignore);
            } 
            Console.WriteLine("attribute:{0}\telement:{1}\tconent:{2}\tignore:{3}", xo.XmlObj.attribute, xo.XmlObj.element, xo.XmlObj.content, xo.XmlObj.ignore);
            //attribute:2     element:two     conent:Test Second;     ignore:0
            //attribute:3     element:three   conent:Test Third;      ignore:0
            //attribute:1     element:one     conent:Test First;      ignore:0
        }
        #endregion

        #region 优化结果
        
        #endregion
    }

    /// <summary>
    /// 用Linq原生方法示例
    /// </summary>
    public class XmlNormalUserCase
    {
        #region  实体类定义
        [XmlType("instance")]   //若无instance，则【节点名】默认为类名xmlObject
        public class xmlObject
        {
            [XmlIgnore]         // 这个属性将不会参与序列化
            public int ignore { get; set; }

            [XmlAttribute("id")]//若无id，则【属性名】默认为属性名attribute
            public int attribute { get; set; }

            [XmlElement("name")] //若无name，则【节点名】默认为属性名element
            public string element { get; set; }

            [XmlText]           //标识为内容
            public string content { get; set; }
        }
        #endregion

        #region XML内容的序列化、反序列化
        /// <summary>
        /// 将XML字符串反序列化为xmlObject对象
        /// </summary>
        /// <param name="strXml"></param>
        /// <returns></returns>
        public xmlObject XmlDeserialize(string strXml)
        {
            //XElement item = XElement.Load(fileName);  //直接从XML文件读取
            XElement item = XElement.Parse(strXml);
            xmlObject xmlObj = new xmlObject();
            xmlObj.attribute = int.Parse(item.Attribute("id").Value);
            xmlObj.element = item.Element("name").Value;
            xmlObj.content = item.Value;

            return xmlObj;
        }

        /// <summary>
        /// 将xmlObject对象序列化为XML字符串
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public string XmlSerialize(xmlObject o)
        {
            XElement item = new XElement("instance",o.content);
            item.Add(new XAttribute("id", o.attribute));
            item.Add(new XElement("name", o.element));

            //item.Save(fileName);//直接保存为XML文件
            using (MemoryStream stream = new MemoryStream())
            {
                item.Save(stream);
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }
        #endregion

        public void TestUse()
        {
            xmlObject o1 = new xmlObject { ignore = 11, attribute = 1, element = "one", content = "Test First;" };
            string strXml = XmlSerialize(o1);
            Console.WriteLine(strXml);
            Console.WriteLine("========");

            xmlObject xo1 = XmlDeserialize(strXml);
            Console.WriteLine("attribute:{0}\telement:{1}\tconent:{2}\tignore:{3}", xo1.attribute, xo1.element, xo1.content, xo1.ignore);

            //<?xml version="1.0" encoding="utf-8"?>
            //<instance id="1">Test First;<name>one</name></instance>
            //========
            //attribute:1     element:one     conent:Test First;one   ignore:0
        }
    }
}
