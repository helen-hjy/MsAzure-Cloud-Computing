using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    //store all tan file data
    public class TanData
    {
        public int allo_No = 0;                                              //read ALLOCATIONS
        public int task_No = 0;                                              //read TASKS
        public int pro_No = 0;                                               //read PROCESSORS
        public double[,] Pro_Fre = new double[100, 100];                     //store processor frequency 2d array                             
        public List<Allocation> Allocate = new List<Allocation>();           //store allocation data

    }

}
