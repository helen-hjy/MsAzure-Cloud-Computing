using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Allocation
    {
        public int ID;                                               //read from file
        public int TaskNo;                                           //read TASKS
        public int ProNo;                                            //read PROCESSORS
        public double[] ProcessorFrequency;                          //store processor frequency 2d array                             
        public int[][] Table;                                        //data of a allocation
        public double program_max_duration;                          //total duration time limits for each processor
        public double Runtime;                                       //calculate processor runtime
        public double Energy;                                        //cal all task energy and sum up
        public List<Task> task = new List<Task>();                   //the task list
        public double[] pro_run;

        //constrcutor
        public Allocation()
        {
            TaskNo = 1;
            ProNo = 1;
            ID = 0;
            ProcessorFrequency = new double[100];
            pro_run = new double[100];
            Table = new int[ProNo][];
            for (int i = 0; i < ProNo; i++)
                Table[i] = new int[TaskNo];
        }

        //calculate allocation runtime according to the table it gets
        public void CalRuntime()
        {
            //assign processor runtime according to array index
            for (int i = 0; i < ProNo; i++)
            {
                for (int j = 0; j < task.Count; j++)
                {
                    if (Table[i][j] == 1)
                    {
                        pro_run[i] += task[j].runtime_underProcessor;
                    }
                }
            }

            //allocation runtime is the biggest processor runtime
            Runtime = Math.Round(pro_run.Max(), 2);                  //keep 2 digits
        }

        //calculate energy by formula
        public void CalEnergy()
        {
            Energy = 0;
            for (int a = 0; a < task.Count; a++)
                Energy += task[a].energy;

            Energy = Math.Round(Energy, 2);                    //keep 2 digits
        }
    }

}
