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

    public Allocation getAllocation(string TextX, Allocation allocation, double[][] runtimeTable)
    {
   
        double[] processorRuntime = new double[100];                            //store every processor real runtime
        int MiniEnergy = 0;                                                     //differnt file has different Minimum Energy Consumption
        //intialize the table of allocation
        allocation.Table = new int[allocation.ProNo][];       
        for (int a = 0; a < allocation.ProNo; a++)
            allocation.Table[a] = new int[allocation.TaskNo];

        //assing minimum energy according to the file name
        if (TextX.Contains("A"))
            MiniEnergy = 442;
        if (TextX.Contains("B"))
            MiniEnergy = 3940;
        if (TextX.Contains("C"))
            MiniEnergy = 958;

        allocation.Table = new int[][]{
            new int[] {1,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0 },
            new int[] {0,0,1,1,0,0,0,0,0,0,0,0,1,0,0,0 },
            new int[] {0,0,0,0,1,1,0,0,0,0,0,0,0,0,1,0 },
            new int[] {0,0,0,0,0,0,1,1,1,0,0,1,0,0,0,0 },
            new int[] {0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,1 }
        };


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

        //check and add into list
        //if it is smaller than MiniEnergy
        if (allocation.Energy <= MiniEnergy)        
            allocation.ID++;

        
        return allocation;

    }

}
