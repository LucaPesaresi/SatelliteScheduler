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
        Instance instance;
        Plan plan;
        Quality Qbest;

        public Tuner(Instance instance, Plan plan)
        {
            this.instance = instance;
            this.plan = plan;
            Qbest = new Quality(0, 0, 0, 0, 0);
            Qbest.SetMaxRank(instance.GetMaxRank());
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

            k_inc = (k_stop - k_start) / k_step;
            noise_inc = (noise_stop - noise_start) / noise_step;

            Console.WriteLine("\nTuning Ruin&Recreate");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("-----------GIRO-" + (i + 1) + "---------------");

                Stopwatch watch = Stopwatch.StartNew();
                SetBestParamsRR(TuningRR());
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("Tempo: " + elapsedMs + " ms");
            }
        }

        public Quality TuningRR()
        {
            for (int k = k_start; k <= k_stop; k += k_inc)
            {
                for (int noise = noise_start; noise <= noise_stop; noise += noise_inc)
                {
                    Quality Qnew = MediumQualityRR(k, noise);

                    //if (Qbest.tot_rank < Qnew.tot_rank)
                    //{
                    //    Qbest = Qnew;
                    //    Qbest.PrintQualityRR();
                    //}

                    if (Qnew.GetGap() < Qbest.GetGap())
                    {
                        Qbest = Qnew;
                        Qbest.PrintQualityRR();
                    }
                    //Qnew.PrintQualityRR();
                    //string line = k + ";" + noise + ";" + n_ar + ";" + tot_rank + ";" + memory;
                    //Writer("testRR_15-30.txt", line);
                }
            }
            return Qbest;
        }

        public Quality MediumQualityRR(int k, int noise)
        {
            List<Plan> Plist = new List<Plan>();
            for (int i = 0; i < 5; i++)
            {
                Plist.Add(RuinRecreate(k, noise));
            }

            double n_ar = Math.Round(Plist.Select(p => p.QualityPlan().n_ar).Average(), 2);
            double tot_rank = Math.Round(Plist.Select(p => p.QualityPlan().tot_rank).Average(), 2);
            double memory = Math.Round(Plist.Select(p => p.QualityPlan().memory).Average(), 2);

            Quality Q = new Quality(k, noise, n_ar, tot_rank, memory);
            Q.SetMaxRank(instance.GetMaxRank());
            return Q;
        }

        public void SetBestParamsRR(Quality Q)
        {
            //Console.WriteLine("\n" + k_start + "\t" + k_stop + "\t" + k_inc +
            //    "\t" + noise_start + "\t" + noise_stop + "\t" + noise_inc);
            //Q.PrintQualityRR();

            if (Q.k - k_inc <= 0) { k_start = 1; } else { k_start = Q.k - k_inc; }
            if (Q.k + k_inc >= 100) { k_stop = 100; } else { k_stop = Q.k + k_inc; }

            if (Q.noise - noise_inc <= 0) { noise_start = 1; } else { noise_start = Q.noise - noise_inc; }
            if (Q.noise + noise_inc >= 100) { noise_stop = 100; } else { noise_stop = Q.noise + noise_inc; }

            int k_diff = k_stop - k_start;
            int n_diff = noise_stop - noise_start;

            k_inc = Convert.ToInt32(k_diff / k_step);
            noise_inc = Convert.ToInt32(n_diff / noise_step);

            k_best = Q.k;
            noise_best = Q.noise;
        }

        public Plan RuinRecreate(int k, int noise, int max_it = 100)
        {
            Plan best_plan = Heuristics.CreateInitialPlan(instance, noise, max_it);
            for (int i = 0; i < max_it; i++)
            {
                Plan star_plan = Plan.Copy(best_plan);
                star_plan = Heuristics.Ruin(instance, star_plan, k);
                star_plan = Heuristics.Recreate(instance, star_plan, noise);
                best_plan = Heuristics.CompareRR(best_plan, star_plan);
            }
            return best_plan;
        }

        public Quality MediumQualitySA(double temp)
        {
            List<Plan> Plist = new List<Plan>();
            for (int i = 0; i < 5; i++)
            {
                Plist.Add(SA(temp));
            }

            double n_ar = Math.Round(Plist.Select(p => p.QualityPlan().n_ar).Average(), 2);
            double tot_rank = Math.Round(Plist.Select(p => p.QualityPlan().tot_rank).Average(), 2);
            double memory = Math.Round(Plist.Select(p => p.QualityPlan().memory).Average(), 2);

            Quality Q = new Quality(temp, 100, n_ar, tot_rank, memory);
            Q.SetMaxRank(instance.GetMaxRank());
            return Q;
        }

        public void TuningSA(double temp_start, double temp_stop, int temp_inc)
        {
            Console.WriteLine("\nTuning Simulated Annealing");
            Stopwatch watch = Stopwatch.StartNew();
            Qbest = new Quality(0.0, 0, 0.0, 0.0, 0.0);
            Qbest.SetMaxRank(instance.GetMaxRank());

            //Writer("testSA_0.001-100.txt", "temp-factor;n_ar;tot_rank,memory");

            for (double temp = temp_start; temp <= temp_stop; temp *= temp_inc)
            {
                Quality Qnew = MediumQualitySA(temp);

                //if (Qbest.tot_rank < Qnew.tot_rank)
                //{
                //    Qbest = Qnew;
                //    Qbest.PrintQualitySA();
                //}
                if (Qnew.GetGap() < Qbest.GetGap())
                {
                    Qbest = Qnew;
                    Qbest.PrintQualitySA();
                }

                //Qnew.PrintQualitySA();
                //string line = temp + ";" + n_ar + ";" + tot_rank + ";" + memory;        
                //Writer("testSA_0001-0010.txt", line);            
            }
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Tempo: " + elapsedMs + " ms");
        }

        public Plan SA(double temp_max, int max_it = 100)
        {
            Plan best_plan = plan;
            double best_obj = best_plan.QualityPlan().tot_rank;
            double t0 = best_obj * temp_max;
            double tf = t0 * temp_max;
            double t = t0;
            double c = Math.Pow(tf / t0, 1 / (double)max_it);

            Plan current_plan = Plan.Copy(best_plan);

            for (int i = 0; i < max_it; i++)
            {
                Plan neighbor_plan = Plan.Copy(current_plan);
                neighbor_plan = Heuristics.Ruin(instance, neighbor_plan, k_best);
                neighbor_plan = Heuristics.Recreate(instance, neighbor_plan, noise_best);
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
