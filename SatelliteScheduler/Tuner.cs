using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SatelliteScheduler
{
    class Tuner
    {
        int k_start, k_stop, k_inc, k_step, k_best;
        int noise_start, noise_stop, noise_inc, noise_step, noise_best;
        double best_gap, temp_best;
        readonly Instance[] instances;

        public Tuner(Instance[] instances)
        {
            this.instances = instances;
            best_gap = 1000;
        }

        public void BuildRR(int k_start, int k_stop, int k_step,
            int n_start, int n_stop, int n_step)
        {
            this.k_start = k_start;
            this.k_stop = k_stop;
            this.k_step = k_step;

            noise_start = n_start;
            noise_stop = n_stop;
            noise_step = n_step;

            k_inc = (int)Math.Floor(Convert.ToDecimal((k_stop - k_start) / k_step));
            noise_inc = (int)Math.Floor(Convert.ToDecimal((noise_stop - noise_start) / noise_step));

            Console.WriteLine("\nTuning Ruin&Recreate");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("-----------GIRO-" + (i + 1) + "---------------");
                TuningRR();
                SetBestParamsRR();
            }
        }

        public void TuningRR()
        {
            string line;
            for (int k = k_start, i=1; k <= k_stop; k = k_start + (k_inc * i), i++)
            {
                for (int noise = noise_start, j = 1; noise <= noise_stop; noise = noise_start + (noise_inc * j), j++)
                {
                    double new_gap = MediumQualityRR(k, noise);
                    
                    if (new_gap <= best_gap)
                    {
                        best_gap = new_gap;
                        k_best = k;
                        noise_best = noise;
                    }
                    line = "Nuovo gap: " + new_gap + "%  " + k + " " + noise;
                    Console.WriteLine(line);
                    Writer("RR-opt.txt", line);
                }
            }
            line = "Migliore: " + best_gap + "%  " + k_best + " " + noise_best;
            Console.Write(line);
            Writer("RR-opt.txt", line);
        }

        public double MediumQualityRR(int k, int noise)
        {
            double[] gaps = new double[instances.Length];
            int c = 0;

            //ciclo delle istanze
            foreach (var inst in instances)
            {
                List<Plan> Plist = new List<Plan>();
                for (int i = 0; i < 5; i++)
                {
                    Plist.Add(RuinRecreate(inst, k, noise));
                }

                double tot_rank = Math.Round(Plist.Select(p => p.QualityPlan().tot_rank).Average(), 2);
                gaps[c] = GetGap(tot_rank, inst.GetMaxRank());
                c++;
            }
            return gaps.Average();
        }

        public double GetGap(double tot_rank, double max_rank)
        {
            return Math.Round(100 - (100 * tot_rank) / max_rank, 3);
        }

        public void SetBestParamsRR()
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

        public double MediumQualitySA(double temp)
        {
            double[] gaps = new double[instances.Length];
            int c = 0;

            //ciclo delle istanze
            foreach (var inst in instances)
            {
                List<Plan> Plist = new List<Plan>();
                for (int i = 0; i < 5; i++)
                {
                    Plist.Add(SA(inst, temp));
                }

                double tot_rank = Math.Round(Plist.Select(p => p.QualityPlan().tot_rank).Average(), 2);
                gaps[c] = GetGap(tot_rank, inst.GetMaxRank());
                c++;
            }
            return gaps.Average();
        }

        public void TuningSA(double temp_start, double temp_stop, int temp_inc)
        {
            Console.WriteLine("\nTuning Simulated Annealing");
            string line;
            best_gap = 1000;
                
            //Writer("testSA_0.001-100.txt", "temp-factor;n_ar;tot_rank,memory");

            for (double temp = temp_start; temp <= temp_stop; temp *= temp_inc)
            {
                double new_gap = MediumQualitySA(temp);

                if (new_gap <= best_gap)
                {
                    best_gap = new_gap;
                    temp_best = temp;
                }
                line = "Nuovo gap: " + new_gap + "%  " + k_best + " " + noise_best + " " + temp;
                Console.WriteLine(line);
                Writer("SA-opt.txt", line);
            }
            line = "Migliore gap: " + best_gap + "%  " + k_best + " " + noise_best + " " + temp_best;
            Console.Write(line);
            Writer("SA-opt.txt", line);

            //string line = Qbest.k + ";" + Qbest.noise + ";" + Qbest.temp + ";" + Qbest.n_ar + ";" + Qbest.tot_rank + ";" + Qbest.GetGap() + ";" + Qbest.memory;
            //Writer("testSA-seed" + Program.seed + ".txt", line);
        }

        public Plan SA(Instance inst, double temp_max, int max_it = 100)
        {
            Plan best_plan = Heuristics.CreateInitialPlan(inst, noise_best, max_it);
            double best_obj = best_plan.QualityPlan().tot_rank;
            double t0 = best_obj * temp_max;
            double tf = t0 * temp_max;
            double t = t0;
            double c = Math.Pow(tf / t0, 1 / (double)max_it);

            Plan current_plan = Plan.Copy(best_plan);

            for (int i = 0; i < max_it; i++)
            {
                Plan neighbor_plan = Plan.Copy(current_plan);
                neighbor_plan = Heuristics.Ruin(inst, neighbor_plan, k_best);
                neighbor_plan = Heuristics.Recreate(inst, neighbor_plan, noise_best);
                List<Plan> ps = Heuristics.CompareSA(best_plan, neighbor_plan, current_plan, t);

                best_plan = (ps[0] != null) ? Plan.Copy(ps[0]) : best_plan;
                current_plan = (ps[1] != null) ? Plan.Copy(ps[1]) : current_plan;

                t *= c;
            }
            //best_plan.QualityPlan().PrintQualitySA();
            return best_plan;
        }

        public async void Writer(string fileName, string line)
        {
            using StreamWriter file = new StreamWriter(fileName, append: true);
            await file.WriteLineAsync(line);
        }
    }
}
