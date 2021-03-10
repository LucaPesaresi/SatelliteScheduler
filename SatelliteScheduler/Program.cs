using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using static JsonCaster;

namespace SatelliteScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            var ars = System.IO.File.ReadAllText(@"day1_0/ARs.json");
            var dtos = System.IO.File.ReadAllText(@"day1_0/DTOs.json");

            List<AR> arList = JsonConvert.DeserializeObject<List<AR>>(ars);
            List<DTO> dtoList = JsonConvert.DeserializeObject<List<DTO>>(dtos);

            Console.WriteLine(arList[0].id +" " + dtoList[0].id);

        }
    }
}
