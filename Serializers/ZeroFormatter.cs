using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFormatter;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// https://github.com/neuecc/ZeroFormatter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ZeroFormatter<T> : TestBase<T, ZeroFormattableAttribute>
    {
        public ZeroFormatter(Func<int, T> testData, Action<T> dataToucher) : base(testData, dataToucher)
        {
        }

        protected override T Deserialize(Stream stream)
        {
            return ZeroFormatterSerializer.Deserialize<T>(stream);
        }

        protected override void Serialize(T obj, Stream stream)
        {
            ZeroFormatterSerializer.Serialize<T>(stream, obj);
        }
    }
}
