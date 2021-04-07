using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SatelliteScheduler
{
    class Tester
    {
        int[] seed = { 0, 0, 4, 4, 4 };
        int[] k = { 25, 43, 13, 11, 39 };
        int[] noise = { 13, 1, 4, 13, 1 };
        double[] temp = { 0.01, 0.0001, 0.001, 0.0001, 0.01 };
        Plan plan;
        Instance instance;

        public Tester(Instance instance, int i)
        {
            this.instance = instance;
            Console.WriteLine("Test Piano in ordine di rank/memoria disturbato");
            instance.SetRandom(seed[i]);
            var watch = Stopwatch.StartNew();
            plan = Heuristics.CreateInitialPlan(instance, noise[i], 100);
            plan.QualityPlan().PrintQuality();
            watch.Stop();
            Console.WriteLine("Tempo medio: " + watch.ElapsedMilliseconds);

            Console.WriteLine("\nTest Piano con parametri ottimali: " + seed[i] + " " + k[i] + " " + noise[i] + " " + temp[i]);
            Quality Q = MediumQuality(k[i], noise[i], temp[i]);
            Q.PrintQualityTime();
        }

        public Quality MediumQuality(int k, int noise, double temp)
        {
            List<Plan> list = new List<Plan>();
            int n_run = 10;

            double[] time = new double[n_run];

            for (int i = 0; i < n_run; i++)
            {
                var watch = Stopwatch.StartNew();
                list.Add(SA(temp, k, noise));
                watch.Stop();
                time[i] = watch.ElapsedMilliseconds;
            }

            double n_ar = Math.Round(list.Select(p => p.QualityPlan().n_ar).Average(), 2);
            double tot_rank = Math.Round(list.Select(p => p.QualityPlan().tot_rank).Average(), 2);
            double memory = Math.Round(list.Select(p => p.QualityPlan().memory).Average(), 2);
            double mean_time = time.Average();

            Quality Q = new Quality(k, noise, temp, 100, n_ar, tot_rank, memory, mean_time);
            Q.SetMaxRank(instance.GetMaxRank());
            return Q;
        }

        //Apllica l'algoritmo Simulated Annealing per ottenere un'ipotetica soluzione migliore
        public Plan SA(double t_max, int k, int noise, int max_it = 100)
        {
            Plan best_plan = plan;

            double best_obj = best_plan.QualityPlan().tot_rank;
            double t0 = best_obj * t_max;
            double tf = t0 * t_max;
            double t = t0;
            double c = Math.Pow(tf / t0, 1 / (double)max_it);

            Plan current_plan = Plan.Copy(best_plan);

            for (int i = 0; i < max_it; i++)
            {
                Plan neighbor_plan = Plan.Copy(current_plan);
                neighbor_plan = Heuristics.Ruin(instance, neighbor_plan, k);
                neighbor_plan = Heuristics.Recreate(instance, neighbor_plan, noise);
                List<Plan> ps = Heuristics.CompareSA(best_plan, neighbor_plan, current_plan, t);

                best_plan = (ps[0] != null) ? Plan.Copy(ps[0]) : best_plan;
                current_plan = (ps[1] != null) ? Plan.Copy(ps[1]) : current_plan;

                t *= c;
            }
            return best_plan;
        }
    }
}