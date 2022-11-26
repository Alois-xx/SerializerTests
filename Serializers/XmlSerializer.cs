using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://docs.microsoft.com/en-us/dotnet/api/system.xml.serialization.xmlserializer", 
                    SerializerTypes.Xml | SerializerTypes.SupportsVersioning)]
    public class XmlSerializer<TSerialize> : TestBase<TSerialize, TSerialize, System.Xml.Serialization.XmlSerializer> where TSerialize : class
    {
        public XmlSerializer(Func<int, TSerialize> testData, Action<TSerialize,int,int> touchAndVerify) : base(testData, touchAndVerify)
        {
            FormatterFactory = () => new System.Xml.Serialization.XmlSerializer(typeof(TSerialize));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            Formatter.Serialize(stream, obj);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TSerialize Deserialize(Stream stream)
        {
            return (TSerialize)Formatter.Deserialize(stream);
        }
    }
}
