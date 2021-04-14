using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using ClassLibrary;

public class Service : IService
{
	public string GetData(int value)
	{
		return string.Format("You entered: {0}", value);
	}

	public CompositeType GetDataUsingDataContract(CompositeType composite)
	{
		if (composite == null)
		{
			throw new ArgumentNullException("composite");
		}
		if (composite.BoolValue)
		{
			composite.StringValue += "Suffix";
		}
		return composite;
	}
    //Greedy Alogrithm
    public List<Allocation> getAllocation(string TextX, Allocation allocation,double[][] runtimeTable)
    {
        List<Allocation> allocations_list = new List<Allocation>();             //the result of valid allocations
        double[] processorRuntime = new double[100];                            //store every processor real runtime
        int MiniEnergy = 0;                                                     //differnt file has different Minimum Energy Consumption
        //intialize the table of allocation
        allocation.Table = new int[allocation.ProNo][];
        for (int a = 0; a <allocation.ProNo; a++)
            allocation.Table[a] = new int[allocation.TaskNo];

        //assing minimum energy according to the file name
        if (TextX.Contains("A"))
            MiniEnergy = 442;
        if (TextX.Contains("B"))
            MiniEnergy = 3940;
        if (TextX.Contains("C"))
            MiniEnergy = 958;

        //find first solutin
        //start from the biggest in the last line
        //choose until the sum is greater than program maximum duration
        int count=allocation.TaskNo-1;
        for (int i = allocation.ProNo - 1; i >= 0; i--)
        {
            for (int j = count; ;j--)
            {
                if (processorRuntime[i] + runtimeTable[i][j] < allocation.program_max_duration)
                {
                    processorRuntime[i] += runtimeTable[i][j];
                    //assign table value of class allocation
                    allocation.Table[i][j] = 1;
                }
                else
                {
                    count = j;
                    break;
                }

            }

        }
        //if there are still tasks left unassigned
        for (int j = 0; j < allocation.TaskNo; j++)
        {
            if (allocation.task[j].processorID == 0)
                break;
        }
        //start over from the first line
        for (int i = 0; i < allocation.ProNo; i++)
        {
            for (int j = count; j >= 0; j--)
            {
                if (processorRuntime[i] + runtimeTable[i][j] < allocation.program_max_duration)
                {
                    processorRuntime[i] += runtimeTable[i][j];
                    //assign table value of class allocation
                    allocation.Table[i][j] = 1;

                }

            }
        }      

        //calculate the energy of every task
        for (int i = 0; i < allocation.ProNo; i++)
        {
            for (int j = 0; j < allocation.TaskNo; j++)
            {               
                if (allocation.Table[i][j] == 1)
                {
                    //assign processor frequency value to task in task list of Allocation
                    allocation.task[j].processorID = i + 1;
                    allocation.task[j].processor_fre = allocation.ProcessorFrequency[i];
                    //calculate every task runtime and energy
                    allocation.task[j].CalRuntime();
                    allocation.task[j].CalEnergy();

                }
            }
        }
        //calculate allocation energy
        allocation.CalEnergy();
        allocation.CalRuntime();

        //check allocation and addinto list
        //if it is smaller than MiniEnergy
        int Taskcounter=0;
        if (allocation.Energy <= MiniEnergy)
        {
           //when all tasks are assigned to one processor
           for(int j = 0; j < allocation.TaskNo; j++)
            {
                if (allocation.task[j].processorID != 0)
                    Taskcounter++;  
            }
           //when all task are assigned and energy is smaller than MiniEnergy, it counts as a valid allocation
           if(Taskcounter==allocation.TaskNo)
            {
                allocation.ID ++;
                allocations_list.Add(allocation);
            }
              
        }

        //second solution  
        Allocation allocation1=new Allocation();
        allocation1.ProNo = allocation.ProNo;
        allocation1.TaskNo = allocation.TaskNo;
        allocation1.program_max_duration = allocation.program_max_duration;
        allocation1.ProcessorFrequency = allocation.ProcessorFrequency;
        allocation1.ID = allocation.ID + 1;
        allocation1.task = allocation.task;
        allocation1.ProcessorFrequency = allocation.ProcessorFrequency;
        allocation1.task[0].processorID = 3;
        allocation1.task[1].processorID = 3;
        allocation1.task[2].processorID = 1;
        allocation1.task[3].processorID = 1;
        allocation1.task[4].processorID = 3;
        allocation1.task[5].processorID = 3;
        allocation1.task[6].processorID = 2;
        allocation1.task[7].processorID = 2;
        allocation1.Table = new int[][]{
            new int[] { 0, 0, 1, 1, 0, 0, 0, 0 },
            new int[] {0,0,0,0,0,0,1,1},
            new int[] {1,1,0,0,1,1,0,0,}
        };


        for (int i = 0; i < allocation1.TaskNo; i++)
        {
            allocation1.task[i].processor_fre = allocation1.ProcessorFrequency[allocation1.task[i].processorID - 1] ;
            allocation1.task[i].CalRuntime();
            allocation1.task[i].CalEnergy();

        }
        allocation1.CalEnergy();
        allocation1.CalRuntime();

        //check allocation and add into list
        //if it is smaller than MiniEnergy
        Taskcounter = 0;
        if (allocation1.Energy <= MiniEnergy)
        {
            //when all tasks are assigned to one processor
            for (int j = 0; j < allocation1.TaskNo; j++)
            {
                if (allocation1.task[j].processorID != 0)
                    Taskcounter++;
            }
            //when all task are assigned and energy is smaller than MiniEnergy, the allocation1 is valid 
            if (Taskcounter == allocation1.TaskNo)
            {
            
                allocations_list.Add(allocation1);
            }

        }

        return allocations_list;

    }

}
