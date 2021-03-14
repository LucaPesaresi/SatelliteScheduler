using System;
using System.Collections.Generic;
using System.Linq;
using static JsonCaster;

namespace SatelliteScheduler
{
    public class Plan
    {
        static List<ARDTO> plan;
        static double max_memory;

        public Plan(List<ARDTO> ardto, double memoryMax)
        {
            max_memory = memoryMax;
            plan = new List<ARDTO>();
            CreatePlan(ardto);
            QualityPlan();
        }

        //Creazione di un piano con una lista randomizzata
        public static void CreatePlan(List<ARDTO> ardto)
        { 
            int max = ardto.Count;
            bool ok; //indica se i vincoli sono rispettati
            bool full = false; //memoria piena
            double current_mem = 0; // memoria attuale

            plan.Add(ardto[0]);

            current_mem += plan[0].memory;

            //Console.WriteLine("id_ar\t\tid_dto\t\trank\thigh\tstart\t\t\tstop\t\t\tmemory");

            int i;
            for (i = 1; i < max && !full; i++)
            {
                ok = true;
                int j = 0;
                for (j = 0; j < plan.Count && ok; j++)
                {
                    //controllo che non ci sia un overlap temporale con i precedenti
                    if (ardto[i].stop_time >= plan[j].start_time ||
                        ardto[i].start_time <= plan[j].stop_time)
                    {
                        i++;
                        ok = false;
                    }
                    //controllo che l'id_ar non sia già presente in quelli aggiunti
                    if (ardto[i].id_ar == plan[j].id_ar)
                    {
                        //se sono uguali ma il nuovo è migliore viene scambiato con il precedente
                        //migliore inteso come: minor tempo di acquisizione e minor memoria
                        double new_lap = ardto[i].stop_time - ardto[i].start_time;
                        double old_lap = plan[j].stop_time - plan[j].start_time;

                        if (ardto[i].memory < plan[j].memory &&
                            new_lap < old_lap && ok)
                        {
                            current_mem -= plan[j].memory;
                            current_mem += ardto[i].memory;
                            plan.RemoveAt(j);
                        }
                        else
                        {
                            i++;
                            ok = false;
                        }
                    }
                }
                //controllo memoria libera
                if (current_mem + ardto[i].memory <= max_memory)
                {
                    //ardto[i].PrintAll();
                    plan.Add(ardto[i]);
                    current_mem += ardto[i].memory;
                }
                else
                {
                    full = true;
                }
            }
        }

        //Stampa le qualità del piano in temrini di rank medio e numero acquisizioni
        private static void QualityPlan()
        {
            double rank_mean = plan.Select(x => x.rank).Average();
            double mem = plan.Select(x => x.memory).Sum();

            Console.WriteLine("\nAcquisizioni: " + plan.Count);
            Console.WriteLine("Rank medio: " + rank_mean);
            Console.WriteLine("Memoria usata: " + mem + " su " + max_memory);
        }
    }
}
