using System;
using System.Collections.Generic;
using System.Linq;
using static JsonCaster;

namespace SatelliteScheduler
{
    public class Plan
    {
        public List<ARDTO> plan { get; set; }
        public List<ARDTO> ardto { get; set; }

        public Plan()
        {
            plan = new List<ARDTO>();
            ardto = new List<ARDTO>();
        }

        public Plan(List<ARDTO> ardto)
        {
            plan = new List<ARDTO>();
            this.ardto = ardto;
            BuildPlan();
        }

        //Creazione di un piano con una lista randomizzata
        public void BuildPlan(double current_mem=0)
        { 
            bool ok; //indica se i vincoli sono rispettati
            int i = 0;

            if (current_mem == 0)
            {
                plan.Add(ardto[0]);
                current_mem += plan[0].memory;
                i++;
            }
            //Console.WriteLine("id_ar\t\tid_dto\t\trank\thigh\tstart\t\t\tstop\t\t\tmemory");

            for (; i < ardto.Count; i++)
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
                    //else
                    //{
                    //    int breakaa = 10;
                    //}
                }
            }
        }

        // Testa i piani generati con un rumore crescente
        public void TestNoisyARDTO(List<ARDTO> ardto, int max_it = 1000)
        {
            double max_rank = ardto.OrderByDescending(d => d.noisy_rank).First().rank;

            for (int i = 0; i <= max_rank; i += (int)max_rank / 50)
            {
                Console.WriteLine("\nTest piano con rumore " + i + "%");
                CreateNoisyARDTO(ardto, i, max_it);
            }
        }

        // Sceglie il piano migliore generando diversi piani disturbato 
        public void CreateNoisyARDTO(List<ARDTO> ardto, int noise, int max_it = 1000)
        {
            List<Plan> plan_list = new List<Plan>();

            for (int j = 0; j < max_it; j++)
            {
                Plan P = new Plan();
                P.AddNoise(ardto, noise);
                P.ardto.OrderByDescending(d => d.noisy_rank).ToList();
                P.BuildPlan();
                plan_list.Add(P);
            }

            Plan best = plan_list.OrderByDescending(p => p.QualityPlan().tot_rank).First();
            best.QualityPlan().PrintQuality();

            this.plan = best.plan;
            this.ardto = best.ardto;
        }

        // Aggiunge un rumore ad una quantità di dati e con una discreta scalabilità
        public void AddNoise(List<ARDTO> ardto, double step_noise)
        {
            List<ARDTO> ardto_noisy = new List<ARDTO>();

            ardto.ForEach(a => ardto_noisy.Add(a));

            //foreach (ARDTO a in ardto_noisy)
            //{
            //    double value_noise = (new Random().NextDouble() * 2 * step_noise) - step_noise;
            //    a.noisy_rank = (a.rank + value_noise) / a.memory;
            //}

            for (int i = 0; i < ardto_noisy.Count; i++)
            {
                double value_noise = (new Random().NextDouble() * 2 * step_noise) - step_noise;
                ardto_noisy[i].noisy_rank = (ardto_noisy[i].rank + value_noise) / ardto_noisy[i].memory;
            }

            this.ardto = ardto_noisy;
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
