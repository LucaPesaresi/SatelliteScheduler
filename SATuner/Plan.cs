﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SATuner
{
    class Plan
    {
        private Instance instance;
        private List<ARDTO> plan;
        private double current_mem;

        public Plan(Instance instance)
        {
            this.instance = instance;
            this.plan = new List<ARDTO>();
            current_mem = 0;
        }

        public static Plan Copy(Plan source) 
        {
            Plan new_plan = new Plan(source.instance);

            foreach (ARDTO elem in source.plan)
            {
                new_plan.plan.Add(elem);
            }

            new_plan.current_mem = source.current_mem;
            return new_plan;
        }

        // Creazione di un piano con una lista randomizzata
        public void BuildPlan()
         { 
            bool ok; //indica se i vincoli sono rispettati
            int i = 0;

            if (current_mem == 0)
            {
                plan.Add(instance.GetARDTO(0));
                current_mem += plan[0].memory;
                i++;
            }
            //Console.WriteLine("id_ar\t\tid_dto\t\trank\thigh\tstart\t\t\tstop\t\t\tmemory");

            for (; i < instance.GetARDTOs().Count; i++)
            {
                ok = true;
                int j;
                for (j = 0; j < plan.Count; j++)
                {
                    //controllo che non ci sia un overlap temporale con i precedenti
                    if (instance.GetARDTO(i).stop_time >= plan[j].start_time &&
                        instance.GetARDTO(i).start_time <= plan[j].stop_time)
                    {
                        ok = false;
                        break;
                    }
                    //controllo che l'id_ar non sia già presente in quelli aggiunti
                    if (instance.GetARDTO(i).id_ar == plan[j].id_ar)
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                {
                    //controllo memoria libera
                    if (current_mem + instance.GetARDTO(i).memory <= instance.GetMaxMem())
                    {
                        //ardto[i].PrintAll();
                        plan.Add(instance.GetARDTO(i));
                        current_mem += instance.GetARDTO(i).memory;
                    }
                }
            }
        }

        public void RemoveAt(int index) 
        {
            current_mem -= plan[index].memory;
            plan.RemoveAt(index);
        }

        public List<ARDTO> GetPlan()
        {
            return this.plan;
        } 

        // Restituisce una qualità del piano in temrini di  numero di acquisizioni, 
        // rank totale e memoria occupata
        public Quality QualityPlan()
        {
            double rank = plan.Select(x => x.rank).Sum();
            double mem = Math.Round(plan.Select(x => x.memory).Sum(), 2);
            return new Quality(plan.Count, rank, mem, instance.GetMaxMem());
        }
    }

    public class Quality
    {
        public double n_ar { get; set; }
        public double tot_rank { get; set; }
        public double memory { get; set; }
        public int k { get; set; }
        public int noise { get; set; }
        public double tot_memory { get; set; }
        public double temp { get; set; }
        public int it_max { get; set; }

        public Quality(double n_ar, double tot_rank, double memory, double tot_memory)
        {
            this.n_ar = n_ar;
            this.tot_rank = tot_rank;
            this.memory = memory;
            this.tot_memory = tot_memory;
        }

        public Quality(double n_ar, double tot_rank, double memory, int k, int noise)
        {
            this.n_ar = n_ar;
            this.tot_rank = tot_rank;
            this.memory = memory;
            this.k = k;
            this.noise = noise;
        }

        public Quality(double n_ar, double tot_rank, double memory, double temp, int it_max)
        {
            this.n_ar = n_ar;
            this.tot_rank = tot_rank;
            this.memory = memory;
            this.temp = temp;
            this.it_max = it_max;
        }

        public string WriteQuality()
        {
            return n_ar + "," + tot_rank + "," + memory;
        }

        public void PrintQuality()
        {
            Console.WriteLine("Acquisizioni: " + n_ar);
            Console.WriteLine("Rank: " + tot_rank);
            Console.WriteLine("Memoria usata: " + memory + " su " + tot_memory + " GB");
        }

        public void PrintQualityRR()
        {
            Console.WriteLine(k.ToString() + "\t" + noise + "\t" + n_ar + "\t" 
                + tot_rank + "\t" + memory + "\t" + tot_memory);
        }

        public void PrintQualitySA()
        {
            Console.WriteLine(temp + "\t" + it_max + "\t" + n_ar + "\t" 
                + tot_rank + "\t" + memory + "\t" + tot_memory);
        }
    }
}
