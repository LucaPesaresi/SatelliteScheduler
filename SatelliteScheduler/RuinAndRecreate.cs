using System;

namespace SatelliteScheduler
{
    class RuinAndRecreate
    {

        public static Plan CreateInitialPlan(Instance instance, double noise, int max_it=1000) {

            Plan best_plan = new Plan(instance);
            instance.SetRandomNoiseAndSortARDTOs(noise);
            best_plan.BuildPlan();
            double best_obj = best_plan.QualityPlan().tot_rank;

            for (int n = 0; n < max_it; n++) {

                Plan candidate_plan = new Plan(instance);
                instance.SetRandomNoiseAndSortARDTOs(noise);
                candidate_plan.BuildPlan();
                double candidate_obj = candidate_plan.QualityPlan().tot_rank;

                if (candidate_obj > best_obj) {
                    best_obj = candidate_obj;
                    best_plan = Plan.Copy(candidate_plan);
                }
            }

            return best_plan;
        }

        // Rimuove k elementi (in percentuale) dal piano
        public static Plan Ruin(Plan plan, int k)
        {
            int k_norm = Convert.ToInt32(plan.GetPlan().Count * k / 100);
            for (int i = 0; i < k_norm; i++)
            {
                plan.RemoveAt(new Random().Next(0, plan.GetPlan().Count));
            }

            return plan;
        }

        // Riempie il piano con una strategia diversificata
        public static Plan Recreate(Instance instance, Plan plan, int noise)
        {
            instance.SetRandomNoiseAndSortARDTOs(noise);
            plan.BuildPlan();
            return plan;
        }

        // Confronta il piano ricreato con quello precedente e nel caso sia migliore lo sostituisce
        public static Plan Compare(Plan P, Plan P_star)
        {
            double best_obj = P.QualityPlan().tot_rank;
            double star_obj = P_star.QualityPlan().tot_rank;

            if (star_obj > best_obj)
            {
                P = Plan.Copy(P_star);
                Console.WriteLine("--------------------------------");
                Console.WriteLine("Il nuovo piano risulta migliore");
                P.QualityPlan().PrintQuality();
            }
            return P;
        }
    }
}