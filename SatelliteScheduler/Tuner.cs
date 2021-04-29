using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SatelliteScheduler
{
    class Tuner
    {
        int k_start, k_stop, k_inc, k_step, k_best;
        int noise_start, noise_stop, noise_inc, noise_step, noise_best;
        double best_gap, time_best;
        readonly Instance[] instances;
        string line;

        public Tuner(Instance[] instances)
        {
            this.instances = instances;
        }

        public void BuildTuning(int H, int k_start, int k_stop, int k_step,
            int n_start, int n_stop, int n_step)
        {
            best_gap = 1000;

            this.k_start = k_start;
            this.k_stop = k_stop;
            this.k_step = k_step;

            noise_start = n_start;
            noise_stop = n_stop;
            noise_step = n_step;

            k_inc = (int)Math.Floor(Convert.ToDecimal((k_stop - k_start) / k_step));
            noise_inc = (int)Math.Floor(Convert.ToDecimal((noise_stop - noise_start) / noise_step));

            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("-----------GIRO-" + (i + 1) + "---------------");
                Tuning(H);
                SetBestParams();
            }
            Writer(H + "-opt.txt", line);
        }

        public void Tuning(int H)
        {
            for (int k = k_start, i=1; k <= k_stop; k = k_start + (k_inc * i), i++)
            {
                for (int noise = noise_start, j = 1; noise <= noise_stop; noise = noise_start + (noise_inc * j), j++)
                {
                    double[] quality = MediumQuality(H, k, noise);
                    double new_gap = quality[0];
                    //double low_gap = quality[1];
                    //double high_gap = quality[2];
                    //double time = quality[3];

                    if (new_gap <= best_gap)
                    {
                        best_gap = new_gap;
                        k_best = k;
                        noise_best = noise;
                        //time_best = time;
                    }
                    line = "Nuovo gap: " + new_gap + "%  " + k + " " + noise /* +" " + time*/;
                    Console.WriteLine(line);
                    //Writer("RR-opt.txt", line);
                }
            }
            line = "Migliore: " + best_gap + "%  " + k_best + " " + noise_best /*+ " " + time_best*/;
            Console.Write(line);
            
        }

        public double[] MediumQuality(int H, int k, int noise, int max_it = 100)
        {
            double[] ranks = new double[5];
            double[] gaps = new double[instances.Length];
            int max_run = 5;
            double[] times = new double[25];
            int c = 2;
            int j = 0;
            //ciclo delle istanze
            //foreach (var inst in instances)
            //{
            List<Plan> Plist = new List<Plan>();
            for (int i = 0; i < max_run; i++)
            {
                for (int s = 0; s < 5; s++, j++)
                {
                    instances[c].SetRandom(s);
                    var watch = Stopwatch.StartNew();

                    if (H == 0)
                    {
                        Plist.Add(RuinRecreate(instances[c], k, noise, max_it));
                    }
                    else
                    {
                        Plist.Add(SA(instances[c], k, noise, 0.01, max_it));
                    }

                    watch.Stop();
                    times[j] = Convert.ToDouble(watch.ElapsedMilliseconds);
                }
            }
            double tot_rank = Math.Round(Plist.Select(p => p.QualityPlan().tot_rank).Average(), 2);
            double min_rank = Plist.Select(p => p.QualityPlan().tot_rank).Min();
            double max_rank = Plist.Select(p => p.QualityPlan().tot_rank).Max();

            gaps[0] = GetGap(max_rank, instances[c].GetMaxRank());
            gaps[1] = GetGap(tot_rank, instances[c].GetMaxRank());
            gaps[2] = GetGap(min_rank, instances[c].GetMaxRank());
            gaps[3] = times.Average();
            c++;
            //}
            return gaps/*.Average()*/;
        }

        public double GetGap(double tot_rank, double max_rank)
        {
            //return Math.Round(100 - (100 * tot_rank) / max_rank, 3);
            return Math.Round(100 * (max_rank - tot_rank) / max_rank, 3);
        }

        public void SetBestParams()
        {
            if (k_best - k_inc <= 0) { k_start = 1; } else { k_start = k_best - k_inc; }
            if (k_best + k_inc >= 100) { k_stop = 100; } else { k_stop = k_best + k_inc; }

            if (noise_best - noise_inc <= 0) { noise_start = 1; } else { noise_start = noise_best - noise_inc; }
            if (noise_best + noise_inc >= 100) { noise_stop = 100; } else { noise_stop = noise_best + noise_inc; }

            k_inc = (int)Math.Floor(Convert.ToDecimal((k_stop - k_start) / k_step));
            noise_inc = (int)Math.Floor(Convert.ToDecimal((noise_stop - noise_start) / noise_step));
        }

        public Plan RuinRecreate(Instance inst, int k, int noise, int max_it = 100)
        {
            Plan best_plan = Heuristics.CreateInitialPlan(inst, noise, max_it);
            for (int i = 0; i < max_it; i++)
            {
                Plan star_plan = Plan.Copy(best_plan);
                star_plan = Heuristics.Ruin(inst, star_plan, k);
                star_plan = Heuristics.Recreate(inst, star_plan, noise);
                best_plan = Heuristics.CompareRR(best_plan, star_plan);
            }
            return best_plan;
        }

        public Plan SA(Instance inst, int k, int noise, double temp = 0.01, int max_it = 100)
        {
            Plan best_plan = Heuristics.CreateInitialPlan(inst, noise, max_it);
            double best_obj = best_plan.QualityPlan().tot_rank;
            double t0 = best_obj * temp;
            double tf = t0 * temp;
            double t = t0;
            double c = Math.Pow(tf / t0, 1 / (double)max_it);

            Plan current_plan = Plan.Copy(best_plan);

            for (int i = 0; i < max_it; i++)
            {
                Plan neighbor_plan = Plan.Copy(current_plan);
                neighbor_plan = Heuristics.Ruin(inst, neighbor_plan, k);
                neighbor_plan = Heuristics.Recreate(inst, neighbor_plan, noise);
                List<Plan> ps = Heuristics.CompareSA(best_plan, neighbor_plan, current_plan, t);

                best_plan = (ps[0] != null) ? Plan.Copy(ps[0]) : best_plan;
                current_plan = (ps[1] != null) ? Plan.Copy(ps[1]) : current_plan;

                t *= c;
            }
            return best_plan;
        }

        public async void Writer(string fileName, string line)
        {
            using StreamWriter file = new StreamWriter(fileName, append: true);
            await file.WriteLineAsync(line);
        }
    }
}
