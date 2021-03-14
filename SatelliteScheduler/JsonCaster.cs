using System;
using System.Collections.Generic;

public class JsonCaster
{
    public class AR
    {
        public int id { get; set; }
        public int rank { get; set; }
        public bool highPriority { get; set; }

        public static implicit operator List<object>(AR v)
        {
            throw new NotImplementedException();
        }
    }

    public class DTO
    {
        public int id { get; set; }
        public int ar_id { get; set; }
        public double start_time { get; set; }
        public double stop_time { get; set; }
        public double memory { get; set; }
        public double roll { get; set; }
        public int pitch { get; set; }
        public int yaw { get; set; }
        public bool look_side { get; set; }
        public int seasonality_score { get; set; }
        public double incidence { get; set; }
    }

    public class MemoryFromPreviousPlan
    {
        public int ARID { get; set; }
        public double memory { get; set; }
        public int rank { get; set; }
        public bool highPriority { get; set; }
    }

    public class Costants
    {
        public int MEMORY_CAP { get; set; }
        public int OPERATIVE_PROFILE_CAP { get; set; }
        public int MISSION_HORIZON_DURATION { get; set; }
        public double MAX_INCIDENCE { get; set; }
        public double MIN_INCIDENCE { get; set; }
        public bool INCIDENCE_FILTER_IS_NOT_ACTIVE { get; set; }
        public bool DLO_FILTER_IS_ACTIVE { get; set; }
        public bool PAW_FILTER_IS_ACTIVE { get; set; }
        public double DOWNLINK_RATE { get; set; }
        public int ACQ_TIME_GAP { get; set; }
        public int STABILIZATION_TIME { get; set; }
        public int INITIAL_PITCH { get; set; }
        public int INITIAL_ROLL { get; set; }
        public int INITIAL_YAW { get; set; }
        public double PITCH_ANGULAR_VELOCITY { get; set; }
        public double ROLL_ANGULAR_VELOCITY { get; set; }
        public double YAW_ANGULAR_VELOCITY { get; set; }
        public int INITIAL_ROLL_DLO { get; set; }
        public int FINAL_ROLL_DLO { get; set; }
        public int ORBIT_PERIOD { get; set; }
        public int PAW_INITIAL_PITCH { get; set; }
        public int PAW_INITIAL_ROLL { get; set; }
        public int PAW_INITIAL_YAW { get; set; }
        public int PAW_FINAL_PITCH { get; set; }
        public int PAW_FINAL_ROLL { get; set; }
        public int PAW_FINAL_YAW { get; set; }
        public List<MemoryFromPreviousPlan> memoryFromPreviousPlan { get; set; }
        public double FINAL_SATELLITE_MEMORY { get; set; }
    }

    public class ARDTO
    {
        public int id_ar { get; set; }
        public int id_dto { get; set; }
        public int rank { get; set; }
        public bool highPriority { get; set; }
        public double start_time { get; set; }
        public double stop_time { get; set; }
        public double memory { get; set; }

        public ARDTO(AR ar, DTO dto)
        {
            if (ar.id == dto.ar_id)
            {
                id_ar = ar.id;
                id_dto = dto.id;
                rank = ar.rank;
                highPriority = ar.highPriority;
                start_time = dto.start_time;
                stop_time = dto.stop_time;
                memory = dto.memory;
            }
        }

        public void PrintAll()
        {
            Console.WriteLine(id_ar + "\t" + id_dto + "\t" + rank + "\t" + highPriority + "\t" + start_time + "\t" + stop_time + "\t" + memory);
        }
    }
}
