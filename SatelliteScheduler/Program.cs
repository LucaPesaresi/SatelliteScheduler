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

            //Si eseguono dei test sui dati per capire i vincoli
            var dto_duplicati = DTOlist.GroupBy(x => x.ar_id)
              .Where(g => g.Count() > 1)
              .Select(y => new { Element = y.Key, Counter = y.Count() })
              .ToList();

            var dto_distinti = DTOlist.GroupBy(x => x.ar_id)
                .Select(y => y.OrderBy(m => m.memory).FirstOrDefault())
                .ToList();

            var dto_distinti2 = DTOlist.GroupBy(x => x.ar_id, (_, g) =>
                g.OrderByDescending(m => m.memory).FirstOrDefault())
                .ToList();


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
            Plan plan_memory = new Plan(ardto_memory);
            plan_memory.QualityPlan().PrintQuality();

            Console.WriteLine("\nTest Piano in ordine di rank");
            List<ARDTO> ardto_rank = ar_dto.OrderByDescending(d => d.rank).ToList();
            Plan plan_rank = new Plan(ardto_rank);
            plan_rank.QualityPlan().PrintQuality();

            int max_it = 100;
            Console.WriteLine("\nTest piano randomizzato");
            TestPlan(ar_dto, max_it);


            for (int noise = 1; noise < 10; noise+=2)
            {
                Console.WriteLine("\nTest piano randomizzato con disturbo " + noise);
                TestPlan(AddNoise(ar_dto, noise), max_it);
            }

            //Console.ReadLine();
        }

        public static void TestPlan(List<ARDTO> ar_dto, int max_it=100)
        {
            List<Quality> qlist = new List<Quality>();
            for (int i = 0; i < max_it; i++)
            {
                List<ARDTO> ardto_rnd = ar_dto.OrderBy(x => new Random().Next()).ToList();
                Plan plan_rnd = new Plan(ardto_rnd);
                qlist.Add(plan_rnd.QualityPlan());
            }
            Quality q = qlist.OrderByDescending(d => d.tot_rank).First();
            q.PrintQuality();
        }

        public static List<ARDTO> AddNoise(List<ARDTO> ardto, int noise)
        {
            ardto.ForEach(a =>
            {
                int value_noise = new Random().Next(0, 5);
                int isToNoise = new Random().Next(0, 10);
                int addOrSub = new Random().Next(0, 10);

                if (isToNoise >= noise)
                {
                    if (addOrSub >= noise)
                    {
                        a.rank += value_noise;
                    }
                    else
                    {
                        a.rank -= value_noise;
                    }
                }
            });

            return ardto;
        }
    }
}
