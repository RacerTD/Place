using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaceFiller
{
    /// <summary>
    /// The PlaceCoodinate stores all relevant information for a given change in the r/place canvas
    /// This includes the time, user, color and coodinate of the pixel that was changed
    /// </summary>
    public class PlaceCoordinate
    {
        public DateTime TimeStamp;
        public string User;
        public short X;
        public short Y;
        public string Color;


        public PlaceCoordinate(DateTime timeStamp, string user, short x, short y, string color)
        {
            TimeStamp = timeStamp;
            User = user;
            X = x;
            Y = y;
            Color = color;
        }

        /// <summary>
        /// Creates a PlaceCoodinate using a string from the 2017 data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static PlaceCoordinate CreateFrom2017(string data)
        {
            // Data Example
            // 2017-04-02 01:09:43.559 UTC,XYoA7YaSBAX2tFn6GOjrfg==,771,440,15

            // Data Split
            // 0. 2017-04-02 01:09:43.559 UTC
            // 1. XYoA7YaSBAX2tFn6GOjrfg==
            // 2. 771
            // 3. 440
            // 4. 15

            // TODO: Update to newly downloaded DataSet

            string[] parts = data.Split(',');
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].Replace(",", "");
            }

            if(parts.Length != 5)
            {
                Console.WriteLine($"more than 5 parts: {data}");
            }

            DateTime timeStamp;

            switch (parts[0].Length)
            {
                case 27:
                    timeStamp = DateTime.ParseExact(parts[0], "yyyy-MM-dd HH:mm:ss.fff UTC", CultureInfo.InvariantCulture);
                    break;

                case 26:
                    timeStamp = DateTime.ParseExact(parts[0], "yyyy-MM-dd HH:mm:ss.ff UTC", CultureInfo.InvariantCulture);
                    break;

                case 25:
                    timeStamp = DateTime.ParseExact(parts[0], "yyyy-MM-dd HH:mm:ss.f UTC", CultureInfo.InvariantCulture);
                    break;

                case 23:
                    timeStamp = DateTime.ParseExact(parts[0], "yyyy-MM-dd HH:mm:ss UTC", CultureInfo.InvariantCulture);
                    break;

                default:
                    Console.WriteLine($"Inconvenient string with {parts[0].Length} chars found: {parts[0]}");
                    timeStamp = DateTime.Now;
                    break;
            }

            string user = parts[1];
            short x = short.Parse(parts[2].Length >= 1 ? parts[2] : "0");
            short y = short.Parse(parts[3].Length >= 1 ? parts[3] : "0");
            byte color = byte.Parse(parts[4].Length >= 1 ? parts[4] : "0");
            return new PlaceCoordinate(timeStamp, user, x, y, ColorPallet.NumberToColor2017(color));
        }

        /// <summary>
        /// Creates a PlaceCoodinate using a string from the 2022 data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<PlaceCoordinate> CreateFrom2022(string data)
        {
            // Data Example
            // 2022-04-04 00:53:51.577 UTC,ovTZk4GyTS1mDQnTbV+vDOCu1f+u6w+CkIZ6445vD4XN8alFy/6GtNkYp5MSic6Tjo/fBCCGe6oZKMAN3rEZHw==,#00CCC0,"826,1048"

            // Data Split
            // 0. 2022-04-04 00:53:51.577 UTC,ovTZk4GyTS1mDQnTbV+vDOCu1f+u6w+CkIZ6445vD4XN8alFy/6GtNkYp5MSic6Tjo/fBCCGe6oZKMAN3rEZHw==,#00CCC0,
            // 1. 826,1048

            // Example for double coordinates
            // 1371,1438,1418,1472

            // Data Part
            // 0. 2022-04-04 00:53:51.577 UTC
            // 1. ovTZk4GyTS1mDQnTbV+vDOCu1f+u6w+CkIZ6445vD4XN8alFy/6GtNkYp5MSic6Tjo/fBCCGe6oZKMAN3rEZHw==
            // 2. #00CCC0

            // TODO: Add case for 4 coordinates instead of 2

            string[] split = data.Split("\"");
            string[] parts = split[0].Split(",");
            string[] coordinates = split[1].Split(",");

            DateTime timeStamp;

            switch (parts[0].Length)
            {
                case 27:
                    timeStamp = DateTime.ParseExact(parts[0], "yyyy-MM-dd HH:mm:ss.fff UTC", CultureInfo.InvariantCulture);
                    break;

                case 26:
                    timeStamp = DateTime.ParseExact(parts[0], "yyyy-MM-dd HH:mm:ss.ff UTC", CultureInfo.InvariantCulture);
                    break;

                case 25:
                    timeStamp = DateTime.ParseExact(parts[0], "yyyy-MM-dd HH:mm:ss.f UTC", CultureInfo.InvariantCulture);
                    break;

                case 23:
                    timeStamp = DateTime.ParseExact(parts[0], "yyyy-MM-dd HH:mm:ss UTC", CultureInfo.InvariantCulture);
                    break;

                default:
                    Console.WriteLine($"Inconvenient string with {parts[0].Length} chars found: {parts[0]}");
                    timeStamp = DateTime.Now;
                    break;
            }

            List<PlaceCoordinate> places = new List<PlaceCoordinate>();

            if(coordinates.Length >= 3)
            {
                short x1 = short.Parse(coordinates[0]);
                short y1 = short.Parse(coordinates[1]);
                short x2 = short.Parse(coordinates[2]);
                short y2 = short.Parse(coordinates[3]);

                for(short x = x1; x <= x2; x++)
                {
                    for(short y = y1; y <= y2; y++)
                    {
                        places.Add(new PlaceCoordinate(timeStamp, parts[1], x, y, parts[2]));
                    }
                }
            }
            else
            {
                places.Add(new PlaceCoordinate(timeStamp, parts[1], short.Parse(coordinates[0]), short.Parse(coordinates[1]), parts[2]));
            }

            return places;
        }

        public override string ToString()
        {
            string result = "Time: ";
            result += TimeStamp.Ticks.ToString() + " s, ";
            result += TimeStamp.Millisecond.ToString("000") + " ms, ";
            result += "Position: " + X.ToString("0000") + " " + Y.ToString("0000") + ", ";
            result += "Color: " + Color;
            return result;
        }
    }
}
