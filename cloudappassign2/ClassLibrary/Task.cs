using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Task
    {
        //---------public variables and classes defined--------//
        public int id;                                         //task id
        public int processorID;                              //processor id from matrix
        public int coef0, coef1, coef2;                        //3 coefficients from file
        public int ref_fre;                                    //reference frequency
        public double runtime;                                 //task rumtime under 2GHz processor
        public double energy;                                  //calculate:energy per task
        public double processor_fre;                           //corresponding processor frequency from the matrix
        public double runtime_underProcessor;                  //calculate: runtime under specific processor

        public Task()
        {
            id = 0;
            processorID = 0;
            processor_fre = 0;
            runtime = 0;
        }

        public void CalRuntime()
        {
            runtime_underProcessor = ref_fre * runtime / processor_fre;
            return;
        }

        public void CalEnergy()
        {
            double EnergyPerSec;
            //calculate energy per second
            EnergyPerSec = coef2 * processor_fre * processor_fre + coef1 * processor_fre + coef0;

            //calculate enery
            energy = runtime_underProcessor * EnergyPerSec;

        }
    }

}
