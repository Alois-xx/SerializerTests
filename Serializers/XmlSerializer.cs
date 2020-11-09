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
    public class XmlSerializer<T> : TestBase<T, System.Xml.Serialization.XmlSerializer> where T : class
    {
        public XmlSerializer(Func<int, T> testData, Action<T,int,int> touchAndVerify) : base(testData, touchAndVerify)
        {
            FormatterFactory = () => new System.Xml.Serialization.XmlSerializer(typeof(T));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
        {
            Formatter.Serialize(stream, obj);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
        {
            return (T)Formatter.Deserialize(stream);
        }
    }
}
