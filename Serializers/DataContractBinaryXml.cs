using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    [SerializerType("https://docs.microsoft.com/en-us/dotnet/api/system.runtime.serialization.datacontractserializer",
                    SerializerTypes.Binary | SerializerTypes.Xml | SerializerTypes.SupportsVersioning)]
    public class DataContractBinaryXml<TSerialize> : TestBase<TSerialize, TSerialize, DataContractSerializer> where TSerialize : class
    {
        public DataContractBinaryXml(Func<int, TSerialize> testData, Action<TSerialize,int,int> touchAndVerify, bool refTracking = false) : base(testData, touchAndVerify, refTracking)
        {
            base.CustomSerialize = SerializeBinaryXml;
            base.CustomDeserialize = DeserializeBinaryXml;
            FormatterFactory = () => new DataContractSerializer(typeof(TSerialize), new DataContractSerializerSettings
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
        TSerialize DeserializeBinaryXml(MemoryStream stream)
        {
            var binaryReader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max);
            TSerialize deserialized = (TSerialize) Formatter.ReadObject(binaryReader);
            return deserialized;
        }


        protected override void Serialize(TSerialize obj, Stream stream)
        {
            throw new NotImplementedException();
        }

        protected override TSerialize Deserialize(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
