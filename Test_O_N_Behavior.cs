using SerializerTests.Serializers;
using SerializerTests.TypesToSerialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerializerTests
{
    class Test_O_N_Behavior
    {
        List<ISerializeDeserializeTester> SerializersToTest;

        public Test_O_N_Behavior(List<ISerializeDeserializeTester> serializersToTest)
        {
            SerializersToTest = serializersToTest;
        }
  
        public  void TestSerialize(int nObjects = 1000 * 1000, int nRuns = 5)
        {
            Console.WriteLine(GetHeader(true));
            foreach (var formatter in SerializersToTest)
            {
                // capture first time init effects by executing serialize/deserialize with N=1
                for (int NBooks = 1; NBooks <= nObjects; NBooks += 100 * 1000)
                {
                    var times = formatter.TestSerialize(nTimes: nRuns, nObjectsToCreate: NBooks);
                    Print(formatter, NBooks, times);

                    if (NBooks == 1)
                    {
                        NBooks = 0;
                    }
                }
            }
        }

        public  void TestDeserialize(int nObjects=1000*1000, int nRuns=5)
        {
            Console.WriteLine(GetHeader(false));
            foreach (var formatter in SerializersToTest)
            {
                // capture first time init effects by executing serialize/deserialize with N=1
                for (int NBooks = 1; NBooks <= nObjects; NBooks += 100 * 1000)
                {
                    var times = formatter.TestDeserialize(nTimes: nRuns, nObjectsToCreate: NBooks);
                    Print(formatter, NBooks, times);

                    if (NBooks == 1)
                    {
                        NBooks = 0;
                    }
                }
            }
        }


        private  void Print(ISerializeDeserializeTester formatter, int NBooks, (double firstS, double averageS, long serializedSize) times)
        {
            string typeName = formatter.GetType().Name;
            typeName = typeName.Substring(0, typeName.Length - 2); // omit generic arg `1 of typename
            Console.WriteLine($"{typeName}<{formatter.GetType().GetGenericArguments()[0].Name}>\t{NBooks}\t{times.averageS:F3}\t{times.serializedSize}\t{formatter.FileVersion}");
        }

         string GetHeader(bool serialize)
        {
            string str = "serialize";
            if (!serialize)
            {
                str = "de" + str;
            }

            return $"Serializer\tObjects\t\"Time to {str} in s\"\t\"Size in bytes\"\tFileVersion";
        }
    }
}
