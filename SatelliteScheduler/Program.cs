using System;

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
            RuinRecreate(instance);
        }

        // Apllica l'algoritmo Ruin&Recreate per ottenere un'ipotetica soluzione migliore
        public static void RuinRecreate(Instance instance)
        {

            Plan best_plan = RuinAndRecreate.CreateInitialPlan(instance, 2, 100);
            best_plan.QualityPlan().PrintQuality();

            Console.WriteLine("--------------------------------");
            Console.WriteLine("RUIN & RECREATE");
            for (int i = 0; i < 1000; i++)
            {
                Plan star_plan = Plan.Copy(best_plan);

                //int k = Convert.ToInt32(new Random().Next(1, 40));
                int k = 40;
                star_plan = RuinAndRecreate.Ruin(star_plan, k);
                star_plan = RuinAndRecreate.Recreate(instance, star_plan, 2);
                best_plan = RuinAndRecreate.Compare(best_plan, star_plan);
            }
        }
    }
}
