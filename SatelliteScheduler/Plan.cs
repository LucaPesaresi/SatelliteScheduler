using System;
using System.Collections.Generic;
using System.Linq;
using static JsonCaster;

namespace SatelliteScheduler
{
    class Plan
    {
        private Instance instance;
        private List<ARDTO> plan;
        private double current_mem;

        public Plan(Instance instance)
        {
            this.instance = instance;
            plan = new List<ARDTO>();
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

            int n = instance.GetARDTOs().Count;

            for (; i < n; i++)
            {
                ARDTO newArdto = instance.GetARDTO(i);
                ok = true;
                int j;
                for (j = 0; j < plan.Count; j++)
                { 
                    //controllo che non ci sia un overlap temporale con i precedenti
                    if (newArdto.stop_time >= plan[j].start_time &&
                        newArdto.start_time <= plan[j].stop_time)
                    {
                        ok = false;
                        break;
                    }
                    //controllo che l'id_ar non sia già presente in quelli aggiunti
                    if (newArdto.id_ar == plan[j].id_ar)
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                {
                    //controllo memoria libera
                    if (current_mem + newArdto.memory <= instance.GetMaxMem())
                    {
                        //ardto[i].PrintAll();
                        plan.Add(newArdto);
                        current_mem += newArdto.memory;
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
            return new Quality(plan.Count, rank, mem, instance.GetMaxMem(), instance.GetMaxRank());
        }
    }


}
