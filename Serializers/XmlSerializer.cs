using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SerializerTests.Serializers
{
    public class XmlSerializer<T> : TestBase<T, System.Xml.Serialization.XmlSerializer> where T : class
    {
        public XmlSerializer(Func<int, T> testData, Action<T> dataToucher) : base(testData, dataToucher)
        {
            FormatterFactory = () => new System.Xml.Serialization.XmlSerializer(typeof(T));
        }

        protected override void Serialize(T obj, Stream stream)
        {
            Formatter.Serialize(stream, obj);
        }

        protected override T Deserialize(Stream stream)
        {
            return (T)Formatter.Deserialize(stream);
        }
    }
}
