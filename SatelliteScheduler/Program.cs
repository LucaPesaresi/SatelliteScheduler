using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using static JsonCaster;

namespace SatelliteScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            string ars = System.IO.File.ReadAllText(@"day1_0/ARs.json");
            string dtos = System.IO.File.ReadAllText(@"day1_0/DTOs.json");
            string consts = System.IO.File.ReadAllText(@"day1_0/constants.json");

            List<AR> ARlist = JsonConvert.DeserializeObject<List<AR>>(ars);
            List<DTO> DTOlist = JsonConvert.DeserializeObject<List<DTO>>(dtos);
            Costants constlist = JsonConvert.DeserializeObject<Costants>(consts);

            //List<DTO> DTOlist1 = new List<DTO>();
            //DTOlist1.Add(DTOlist[0]);

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

            //double sum = 0;
            //dto_distinti2.ForEach(d =>
            //{
            //    sum += d.memory;
            //});

            //var dto_no_overlap1 = DTOlist
            //    .Where(y => DTOlist
            //        .Any(x => x.stop_time < y.start_time))
            //    .Where(y => DTOlist
            //        .Any(x => x.start_time > y.stop_time))
            //    .ToList();

            //merge AR con DTO
            List<ARDTO> ar_dto = new List<ARDTO>();
            DTOlist.ForEach(d =>
            {
                ARDTO ardto = new ARDTO(ARlist.Find(a => a.id == d.ar_id), d);
                ar_dto.Add(ardto);
            });

            //Lista ordinata per ordine: casuale, memoria minore, rank maggiore
            List<ARDTO> ardto_rnd = ar_dto.OrderBy(x => new Random().Next()).ToList();
            Plan plan_rnd = new Plan(ardto_rnd, constlist.MEMORY_CAP);

            List<ARDTO> ardto_memory = ar_dto.OrderBy(d => d.memory).ToList();
            Plan plan_memory = new Plan(ardto_memory, constlist.MEMORY_CAP);

            List<ARDTO> ardto_rank = ar_dto.OrderByDescending(d => d.rank).ToList();
            Plan plan_rank = new Plan(ardto_rank, constlist.MEMORY_CAP);

        }
    }
}
