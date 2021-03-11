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

            List<AR> ARlist = JsonConvert.DeserializeObject<List<AR>>(ars);
            List<DTO> DTOlist = JsonConvert.DeserializeObject<List<DTO>>(dtos);

            //Si eseguono dei test sui dati per capire i vincoli

            var dto_duplicati = DTOlist.GroupBy(x => x.ar_id)
              .Where(g => g.Count() > 1)
              .Select(y => new { Element = y.Key, Counter = y.Count() })
              .ToList();

            var dto_distinti = DTOlist.GroupBy(x => x.ar_id)
                .Select(y => y.ToList().OrderBy(m => m.memory).First())
                .ToList();

            //non funziona l'OR e non filtra gli overlap temporali
            var dto_no_overlap = dto_distinti
                .Where(y => dto_distinti
                    .Any(x => x.stop_time < y.start_time || x.start_time > y.stop_time))
                .ToList();

            //non funzionano gli operatori OR-AND e non filtra gli overlap temporali
            var dto_no_overlap2 = dto_distinti
                .Where(y => !dto_distinti
                    .Any(x => (x.start_time > y.start_time && x.start_time < y.stop_time) || (x.stop_time > y.start_time && x.stop_time < y.stop_time)))
                .ToList();


            //CreatePlan(ARlist, DTOList);     

            Console.WriteLine(ARlist[0].id + " " + DTOlist[0].id);

        }


        //private static void CreatePlan(List<AR> ARlist, List<DTO> DTOList)
        //{
        //    int n = 500;
        //    int max = ARlist.Count;
        //    bool duplicato = false;

        //    var rnd = new Random();
        //    List<int> randomNumbers = Enumerable.Range(0, max).OrderBy(x => rnd.Next()).Take(n).ToList();
        //    List<AR> ARlim = new List<AR>(n);

        //    int i;
        //    for (i = 0;  i < ARlist.Count; i++)
        //    {
        //        //controllo che l'id_ar dto non sia già presente in quelli aggiunti
        //        int j = 0;
        //        do
        //        {
        //            duplicato = true;
        //        }
        //        while (ARlim[j].id != DTOList[randomNumbers[i]].ar_id && j < ARlim.Count);


        //        if (ARlist[i].id != DTOList[randomNumbers[i]].ar_id)
        //        {
                    
        //        }
        //        else
        //        {

        //        }
        //        ARlim.Add(ARlist[randomNumbers[i]]);
        //    }            

        //    int a = 2;
        //}
    }
}
