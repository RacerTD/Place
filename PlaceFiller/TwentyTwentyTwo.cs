//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Data.SQLite;
//using System.Data;
//using System.Data.Entity.Core.Mapping;
//using System.Diagnostics.Metrics;
//using System.Reflection;
//using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Transactions;
//using System.Collections;

//namespace PlaceFiller
//{
//    public class TwentyTwentyTwo
//    {
//        private static string Path = "C:\\Users\\Tobias Deyle\\Documents\\GitHub\\Place\\PlaceFiller\\2022.csv";
//        private static string DataBaseConnectionString = "Data Source=C:\\Users\\Tobias Deyle\\Documents\\GitHub\\Place\\PlaceFiller\\database.sqlite;Version=3;";
//        private static string ResultFilePath = "C:\\Users\\Tobias Deyle\\Documents\\GitHub\\Place\\PlaceFiller\\2022\\";
//        private static int Total2022 = 160353104;
//        private static int Real2022 = 160455380;

//        public void CreateCameraPositons(int range = 100)
//        {
//            Place2022Dataset set = LoadPlaceDataSet(300000);
//            Console.WriteLine(set.ToString());

//            foreach(PlaceCoordinate coord in set.changedCoordinates)
//            {
//                Console.WriteLine(coord.ToString());
//            }

//            return;

//            List<Place2022Dataset> dataBuffer = new List<Place2022Dataset>();

//            // Initial loading of data
//            using (var connection = new SQLiteConnection(DataBaseConnectionString))
//            {
//                connection.Open();

//                for(int i = 0; i < range; i++)
//                {
//                    dataBuffer.Add(LoadPlaceDataSet(i));
//                }

//                Console.WriteLine("Loaded initial datasets");
//            }

//            // Runtime
//            List<Vector2> positions = new List<Vector2>();

//            using (var connection = new SQLiteConnection(DataBaseConnectionString))
//            {
//                // One calc for every dataset
//                for(int i = 1; i <= 3492000; i++)
//                {
//                    // Percentage on screen
//                    if (i % 10 == 0)
//                    {
//                        double percentage = Math.Clamp((double)i / 3492000 * 100, 0, 100);
//                        //Console.Write("\rProgress: {0:F2}%", percentage);
//                    }

//                    if (i + range <= 3492000)
//                    {
//                        dataBuffer.Add(LoadPlaceDataSet(i + range));
//                    }

//                    if (dataBuffer.Count >= range * 2 + 1)
//                    {
//                        dataBuffer.RemoveAt(0);
//                    }

//                    int x = 0;
//                    int y = 0;
//                    int count = 1;

//                    foreach(Place2022Dataset dataSet in dataBuffer)
//                    {
//                        if(dataSet != null
//                            && dataSet.changedCoordinates != null
//                            && dataSet.changedCoordinates.Count > 0) 
//                        {
//                            foreach(PlaceCoordinate coordinate in dataSet.changedCoordinates)
//                            {
//                                count++;
//                                x += coordinate.X;
//                                y += coordinate.Y;
//                            }
//                        }
//                    }

//                    x = x / count;
//                    y = y / count;

//                    positions.Add(new Vector2((short)x, (short)y));

//                    if(i % 1000 == 0)
//                        Console.WriteLine($"Coordinate {i}  {positions.Last()}, dataBuffer.Count: {dataBuffer.Count()}");
//                }
//            }
//        }

//        /// <summary>
//        /// Returns a complete placeDataset from the database
//        /// </summary>
//        /// <param name="index"></param>
//        /// <returns></returns>
//        public Place2022Dataset LoadPlaceDataSet(int index)
//        {
//            Place2022Dataset result = null;

//            using (var connection = new SQLiteConnection(DataBaseConnectionString))
//            {
//                connection.Open();
//                using (var command = connection.CreateCommand())
//                {
//                    command.CommandText = "SELECT * FROM DataSets2022 WHERE id = @id";
//                    command.Parameters.AddWithValue("@id", index);
//                    using (var reader = command.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            Console.WriteLine(reader.GetString(3));

//                            result = new Place2022Dataset(reader.GetDateTime(1), reader.GetDateTime(2));
//                            result.changeIndexes.AddRange(DeserializeIntList(reader.GetString(3)));

//                            result.changedCoordinates.AddRange(GetPlaceCoordinatesFromDatabase(result.changeIndexes));
//                        }
//                    }
//                }
//            }

//            //Console.WriteLine($"Datas Loaded for index {index}: {result.changedCoordinates.Count()}");

//            return result;
//        }

//        /// <summary>
//        /// Returns a list of PlaceCoordinates from the database
//        /// </summary>
//        /// <returns></returns>
//        public List<PlaceCoordinate> GetPlaceCoordinatesFromDatabase(List<int> indexes)
//        {
//            List<PlaceCoordinate> list = new List<PlaceCoordinate>();
//            PlaceCoordinate coordinate = null;

//            using (var connection = new SQLiteConnection(DataBaseConnectionString))
//            {
//                connection.Open();

