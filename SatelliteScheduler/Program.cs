using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using static JsonCaster;

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

            List<AR> ARlist = JsonConvert.DeserializeObject<List<AR>>(ars);
            List<DTO> DTOlist = JsonConvert.DeserializeObject<List<DTO>>(dtos);
            Costants constlist = JsonConvert.DeserializeObject<Costants>(consts);

            max_mem = constlist.MEMORY_CAP;

            //merge AR con DTO
            List<ARDTO> ar_dto = new List<ARDTO>();
            DTOlist.ForEach(d =>
            {
                ARDTO ardto = new ARDTO(ARlist.Find(a => a.id == d.ar_id), d);
                ar_dto.Add(ardto);
            });

            GeneratePlans(ar_dto);

            Console.WriteLine("\nSTOP");
            Console.ReadLine();
        }

        // Genera 4 piani ordinati per: memoria, rank, rank/memoria, rank/memoria disturbato
        public static void GeneratePlans(List<ARDTO> ar_dto)
        {
            Console.WriteLine("Test Piano in ordine di memoria");
            List<ARDTO> ardto_memory = ar_dto.OrderByDescending(d => d.memory).ToList();
            Plan plan_mem = new Plan();
            plan_mem.BuildPlan(ardto_memory);
            plan_mem.QualityPlan().PrintQuality();

            Console.WriteLine("\nTest Piano in ordine di rank");
            List<ARDTO> ardto_rank = ar_dto.OrderByDescending(d => d.rank).ToList();
            Plan plan_rank = new Plan();
            plan_rank.BuildPlan(ardto_rank);
            plan_rank.QualityPlan().PrintQuality();

            Console.WriteLine("\nTest Piano in ordine di rank/memoria");
            List<ARDTO> ardto_rankmem = ar_dto.OrderByDescending(d => d.rank / d.memory).ToList();
            Plan plan_rankmem = new Plan();
            plan_rankmem.BuildPlan(ardto_rankmem);
            plan_rankmem.QualityPlan().PrintQuality();

            Console.WriteLine("\nTest Piano in ordine di rank/memoria disturbato");
            RuinRecreate(ardto_rankmem);
        }

        // Apllica l'algoritmo Ruin&Recreate per ottenere un'ipotetica soluzione migliore
        public static void RuinRecreate(List<ARDTO> ardto)
        {
            Plan P = new Plan();
            P.CreateNoisyARDTO(ardto, 2, 100);
           
            Console.WriteLine("--------------------------------");
            Console.WriteLine("RUIN & RECREATE");
            for (int i = 0; i < 1000; i++)
            {
                Plan P_star = PlanManager.CopyPlan(P);

                //int k = Convert.ToInt32(new Random().Next(1, 40));
                int k = 40;
                P = PlanManager.Ruin(P, k);

                PlanManager.Recreate(P, ardto, 2);

                if (PlanManager.Compare(P, P_star))
                {
                    P_star = PlanManager.CopyPlan(P);
                    Console.WriteLine("--------------------------------");
                    Console.WriteLine("Il nuovo piano risulta migliore");
                    P.QualityPlan().PrintQuality();
                }
                else
                {
                    P = PlanManager.CopyPlan(P_star);
                }
            }
        }
    }
}
