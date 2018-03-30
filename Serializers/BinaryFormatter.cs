using System;
using System.IO;

namespace SerializerTests
{
    public class BinaryFormatter<T> : TestBase<T, System.Runtime.Serialization.Formatters.Binary.BinaryFormatter> where T : class
    {
        public BinaryFormatter(Func<int, T> testData, Action<T> dataToucher) : base(testData, dataToucher)
        {
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
