using System;

namespace SatelliteScheduler
{
    public class Quality
    {
        public double n_ar { get; set; }
        public double tot_rank { get; set; }
        public double memory { get; set; }
        public double tot_memory { get; set; }
        public int k { get; set; }
        public int noise { get; set; }
        public double temp { get; set; }
        public int it_max { get; set; }
        public double max_rank { get; set; }
        public double time { get; set; }

        public Quality(double n_ar, double tot_rank, double memory, double tot_memory, double max_rank)
        {
            this.n_ar = n_ar;
            this.tot_rank = tot_rank;
            this.memory = memory;
            this.tot_memory = tot_memory;
            this.max_rank = max_rank;
        }

        public Quality(int k, int noise, double n_ar, double tot_rank, double memory, double time)
        {
            this.k = k;
            this.noise = noise;
            this.n_ar = n_ar;
            this.tot_rank = tot_rank;
            this.memory = memory;
        }

        public Quality(int k, int noise, double temp, int it_max, double n_ar, double tot_rank, double memory)
        {
            this.k = k;
            this.noise = noise;
            this.temp = temp;
            this.it_max = it_max;
            this.n_ar = n_ar;
            this.tot_rank = tot_rank;
            this.memory = memory;
            this.time = time;
        }

        public Quality(int k, int noise, double temp, int it_max, double n_ar, double tot_rank, double memory, double time)
        {
            this.k = k;
            this.noise = noise;
            this.temp = temp;
            this.it_max = it_max;
            this.n_ar = n_ar;
            this.tot_rank = tot_rank;
            this.memory = memory;
            this.time = time;
        }

        public void SetMaxRank(double value)
        {
            max_rank = value;
        }

        public double GetGap()
        {
            return Math.Round(100 - (100 * tot_rank) / max_rank, 3);
        } 

        public string WriteQuality()
        {
            return n_ar + "," + tot_rank + "," + memory;
        }

        public void PrintQuality()
        {
            Console.WriteLine("Acquisizioni: " + n_ar);
            Console.WriteLine("Rank: " + tot_rank + " (" + GetGap() + ")");
            Console.WriteLine("Memoria usata: " + memory + " su " + tot_memory + " GB");
        }

        public void PrintQualityTime()
        {
            Console.WriteLine("Acquisizioni: " + n_ar);
            Console.WriteLine("Rank: " + tot_rank + " (" + GetGap() + ")");
            Console.WriteLine("Memoria usata: " + memory);
            Console.WriteLine("Tempo medio: " + time);
        }

        public void PrintQualityRR()
        {
            Console.WriteLine(k.ToString() + "\t" + noise + "\t" + n_ar + "\t"
                + tot_rank + " (" + GetGap() + ")" + "\t" + memory);
        }

        public void PrintQualitySA()
        {
            Console.WriteLine(temp + "\t" + it_max + "\t" + n_ar + "\t"
                + tot_rank + " (" + GetGap() + ")" + "\t" + memory);
        }
    }
}
