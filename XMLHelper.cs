using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace ALDI
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

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, o);
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
                StreamReader reader = new StreamReader(stream,coder);
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
}
