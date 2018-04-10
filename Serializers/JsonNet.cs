using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    // http://www.newtonsoft.com/json
    public class JsonNet<T> : TestBase<T,JsonSerializer> where T : class
    {
        public JsonNet(Func<int, T> testData, Action<T> dataToucher) : base(testData, dataToucher)
        {
            FormatterFactory = () => new JsonSerializer();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
        {
            var text = new StreamWriter(stream);
            Formatter.Serialize(text, obj);
            text.Flush();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
        {
            TextReader text = new StreamReader(stream);
            return (T)Formatter.Deserialize(text, typeof(T));
        }
    }
}
