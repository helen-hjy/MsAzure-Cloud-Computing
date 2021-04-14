using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using ClassLibrary;
using Task = ClassLibrary.Task;
using System.Net.Sockets;

namespace AWSassignment2
{

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Assignment2 by Jiayu Han";
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "Copyright ©2019 Jiayu Han Deakin Uni.All rights reserved. ";
            string caption = "About";
            MessageBox.Show(message, caption);
        }

        private void openTANFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
       
        //method to check if the string contain english char(Only contains numbers)
        private bool ContainChar(string candidate)
        {
            //if contains any english char
            foreach (char c in candidate)
            {
                if ((c >= 'a' && c <= 'z') | (c >= 'A' && c <= 'Z'))
                    return true;
            }
            return false;
        }

        private void AddText(string text)
        {
            textBox1.Text += text;
            
        }
        //assignment2
        private void GObutton_Click(object sender, EventArgs e)
        {
            //read the file from cloud through web client
            string source = @URLcomboBox.Text;                     //input the source address from combobox
            WebClient webClient = new WebClient();
            Stream stream = webClient.OpenRead(source);
            StreamReader sr = new StreamReader(stream);            //read all file
            textBox1.Text = sr.ReadToEnd();                        //temporarily put into textbox
            sr.Close();
            
            //-----store data into variables-----//
            //create all variables for class members and data in the file
            string[] Line = textBox1.Lines;
            string[] WordPerLine;
            int LineNo=0;                                          //store line no
            int runtime_prefer_fre = 0;
            int program_tasks=0;
            int program_processors=0;
            double program_maxi_duration=0;
            double[,] tasks_run=new double[100,2];
            double[,] procoessor_frequency=new double[100,2];
            int[] coef_value= new int[3];
            //create classes instances
            List<Task> task_list=new List<Task>();
            Allocation allocation=new Allocation();
           

            //go through every line of file
            //read the data into variables
            for (; LineNo < Line.Length; LineNo++)
            {
                string LineTest = Line[LineNo];
                if (LineTest.Contains(','))
                {
                    WordPerLine = LineTest.Split(',');
                    if (LineTest.Contains("PROGRAM-TASKS"))
                    {
                        program_tasks = Convert.ToInt32(WordPerLine[1]);

                    }
                    else if (LineTest.Contains("PROGRAM-PROCESSORS"))
                    {
                        program_processors = Convert.ToInt32(WordPerLine[1]);
                    }
                    else if (LineTest.Contains("PROGRAM-MAXIMUM-DURATION"))
                    {
                        program_maxi_duration = Convert.ToDouble(WordPerLine[1]);

                    }
                    else if (LineTest.Contains("RUNTIME-REFERENCE-FREQUENCY"))
                    {
                        runtime_prefer_fre = Convert.ToInt32(WordPerLine[1]);
                    }
                    else if (LineTest.Contains("TASK-ID"))
                    {
                        int TaskLine = 0;                                 //store line No
                        //go through following TASK lines 
                        for (TaskLine = LineNo + 1; !ContainChar(Line[TaskLine]) && Line[TaskLine].Trim().Replace(",", "").Length != 0; TaskLine++)
                        {
                            //use a new array to store runtime and id in EVERYLINE
                            string[] TaskID_Runtime = Line[TaskLine].Trim().Split(',');
                            //store line index
                            int LineIndex = TaskLine - LineNo - 1;

                            double ID = double.Parse(TaskID_Runtime[0]);
                            double runtime = Convert.ToDouble(TaskID_Runtime[1]);
                            tasks_run[LineIndex, 0] = ID;
                            tasks_run[LineIndex, 1] = runtime;

                        }
                    }
                    else if (LineTest.Contains("PROCESSOR-ID"))
                    {
                        int ProLine;                                                  //use new int to note the lineNo
                        //go through following PROCESSOR lines
                        for (ProLine = LineNo + 1; !ContainChar(Line[ProLine]) && Line[ProLine].Trim().Replace(",", "").Length != 0; ProLine++)
                        {
                            int lineindex = ProLine - LineNo - 1;                     //line index for 2d array
                            string[] ProID_freque = Line[ProLine].Split(',');         //store everyline after processor-id line

                            double ID = double.Parse(ProID_freque[0]);
                            double frequency = double.Parse(ProID_freque[1]);
                            procoessor_frequency[lineindex, 0] = ID;
                            procoessor_frequency[lineindex, 1] = frequency;
                         
                        }
                    }
                    else if (LineTest.Contains("COEFFICIENT-ID"))
                    {
                        int coefLine;                                               //use new int to note the lineNo
                        int coefIndex;                                              //index of lines
                        //go through following COEFFICIENT lines
                        for (coefLine = LineNo + 1; !ContainChar(Line[coefLine]) && coefLine < LineNo + 4; coefLine++)       //go through every line of data                         
                        {
                            coefIndex = coefLine - 1 - LineNo;
                            string[] value = Line[coefLine].Split(',');

                            coef_value[coefIndex] = Convert.ToInt32(value[1]);      //store data into array
                        }

                    }
                }

            }

            //end data storation... clear the textbox
            textBox1.Clear();                                    

            //---assign value to classes members---//
            //assgin value to class Allocation           
            allocation.ProNo = program_processors;
            allocation.TaskNo = program_tasks;
            allocation.Table = new int[allocation.ProNo][];     //initialize the table of 0,1        
            allocation.program_max_duration = program_maxi_duration;
            for(int a=0;a<allocation.ProNo;a++)
                allocation.ProcessorFrequency[a] = procoessor_frequency[a, 1];
            

            //create class tasks and add into list
            //assign value to class Tasks
            for(int i=0;i<allocation.TaskNo;i++)
            {
                Task task=new Task();
                task.id = i + 1;
                task.coef0 = coef_value[0];
                task.coef1 = coef_value[1];
                task.coef2 = coef_value[2];
                task.ref_fre = runtime_prefer_fre;
                task.runtime = tasks_run[i, 1];
                
                task_list.Add(task);                                            //add into task list                              

            }

            allocation.task = task_list;                                        //assign list to allocation class member

            //create a real runtime table
            double[][] runtimetable=new double[allocation.ProNo][];
            for(int k=0;k<allocation.ProNo;k++)
            {
                runtimetable[k] = new double[100];
            }

            for(int i=0;i<program_processors;i++)
            {
                for(int j=0;j<program_tasks;j++)
                {
                    runtimetable[i][j] = runtime_prefer_fre * task_list[j].runtime / procoessor_frequency[i, 1];
                    
                }
            }

        
            Allocation[] allocationsArray;                                                          //create an array for allocations list
            try
            {
                // socket connection...

                Alg1Reference.ServiceClient ALG1 = new Alg1Reference.ServiceClient();                   //create wcf reference service client

                if (source.Contains('A'))
                {
                    allocationsArray = ALG1.getAllocation(source, allocation, runtimetable);

                    //output the allocation list
                    for (int num = 0; num < allocationsArray.Length; num++)
                    {
                        AddText("Allocation-ID: " + allocationsArray[num].ID + Environment.NewLine);
                        for (int i = 0; i < program_processors; i++)
                        {
                            for (int j = 0; j < program_tasks; j++)
                            {
                                AddText(allocationsArray[num].Table[i][j] + " ");
                            }
                            AddText(Environment.NewLine);
                        }

                        AddText("Energy: " + allocationsArray[num].Energy + ", Time:" + allocationsArray[num].Runtime);
                        AddText(Environment.NewLine);
                    }
                }
                
                Alg2Reference.ServiceClient ALG2 = new Alg2Reference.ServiceClient();                   //create wcf reference service client

                //second Algorithm
                if (source.Contains('B'))
                {
                    Allocation allocation2 = ALG2.getAllocation(source, allocation, runtimetable);

                    //output the allocation list
                    AddText("Allocation-ID: " + allocation2.ID + Environment.NewLine);
                    for (int i = 0; i < program_processors; i++)
                    {
                        for (int j = 0; j < program_tasks; j++)
                        {
                            AddText(allocation2.Table[i][j] + " ");
                        }
                        AddText(Environment.NewLine);
                    }

                    AddText("Energy: " + allocation2.Energy + ", Time:" + allocation2.Runtime);
                    AddText(Environment.NewLine);

                }


                if(source.Contains('C'))
                {
                    Alg3Reference.ServiceClient ALG3 = new Alg3Reference.ServiceClient();

                    Allocation allocation3 = ALG3.getAllocation(source, allocation, runtimetable);
                    //output the allocation list
                    AddText("Allocation-ID: " + allocation3.ID + Environment.NewLine);
                    for (int i = 0; i < program_processors; i++)
                    {
                        for (int j = 0; j < program_tasks; j++)
                        {
                            AddText(allocation3.Table[i][j] + " ");
                        }
                        AddText(Environment.NewLine);
                    }

                    AddText("Energy: " + allocation3.Energy + ", Time:" + allocation3.Runtime);
                    AddText(Environment.NewLine);
                }

            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
                {
                    // retry with a new instance socket.
                }
            }

             //write the log file
             string current_directory = Directory.GetCurrentDirectory().Replace("cloudappassign2\\bin\\Debug", "");
             StreamWriter sw = new StreamWriter(current_directory + "AT2-log.txt", true);
             sw.WriteLine(DateTime.Now.ToString() + textBox1.Text);
             sw.Close();            


        }


    }

}
