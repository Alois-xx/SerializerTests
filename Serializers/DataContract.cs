using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SerializerTests.Serializers
{
    public class DataContract<T> : TestBase<T, DataContractSerializer> where T : class
    {
        public DataContract(Func<int,T> testData)
        {
            base.CreateNTestData = testData;
            FormatterFactory = () => new DataContractSerializer(typeof(T));
        }

        protected override void Serialize(T obj, Stream stream)
        {
            Formatter.WriteObject(stream, obj);
        }

        protected override T Deserialize(Stream stream)
        {
            return (T)Formatter.ReadObject(stream);
        }
    }
}
