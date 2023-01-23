// See https://aka.ms/new-console-template for more information
using PlaceFiller;
using System.Drawing;
using System.Linq;

internal class Program
{
    public static string Path2022 = "C:\\Users\\Tobias Deyle\\source\\repos\\PlaceFiller\\2022.csv";
    public static int Total2022 = 160353104;
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        TwentySevenTeen twentySevenTeen = new TwentySevenTeen();
        twentySevenTeen.Calc2017(false);

        //TwentyTwentyTwo twentyTwentyTwo = new TwentyTwentyTwo();
        //twentyTwentyTwo.CountData();
        //twentyTwentyTwo.ReadDataIntoDatabase();
        //twentyTwentyTwo.CreateDatasets();
        //twentyTwentyTwo.CreateCameraPositons();

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("All done.");
        Console.ReadLine();
    }

    //private static void Calc2022()
    //{
    //    using (StreamReader reader = new StreamReader(Path2022))
    //    {
    //        reader.ReadLine();
    //        int counter = 0;
    //        double percentage = 0;
    //        double oldPercentage = 0;

    //        for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
    //        {
    //            // Percent on screen
    //            counter++;
    //            if (counter % 10000 == 0)
    //            {
    //                percentage = (double)counter / Total2022 * 100;
    //                if (oldPercentage != percentage)
    //                {
    //                    Console.Write("\rProgress: {0:F2}%", percentage);
    //                    oldPercentage = percentage;
    //                }
    //            }
    //        }

    //        Console.WriteLine("Total things: " + counter.ToString());
    //    }
    //}
}
