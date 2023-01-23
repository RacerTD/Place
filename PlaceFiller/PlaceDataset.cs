using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PlaceFiller
{
    public class PlaceDataset
    {
        public DateTime startTime;
        public DateTime endTime;
        public List<PlaceCoordinate> changeList;

        public PlaceDataset(DateTime startTime, DateTime endTime)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.changeList = new List<PlaceCoordinate>();
        }

        public string ToFileString()
        {
            // TODO Add filtering for double set pixels

            string result = (startTime.Ticks - 636266016310000000).ToString();
            foreach(PlaceCoordinate coord in changeList)
            {
                result += $";{coord.X},{coord.Y},{coord.Color}";
            }
            return result;
        }
    }

    //public class Vector2
    //{
    //    public short x;
    //    public short y;

    //    public Vector2(short x, short y)
    //    {
    //        this.x = x;
    //        this.y = y;
    //    }

    //    public override string ToString()
    //    {
    //        return $"{x},{y}";
    //    }
    //}
}
