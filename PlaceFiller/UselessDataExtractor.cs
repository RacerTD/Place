using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaceFiller
{
    public static class UselessDataExtractor
    {
        /// <summary>
        /// Lists the players by placed tiles
        /// </summary>
        /// <param name="data"></param>
        public static void GetPlayerRanking(ref List<PlaceCoordinate> data)
        {
            Console.WriteLine();
            Console.WriteLine($"Start generating Player List");

            // Percentage Stuff
            int counter = 0;
            double percentage = 0;

            // Useful Stuff
            Dictionary<string, int> ranking = new Dictionary<string, int>();

            foreach (PlaceCoordinate place in data)
            {
                if (ranking.TryGetValue(place.User, out int count))
                {
                    ranking[place.User] = count + 1;
                }
                else
                {
                    ranking[place.User] = 1;
                }

                // Percent on screen
                counter++;
                if (counter % 1000 == 0)
                {
                    percentage = Math.Clamp((double)counter / TwentySevenTeen.Total2017 * 100, 0, 100);
                    Console.Write("\rProgress: {0:F2}%", percentage);
                }
            }

            ranking = ranking.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            double average = (double)TwentySevenTeen.Total2017 / (double)ranking.Count;

            Console.WriteLine();
            Console.WriteLine($"Success, {ranking.Count()} users where scanned");
            Console.WriteLine("On average each user placed {0:F2} tiles", average);
            Console.WriteLine();

            foreach(KeyValuePair<string, int> user in ranking.Take(10))
            {
                Console.WriteLine($"User: {user.Key}, Placed Tiles: {user.Value}");
            }
        }

        /// <summary>
        /// Lists the tiles that were placed the most
        /// </summary>
        /// <param name="data"></param>
        public static void GetTileRanking(ref List<PlaceCoordinate> data)
        {
            Console.WriteLine();
            Console.WriteLine($"Start generating Tile List");

            // Percentage Stuff
            int counter = 0;
            double percentage = 0;

            // Useful Stuff
            Dictionary<Tuple<short, short>, int> ranking = new Dictionary<Tuple<short, short>, int>();
            Tuple<short, short> value = new Tuple<short, short>(0, 0);

            foreach (PlaceCoordinate place in data)
            {
                value = new Tuple<short, short>(place.X, place.Y);

                if (ranking.TryGetValue(value, out int count))
                {
                    ranking[value] = count + 1;
                }
                else
                {
                    ranking[value] = 1;
                }

                // Percent on screen
                counter++;
                if (counter % 1000 == 0)
                {
                    percentage = Math.Clamp((double)counter / TwentySevenTeen.Total2017 * 100, 0, 100);
                    Console.Write("\rProgress: {0:F2}%", percentage);
                }
            }

            ranking = ranking.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            double average = (double)TwentySevenTeen.Total2017 / (double)ranking.Count;

            Console.WriteLine();
            Console.WriteLine($"Success, {ranking.Count()} tiles where scanned");
            Console.WriteLine("On average each tile was placed {0:F2} times", average);
            Console.WriteLine();

            foreach (KeyValuePair<Tuple<short, short>, int> tile in ranking.Take(10))
            {
                Console.WriteLine($"Tile: {tile.Key}, Placements: {tile.Value}");
            }
        }
    
        /// <summary>
        /// Lists the colors by their use count
        /// </summary>
        /// <param name="data"></param>
        public static void GetColorRanking(ref List<PlaceCoordinate> data)
        {
            Console.WriteLine();
            Console.WriteLine($"Start generating Color List");

            // Percentage Stuff
            int counter = 0;
            double percentage = 0;

            // Useful Stuff
            Dictionary<string, int> ranking = new Dictionary<string, int>();

            foreach (PlaceCoordinate place in data)
            {
                if (ranking.TryGetValue(place.Color, out int count))
                {
                    ranking[place.Color] = count + 1;
                }
                else
                {
                    ranking[place.Color] = 1;
                }

                // Percent on screen
                counter++;
                if (counter % 1000 == 0)
                {
                    percentage = Math.Clamp((double)counter / TwentySevenTeen.Total2017 * 100, 0, 100);
                    Console.Write("\rProgress: {0:F2}%", percentage);
                }
            }

            ranking = ranking.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            Console.WriteLine();
            Console.WriteLine($"Success, {ranking.Count()} Colors where scanned");
            Console.WriteLine();

            foreach (KeyValuePair<string, int> color in ranking)
            {
                Console.WriteLine($"Color: {color.Key}, Placed: {color.Value}");
            }
        }
    }
}
