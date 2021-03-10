public class JsonCaster
{
    public class AR
    {
        public int id { get; set; }
        public int rank { get; set; }
        public bool highPriority { get; set; }
    }

    public class DTO
    {
        public int id { get; set; }
        public int ar_id { get; set; }
        public double Start_time { get; set; }
        public double stop_time { get; set; }
        public double memory { get; set; }
        public double roll { get; set; }
        public int pitch { get; set; }
        public int yaw { get; set; }
        public bool look_side { get; set; }
        public int seasonality_score { get; set; }
        public double incidence { get; set; }
    }
}
