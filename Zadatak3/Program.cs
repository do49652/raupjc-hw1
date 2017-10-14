using System;

namespace Zadatak3
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var stringList = new GenericList<string>();

            stringList.Add("1");
            stringList.Add("asd");
            stringList.Add("asdasdew");
            stringList.Add("234234");
            stringList.Add("1111111");
            stringList.Add("1444");

            // foreach
            foreach (var value in stringList)
                Console.WriteLine(value);
            // foreach without the syntax sugar
            var enumerator = stringList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var value = enumerator.Current;
                Console.WriteLine(value);
            }
            Console.ReadLine();
        }
    }
}