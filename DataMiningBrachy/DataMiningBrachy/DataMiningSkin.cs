using DataBaseStructure;
using DataBaseStructure.AriaBase;
using DataWritingTools;
using System.Collections.Generic;
using System.IO;

namespace DataMiningBrachy
{
    public class OutPatientSkin
    {
        public string PatientID { get; set; }
        public int Year { get; set; }
        public string DateTreated { get; set; }
        public string TreatmentType { get; set; }
        public string CourseName { get; set; }
        public string PlanName { get; set; }
        public string Type { get; set; }
        public double Depth { get; set; }
        public string Notes { get; set; }
    }
    class DataMiningSkin
    {
        static List<OutPatientSkin> FindBrachySkinPatients(List<PatientClass> patients)
        {
            List<OutPatientSkin> outPatients = new List<OutPatientSkin>();
            foreach (PatientClass patient in patients)
            {
                bool is_skin = false;
                bool added = false;
                foreach (CourseClass course in patient.Courses)
                {
                    if (is_skin & added)
                    {
                        break;
                    }
                    if (course.Name.ToLower().Contains("gyn") || course.Name.ToLower().Contains("pelv"))
                    {
                        continue;
                    }
                    foreach (TreatmentPlanClass planClass in course.TreatmentPlans)
                    {
                        if (planClass.PlanName.ToLower().Contains("not"))
                        {
                            continue;
                        }
                        if (planClass.PlanType == "Brachy")
                        {
                            foreach (ApplicatorSetClass applicatorSet in planClass.ApplicatorSets)
                            {
                                PrescriptionClass prescription = applicatorSet.Prescription;
                                if (prescription == null)
                                {
                                    continue;
                                }
                                foreach (var target in prescription.PrescriptionTargets)
                                {
                                    if (target.DosePerFraction == 600 && target.NumberOfFractions == 5)
                                    {
                                        continue;
                                    }
                                    if (target.Type == "Depth")
                                    {
                                        is_skin = true;
                                        OutPatientSkin outPatient = new OutPatientSkin()
                                        {
                                            PatientID = patient.MRN,
                                            Year = planClass.Review.ReviewTime.Year,
                                            DateTreated = $"{planClass.Review.ReviewTime.Month:D2}" +
                                                            $"/{planClass.Review.ReviewTime.Day:D2}" +
                                                            $"/{planClass.Review.ReviewTime.Year}",
                                            TreatmentType = planClass.PlanType,
                                            CourseName = course.Name,
                                            PlanName = planClass.PlanName,
                                            Type = target.Type,
                                            Depth = target.Value,
                                            Notes = prescription.Notes.Replace("\n", " ").Replace("\r", " ")
                                        };
                                        added = true;
                                        outPatients.Add(outPatient);
                                        break;
                                    }
                                }
                                if (added)
                                {
                                    break;
                                }
                                if (added)
                                {
                                    break;
                                }

                            }
                        }
                        if (is_skin)
                        {
                            break;
                        }
                    }
                }
            }
            return outPatients;
        }
        public void Main(string[] args)
        {
            string dataDirectory = @"\\ad.ucsd.edu\ahs\CANC\RADONC\BMAnderson\DataBases";
            List<string> jsonFiles = new List<string>();
            jsonFiles = AriaDataBaseJsonReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2025", jsonFiles, "*.json", SearchOption.AllDirectories);
            jsonFiles = AriaDataBaseJsonReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2024", jsonFiles, "*.json", SearchOption.AllDirectories);
            jsonFiles = AriaDataBaseJsonReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2023", jsonFiles, "*.json", SearchOption.AllDirectories);
            //jsonFiles = AriaDataBaseJsonReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2022", jsonFiles, "*.json", SearchOption.AllDirectories);
            List<PatientClass> allPatients = new List<PatientClass>();
            allPatients = AriaDataBaseJsonReader.ReadPatientFiles(jsonFiles);
            var skinPatients = FindBrachySkinPatients(allPatients);
            string outputCsvPath = Path.Combine(dataDirectory, "SkinPatients.csv");
            CsvTools.WriteToCsv<OutPatientSkin>(skinPatients, outputCsvPath);
        }
    }
}