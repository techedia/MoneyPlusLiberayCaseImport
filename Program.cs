using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;

namespace MoneyPlusLiberayCaseImport
{
    class Program
    {

        static Log objLogging = new Log();

        static void Main(string[] args)
        {

            //
            // Process a main CSV file and get the first x rows and create a new case import for the first x rows to a specific folder and likewise for the y rows so we have 2x CSV case import files
            // in 2 different import folders
            //
            // x rows if from an app config setting
            //
            // Update (DELETE\CREATE) a new main CSV file with these 20 first rows removed ready for processing again
            //
            // Do not do the above if;
            //
            //      Outside of operational hours - we want to be doing case imports when the Proclaim task servers and DB are running
            //      Case import CSV files exist - they have been processed so the next batch of cases is ok to setup for import otherwise not
            //


            try
            {
                objLogging.LogToFile("", false);
                objLogging.LogToFile("", false);
                objLogging.LogToFile("* * * * * NEW RUN STARTED * * * * * * * * * * * * * * * * * * * * * * * * * * *", false);

                string strCSVFileName = ConfigurationManager.AppSettings["SourceCSVFile_Folder"].ToString() + ConfigurationManager.AppSettings["SourceCSVFile_FileName"].ToString();

                if (File.Exists(strCSVFileName))
                {

                    //Check we are allowed to run \ in operational hours
                    bool boolInOperationalHours = true;

                    DateTime time = DateTime.Now;
                    String currentTime = DateTime.Now.ToString("HH:mm"); // time.ToString("HH:mm ");

                    DateTime firstTime = DateTime.ParseExact(ConfigurationManager.AppSettings["ProcessingHoursStart"].ToString(), "HH:mm", null);
                    String firstTime1 = firstTime.ToString("HH:mm ");
                    DateTime lastTime = DateTime.ParseExact(ConfigurationManager.AppSettings["ProcessingHoursEnd"].ToString(), "HH:mm", null);
                    String lastTime1 = lastTime.ToString("HH:mm ");

                    objLogging.LogToFile("Operational hours : firstTime1 : " + firstTime1.ToString(), false);
                    objLogging.LogToFile("Operational hours: lastTime1 : " + lastTime1.ToString(), false);


                    int intNumberOfCasesForEachImport = Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfCasesForEachImport"]);
                    int intNumberOfCasesForEachImport2 = intNumberOfCasesForEachImport * 2;

                    objLogging.LogToFile("intNumberOfCasesForEachImport : " + intNumberOfCasesForEachImport.ToString(), false);
                    objLogging.LogToFile("intNumberOfCasesForEachImport2 : " + intNumberOfCasesForEachImport2.ToString(), false);

                    if (DateTime.Parse(currentTime) >= DateTime.Parse(firstTime1) && (DateTime.Parse(currentTime) <= DateTime.Parse(lastTime1)))
                    {
                        boolInOperationalHours = true;
                    }
                    else
                    {
                        boolInOperationalHours = false;
                    }

                    if (boolInOperationalHours)
                    {

                        objLogging.LogToFile("In operational hours", false);

                        //Check if there are any import files not processed ie Proclaim is still importing cases
                        bool boolNoImportFilesWaiting = true;

                        if (System.IO.Directory.GetFiles(ConfigurationManager.AppSettings["NewCaseImportFolderPath_1"].ToString()).Length == 0)
                        {
                            objLogging.LogToFile("Check 1st CSV import files exists : no file found", false);
                        }
                        else
                        {
                            boolNoImportFilesWaiting = false;
                            objLogging.LogToFile("Check 1st CSV import files exists : ** FILE found", false);
                        }

                        if (System.IO.Directory.GetFiles(ConfigurationManager.AppSettings["NewCaseImportFolderPath_2"].ToString()).Length == 0)
                        {
                            objLogging.LogToFile("Check 2nd CSV import files exists : no file found", false);
                        }
                        else
                        {
                            boolNoImportFilesWaiting = false;
                            objLogging.LogToFile("Check 2nd CSV import files exists : ** FILE found", false);
                        }

                        if (boolNoImportFilesWaiting)
                        {
                            DateTime dt = DateTime.Now;
                            string strDateTIme = dt.ToString("ddMMyyHHmmss");

                            string strCSVFileNameBackUp = ConfigurationManager.AppSettings["SourceCSVFile_BackupFolder"].ToString() + "BackUpCopy_" + strDateTIme + ".csv";

                            //Make a backup copy of the file to a backup folder (just in case!)
                            File.Copy(strCSVFileName, strCSVFileNameBackUp);

                            objLogging.LogToFile("Made a backup copy : " + strCSVFileName + " : " + strCSVFileNameBackUp, false);


                            //Read in CSV data and build up 3 strings, 2 of x rows each for the 2x import CSV files and one for the new main CSV with the first 20 rows removed
                            int intRowCount = 0;

                            var lines_newfile = "";
                            var lines_newcaseimport_1 = "";
                            var lines_newcaseimport_2 = "";

                            using (StreamReader sr = new StreamReader(strCSVFileName))
                            {
                                while (sr.Peek() >= 0)
                                {
                                    intRowCount++;

                                    var line = sr.ReadLine();

                                    if (intRowCount <= intNumberOfCasesForEachImport)
                                    {
                                        //First CSV import file
                                        lines_newcaseimport_1 = lines_newcaseimport_1 + line + Environment.NewLine;
                                    }
                                    else if (intRowCount <= intNumberOfCasesForEachImport2)
                                    {
                                        //Second CSV import file
                                        lines_newcaseimport_2 = lines_newcaseimport_2 + line + Environment.NewLine;
                                    }
                                    else
                                    {
                                        //New main CSV data file
                                        lines_newfile = lines_newfile + line + Environment.NewLine;
                                    }

                                    if (intRowCount % 1000 == 0)
                                    {
                                        objLogging.LogToFile("processing row : " + intRowCount.ToString(), false);
                                    }
                                }
                            }

                            objLogging.LogToFile("lines_newcaseimport_1 : " + lines_newcaseimport_1, false);
                            objLogging.LogToFile("lines_newcaseimport_2 : " + lines_newcaseimport_2, false);


                            //Create case import file 1
                            string strFile_newcaseimport_1 = ConfigurationManager.AppSettings["NewCaseImportFolderPath_1"].ToString();
                            strFile_newcaseimport_1 = strFile_newcaseimport_1 + "newcases_" + strDateTIme + ".csv";
                            File.WriteAllText(strFile_newcaseimport_1, lines_newcaseimport_1);

                            objLogging.LogToFile("CSV file created : strFile_newcaseimport_1 : " + strFile_newcaseimport_1, false);


                            //Create case import file 2
                            string strFile_newcaseimport_2 = ConfigurationManager.AppSettings["NewCaseImportFolderPath_2"].ToString();
                            strFile_newcaseimport_2 = strFile_newcaseimport_2 + "newcases_" + strDateTIme + ".csv";
                            File.WriteAllText(strFile_newcaseimport_2, lines_newcaseimport_2);

                            objLogging.LogToFile("CSV file created : strFile_newcaseimport_2 : " + strFile_newcaseimport_2, false);


                            //Create new ALL case import with first y rows removed (delete and create\write)
                            File.Delete(strCSVFileName);
                            File.WriteAllText(strCSVFileName, lines_newfile);

                            objLogging.LogToFile("New main CSV file deleted and created : strCSVFileName : " + strCSVFileName, false);
                        }
                    }
                    else
                    {
                        objLogging.LogToFile("** Outside of  operational hours", false);
                    }
                }

            }
            catch (Exception ex)
            {
                objLogging.LogToFile("EXCEPTION : " + ex.Message, true);
            }
        }
    }
}
