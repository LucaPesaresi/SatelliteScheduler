using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SATuner
{
    class Tuner
    {
        int k_start, k_stop, k_inc, k_step, k_best;
        int noise_start, noise_stop, noise_inc, noise_step, noise_best;
        int seed;
        Instance instance;
        Plan plan;
        List<Quality> list;

        public List<Quality> GetList()
        {
            return list;
        }

        public Tuner(Instance instance, Plan plan, int seed)
        {
            this.instance = instance;
            this.plan = plan;
            this.seed = seed;
            //Console.WriteLine("\nTuning Simulated Annealing");
            //TuningSA(instance, plan);
        }

        public void RRSingle(int k, int noise)
        {
            list = new List<Quality>();
            k_start = k;
            noise_start = noise;
            list.Add(MediumQuality(k, noise));
        }

        public void BuildTuningRR(int k_start, int k_stop, int k_step, int noise_start, int noise_stop, int noise_step)
        {
            list = new List<Quality>();
            this.k_start = k_start;
            this.k_stop = k_stop;
            this.k_step = k_step;
            this.noise_start = noise_start;
            this.noise_stop = noise_stop;
            this.noise_step = noise_step;

            k_inc = Convert.ToInt32((k_stop - k_start) / k_step);
            noise_inc = Convert.ToInt32((noise_stop - noise_start) / noise_step);

            bool loop = true;
            Console.WriteLine("\nTuning Ruin&Recreate");
            while (loop)
            {
                loop = SetBestParamsRR(TuningRR());
            }
        }

        public Quality MediumQuality(int k, int noise)
        {
            List<Plan> Plist = new List<Plan>();
            for (int i = 0; i < 3; i++)
            {
                Plist.Add(RuinRecreate(k, noise));
            }

            double n_ar = Math.Round(Plist.Select(p => p.QualityPlan().n_ar).Average(), 2);
            double tot_rank = Math.Round(Plist.Select(p => p.QualityPlan().tot_rank).Average(), 2);
            double memory = Math.Round(Plist.Select(p => p.QualityPlan().memory).Average(), 2);

            return new Quality(n_ar, tot_rank, memory, k, noise);
        }

        public Quality TuningRR()
        {
            Quality Qbest = new Quality(0, 0, 0, 0, 0);

            for (int k = k_start; k <= k_stop; k += k_inc)
            {
                for (int noise = noise_start; noise <= noise_stop; noise += noise_inc)
                {
                    Quality Qnew = MediumQuality(k, noise);

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
            //Console.WriteLine("\n" + k_start + "\t" + k_stop + "\t" + k_inc +
            //    "\t" + noise_start + "\t" + noise_stop + "\t" + noise_inc);

            //Q.PrintQualityRR();

            list.Add(Q);

            if (Q.k - k_inc < 0) { k_start = 1; } else { k_start = Q.k - k_inc; }
            if (Q.k + k_inc > 100) { k_stop = 100; } else { k_stop = Q.k + k_inc; }

            if (Q.noise - noise_inc < 0) { noise_start = 1; } else { noise_start = Q.noise - noise_inc; }
            if (Q.noise + noise_inc > 100) { noise_stop = 100; } else { noise_stop = Q.noise + noise_inc; }

            k_inc = Convert.ToInt32((k_stop - k_start) / k_step);
            noise_inc = Convert.ToInt32((noise_stop - noise_start) / noise_step);
            if (k_stop - k_start <= 10 || noise_stop - noise_start <= 10)
            {
                k_best = Q.k;
                noise_best = Q.noise;
                return false;
            }

            return true;
        }

        public Plan RuinRecreate(int k, int noise)
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

        public void TuningSA()
        {
            Plan best_plan = plan;
            Plan current_plan = Plan.Copy(best_plan);

            //Writer("testSA_0.001-100.txt", "temp-factor;n_ar;tot_rank,memory");

            Quality Qbest = new Quality(0, 0, 0, 0.0, 0);

            for (double temp = 0.0001; temp < 1; temp*=10)
            {
                for (int it = 100; it <= 10000; it*=10)
                {
                    List<Plan> list = new List<Plan>();
                    for (int j = 0; j < 5; j++)
                    {
                        list.Add(SA(temp, it, seed));
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

        public Plan SA(double temp_max, int seed, int max_it = 1000)
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
                neighbor_plan = Euristics.Ruin(instance, neighbor_plan, k_best);
                neighbor_plan = Euristics.Recreate(instance, neighbor_plan, noise_best);
                List<Plan> ps = Euristics.CompareSA(best_plan, neighbor_plan, current_plan, t, seed);

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
