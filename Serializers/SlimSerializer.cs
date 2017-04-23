using System.IO;
using NFX.Serialization.Slim;
using System;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// https://github.com/aumcode/nfx/tree/master/Source/NFX/Serialization/Slim
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class SlimSerializer<T> : TestBase<T, SlimSerializer> where T:class
    {
       
        public SlimSerializer(Func<int, T> testData)
        { 
             base.CreateNTestData = testData;
             FormatterFactory = () => new SlimSerializer();
        }

        protected override T Deserialize(Stream stream)
        {
            return (T) Formatter.Deserialize(stream);
        }

        protected override void Serialize(T obj, Stream stream)
        {
            Formatter.Serialize(stream, obj);
        }
    }
}
