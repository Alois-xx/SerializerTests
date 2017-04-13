using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SerializerTests
{
    public class BinaryFormatter<T> : TestBase<T, System.Runtime.Serialization.Formatters.Binary.BinaryFormatter> where T : class
    {
        public BinaryFormatter(Func<int,T> testData)
        {
            base.CreateNTestData = testData;
            FormatterFactory = () => new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
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
