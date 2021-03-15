using System;
using System.Collections.Generic;
using System.Linq;
using static JsonCaster;

namespace SatelliteScheduler
{
    public class Plan
    {
        List<ARDTO> plan;
       
        public Plan(List<ARDTO> ardto)
        {
            plan = new List<ARDTO>();
            CreatePlan(ardto);
        }

        //Creazione di un piano con una lista randomizzata
        public void CreatePlan(List<ARDTO> ardto)
        { 
            int max = ardto.Count;
            bool ok; //indica se i vincoli sono rispettati
            double current_mem = 0; // memoria attuale

            plan.Add(ardto[0]);

            current_mem += plan[0].memory;

            //Console.WriteLine("id_ar\t\tid_dto\t\trank\thigh\tstart\t\t\tstop\t\t\tmemory");

            int i;
            for (i = 1; i < max; i++)
            {
                ok = true;
                int j;
                for (j = 0; j < plan.Count; j++)
                {
                    //controllo che non ci sia un overlap temporale con i precedenti
                    if (ardto[i].stop_time >= plan[j].start_time &&
                        ardto[i].start_time <= plan[j].stop_time)
                    {
                        ok = false;
                        break;
                    }
                    //controllo che l'id_ar non sia già presente in quelli aggiunti
                    if (ardto[i].id_ar == plan[j].id_ar)
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                {
                    //controllo memoria libera
                    if (current_mem + ardto[i].memory <= Program.max_mem)
                    {
                        //ardto[i].PrintAll();
                        plan.Add(ardto[i]);
                        current_mem += ardto[i].memory;
                    }
                }
            }
        }

        // Restituisce una qualità del piano in temrini di  numero di acquisizioni, 
        // rank totale e memoria occupata
        public Quality QualityPlan()
        {
            double rank = plan.Select(x => x.rank).Sum();
            double mem = Math.Round(plan.Select(x => x.memory).Sum(), 2);
            return new Quality(plan.Count, rank, mem);
        }
    }

    public class Quality
    {
        public int n_ar { get; set; }
        public double tot_rank { get; set; }
        public double memory { get; set; }

        public Quality(int n_ar, double tot_rank, double memory)
        {
            this.n_ar = n_ar;
            this.tot_rank = tot_rank;
            this.memory = memory;
        }

        public void PrintQuality()
        {
            Console.WriteLine("Acquisizioni: " + n_ar);
            Console.WriteLine("Rank: " + tot_rank);
            Console.WriteLine("Memoria usata: " + memory + " su " + Program.max_mem + " GB");
        }
    }
}