//                using (SQLiteCommand command = connection.CreateCommand())
//                {
//                    // Create a string to hold the list of ID's
//                    string idList = string.Join(",", indexes);

//                    // Build the query
//                    command.CommandText = "SELECT * FROM DataSets2022 WHERE id IN (@ids)";
//                    command.Parameters.AddWithValue("@ids", idList);

//                    // Execute the query
//                    using (SQLiteDataReader reader = command.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            coordinate = new PlaceCoordinate(reader.GetDateTime(1), reader.GetString(2), reader.GetInt16(3), reader.GetInt16(4), reader.GetString(5));
//                            list.Add(coordinate);
//                        }
//                    }
//                }
//            }

//            return list;
//        }

//        public void CreateDatasets()
//        {
//            Console.WriteLine();
//            Console.WriteLine("Start creating 3492000 datasets");

//            List<Place2022Dataset> datasets = new List<Place2022Dataset>();
//            DateTime startTime = new DateTime(2022, 04, 01, 00, 00, 00, 000);
//            DateTime endTime = startTime.AddMilliseconds(100);

//            // Creating all the subsets
//            for(int i = 0; i < 3492000; i++)
//            {
//                datasets.Add(new Place2022Dataset(startTime, endTime));
//                startTime = startTime.AddMilliseconds(100);
//                endTime = endTime.AddMilliseconds(100);
//            }

//            Console.WriteLine($"Created all the datasets, First timestamp: {datasets.First().startTime.ToString("yyyy-MM-dd HH:mm:ss.fff UTC")}, Last timestamp: {datasets.Last().endTime.ToString("yyyy-MM-dd HH:mm:ss.fff UTC")}");

//            using (var connection = new SQLiteConnection(DataBaseConnectionString))
//            {
//                connection.Open();

//                for (int i = 0; i < Real2022; i++)
//                {
//                    // Percentage on screen
//                    if (i % 100 == 0)
//                    {
//                        double percentage = Math.Clamp((double)i / Total2022 * 100, 0, 100);
//                        Console.Write("\rProgress: {0:F2}%", percentage);
//                    }

//                    using (var command = connection.CreateCommand())
//                    {
//                        command.CommandText = "SELECT * FROM Data2022 WHERE id = @id";
//                        command.Parameters.AddWithValue("@id", i);

//                        using (var reader = command.ExecuteReader())
//                        {
//                            while (reader.Read())
//                            {
//                                int index = GetRightSubset(reader.GetDateTime(1));
//                                if(reader.GetDateTime(1) >= datasets[index].startTime
//                                    && reader.GetDateTime(1) < datasets[index].endTime)
//                                {
//                                    datasets[index].changeIndexes.Add(i);
//                                }
//                                else
//                                {
//                                    Console.WriteLine("Found something thats not right.");
//                                }
//                            }
//                        }
//                    }
//                }

//                Console.WriteLine();
//                Console.WriteLine("Assigned all coordinates to a dataset");

//                datasets = datasets.OrderBy(p => p.startTime.Ticks)
//                    .ThenBy(p => p.startTime.Millisecond)
//                    .ToList();

//                Console.WriteLine();
//                Console.WriteLine("Saving this to the database");

//                // Create table
//                using (var command = connection.CreateCommand())
//                {
//                    command.CommandText =
//                        "CREATE TABLE DataSets2022 (" +
//                        "id INTEGER PRIMARY KEY AUTOINCREMENT," +
//                        "startTime DATETIME," +
//                        "endTime DATETIME," +
//                        "changes TEXT" +
//                        ")";
//                    command.ExecuteNonQuery();
//                }

//                using (var transaction = connection.BeginTransaction())
//                {
//                    using (var command = connection.CreateCommand())
//                    {
//                        command.CommandText = "INSERT INTO DataSets2022 (startTime, endTime, changes) VALUES (@startTime, @endTime, @changes)";
//                        command.Parameters.Add("@startTime", DbType.DateTime);
//                        command.Parameters.Add("@endTime", DbType.DateTime);
//                        command.Parameters.Add("@changes", DbType.String);
                    
//                        // Fill table
//                        for(int i = 0; i < datasets.Count; i++)
//                        {
//                            // Percentage on screen
//                            if (i % 100 == 0)
//                            {
//                                double percentage = Math.Clamp((double)i / datasets.Count * 100, 0, 100);
//                                Console.Write("\rProgress: {0:F2}%", percentage);
//                            }

//                            command.Parameters["@startTime"].Value = datasets[i].startTime;
//                            command.Parameters["@endTime"].Value = datasets[i].endTime;
//                            command.Parameters["@changes"].Value = SerializeIntList(datasets[i].changeIndexes);
//                            command.ExecuteNonQuery();
//                        }
//                    }
//                    transaction.Commit();
//                }
//            }

//            Console.WriteLine();
//            Console.WriteLine("Everything saved to the database");

//            //Console.WriteLine();
//            //foreach (var dataset in datasets)
//            //    Console.WriteLine(dataset.ToString());
//        }

