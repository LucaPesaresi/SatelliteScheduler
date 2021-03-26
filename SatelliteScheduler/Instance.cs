using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using static JsonCaster;


namespace SatelliteScheduler
{
    class Instance
    {

        private readonly double max_mem;
        private List<ARDTO> ar_dto;
        private Random rnd;

        public Instance(string ars, string dtos, string consts, int seed)
        {

            List<AR> ARlist = JsonConvert.DeserializeObject<List<AR>>(ars);
            List<DTO> DTOlist = JsonConvert.DeserializeObject<List<DTO>>(dtos);
            Costants constlist = JsonConvert.DeserializeObject<Costants>(consts);

            rnd = new Random(seed);

            max_mem = constlist.MEMORY_CAP;

            //merge AR con DTO
            ar_dto = new List<ARDTO>();
            DTOlist.ForEach(d =>
            {
                ARDTO ardto = new ARDTO(ARlist.Find(a => a.id == d.ar_id), d);
                ar_dto.Add(ardto);
            });

        }
        public double GetMaxMem() { return max_mem; }

        public void SortARDTOsByMemory() 
        {
            ar_dto = ar_dto.OrderByDescending(d => d.memory).ToList();
        }

        public void SortARDTOsByDecreasingRank()
        {
            ar_dto = ar_dto.OrderByDescending(d => d.rank).ToList();
        }

        public void SortARDTOsByDecreasingRankOverMemory() 
        {
            ar_dto = ar_dto.OrderByDescending(d => d.rank / d.memory).ToList();
        }

        public void SetRandomNoiseAndSortARDTOs(double max_noise)
        {

            foreach (ARDTO a in ar_dto)
            {
                double value_noise = (rnd.NextDouble() * 2 * max_noise) - max_noise;
                a.noisy_rank = (a.rank + value_noise) / a.memory;
            }

            ar_dto = ar_dto.OrderByDescending(d => d.noisy_rank).ToList();
        }

        public List<ARDTO> GetARDTOs() 
        {
            return ar_dto;
        }

        public ARDTO GetARDTO(int index) 
        {
            return ar_dto[index];
        }

        public Random GetRandom()
        {
            return rnd;
        }
    }
}
