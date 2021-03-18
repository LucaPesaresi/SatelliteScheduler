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

        // Testa i piani generati con un rumore crescente
        public void TestNoisyPlan(List<ARDTO> ardto, int max_it = 1000)
        {
            double max_rank = ardto.OrderByDescending(d => d.noisy_rank).First().rank;
            
            for (int i = 0; i <= max_rank;  i += (int)max_rank / 50)
            {
                Console.WriteLine("\nTest piano con rumore " + i + "%");
                CreateNoisyPlan(ardto, i, max_it);
            }
        }

        // Sceglie il piano migliore generando diversi piani disturbato 
        public void CreateNoisyPlan(List<ARDTO> ardto, int noise, int max_it = 1000)
        {
            List<Plan> plan_list = new List<Plan>();

            for (int j = 0; j < max_it; j++)
            {
                List<ARDTO> ardto_noisy = AddNoise(ardto, noise)
                    .OrderByDescending(d => d.noisy_rank / d.memory).ToList();

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

        // Inizializza l'algortimo mantendendo una copia del piano
        public void Start()
        {
            best_star = new Plan();

            foreach (ARDTO a in best.plan)
            {
                best_star.plan.Add(a);
            }
        }

        // Rimuove k elementi (in percentuale) dal piano
        public void Ruin(int k)
        {
            int k_norm = Convert.ToInt32((best.plan.Count * k) / 100);
            for (int i = 0; i < k_norm; i++)
            {
                best.plan.RemoveAt(new Random().Next(0, best.plan.Count));
            }
        }

        // Riempie il piano con una strategia diversificata
        public void Recreate(List<ARDTO> ardto, int step_noise)
        {
            double mem = best.QualityPlan().memory;
            List<ARDTO> noisy_ardto = AddNoise(ardto, step_noise)
                .OrderByDescending(d => d.noisy_rank / d.memory)
                .ToList();  
            best.BuildPlan(noisy_ardto, mem);
        }

        // Confronta il piano ricreato con quello precedente e nel caso sia migliore lo sostituisce
        public void Compare()
        {
            Quality q_new = best.QualityPlan();
            Quality q_star = best_star.QualityPlan();

            if (q_new.tot_rank > q_star.tot_rank)
            {
                best_star = best;
                Console.WriteLine("--------------------------------");
                Console.WriteLine("Il nuovo piano risulta migliore");
                q_new.PrintQuality();
            }
            else
            {
                best = best_star;
            }
        }
    }
}
