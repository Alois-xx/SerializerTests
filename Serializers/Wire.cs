using System;
using System.IO;
using System.Text;
using Wire;

namespace SerializerTests.Serializers
{
    // https://github.com/rogeralsing/Wire
    class Wire<T> : TestBase<T, Serializer> where T : class
    {
        public Wire(Func<int, T> testData)
        {
            base.CreateNTestData = testData;
            FormatterFactory = () =>
            {
                var lret = new Serializer();
                return lret;
            };
        }

        protected override void Serialize(T obj, Stream stream)
        {
            Formatter.Serialize(obj, stream);
        }

        protected override T Deserialize(Stream stream)
        {
            return Formatter.Deserialize<T>(stream);
        }
    }
}
