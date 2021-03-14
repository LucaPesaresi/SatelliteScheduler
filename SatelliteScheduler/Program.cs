using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using static JsonCaster;

namespace SatelliteScheduler
{
    class Program
    {
        static double memoryMax;

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

            memoryMax = constlist.MEMORY_CAP;

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

            //creazione di un piano di acquisizioni
            List<ARDTO> plan = CreatePlan(ar_dto);

            double rank_mean = plan.Select(x => x.rank).Average();

            double mem = plan.Select(x => x.memory).Sum();

            Console.WriteLine("Acquisizioni: "+ plan.Count);
            Console.WriteLine("Rank medio: " + rank_mean);
            Console.WriteLine("Memoria usata: " + mem + " su " + memoryMax);
        }

        //private static bool isDuplicated(DTO dto, List<DTO> list)
        //{
        //    foreach (DTO d in list)
        //    {
        //        if (d.ar_id == dto.ar_id)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //Creazione di un piano con una lista randomizzata

        private static List<ARDTO> CreatePlan(List<ARDTO> ardto)
        {
            //int n = 500;
            int max = ardto.Count;
            bool ok = true;
            bool full = false;
            double current_mem = 0;

            var rnd = new Random();
            //List<int> randomNumbers = Enumerable.Range(0, max).OrderBy(x => rnd.Next())/*.Take(n)*/.ToList();

            //lista ordinata per ordine: casuale, memoria minore, rank maggiore
            List<ARDTO> ardto_ordered = ardto.OrderBy(x => rnd.Next()).ToList();
            //List<ARDTO> ardto_ordered = ardto.OrderBy(d => d.memory).ToList();
            //List<ARDTO> ardto_ordered = ardto.OrderByDescending(d => d.rank).ToList();

            //lista finale del piano
            List<ARDTO> list = new List<ARDTO>(/*n*/);

            //list.Add(ardto[randomNumbers[0]]);
            list.Add(ardto_ordered[0]);
            current_mem += list[0].memory;

            int i;
            for (i = 1; i < max && !full; i++)
            {
                ok = true;
                int j = 0;
                for (j=0; j < list.Count && ok; j++)
                {
                    //controllo che l'id_ar non sia già presente in quelli aggiunti
                    if (ardto_ordered[i].id_ar == list[j].id_ar)
                    {
                        // vai al successivo
                        i++;
                        ok = false;
                    }
                    //controllo che non ci sia un overlap temporale con i precedenti
                    if (ardto_ordered[i].stop_time >= list[j].start_time ||
                        ardto_ordered[i].start_time <= list[j].stop_time)
                    {
                        i++;
                        ok = false;
                    }
                }
                //controllo memoria libera
                if (current_mem + ardto_ordered[i].memory <= memoryMax)
                {
                    ardto_ordered[i].PrintAll();
                    list.Add(ardto_ordered[i]);
                    current_mem += ardto_ordered[i].memory;
                }
                else
                {
                    full = true;
                }
            }
            return list;
        }
    }
}
