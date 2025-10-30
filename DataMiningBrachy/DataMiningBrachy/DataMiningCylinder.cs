using DataBaseStructure;
using DataBaseStructure.AriaBase;
using DataWritingTools;
using DataBaseFileManager;

namespace DataMiningBrachy
{
    public class OutPatient
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
        public double DosePerFraction { get; set; }
        public int NumberOfFractions { get; set; }
    }
    class DataMiningCylinder
    {
        static List<OutPatient> FindBrachyCylinderpatients(List<PatientClass> patients)
        {
            List<OutPatient> outPatients = new List<OutPatient>();
            foreach(PatientClass patient in patients)
            {
                bool is_cylinder = false;
                bool added = false;
                foreach (CourseClass course in patient.Courses)
                {
                    if (is_cylinder & added)
                    {
                        break;
                    }
                    foreach (TreatmentPlanClass planClass in course.TreatmentPlans)
                    {

                        if (planClass.PlanType == "Brachy")
                        {
                            foreach(ApplicatorSetClass applicatorSet in planClass.ApplicatorSets)
                            {
                                PrescriptionClass prescription = applicatorSet.Prescription;
                                if (prescription == null)
                                {
                                    continue;
                                }
                                foreach (var target in prescription.PrescriptionTargets)
                                {
                                    if ((target.DosePerFraction != 600 || target.NumberOfFractions != 5) && (target.DosePerFraction != 700 || target.NumberOfFractions != 3))
                                    {
                                        continue;
                                    }
                                    if (target.Type == "Depth")
                                    {
                                        is_cylinder = true;
                                        OutPatient outPatient = new OutPatient()
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
                                            Notes = prescription.Notes.Replace("\n", " ").Replace("\r", " "),
                                            DosePerFraction = target.DosePerFraction,
                                            NumberOfFractions = target.NumberOfFractions
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
                                else if (applicatorSet.Applicators.Count == 1)
                                {
                                    ApplicatorClass applicator = applicatorSet.Applicators.FirstOrDefault();
                                    if (applicator.Id.ToLower().Contains("cyli"))
                                    {
                                        is_cylinder = true;
                                        OutPatient outPatient = new OutPatient()
                                        {
                                            PatientID = patient.MRN,
                                            Year = planClass.Review.ReviewTime.Year,
                                            DateTreated = $"{planClass.Review.ReviewTime.Month:D2}" +
                                                            $"/{planClass.Review.ReviewTime.Day:D2}" +
                                                            $"/{planClass.Review.ReviewTime.Year}",
                                            TreatmentType = planClass.PlanType,
                                            CourseName = course.Name,
                                            PlanName = planClass.PlanName,
                                            Type = "",
                                            Depth = 0,
                                            Notes = prescription.Notes.Replace("\n", " ").Replace("\r", " "),
                                            DosePerFraction = applicatorSet.Prescription.PrescriptionTargets[0].DosePerFraction,
                                            NumberOfFractions = applicatorSet.Prescription.PrescriptionTargets[0].NumberOfFractions
                                        };
                                        added = true;
                                        outPatients.Add(outPatient);
                                    }
                                }
                                if (added)
                                {
                                    break;
                                }

                            }
                        }
                        if (is_cylinder)
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
            //FileSync.SyncFilesParallel(@"\\ad.ucsd.edu\ahs\CANC\RADONC\BMAnderson\DataBases", @"C:\Users\BRA008\Modular_Projects\LocalDatabases", maxDegreeOfParallelism: 8);
            //MainRun.Update();
            //return;
            string dataDirectory = @"\\ad.ucsd.edu\ahs\CANC\RADONC\BMAnderson\DataBases";
            List<string> jsonFiles = new List<string>();
            jsonFiles = AriaDataBaseJsonReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2025", jsonFiles, "*.json", SearchOption.AllDirectories);
            jsonFiles = AriaDataBaseJsonReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2024", jsonFiles, "*.json", SearchOption.AllDirectories);
            jsonFiles = AriaDataBaseJsonReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2023", jsonFiles, "*.json", SearchOption.AllDirectories);
            jsonFiles = AriaDataBaseJsonReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2022", jsonFiles, "*.json", SearchOption.AllDirectories);
            List<PatientClass> allPatients = new List<PatientClass>();
            allPatients = AriaDataBaseJsonReader.ReadPatientFiles(jsonFiles);
            var cylinderPatients = FindBrachyCylinderpatients(allPatients);
            string outputCsvPath = Path.Combine(dataDirectory, "CylinderPatients.csv");
            CsvTools.WriteToCsv<OutPatient>(cylinderPatients, outputCsvPath);
        }
    }
}