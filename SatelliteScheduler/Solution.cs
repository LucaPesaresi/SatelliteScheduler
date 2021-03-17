using System;
using System.Collections.Generic;
using System.Linq;
using static JsonCaster;

namespace SatelliteScheduler
{
    class Solution
    {
        Plan best { get; set; }
        Plan best_star { get; set; }

        public void TestNoisyPlan(List<ARDTO> ardto, int max_it = 1000)
        {
            double max_rank = ardto.OrderByDescending(d => d.noisy_rank).First().rank;
            
            for (int i = 0; i <= max_rank;  i += (int)max_rank / 50)
            {
                Console.WriteLine("\nTest piano con rumore " + i + "%");
                CreateNoisyPlan(ardto, i, max_it);
            }
        }

        public void CreateNoisyPlan(List<ARDTO> ardto, int noise, int max_it = 1000)
        {
            List<Plan> plan_list = new List<Plan>();

            for (int j = 0; j < max_it; j++)
            {
                List<ARDTO> ardto_noisy = AddNoise(ardto, noise)
                    .OrderByDescending(d => d.noisy_rank)
                    .ThenByDescending(m => m.memory).ToList();

                Plan plan = new Plan(ardto_noisy);
                plan_list.Add(plan);
            }

            best = plan_list.OrderByDescending(p => p.QualityPlan().tot_rank).First();
            best.QualityPlan().PrintQuality();
        }

        // Aggiunge un rumore ad una quantità di dati e con una discreta scalabilità
        public List<ARDTO> AddNoise(List<ARDTO> ardto, double step_noise)
        {
            foreach (ARDTO a in ardto)
            {
                double value_noise = (new Random().NextDouble() * 2 * step_noise) - step_noise;
                a.noisy_rank = (a.rank + value_noise) / a.memory;
            }
            return ardto;
        }

        public void Start()
        {
            best_star = new Plan();

            foreach (var i in best.plan)
            {
                best_star.plan.Add(i);
            }
        }

        // Rimuove k elementi dal piano
        public void Ruin(int k)
        {
            List<ARDTO> plan = best.plan;
            int k_norm = Convert.ToInt32((plan.Count * k) / 100);
            for (int i = 0; i < k_norm; i++)
            {
                plan.RemoveAt(new Random().Next(0, plan.Count));
            }

            best.plan = plan;
        }

        public void Recreate(List<ARDTO> ardto, int step_noise)
        {
            double mem = best.QualityPlan().memory;
            best.BuildPlan(AddNoise(ardto, step_noise), mem);
        }

        public void Compare()
        {
            Quality q_new = best.QualityPlan();
            Quality q_star = best_star.QualityPlan();

            Console.WriteLine("-------------------------");
            if (q_new.tot_rank / q_new.memory > q_star.tot_rank / q_star.memory)
            {
                best_star = best;
                Console.WriteLine("\nIl nuovo piano risulta migliore");
                q_new.PrintQuality();

                Console.WriteLine("\nIl vecchio piano risulta");
                q_star.PrintQuality();
            }
            else
            {
                best = best_star;
                Console.WriteLine("\nIl nuovo piano risulta");
                q_new.PrintQuality();

                Console.WriteLine("\nIl vecchio piano risulta migliore");
                q_star.PrintQuality();
            }
            Console.WriteLine("-------------------------");
        }
    }
}
