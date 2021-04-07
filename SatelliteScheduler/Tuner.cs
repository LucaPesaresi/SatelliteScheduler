using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SatelliteScheduler
{
    class Tuner
    {
        int k_start, k_stop, k_inc, k_step;
        int noise_start, noise_stop, noise_inc, noise_step;
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

            k_inc = (int)Math.Floor(Convert.ToDecimal((k_stop - k_start) / k_step));
            noise_inc = (int)Math.Floor(Convert.ToDecimal((noise_stop - noise_start) / noise_step));

            Console.WriteLine("\nTuning Ruin&Recreate");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("-----------GIRO-" + (i + 1) + "---------------");
                TuningRR();
                SetBestParamsRR();
            }
            string line = Qbest.k + ";" + Qbest.noise + ";" + Qbest.n_ar + ";" + Qbest.tot_rank + ";" + Qbest.GetGap() + ";" + Qbest.memory;
            Writer("testRR-seed" + Program.seed +".txt", line);
        }

        public void TuningRR()
        {
            for (int k = k_start, i=1; k <= k_stop; k = k_start + (k_inc * i), i++)
            {
                for (int noise = noise_start, j=1; noise <= noise_stop; noise = noise_start + (noise_inc * j), j++)
                {
                    Quality Qnew = MediumQualityRR(k, noise);

                    if (Qnew.GetGap() <= Qbest.GetGap())
                    {
                        Qbest = Qnew;
                        //Qbest.PrintQualityRR();
                    }
                    Qnew.PrintQualityRR();
                }
            }
            Console.Write("Migliore: ");
            Qbest.PrintQualityRR();
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

        public void SetBestParamsRR()
        {
            //Console.WriteLine("\n" + k_start + "\t" + k_stop + "\t" + k_inc +
            //    "\t" + noise_start + "\t" + noise_stop + "\t" + noise_inc);
            //Q.PrintQualityRR();

            if (Qbest.k - k_inc <= 0) { k_start = 1; } else { k_start = Qbest.k - k_inc; }
            if (Qbest.k + k_inc >= 100) { k_stop = 100; } else { k_stop = Qbest.k + k_inc; }

            if (Qbest.noise - noise_inc <= 0) { noise_start = 1; } else { noise_start = Qbest.noise - noise_inc; }
            if (Qbest.noise + noise_inc >= 100) { noise_stop = 100; } else { noise_stop = Qbest.noise + noise_inc; }

            k_inc = (int)Math.Floor(Convert.ToDecimal((k_stop - k_start) / k_step));
            noise_inc = (int)Math.Floor(Convert.ToDecimal((noise_stop - noise_start) / noise_step));
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

            Quality Q = new Quality(Qbest.k, Qbest.noise, temp, 100, n_ar, tot_rank, memory);
            Q.SetMaxRank(instance.GetMaxRank());
            return Q;
        }

        public void TuningSA(double temp_start, double temp_stop, int temp_inc)
        {
            Console.WriteLine("\nTuning Simulated Annealing");

            Qbest.tot_rank = 0;
                
            //Writer("testSA_0.001-100.txt", "temp-factor;n_ar;tot_rank,memory");

            for (double temp = temp_start; temp <= temp_stop; temp *= temp_inc)
            {
                Quality Qnew = MediumQualitySA(temp);

                if (Qnew.GetGap() <= Qbest.GetGap())
                {
                    Qbest = Qnew;
                    //Qbest.PrintQualitySA();
                }

                Qnew.PrintQualitySA();
                //string line = temp + ";" + n_ar + ";" + tot_rank + ";" + memory;        
                //Writer("testSA_0001-0010.txt", line);            
            }
            Console.Write("Migliore: con " + Qbest.k + " " + Qbest.noise + " ");
            Qbest.PrintQualitySA();

            string line = Qbest.k + ";" + Qbest.noise + ";" + Qbest.temp + ";" + Qbest.n_ar + ";" + Qbest.tot_rank + ";" + Qbest.GetGap() + ";" + Qbest.memory;
            Writer("testSA-seed" + Program.seed + ".txt", line);
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
                neighbor_plan = Heuristics.Ruin(instance, neighbor_plan, Qbest.k);
                neighbor_plan = Heuristics.Recreate(instance, neighbor_plan, Qbest.noise);
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
