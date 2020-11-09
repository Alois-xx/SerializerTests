using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    [SerializerType("https://docs.microsoft.com/en-us/dotnet/api/system.runtime.serialization.datacontractserializer",
                    SerializerTypes.Binary | SerializerTypes.Xml | SerializerTypes.SupportsVersioning)]
    public class DataContractBinaryXml<T> : TestBase<T, DataContractSerializer> where T : class
    {
        public DataContractBinaryXml(Func<int, T> testData, Action<T,int,int> touchAndVerify, bool refTracking = false) : base(testData, touchAndVerify, refTracking)
        {
            base.CustomSerialize = SerializeBinaryXml;
            base.CustomDeserialize = DeserializeBinaryXml;
            FormatterFactory = () => new DataContractSerializer(typeof(T), new DataContractSerializerSettings
                                                                           { PreserveObjectReferences = RefTracking });

        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        void SerializeBinaryXml(MemoryStream stream)
        {
            var binaryWriter = XmlDictionaryWriter.CreateBinaryWriter(stream);
            Formatter.WriteObject(binaryWriter, TestData);
            binaryWriter.Flush();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
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
