using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaceFiller
{
    public static class TwentySevenTeen
    {
        public static string Path2017 = "C:\\Users\\Tobias Deyle\\source\\repos\\PlaceFiller\\2017.csv";
        public static int Total2017 = 16559897;

        public static void Calc2017()
        {
            //PlaceCoodinate[] placeCoodinates = new PlaceCoodinate[Total2017];
            List<PlaceCoodinate> placeCoodinates = new List<PlaceCoodinate>();

            using (StreamReader reader = new StreamReader(Path2017))
            {
                reader.ReadLine();
                int counter = 0;
                double percentage = 0;
                double oldPercentage = 0;
                PlaceCoodinate currentPlaceCoodinate;

                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    currentPlaceCoodinate = PlaceCoodinate.CreateFrom2017(line);

                    //if(currentMinute != currentPlaceCoodinate.TimeStamp.Minute)
                    //{
                    //    Console.WriteLine("New Minute " + currentMinute.ToString());

                    //    List<int> x = new List<int>();
                    //    List<int> y = new List<int>();

                    //    for(int i = placeCoodinates.Count - 1; i >= counterAtStartOfMinute; i--)
                    //    {
                    //        if (x.Contains(placeCoodinates[i].X)
                    //            && y.Contains(placeCoodinates[i].Y))
                    //        {
                    //            placeCoodinates.RemoveAt(i);
                    //        }
                    //        else
                    //        {
                    //            x.Add(placeCoodinates[i].X);
                    //            y.Add(placeCoodinates[i].Y);
                    //        }
                    //    }

                    //    currentMinute = currentPlaceCoodinate.TimeStamp.Minute;
                    //    counterAtStartOfMinute = placeCoodinates.Count;
                    //}

                    placeCoodinates.Add(currentPlaceCoodinate);

                    // Percent on screen
                    counter++;
                    if (counter % 1000 == 0)
                    {
                        percentage = (double)counter / Total2017 * 100;
                        if (oldPercentage != percentage)
                        {
                            Console.Write("\rProgress: {0:F2}%", percentage);
                            oldPercentage = percentage;
                        }
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Total things: " + counter.ToString());
                Console.WriteLine("New Total things: " + placeCoodinates.Count.ToString());

                Console.WriteLine("Starting Ordering");
                placeCoodinates = placeCoodinates.OrderBy(p => p.TimeStamp.Ticks).ToList();
                Console.WriteLine("Finished Ordering");

                Console.WriteLine();

                Console.WriteLine("Now starting the crazy shit");

                List<int> indexesToDelete = new List<int>();
                int currentMinute = 0;
                int counterAtStartOfMinute = 0;
                List<Coodinates> checkedCoordinates = new List<Coodinates>();

                for (int i = 0; i < placeCoodinates.Count; i++)
                {
                    if (placeCoodinates[i].TimeStamp.Minute != currentMinute)
                    {
                        Console.WriteLine("Switched to new Minute: " + currentMinute.ToString());
                        Console.WriteLine("Current Index: " + i.ToString() + ", Minute Start Index: " + counterAtStartOfMinute.ToString());

                        int steps = i - counterAtStartOfMinute;

                        Coodinates coodinate = new Coodinates(0, 0);

                        // Switch minute here
                        for (int j = i - 1; j >= counterAtStartOfMinute; j--)
                        {
                            coodinate = new Coodinates(placeCoodinates[j].X, placeCoodinates[j].Y);

                            if (checkedCoordinates.Contains(coodinate))
                            {
                                indexesToDelete.Add(j);
                            }
                            else
                            {
                                checkedCoordinates.Add(coodinate);
                            }

                            if (j % (int)((double)steps / 10000) == 0)
                            {
                                percentage = (double)(i - j) / steps * 100;
                                Console.Write("\rProgress: {0:F2}%", percentage);
                            }
                        }

                        Console.WriteLine("Coordinates Killed so far: " + indexesToDelete.Count);

                        checkedCoordinates.Clear();

                        currentMinute = placeCoodinates[i].TimeStamp.Minute;
                        counterAtStartOfMinute = i;
                    }
                }
            }
        }

        public struct Coodinates
        {
            public int X { get; private set; }
            public int Y { get; private set; }

            public Coodinates(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
    }
}
