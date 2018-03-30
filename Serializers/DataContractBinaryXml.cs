using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace SerializerTests.Serializers
{
    public class DataContractBinaryXml<T> : TestBase<T, DataContractSerializer> where T : class
    {
        public DataContractBinaryXml(Func<int, T> testData, Action<T> dataToucher) : base(testData, dataToucher)
        {
            base.CustomSerialize = SerializeBinaryXml;
            base.CustomDeserialize = DeserializeBinaryXml;
            FormatterFactory = () => new DataContractSerializer(typeof(T));

        }

        void SerializeBinaryXml(MemoryStream stream)
        {
            var binaryWriter = XmlDictionaryWriter.CreateBinaryWriter(stream);
            Formatter.WriteObject(binaryWriter, TestData);
            binaryWriter.Flush();
        }

        T DeserializeBinaryXml(MemoryStream stream)
        {
            var binaryReader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max);
            T deserialized = (T) Formatter.ReadObject(binaryReader);
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
