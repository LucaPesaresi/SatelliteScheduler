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



            //lista ordinata per ordine di: memoria meggiore, rank maggiore, casuale, casuale con disturbo
            Console.WriteLine("Test Piano in ordine di memoria");
            List<ARDTO> ardto_memory = ar_dto.OrderByDescending(d => d.memory).ToList();
            Plan plan_mem = new Plan(ardto_memory);
            plan_mem.QualityPlan().PrintQuality();

            Console.WriteLine("\nTest Piano in ordine di rank");
            List<ARDTO> ardto_rank = ar_dto.OrderByDescending(d => d.rank)
                .ThenByDescending(m => m.memory).ToList();
            Plan plan_rank = new Plan(ardto_rank);
            plan_rank.QualityPlan().PrintQuality();

            Console.WriteLine("\nTest Piano in ordine di rank/memoria");
            List<ARDTO> ardto_rankmem = ar_dto.OrderByDescending(d => d.rank / d.memory).ToList();
            Plan plan_rankmem = new Plan(ardto_rankmem);
            plan_rankmem.QualityPlan().PrintQuality();

            Console.WriteLine("\nTest Piano in ordine di rank/memoria disturbato");
            Solution S_rankmem = new Solution();
            Solution S_star = new Solution();
           
            S_rankmem.CreateNoisyPlan(ardto_rankmem, 1, 100);

            for (int i = 0; i < 10; i++)
            {
                S_rankmem.Start();
                S_rankmem.Ruin(20);
                S_rankmem.Recreate(ardto_rankmem, 3);
                S_rankmem.Compare();
            }

            Console.ReadLine();
        }

    }
}
