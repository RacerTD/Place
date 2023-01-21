namespace PlaceFiller
{
    public class Place2022Dataset
    {
        public DateTime startTime;
        public DateTime endTime;
        public List<int> changeIndexes;

        public Place2022Dataset(DateTime startTime, DateTime endTime)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.changeIndexes = new List<int>();
        }

        public override string ToString()
        {
            string result = startTime.ToString("yyyy-MM-dd HH:mm:ss.fff UTC");
            result += " , ";
            foreach (int i in changeIndexes)
                result += $"{i}, ";
            return result;
        }
    }
}