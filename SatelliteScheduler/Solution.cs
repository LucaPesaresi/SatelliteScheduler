using System;
using System.Collections.Generic;
using System.Linq;
using static JsonCaster;

namespace SatelliteScheduler
{
    class Solution
    {
        static Quality Q { get; set; }

        public void TestNoisyPlan(List<ARDTO> ardto, int max_it = 1000)
        {
            double max_rank = ardto.OrderByDescending(d => d.noisy_rank).First().rank;
            
            for (int i = 0; i <= max_rank;  i += (int)max_rank / 50)
            {
                List<Quality> qlist = new List<Quality>();
                Console.WriteLine("\nTest piano con rumore " + i + "%");

                for (int j = 0; j < max_it; j++)
                {
                    List<ARDTO> ardto_noisy = AddNoise(ardto, i)
                        .OrderByDescending(d => d.noisy_rank)
                        .ThenByDescending(m => m.memory).ToList();

                    Plan plan = new Plan(ardto_noisy);
                    qlist.Add(plan.QualityPlan());
                }
        
                Q = qlist.OrderByDescending(d => d.tot_rank).First();
                Q.PrintQuality();
            }
        }

        //public void CreateNoisyPlan(List<ARDTO> ardto, int percent, int max_it=1000)
        //{
        //    List<Plan> plan_list = new List<Plan>();

        //    for (int j = 0; j < max_it; j++)
        //    {
        //        List<ARDTO> ardto_noisy = AddNoise(ardto, percent)
        //            .OrderByDescending(d => d.noisy_rank)
        //            .ThenByDescending(m => m.memory).ToList();

        //        Plan plan = new Plan(ardto_noisy);
        //        plan_list.Add(plan);
        //    }

        //    Plan best = plan_list.OrderByDescending(p => p.QualityPlan().tot_rank).First();
        //    best.QualityPlan().PrintQuality();
        //}

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

        public void Ruin(int k)
        {

        }

        public void Recreate()
        {

        }
    }
}
