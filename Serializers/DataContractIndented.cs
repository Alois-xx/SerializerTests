using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Runtime.CompilerServices;
using static fastJSON.Reflection;

namespace SerializerTests.Serializers
{
    [SerializerType("https://docs.microsoft.com/en-us/dotnet/api/system.runtime.serialization.datacontractserializer",
                    SerializerTypes.Xml | SerializerTypes.SupportsVersioning)]
    class DataContractIndented<TSerialize> : TestBase<TSerialize, TSerialize, DataContractSerializer> where TSerialize : class
    {
        public DataContractIndented(Func<int, TSerialize> testData, Action<TSerialize,int,int> touchAndVerify, bool refTracking = false) : base(testData, touchAndVerify, refTracking)
        {
            base.CustomSerialize = SerializeXmlIndented;
            base.CustomDeserialize = DeserializeXmlIndented;
            FormatterFactory = () => new DataContractSerializer(typeof(TSerialize), new DataContractSerializerSettings
                                                                          { PreserveObjectReferences = RefTracking });
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        void SerializeXmlIndented(MemoryStream stream)
        {
            var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true });
            Formatter.WriteObject(xmlWriter, TestData);
            xmlWriter.Flush();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        TSerialize DeserializeXmlIndented(MemoryStream stream)
        {
            var xmlReader = XmlReader.Create(stream);
            TSerialize deserialized = (TSerialize)Formatter.ReadObject(xmlReader);
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
