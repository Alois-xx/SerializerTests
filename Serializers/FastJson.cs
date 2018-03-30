using System;
using System.IO;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// https://github.com/mgholam/fastJSON/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class FastJson<T> : TestBase<T, fastJSON.JSONParameters> where T : class
    {
        public FastJson(Func<int, T> testData, Action<T> dataToucher) : base(testData, dataToucher)
        {
        }

        protected override void Serialize(T obj, Stream stream)
        {
            var text = new StreamWriter(stream);
            var jsonString = fastJSON.JSON.ToJSON(obj);
            text.WriteLine(jsonString);
            text.Flush();
        }

        protected override T Deserialize(Stream stream)
        {
            TextReader text = new StreamReader(stream);
            string fullText = text.ReadToEnd();
            var lret = fastJSON.JSON.ToObject<T>(fullText);
            return lret;
        }
    }
}
