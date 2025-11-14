using DataBaseStructure;
using DataBaseStructure.AriaBase;
using DataWritingTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMiningCervix
{
    public class CervixPatient
    {
        public string MRN { get; set; }
        public string CourseName { get; set; }
        public string PlanName { get; set; }
        public string Type { get; set; }
        public string Review { get; set; }
    }
    public class DataMiningCervixClass
    {
        public static List<CervixPatient> FindCervixPatients(List<PatientClass> patients)
        {
            List<CervixPatient> outPatients = new List<CervixPatient>();
            foreach (PatientClass patient in patients)
            {
                foreach (CourseClass course in patient.Courses)
                {
                    foreach (DiagnosisCodeClass diagCode in course.DiagnosisCodes)
                    {
                        if (diagCode.DiagnosisCode.Contains("C53.0") || 
                            diagCode.DiagnosisCode.Contains("C53.1"))
                        {
                            foreach (TreatmentPlanClass planClass in course.TreatmentPlans)
                            {
                                if (planClass.Review.ApprovalStatus != "TreatmentApproved")
                                {
                                    continue;
                                }
                                CervixPatient outPatient = new CervixPatient()
                                {
                                    MRN = patient.MRN,
                                    Review = planClass.Review.ReviewTime.ToString(),
                                    Type = planClass.PlanType,
                                    CourseName = course.Name,
                                    PlanName = planClass.PlanName,
                                };
                                outPatients.Add(outPatient);
                            }
                        }
                    }
                }
            }
            return outPatients;
        }

        public static void MainRun()
        {
            string dataDirectory = @"\\ad.ucsd.edu\ahs\CANC\RADONC\BMAnderson\DataBases";
            List<string> jsonFiles = new List<string>();
            jsonFiles = AriaDataBaseJsonReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2025", jsonFiles, "*.json", SearchOption.AllDirectories);
            jsonFiles = AriaDataBaseJsonReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2024", jsonFiles, "*.json", SearchOption.AllDirectories);
            jsonFiles = AriaDataBaseJsonReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2023", jsonFiles, "*.json", SearchOption.AllDirectories);
            jsonFiles = AriaDataBaseJsonReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2022", jsonFiles, "*.json", SearchOption.AllDirectories);
            jsonFiles = AriaDataBaseJsonReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2021", jsonFiles, "*.json", SearchOption.AllDirectories);
            jsonFiles = AriaDataBaseJsonReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2020", jsonFiles, "*.json", SearchOption.AllDirectories);
            jsonFiles = AriaDataBaseJsonReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2019", jsonFiles, "*.json", SearchOption.AllDirectories);
            jsonFiles = AriaDataBaseJsonReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2018", jsonFiles, "*.json", SearchOption.AllDirectories);
            List<PatientClass> allPatients = new List<PatientClass>();
            allPatients = AriaDataBaseJsonReader.ReadPatientFiles(jsonFiles);
            var cervixPatients = FindCervixPatients(allPatients);
            string outputCsvPath = Path.Combine(dataDirectory, "CervixPatients.csv");
            CsvTools.WriteToCsv<CervixPatient>(cervixPatients, outputCsvPath);
        }
    }
}
