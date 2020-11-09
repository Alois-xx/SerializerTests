using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://github.com/mgholam/fastJSON/",
                    SerializerTypes.Json | SerializerTypes.SupportsVersioning)]
    class FastJson<T> : TestBase<T, fastJSON.JSONParameters> where T : class
    {
        public FastJson(Func<int, T> testData, Action<T,int,int> touchAndVerify) : base(testData, touchAndVerify)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
        {
            var text = new StreamWriter(stream);
            var jsonString = fastJSON.JSON.ToJSON(obj);
            text.WriteLine(jsonString);
            text.Flush();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
        {
            // FastJson does not support reading from stream ... but it is much slower anyway so this does not cost much
            TextReader text = new StreamReader(stream);
            string fullText = text.ReadToEnd();
            var lret = fastJSON.JSON.ToObject<T>(fullText);
            return lret;
        }
    }
}
