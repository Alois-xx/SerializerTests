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
    class FastJson<TSerialize> : TestBase<TSerialize, TSerialize, fastJSON.JSONParameters> where TSerialize : class
    {
        public FastJson(Func<int, TSerialize> testData, Action<TSerialize,int,int> touchAndVerify) : base(testData, touchAndVerify)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            var text = new StreamWriter(stream);
            var jsonString = fastJSON.JSON.ToJSON(obj);
            text.WriteLine(jsonString);
            text.Flush();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TSerialize Deserialize(Stream stream)
        {
            // FastJson does not support reading from stream ... but it is much slower anyway so this does not cost much
            TextReader text = new StreamReader(stream);
            string fullText = text.ReadToEnd();
            var lret = fastJSON.JSON.ToObject<TSerialize>(fullText);
            return lret;
        }
    }
}
