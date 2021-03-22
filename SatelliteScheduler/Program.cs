using System;
using System.Collections.Generic;

namespace SatelliteScheduler
{
    class Program
    {
        public static double max_mem;

        static void Main(string[] args)
        {
            string ars = System.IO.File.ReadAllText(@"day1_0/ARs.json");
            string dtos = System.IO.File.ReadAllText(@"day1_0/DTOs.json");
            string consts = System.IO.File.ReadAllText(@"day1_0/constants.json");

            Instance instance = new Instance(ars, dtos, consts);

            GeneratePlans(instance);

            Console.WriteLine("\nSTOP");
            Console.ReadLine();
        }

        // Genera 4 piani ordinati per: memoria, rank, rank/memoria, rank/memoria disturbato
        public static void GeneratePlans(Instance instance)
        {
            Console.WriteLine("Test Piano in ordine di memoria");
            instance.SortARDTOsByMemory();
            Plan plan_mem = new Plan(instance);
            plan_mem.BuildPlan();
            plan_mem.QualityPlan().PrintQuality();

            Console.WriteLine("\nTest Piano in ordine di rank");
            instance.SortARDTOsByDecreasingRank();
            Plan plan_rank = new Plan(instance);
            plan_rank.BuildPlan();
            plan_rank.QualityPlan().PrintQuality();

            Console.WriteLine("\nTest Piano in ordine di rank/memoria");
            instance.SortARDTOsByDecreasingRankOverMemory();
            Plan plan_rankmem = new Plan(instance);
            plan_rankmem.BuildPlan();
            plan_rankmem.QualityPlan().PrintQuality();

            Console.WriteLine("\nTest Piano in ordine di rank/memoria disturbato");
            Plan plan_noisyrankmem = Euristics.CreateInitialPlan(instance, 2, 100);
            plan_noisyrankmem.QualityPlan().PrintQuality();

            Plan rr = RuinRecreate(instance, plan_noisyrankmem);
            Console.WriteLine("--------------------------------");
            rr.QualityPlan().PrintQuality();

            Plan sa = SA(instance, plan_noisyrankmem);
            Console.WriteLine("--------------------------------");
            sa.QualityPlan().PrintQuality();

            //Tuner T = new Tuner(instance);
        }

        // Apllica l'algoritmo Ruin&Recreate per ottenere un'ipotetica soluzione migliore
        public static Plan RuinRecreate(Instance instance, Plan best_plan)
        {
            Console.WriteLine("--------------------------------");
            Console.WriteLine("RUIN & RECREATE");

            for (int i = 0; i < 1000; i++)
            {
                Plan star_plan = Plan.Copy(best_plan);
                int k = Convert.ToInt32(new Random().Next(1, 40));
                //int k = 40;
                star_plan = Euristics.Ruin(star_plan, k);
                star_plan = Euristics.Recreate(instance, star_plan, 2);
                best_plan = Euristics.Compare(best_plan, star_plan);
            }
            return best_plan;
        }

        // Apllica l'algoritmo Simulated Annealing per ottenere un'ipotetica soluzione migliore
        public static Plan SA(Instance instance, Plan best_plan, int max_it=1000)
        {
            Console.WriteLine("--------------------------------");
            Console.WriteLine("SIMULATED ANNEALING");

            double best_obj = best_plan.QualityPlan().tot_rank;
            double t0 = best_obj / 100;
            double tf = t0 / 100;
            double t = t0;
            double c = Math.Pow(tf / t0, 1 / (double)max_it);

            Plan current_plan = Plan.Copy(best_plan);

            for (int i = 0; i < max_it; i++)
            {
                Plan neighbor_plan = Plan.Copy(current_plan);
                //int k = Convert.ToInt32(new Random().Next(1, 40));
                int k = 40;
                neighbor_plan = Euristics.Ruin(neighbor_plan, k);
                neighbor_plan = Euristics.Recreate(instance, neighbor_plan, 2);
                List<Plan> ps = SimulatedAnnealing.Compare(best_plan, neighbor_plan, current_plan, t);

                if (ps[0] != null)
                {
                    best_plan = Plan.Copy(ps[0]);
                }

                if (ps[1] != null)
                {
                    current_plan = Plan.Copy(ps[1]);
                }

                t = c * t;
            }
            return best_plan;
        }


    }
}
