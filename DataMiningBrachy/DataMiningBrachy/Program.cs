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
    }
    class Program
    {
        static List<OutPatient> FindBrachypatients(List<PatientClass> patients)
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
                                    if (target.DosePerFraction != 600 || target.NumberOfFractions != 5)
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
                                            Notes = prescription.Notes.Replace("\n", " ").Replace("\r", " ")
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
        static List<PatientClass> FindPatients(List<PatientClass> patients)
        {
            List<PatientClass> filteredPatients = new List<PatientClass>();
            foreach (PatientClass patient in patients)
            {
                if (patient.MRN == "32570512")
                {
                    int debugVar = 1;
                }
                if (patient.Courses.Count == 0)
                {
                    continue;
                }
                foreach(CourseClass course in patient.Courses)
                {
                    foreach (TreatmentPlanClass treatment in course.TreatmentPlans)
                    {
                        if (treatment.Review.ApprovalStatus != "TreatmentApproved")
                        {
                            continue;
                        }
                        bool addPatient = false;
                        if (treatment.PlanType.ToLower().Contains("brachy"))
                        {
                            if (treatment.PlanName.ToLower().Contains("cylinder") ||
                                treatment.PlanName.ToLower().Contains("skin") ||
                                treatment.PlanName.ToLower().Contains("scalp") ||
                                treatment.PlanName.ToLower().Contains("nose") ||
                                treatment.PlanName.ToLower().Contains("nasal"))
                            {
                                addPatient = true;
                            }
                        }
                        if (addPatient)
                        {
                            filteredPatients.Add(patient);
                            Console.WriteLine($"{patient.MRN}^{course.Name}^{treatment.PlanName}^{treatment.Review.ReviewTime}");
                        }
                    }
                }
            }
            return filteredPatients;
        }
        static void Main(string[] args)
        {
            //FileSync.SyncFilesParallel(@"\\ad.ucsd.edu\ahs\CANC\RADONC\BMAnderson\DataBases", @"C:\Users\BRA008\Modular_Projects\LocalDatabases", maxDegreeOfParallelism: 8);
            MainRun.Update();
            return;
            string dataDirectory = @"\\ad.ucsd.edu\ahs\CANC\RADONC\BMAnderson\DataBases";
            List<string> jsonFiles = new List<string>();
            jsonFiles = AriaDataBaseJsonReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2025", jsonFiles, "*.json", SearchOption.AllDirectories);
            jsonFiles = AriaDataBaseJsonReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2024", jsonFiles, "*.json", SearchOption.AllDirectories);
            jsonFiles = AriaDataBaseJsonReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2023", jsonFiles, "*.json", SearchOption.AllDirectories);
            jsonFiles = AriaDataBaseJsonReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2022", jsonFiles, "*.json", SearchOption.AllDirectories);
            List<PatientClass> allPatients = new List<PatientClass>();
            allPatients = AriaDataBaseJsonReader.ReadPatientFiles(jsonFiles);
            var cylinderPatients = FindBrachypatients(allPatients);
            string outputCsvPath = Path.Combine(dataDirectory, "BrachyPatients_4cmPlusDepth_2025.csv");
            CsvTools.WriteToCsv<OutPatient>(cylinderPatients, outputCsvPath);
        }
    }
}