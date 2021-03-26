using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SatelliteScheduler
{
    class Tuner
    {
        int k_start, k_stop, k_inc, k_best;
        int noise_start, noise_stop, noise_inc, noise_best;

        public Tuner(Instance instance, Plan plan)
        {
            k_start = 1; k_stop = 100; 
            k_inc = Convert.ToInt32(k_stop / 3);

            noise_start = 1; noise_stop = 100; 
            noise_inc = Convert.ToInt32(noise_stop / 3);

            bool loop = true;
            Console.WriteLine("\nTuning Ruin&Recreate");
            while (loop)
            {
                loop = SetBestParamsRR(TuningRR(instance));
            }
            Console.WriteLine("\nTuning Simulated Annealing");
            TuningSA(instance, plan);
        }

        public Quality MediumQuality(Instance ins, int k, int noise)
        {
            List<Plan> Plist = new List<Plan>();
            for (int i = 0; i < 1; i++)
            {
                Plist.Add(RuinRecreate(ins, k, noise));
            }

            double n_ar = Math.Round(Plist.Select(p => p.QualityPlan().n_ar).Average(), 2);
            double tot_rank = Math.Round(Plist.Select(p => p.QualityPlan().tot_rank).Average(), 2);
            double memory = Math.Round(Plist.Select(p => p.QualityPlan().memory).Average(), 2);

            return new Quality(n_ar, tot_rank, memory, k, noise);
        }

        public Quality TuningRR(Instance instance)
        {
            Quality Qbest = new Quality(0, 0, 0, 0, 0);

            for (int k = k_start; k <= k_stop; k += k_inc)
            {
                for (int noise = noise_start; noise <= noise_stop; noise += noise_inc)
                {
                    Quality Qnew = MediumQuality(instance, k, noise);

                    if (Qbest.tot_rank < Qnew.tot_rank)
                    {
                        Qbest = Qnew;
                    }
                    //string line = k + ";" + noise + ";" + n_ar + ";" + tot_rank + ";" + memory;
                    //Writer("testRR_15-30.txt", line);
                }
            }
            return Qbest;
        }

        public bool SetBestParamsRR(Quality Q)
        {
            Console.WriteLine("\n" + k_start + "\t" + k_stop + "\t" + k_inc +
                "\t" + noise_start + "\t" + noise_stop + "\t" + noise_inc);
            Q.PrintQualityRR();

            if (Q.k - k_inc < 0) { k_start = 1; } else { k_start = Q.k - k_inc; }
            if (Q.k + k_inc > 100) { k_stop = 100; } else { k_stop = Q.k + k_inc; }

            if (Q.noise - noise_inc < 0) { noise_start = 1; } else { noise_start = Q.noise - noise_inc; }
            if (Q.noise + noise_inc > 100) { noise_stop = 100; } else { noise_stop = Q.noise + noise_inc; }

            k_inc = Convert.ToInt32((k_stop - k_start) / 4);
            noise_inc = Convert.ToInt32((noise_stop - noise_start) / 4);
            if (k_stop - k_start <= 5 || noise_stop - noise_start <= 5)
            {
                k_best = Q.k;
                noise_best = Q.noise;
                return false;
            }

            return true;
        }

        public Plan RuinRecreate(Instance instance, int k, int noise)
        {
            Plan best_plan = Euristics.CreateInitialPlan(instance, noise, 100);
            for (int i = 0; i < 100; i++)
            {
                Plan star_plan = Plan.Copy(best_plan);
                star_plan = Euristics.Ruin(instance, star_plan, k);
                star_plan = Euristics.Recreate(instance, star_plan, noise);
                best_plan = Euristics.CompareRR(best_plan, star_plan);
            }
            return best_plan;
        }

        public void TuningSA(Instance instance, Plan best_plan)
        {
            Plan current_plan = Plan.Copy(best_plan);

            //Writer("testSA_0.001-100.txt", "temp-factor;n_ar;tot_rank,memory");

            Quality Qbest = new Quality(0, 0, 0, 0.0, 0);

            for (double temp = 0.0001; temp < 1; temp*=10)
            {
                for (int it = 100; it <= 10000; it*=10)
                {
                    List<Plan> list = new List<Plan>();
                    for (int j = 0; j < 10; j++)
                    {
                        list.Add(SA(instance, best_plan, temp, it));
                    }

                    double n_ar = Math.Round(list.Select(p => p.QualityPlan().n_ar).Average(), 2);
                    double tot_rank = Math.Round(list.Select(p => p.QualityPlan().tot_rank).Average(), 2);
                    double memory = Math.Round(list.Select(p => p.QualityPlan().memory).Average(), 2);
                    
                    Quality Qnew = new Quality(n_ar, tot_rank, memory, temp, it);
                    if (Qbest.tot_rank < Qnew.tot_rank)
                    {
                        Qbest = Qnew;
                        Qbest.PrintQualitySA();
                    }
                }
                //string line = temp + ";" + n_ar + ";" + tot_rank + ";" + memory;        
                //Writer("testSA_0001-0010.txt", line);            
            }
        }

        public Plan SA(Instance instance, Plan best_plan, double temp_max, int max_it = 1000)
        {
            double best_obj = best_plan.QualityPlan().tot_rank;
            double t0 = best_obj * temp_max;
            double tf = t0 * temp_max;
            double t = t0;
            double c = Math.Pow(tf / t0, 1 / (double)max_it);

            Plan current_plan = Plan.Copy(best_plan);

            for (int i = 0; i < max_it; i++)
            {
                Plan neighbor_plan = Plan.Copy(current_plan);
                neighbor_plan = Euristics.Ruin(instance, neighbor_plan, k_best);
                neighbor_plan = Euristics.Recreate(instance, neighbor_plan, noise_best);
                List<Plan> ps = Euristics.CompareSA(best_plan, neighbor_plan, current_plan, t);

                best_plan = (ps[0] != null) ? Plan.Copy(ps[0]) : best_plan;
                current_plan = (ps[1] != null) ? Plan.Copy(ps[1]) : current_plan;

                t*= c;
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
