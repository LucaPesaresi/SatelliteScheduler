using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SatelliteScheduler
{
    class Tuner
    {
        public Tuner(Instance instance)
        {
            Tuning(instance);
        }

        public async void Tuning(Instance instance)
        {
            Console.WriteLine("\nTUNING");

            for (int k = 1; k <= 60; k += 5)
            {
                for (int noise = 1; noise <= 10; noise++)
                {
                    List<Plan> list = new List<Plan>();
                    for (int i = 0; i < 10; i++)
                    {
                        list.Add(RuinRecreate(instance, k, noise));
                    }
                    //Plan min = list.Select(x => (x.QualityPlan().tot_rank, x)).Min().x;
                    //Plan max = list.Select(x => (x.QualityPlan().tot_rank, x)).Max().x;
                    Plan min = list.OrderBy(p => p.QualityPlan().tot_rank).First();
                    Plan max = list.OrderByDescending(p => p.QualityPlan().tot_rank).First();

                    string line = k + "," + noise + "," 
                        + min.QualityPlan().WriteQuality() + ","
                        + max.QualityPlan().WriteQuality();

                    using StreamWriter file = new StreamWriter("test.txt", append: true);
                    await file.WriteLineAsync(line);
                }
            }
        }

        public static Plan RuinRecreate(Instance instance, int k, int noise)
        {
            Plan best_plan = Euristics.CreateInitialPlan(instance, noise, 100);
            for (int i = 0; i < 100; i++)
            {
                Plan star_plan = Plan.Copy(best_plan);
                //int k = Convert.ToInt32(new Random().Next(1, 40));
                star_plan = Euristics.Ruin(star_plan, k);
                star_plan = Euristics.Recreate(instance, star_plan, noise);
                best_plan = Euristics.Compare(best_plan, star_plan);
            }
            return best_plan;
        }
    }
}
