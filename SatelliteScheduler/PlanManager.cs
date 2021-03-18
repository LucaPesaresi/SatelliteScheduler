using System;
using System.Collections.Generic;
using System.Linq;
using static JsonCaster;

namespace SatelliteScheduler
{
    class PlanManager
    {

        // Inizializza l'algortimo mantendendo una copia del piano
        public Plan CopyPlan(Plan old)
        {
            Plan best_star = new Plan();

            foreach (ARDTO a in old.plan)
            {
                best_star.plan.Add(a);
            }

            return best_star;
        }

        // Rimuove k elementi (in percentuale) dal piano
        public Plan Ruin(Plan P, int k)
        {
            int k_norm = Convert.ToInt32(P.plan.Count * k / 100);
            for (int i = 0; i < k_norm; i++)
            {
                P.plan.RemoveAt(new Random().Next(0, P.plan.Count));
            }

            return P;
        }

        // Riempie il piano con una strategia diversificata
        public Plan Recreate(Plan P, List<ARDTO> ardto, int step_noise)
        {
            double mem = P.QualityPlan().memory;

            P.AddNoise(ardto, step_noise);
            P.ardto.OrderByDescending(d => d.noisy_rank).ToList();

            P.BuildPlan(mem);
            return P;
        }

        // Confronta il piano ricreato con quello precedente e nel caso sia migliore lo sostituisce
        public bool Compare(Plan P, Plan P_star)
        {
            Quality q_new = P.QualityPlan();
            Quality q_star = P_star.QualityPlan();
            return (q_new.tot_rank > q_star.tot_rank);
        }
    }
}