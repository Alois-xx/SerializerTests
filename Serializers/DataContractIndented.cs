using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace SerializerTests.Serializers
{
    class DataContractIndented<T> : TestBase<T, DataContractSerializer> where T : class
    {
        public DataContractIndented(Func<int, T> testData, Action<T> dataToucher) : base(testData, dataToucher)
        {
            base.CustomSerialize = SerializeXmlIndented;
            base.CustomDeserialize = DeserializeXmlIndented;
            FormatterFactory = () => new DataContractSerializer(typeof(T));
        }

        void SerializeXmlIndented(MemoryStream stream)
        {
            var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true });
            Formatter.WriteObject(xmlWriter, TestData);
            xmlWriter.Flush();
        }

        T DeserializeXmlIndented(MemoryStream stream)
        {
            var xmlReader = XmlReader.Create(stream);
            T deserialized = (T)Formatter.ReadObject(xmlReader);
            return deserialized;
        }


        protected override void Serialize(T obj, Stream stream)
        {
            throw new NotImplementedException();
        }

        protected override T Deserialize(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
