using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SatelliteScheduler
{
    class Program
    {
        //public static double max_mem;
        public static int seed = 0;
        public static int k_ruin = 7;
        public static int noise = 4;
        public static double t_max = 0.0001;
        public static int max_it = 10000;

        static void Main(string[] args)
        {
            Console.WriteLine("***************************************");
            Instance[] instances = new Instance[5];

            for (int i = 0; i < 5; i++)
            {
                //Console.WriteLine("\nTuning su istanza " + (i+1) + "\n");

                args = new string[] { "0", "0", "0", "0", "0" };
                args[0] = Environment.CurrentDirectory + @"\day1_" + i + "\\";

                string ars = System.IO.File.ReadAllText(args[0] + "ARs.json");
                string dtos = System.IO.File.ReadAllText(args[0] + "DTOs.json");
                string consts = System.IO.File.ReadAllText(args[0] + "constants.json");

                instances[i] = new Instance(ars, dtos, consts, seed);
                //GeneratePlans(instances[i]);
            }

            //Tuner T = new Tuner(instances);
            //T.BuildTuning(0, 10, 30, 3, 10, 30, 3);
            //T.BuildTuning(1, 10, 30, 3, 10, 30, 3);

            Tuner T = new Tuner(instances);
            Console.WriteLine("R&R");
            //k_ruin = 18; noise = 8;
            k_ruin = 24; noise = 16;
            double[] q = T.MediumQuality(0, k_ruin, noise, max_it);
            Console.WriteLine("Nuovo gap: " + q[0] + " " + q[1] + " " + q[2] + " " + k_ruin + " " + noise + " " + q[3]);

            Console.WriteLine("\nSA");
            //k_ruin = 10; noise = 10;
            k_ruin = 20; noise = 12;
            q = T.MediumQuality(1, k_ruin, noise, max_it);
            Console.WriteLine("Nuovo gap: " + q[0] + " " + q[1] + " " + q[2] + " " + k_ruin + " " + noise + " " + q[3]);
        }

        // Genera 4 piani ordinati per: memoria, rank, rank/memoria, rank/memoria disturbato
        public static void GeneratePlans(Instance instance)
        {
            //Console.WriteLine("Test Piano in ordine di memoria");
            //instance.SortARDTOsByMemory();
            //Plan plan_mem = new Plan(instance);
            //plan_mem.BuildPlan();
            //plan_mem.QualityPlan().PrintQuality();

            //Console.WriteLine("\nTest Piano in ordine di rank");
            //instance.SortARDTOsByDecreasingRank();
            //Plan plan_rank = new Plan(instance);
            //plan_rank.BuildPlan();
            //plan_rank.QualityPlan().PrintQuality();

            //Console.WriteLine("\nTest Piano in ordine di rank/memoria");
            //instance.SortARDTOsByDecreasingRankOverMemory();
            //Plan plan_rankmem = new Plan(instance);
            //plan_rankmem.BuildPlan();
            //plan_rankmem.QualityPlan().PrintQuality();

            Console.WriteLine("\nTest Piano in ordine di rank/memoria disturbato");
            Plan plan_noisyrankmem = Heuristics.CreateInitialPlan(instance, noise, max_it);
            plan_noisyrankmem.QualityPlan().PrintQuality();

            //Stopwatch watch = Stopwatch.StartNew();
            //Plan rr = RuinRecreate(instance, plan_noisyrankmem, max_it);
            //Console.WriteLine("--------------------------------");
            //rr.QualityPlan().PrintQuality();
            //watch.Stop();
            //var elapsedMs = watch.ElapsedMilliseconds;
            //Console.WriteLine("Tempo: " + elapsedMs + " ms");

            //watch = Stopwatch.StartNew();
            //Plan sa = SA(instance, plan_noisyrankmem, t_max, max_it);
            //Console.WriteLine("--------------------------------");
            //sa.QualityPlan().PrintQuality();
            //watch.Stop();
            //elapsedMs = watch.ElapsedMilliseconds;
            //Console.WriteLine("Tempo: " + elapsedMs + " ms");
        }

        //Apllica l'algoritmo Ruin&Recreate per ottenere un'ipotetica soluzione migliore
        public static Plan RuinRecreate(Instance instance, Plan best_plan, int max_it)
        {
            Console.WriteLine("--------------------------------");
            Console.WriteLine("RUIN & RECREATE");

            for (int i = 0; i < max_it; i++)
            {
                Plan star_plan = Plan.Copy(best_plan);
                star_plan = Heuristics.Ruin(instance, star_plan, k_ruin);
                star_plan = Heuristics.Recreate(instance, star_plan, noise);
                best_plan = Heuristics.CompareRR(best_plan, star_plan);
            }

            return best_plan;
        }

        //Apllica l'algoritmo Simulated Annealing per ottenere un'ipotetica soluzione migliore
        public static Plan SA(Instance instance, Plan best_plan, double t_max, int max_it)
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
                neighbor_plan = Heuristics.Ruin(instance, neighbor_plan, k_ruin);
                neighbor_plan = Heuristics.Recreate(instance, neighbor_plan, noise);
                List<Plan> ps = Heuristics.CompareSA(best_plan, neighbor_plan, current_plan, t);

                best_plan = (ps[0] != null) ? Plan.Copy(ps[0]) : best_plan;
                current_plan = (ps[1] != null) ? Plan.Copy(ps[1]) : current_plan;

                t *= c;
            }
            return best_plan;
        }
    }
}
