using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

            for (int k = 30; k <= 60; k += 5)
            {
                for (int noise = 1; noise <= 10; noise++)
                {
                    List<Plan> list = new List<Plan>();
                    for (int i = 0; i < 10; i++)
                    {
                        list.Add(RuinRecreate(instance, k, noise));
                    }
                    Plan min = list.Select(x => (x.QualityPlan().tot_rank, x)).Min().x;
                    Plan max = list.Select(x => (x.QualityPlan().tot_rank, x)).Max().x;

                    string[] lines = new string[]
                    {
                        "---------------------------",
                        "K-ruin: " + k + "\tNoise: " + noise,
                        "Piano minimo",
                        "---------------------------"
                    };

                    var x = lines.Union(min.QualityPlan().WriteQuality())
                        .Union(new string[] { "Piano massimo" })
                        .Union(max.QualityPlan().WriteQuality()).ToArray();

                    await ExampleAsync(x);
                }
            }
        }

        public static async Task ExampleAsync(string[] lines)
        {
            using StreamWriter file = new("test.txt", append: true);

            foreach (string line in lines)
            {
                await file.WriteLineAsync(line);
            }
        }

        public static Plan RuinRecreate(Instance instance, int k, int noise)
        {
            Plan best_plan = RuinAndRecreate.CreateInitialPlan(instance, noise, 100);
            for (int i = 0; i < 1000; i++)
            {
                Plan star_plan = Plan.Copy(best_plan);
                //int k = Convert.ToInt32(new Random().Next(1, 40));
                star_plan = RuinAndRecreate.Ruin(star_plan, k);
                star_plan = RuinAndRecreate.Recreate(instance, star_plan, noise);
                best_plan = RuinAndRecreate.Compare(best_plan, star_plan);
            }
            return best_plan;
        }
    }
}