//        /// <summary>
//        /// Creates a string from a list of ints to save in the database
//        /// </summary>
//        /// <param name="list"></param>
//        /// <returns></returns>
//        private string SerializeIntList(List<int> list)
//        {
//            StringBuilder writer = new StringBuilder();
//            foreach(int i in list)
//                writer.Append(i.ToString() + ",");
//            return writer.ToString();
//        }

//        /// <summary>
//        /// Creates a list of ints from a string saved in the database
//        /// </summary>
//        /// <param name="list"></param>
//        /// <returns></returns>
//        private List<int> DeserializeIntList(string listString)
//        {
//            List<int> list = new List<int>();
//            string[] elements = listString.Split(',');
//            foreach (string element in elements)
//            {
//                if (!string.IsNullOrEmpty(element))
//                    list.Add(int.Parse(element));
//            }
//            return list;
//        }

//        /// <summary>
//        /// Returns the index for the right dataset created in CreateDatasets()
//        /// </summary>
//        /// <param name="time"></param>
//        /// <returns></returns>
//        private int GetRightSubset(DateTime time)
//        {
//            return (time.Day - 1) * 24 * 60 * 60 * 10 + time.Hour * 60 * 60 * 10 + time.Minute * 60 * 10 + time.Second * 10 + time.Millisecond / 100;
//        }

//        /// <summary>
//        /// Reads all the data from the filepath to the database
//        /// </summary>
//        public void ReadDataIntoDatabase()
//        {
//            Console.WriteLine();
//            Console.WriteLine("Starting Import into database");

//            using (var connection = new SQLiteConnection(DataBaseConnectionString))
//            {
//                connection.Open();
//                using (var command = connection.CreateCommand())
//                {
//                    command.CommandText =
//                        "CREATE TABLE Data2022 (" +
//                        "id INTEGER PRIMARY KEY AUTOINCREMENT," +
//                        "TimeStamp DATETIME," +
//                        "User TEXT," +
//                        "X INTEGER," +
//                        "Y INTEGER," +
//                        "Color TEXT" +
//                        ")";

//                    command.ExecuteNonQuery();
//                }
//            }

//            List<PlaceCoordinate> placeCoordinates = new List<PlaceCoordinate>();

//            using (StreamReader reader = new StreamReader(Path))
//            {
//                reader.ReadLine();

//                int counter = 0;
//                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
//                {
//                    // Percentage on screen
//                    counter++;
//                    if(counter % 10000 == 0)
//                    {
//                        double percentage = Math.Clamp((double)counter / Total2022 * 100, 0, 100);
//                        Console.Write("\rProgress: {0:F2}%", percentage);
//                    }

//                    // Reading the actual data
//                    placeCoordinates.AddRange(PlaceCoordinate.CreateFrom2022(line));

//                    // Saving data and clearing the list
//                    if (placeCoordinates.Count > 100000)
//                    {
//                        SaveChunk(placeCoordinates);
//                        placeCoordinates.Clear();
//                    }
//                }

//                SaveChunk(placeCoordinates);
//                placeCoordinates.Clear();
//            }

//            Console.WriteLine();
//            Console.WriteLine("All data written");
//        }

//        /// <summary>
//        /// Counts the data in the csv file
//        /// Was created for the Total2022 variable
//        /// </summary>
//        public void CountData()
//        {
//            int counter = 0;

//            Console.WriteLine();
//            Console.WriteLine($"Start counting from path: {Path}");

//            using (StreamReader reader = new StreamReader(Path))
//            {
//                reader.ReadLine();

//                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
//                {
//                    counter++;
//                }
//            }

//            Console.WriteLine($"{counter} Total entries in the csv file");
//        }

//        /// <summary>
//        /// Saves a chunk of data to the database
//        /// </summary>
//        /// <param name="coordinates"></param>
//        private void SaveChunk(List<PlaceCoordinate> coordinates)
//        {
//            using (var connection = new SQLiteConnection(DataBaseConnectionString))
//            {
//                connection.Open();

//                using (var transaction = connection.BeginTransaction())
//                {
//                    using (var command = connection.CreateCommand())
//                    {
//                        command.CommandText = "INSERT INTO Data2022 (TimeStamp, User, X, Y, Color) VALUES (@TimeStamp, @User, @X, @Y, @Color)";
//                        command.Parameters.Add("@TimeStamp", DbType.DateTime);
//                        command.Parameters.Add("@User", DbType.String);
//                        command.Parameters.Add("@X", DbType.Int32);
//                        command.Parameters.Add("@Y", DbType.Int32);
//                        command.Parameters.Add("@Color", DbType.String);

//                        foreach(PlaceCoordinate pc in coordinates)
//                        {
//                            command.Parameters["@TimeStamp"].Value = pc.TimeStamp;
//                            command.Parameters["@User"].Value = pc.User;
//                            command.Parameters["@X"].Value = pc.X;
//                            command.Parameters["@Y"].Value = pc.Y;
//                            command.Parameters["@Color"].Value = pc.Color;
//                            command.ExecuteNonQuery();
//                        }
//                    }
//                    transaction.Commit();
//                }
//            }
//        }
//    }
//}
