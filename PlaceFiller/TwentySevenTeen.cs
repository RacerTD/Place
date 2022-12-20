using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaceFiller
{
    public class TwentySevenTeen
    {
        public string Path2017 = "C:\\Users\\Tobias Deyle\\source\\repos\\PlaceFiller\\2017.csv";
        public int Total2017 = 16559897;

        public void Calc2017()
        {
            //PlaceCoodinate[] placeCoodinates = new PlaceCoodinate[Total2017];
            List<PlaceCoodinate> placeCoodinates = new List<PlaceCoodinate>();

            ReadCoordinatesToList(ref placeCoodinates, Path2017);
            OrderList(ref placeCoodinates);

            int counter = 0;
            double percentage = 0;


            //Console.WriteLine();

            //Console.WriteLine("Now starting the crazy shit");

            //List<int> indexesToDelete = new List<int>();
            //int currentMinute = 0;
            //int counterAtStartOfMinute = 0;
            //List<Coodinates> checkedCoordinates = new List<Coodinates>();

            //for (int i = 0; i < placeCoodinates.Count; i++)
            //{
            //    if (placeCoodinates[i].TimeStamp.Minute != currentMinute)
            //    {
            //        Console.WriteLine("Switched to new Minute: " + currentMinute.ToString());
            //        Console.WriteLine("Current Index: " + i.ToString() + ", Minute Start Index: " + counterAtStartOfMinute.ToString());

            //        int steps = i - counterAtStartOfMinute;

            //        Coodinates coodinate = new Coodinates(0, 0);

            //        // Switch minute here
            //        for (int j = i - 1; j >= counterAtStartOfMinute; j--)
            //        {
            //            coodinate = new Coodinates(placeCoodinates[j].X, placeCoodinates[j].Y);

            //            if (checkedCoordinates.Contains(coodinate))
            //            {
            //                indexesToDelete.Add(j);
            //            }
            //            else
            //            {
            //                checkedCoordinates.Add(coodinate);
            //            }

            //            if (j % (int)((double)steps / 10000) == 0)
            //            {
            //                percentage = (double)(i - j) / steps * 100;
            //                Console.Write("\rProgress: {0:F2}%", percentage);
            //            }
            //        }

            //        Console.WriteLine("Coordinates Killed so far: " + indexesToDelete.Count);

            //        checkedCoordinates.Clear();

            //        currentMinute = placeCoodinates[i].TimeStamp.Minute;
            //        counterAtStartOfMinute = i;
            //    }
            //}
        }

        /// <summary>
        /// Reads all placeCoodinates from the path and puts them into a provided list
        /// </summary>
        /// <param name="coodinates"></param>
        /// <param name="path"></param>
        private void ReadCoordinatesToList(ref List<PlaceCoodinate> coodinates, string path)
        {
            coodinates = new List<PlaceCoodinate>();

            Console.WriteLine();
            Console.WriteLine($"Start reading from path: {path}");

            using (StreamReader reader = new StreamReader(path))
            {
                // Skip first Line
                reader.ReadLine();
                int counter = 0;
                double percentage = 0;
                PlaceCoodinate coodinate = null;

                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    coodinate = PlaceCoodinate.CreateFrom2017(line);
                    coodinates.Add(coodinate);

                    // Percent on screen
                    counter++;
                    if (counter % 1000 == 0)
                    {
                        percentage = (double)counter / Total2017 * 100;
                        Console.Write("\rProgress: {0:F2}%", percentage);
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Success, {coodinates.Count()} items were loaded from the disk");
        }

        /// <summary>
        /// Orders a giant list of PlaceCoodinates by the datetime timestamp
        /// </summary>
        /// <param name="coodinates"></param>
        private void OrderList(ref List<PlaceCoodinate> coodinates)
        {
            Console.WriteLine();
            Console.WriteLine("Starting sorting");

            coodinates = coodinates.OrderBy(p => p.TimeStamp.Ticks).ThenBy(p => p.TimeStamp.Millisecond).ToList();

            Console.WriteLine();
            Console.WriteLine("Sorting finished");
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
