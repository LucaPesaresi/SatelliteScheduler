using System;
using System.Collections.Generic;

namespace SatelliteScheduler
{
    class Program
    {
        public static double max_mem;
        public static int seed = 2;
        public static int k_ruin = 20;
        public static int noise = 15;
        public static double t_max = 0.01;
        public static int max_it = 1000;

        static void Main(string[] args)
        {
            Console.WriteLine("***************************************");
            //Console.WriteLine("Configurazione: " + string.Join(",", args));
            //SetParams(args);

            args = new string[] { "0", "0", "0", "0", "0" };
            args[0] = Environment.CurrentDirectory + @"\day1_1\";

            string ars = System.IO.File.ReadAllText(args[0] + "ARs.json");
            string dtos = System.IO.File.ReadAllText(args[0] + "DTOs.json");
            string consts = System.IO.File.ReadAllText(args[0] + "constants.json");


            Instance instance = new Instance(ars, dtos, consts, seed);

            GeneratePlans(instance);
        }

        private static void SetParams(string[] args)
        {
            bool res;
            res = int.TryParse(args[1], out seed);
            res = int.TryParse(args[2], out k_ruin);
            res = int.TryParse(args[3], out noise);
            res = double.TryParse(args[4], out t_max);
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

            //Console.WriteLine("\nTest Piano in ordine di rank/memoria disturbato");
            Plan plan_noisyrankmem = Euristics.CreateInitialPlan(instance, noise, max_it);
            //plan_noisyrankmem.QualityPlan().PrintQuality();

            Plan rr = RuinRecreate(instance, plan_noisyrankmem, max_it);
            Console.WriteLine("--------------------------------");
            rr.QualityPlan().PrintQuality();

            Plan sa = SA(instance, plan_noisyrankmem, t_max, max_it);
            Console.WriteLine("--------------------------------");
            sa.QualityPlan().PrintQuality();

            //Tuner T = new Tuner(instance, plan_noisyrankmem);
        }

        // Apllica l'algoritmo Ruin&Recreate per ottenere un'ipotetica soluzione migliore
        public static Plan RuinRecreate(Instance instance, Plan best_plan, int max_it=1000)
        {
            Console.WriteLine("--------------------------------");
            Console.WriteLine("RUIN & RECREATE");

            for (int i = 0; i < max_it; i++)
            {
                Plan star_plan = Plan.Copy(best_plan);
                star_plan = Euristics.Ruin(instance, star_plan, k_ruin);
                star_plan = Euristics.Recreate(instance, star_plan, noise);
                best_plan = Euristics.CompareRR(best_plan, star_plan);
            }
            return best_plan;
        }

        // Apllica l'algoritmo Simulated Annealing per ottenere un'ipotetica soluzione migliore
        public static Plan SA(Instance instance, Plan best_plan, double t_max, int max_it = 1000)
        {
            Console.WriteLine("--------------------------------");
            Console.WriteLine("SIMULATED ANNEALING");

            double best_obj = best_plan.QualityPlan().tot_rank;
            double t0 = best_obj * t_max;
            double tf = t0 * t_max;
            double t = t0;
            double c = Math.Pow(tf / t0, 1 / (double)max_it);

            Plan current_plan = Plan.Copy(best_plan);

            for (int i = 0; i < max_it; i++)
            {
                Plan neighbor_plan = Plan.Copy(current_plan);
                neighbor_plan = Euristics.Ruin(instance, neighbor_plan, k_ruin);
                neighbor_plan = Euristics.Recreate(instance, neighbor_plan, noise);
                List<Plan> ps = Euristics.CompareSA(best_plan, neighbor_plan, current_plan, t);

                best_plan = (ps[0] != null) ? Plan.Copy(ps[0]) : best_plan;
                current_plan = (ps[1] != null) ? Plan.Copy(ps[1]) : current_plan;

                t *= c;
            }
            return best_plan;
        }
    }
}
