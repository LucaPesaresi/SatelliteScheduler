using System;
using System.Collections.Generic;
using System.Linq;
using static JsonCaster;

namespace SatelliteScheduler
{
    class PlanManager
    {

        // Crea una una copia del piano
        public static Plan CopyPlan(Plan old)
        {
            Plan best_star = new Plan();

            foreach (ARDTO a in old.GetPlan())
            {
                best_star.GetPlan().Add(a);
            }

            return best_star;
        }

        // Rimuove k elementi (in percentuale) dal piano
        public static Plan Ruin(Plan plan, int k)
        {
            int k_norm = Convert.ToInt32(plan.GetPlan().Count * k / 100);
            for (int i = 0; i < k_norm; i++)
            {
                plan.GetPlan().RemoveAt(new Random().Next(0, plan.GetPlan().Count));
            }

            return plan;
        }

        // Riempie il piano con una strategia diversificata
        public static Plan Recreate(Plan P, List<ARDTO> ardto, int step_noise)
        {
            double mem = P.QualityPlan().memory;
            ardto.OrderByDescending(d => d.noisy_rank).ToList();
            P.BuildPlan(Plan.AddNoise(ardto, step_noise), mem);
            return P;
        }

        // Confronta il piano ricreato con quello precedente e nel caso sia migliore lo sostituisce
        public static bool Compare(Plan P, Plan P_star)
        {
            Quality q_new = P.QualityPlan();
            Quality q_star = P_star.QualityPlan();
            return (q_new.tot_rank > q_star.tot_rank);
        }
    }
}