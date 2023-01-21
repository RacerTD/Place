using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Data.Entity.Core.Mapping;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace PlaceFiller
{
    public class TwentySevenTeen
    {
        public string Path = "C:\\Users\\Tobias Deyle\\Documents\\GitHub\\Place\\PlaceFiller\\2017.csv";
        public static string DataBaseConnectionString = "Data Source=C:\\Users\\Tobias Deyle\\Documents\\GitHub\\Place\\PlaceFiller\\2017.sqlite;Version=3;";
        public static int Total2017 = 16559897;

        /// <summary>
        /// Reads data from the disk and does things
        /// </summary>
        /// <param name="addToDataBase">Should this add stuff to the database</param>
        public void Calc2017(bool addToDataBase)
        {
            //PlaceCoodinate[] placeCoodinates = new PlaceCoodinate[Total2017];
            List<PlaceCoordinate> placeCoodinates = new List<PlaceCoordinate>();

            ReadCoordinatesToList(ref placeCoodinates, Path);
            OrderList(ref placeCoodinates);

            Console.WriteLine("Data Count: " + placeCoodinates.Count);
            placeCoodinates = placeCoodinates.Where(c => c.TimeStamp.Ticks >= 636266016310000000).ToList();
            Console.WriteLine("Data Count: " + placeCoodinates.Count);

            //Console.WriteLine();
            //Console.WriteLine("First Placement: " + placeCoodinates.First().ToString());
            //Console.WriteLine(placeCoodinates.First().TimeStamp.ToShortDateString() + " " + placeCoodinates.First().TimeStamp.ToShortTimeString());
            //Console.WriteLine("Last Placement:  " + placeCoodinates.Last().ToString());
            //Console.WriteLine(placeCoodinates.Last().TimeStamp.ToShortDateString() + " " + placeCoodinates.Last().TimeStamp.ToShortTimeString());

            if (addToDataBase)
            {
                CopyToDataBase(ref placeCoodinates);
            }

            //SaveToAssetFiles(ref placeCoodinates);
            SaveToAssetFilesNewDataFormat(ref placeCoodinates, 10);

            placeCoodinates.Clear();

            Console.WriteLine("Finished everything, you can close this now");
        }

        /// <summary>
        /// Reads all placeCoodinates from the path and puts them into a provided list
        /// </summary>
        /// <param name="coodinates"></param>
        /// <param name="path"></param>
        private void ReadCoordinatesToList(ref List<PlaceCoordinate> coodinates, string path)
        {
            coodinates = new List<PlaceCoordinate>();

            Console.WriteLine();
            Console.WriteLine($"Start reading from path: {path}");

            using (StreamReader reader = new StreamReader(path))
            {
                // Percentage Stuff
                int counter = 0;
                double percentage = 0;

                // Useful Stuff
                // Skip first Line
                reader.ReadLine();
                PlaceCoordinate coodinate;

                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    coodinate = PlaceCoordinate.CreateFrom2017(line);
                    coodinates.Add(coodinate);

                    // Percent on screen
                    counter++;
                    if (counter % 1000 == 0)
                    {
                        percentage = Math.Clamp((double)counter / Total2017 * 100, 0, 100);
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
        private void OrderList(ref List<PlaceCoordinate> coodinates)
        {
            Console.WriteLine();
            Console.WriteLine("Starting sorting");

            coodinates = coodinates.OrderBy(p => p.TimeStamp.Ticks)
                .ThenBy(p => p.TimeStamp.Millisecond)
                .ToList();

            Console.WriteLine("Sorting finished");
        }

        /// <summary>
        /// Copies all data to a database
        /// </summary>
        private void CopyToDataBase(ref List<PlaceCoordinate> coodinates, bool useBatching = false)
        {
            Console.WriteLine();
            Console.WriteLine("Starting Import into database");

            using (var connection = new SQLiteConnection(DataBaseConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "CREATE TABLE Data2017 (" +
                        "TimeStamp DATETIME," +
                        "User TEXT," +
                        "X INTEGER," +
                        "Y INTEGER," +
                        "Color TEXT" +
                        ")";

                    command.ExecuteNonQuery();
                }

                if(useBatching)
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "INSERT INTO Data2017 (TimeStamp, User, X, Y, Color) VALUES (@TimeStamp, @User, @X, @Y, @Color)";
                            command.Parameters.Add("@TimeStamp", DbType.DateTime);
                            command.Parameters.Add("@User", DbType.String);
                            command.Parameters.Add("@X", DbType.Int32);
                            command.Parameters.Add("@Y", DbType.Int32);
                            command.Parameters.Add("@Color", DbType.String);

                            // Percentage Stuff
                            int counter = 0;
                            double percentage = 0;

                            for (int i = 0; i < coodinates.Count; i++)
                            {
                                command.Parameters["@TimeStamp"].Value = coodinates[i].TimeStamp;
                                command.Parameters["@User"].Value = coodinates[i].User;
                                command.Parameters["@X"].Value = coodinates[i].X;
                                command.Parameters["@Y"].Value = coodinates[i].Y;
                                command.Parameters["@Color"].Value = coodinates[i].Color;
                                command.ExecuteNonQuery();

                                // Percent on screen
                                counter++;
                                if (counter % 1000 == 0)
                                {
                                    percentage = Math.Clamp((double)counter / TwentySevenTeen.Total2017 * 100, 0, 100);
                                    Console.Write("\rProgress: {0:F2}%", percentage);
                                }
                            }
                        }
                        transaction.Commit();
                    }
                }
                else
                {
                    int counter = 0;
                    double percentage = 0;

                    for (int i = 0; i < coodinates.Count; i++)
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "INSERT INTO Data2017 (TimeStamp, User, X, Y, Color) VALUES (@TimeStamp, @User, @X, @Y, @Color)";
                            command.Parameters.AddWithValue("@TimeStamp", coodinates[i].TimeStamp);
                            command.Parameters.AddWithValue("@User", coodinates[i].User);
                            command.Parameters.AddWithValue("@X", coodinates[i].X);
                            command.Parameters.AddWithValue("@Y", coodinates[i].Y);
                            command.Parameters.AddWithValue("@Color", coodinates[i].Color);
                            command.ExecuteNonQuery();
                        }

                        counter++;
                        if (counter % 1000 == 0)
                        {
                            percentage = Math.Clamp((double)counter / TwentySevenTeen.Total2017 * 100, 0, 100);
                            Console.Write("\rProgress: {0:F2}%", percentage);
                        }
                    }
                }
            }

            coodinates.Clear();

            Console.WriteLine();
            Console.WriteLine("Data saved sucessfully");
        }

        /// <summary>
        /// Writes the data to disk in a very simple csv format
        /// This saves about 50% of filesize
        /// </summary>
        /// <param name="coordinates"></param>
        private void SaveToAssetFiles(ref List<PlaceCoordinate> coordinates)
        {
            Console.WriteLine();
            Console.WriteLine("Starting writing to disk");

            string filePath = "C:\\Users\\Tobias Deyle\\Documents\\GitHub\\Place\\PlaceFiller\\2017\\";
            string fileExtension = ".csv";

            // Timespan of r/place 2017
            // 21.03.2017 17:00
            // 27.04.2017 23:00

            DateTime officialStarTime = new DateTime(2017, 04, 01, 0, 0, 0);
            DateTime startTime = new DateTime(2017, 03, 21, 17, 0, 0, 0);
            DateTime endTime = new DateTime(2017, 03, 21, 18, 0, 0, 0);

            Console.WriteLine("Official Time Ticks: " + officialStarTime.Ticks);

            int fileCounter = 0;
            //string data = "Tick,X,Y,Color";
            //data += "\n";

            StringBuilder data = new StringBuilder();
            data.AppendLine("Tick,X,Y,Color");

            for (int i = 0; i < Total2017; i++)
            {
                if (coordinates[i].TimeStamp.Ticks < officialStarTime.Ticks)
                {
                    continue;
                }
                else
                {
                    if (coordinates[i].TimeStamp.Ticks > endTime.Ticks)
                    {
                        File.WriteAllText(filePath + fileCounter + fileExtension, data.ToString());
                        fileCounter++;

                        data.Clear();
                        data.AppendLine("Tick,X,Y,Color");

                        startTime = startTime.AddMinutes(10);
                        endTime = endTime.AddMinutes(10);
                    }

                    data.AppendLine($"{coordinates[i].TimeStamp.Ticks.ToString("G")},{coordinates[i].X},{coordinates[i].Y},{ColorPallet.ColorToNumber2017(coordinates[i].Color)}");

                    // Percent on screen
                    if (i % 1000 == 0)
                    {
                        double percentage = Math.Clamp((double)i / TwentySevenTeen.Total2017 * 100, 0, 100);
                        Console.Write("\rProgress: {0:F2}%", percentage);
                    }
                }
            }

            File.WriteAllText(filePath + fileCounter + fileExtension, data.ToString());

            Console.WriteLine();
            Console.WriteLine("All data written");
        }

        /// <summary>
        /// Writes the data to disk in a very simple csv format
        /// This saves about 50% of filesize
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="fileLength">The filelenght in minutes</param>
        private void SaveToAssetFilesNewDataFormat(ref List<PlaceCoordinate> coordinates, int fileLength)
        {
            Console.WriteLine();
            Console.WriteLine("Preparing Data");

            string filePath = "C:\\Users\\Tobias Deyle\\Documents\\GitHub\\Place\\PlaceFiller\\2017\\";
            string fileExtension = ".txt";

            // Timespan of r/place 2017
            // 21.03.2017 17:00
            // 27.04.2017 23:00

            DateTime officialStarTime = new DateTime(2017, 04, 01, 0, 0, 0);
            DateTime startTimeFile = new DateTime(2017, 03, 21, 17, 0, 0, 0);
            DateTime endTimeFile = new DateTime(2017, 03, 21, 18, 0, 0, 0);

            DateTime startTimeBlock = new DateTime(2017, 03, 21, 17, 0, 0, 0);
            DateTime endTimeBlock = new DateTime(2017, 03, 21, 17, 0, 1, 0);

            //Console.WriteLine("Official Time Ticks: " + officialStarTime.Ticks);

            DateTime temp = coordinates.First().TimeStamp;
            temp = temp.AddMilliseconds(-temp.Millisecond);

            startTimeBlock = temp;
            endTimeBlock = temp.AddMilliseconds(100);

            startTimeFile = startTimeBlock;
            endTimeFile = startTimeFile.AddMinutes(fileLength);

            //Creating Datasets

            PlaceDataset currentDataset = new PlaceDataset(startTimeBlock, endTimeBlock);
            List<PlaceDataset> places = new List<PlaceDataset>();

            for (int i = 0; i < coordinates.Count; i++)
            {
                // Percent on screen
                if (i % 1000 == 0)
                {
                    double percentage = Math.Clamp((double)i / coordinates.Count * 100, 0, 100);
                    Console.Write("\rProgress: {0:F2}%", percentage);
                }

                if (coordinates[i].TimeStamp.Ticks > endTimeBlock.Ticks)
                {
                    temp = coordinates[i].TimeStamp;
                    temp = temp.AddMilliseconds(-(temp.Millisecond % 100));

                    startTimeBlock = temp;
                    endTimeBlock = temp.AddMilliseconds(100);

                    places.Add(currentDataset);
                    currentDataset = new PlaceDataset(startTimeBlock, endTimeBlock);
                }

                currentDataset.changeList.Add(coordinates[i]);
            }

            Console.WriteLine();
            Console.WriteLine("Created " + places.Count() + " datasets");
            Console.WriteLine();
            Console.WriteLine("Starting creation of Camera Positions");

            // Now creating the camera positions
            List<PlaceDataset> tempPlaces = new List<PlaceDataset>();
            for(int i = 0; i < places.Count; i++)
            {
                // Percent on screen
                if (i % 10 == 0)
                {
                    double percentage = Math.Clamp((double)i / places.Count * 100, 0, 100);
                    Console.Write("\rProgress: {0:F2}%", percentage);
                }

                tempPlaces = places.GetRange(Math.Max(0, i - 150), Math.Min(places.Count - Math.Max(0, i - 150), 300));

                long x = 0;
                long y = 0;
                int count = 0;

                foreach (PlaceDataset place in tempPlaces)
                {
                    foreach (PlaceCoordinate p in place.changeList)
                    {
                        x += p.X;
                        y += p.Y;
                        count++;
                    }
                }

                places[i].cameraPos = new Vector2((short)(x / count), (short)(y / count));
            }

            Console.WriteLine();
            Console.WriteLine("Camera Positions created");
            Console.WriteLine();
            Console.WriteLine("Starting Writing Files");

            int fileCounter = 0;
            StringBuilder data = new StringBuilder();

            // Now writing to disk
            for (int i = 0; i < places.Count; i++)
            {
                // Percent on screen
                if (i % 10 == 0)
                {
                    double percentage = Math.Clamp((double)i / places.Count * 100, 0, 100);
                    Console.Write("\rProgress: {0:F2}%", percentage);
                }

                if (places[i].startTime.Ticks > endTimeFile.Ticks)
                {
                    if (!string.IsNullOrEmpty(data.ToString()))
                    {
                        File.WriteAllText(filePath + fileCounter + fileExtension, data.ToString());
                        fileCounter++;
                    }

                    data.Clear();

                    startTimeFile = startTimeFile.AddMinutes(fileLength);
                    endTimeFile = endTimeFile.AddMinutes(fileLength);
                }

                data.AppendLine(places[i].ToFileString());
            }

            File.WriteAllText(filePath + fileCounter + fileExtension, data.ToString());

            Console.WriteLine();
            Console.WriteLine("All data written");
        }
    }
}
