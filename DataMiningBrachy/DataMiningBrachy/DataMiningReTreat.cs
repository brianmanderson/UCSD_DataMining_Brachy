using DataBaseFileManager;
using DataBaseStructure;
using DataBaseStructure.AriaBase;
using DataWritingTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace DataMining
{
    public class ReTreatPatient
    {
        public string MRN { get; set; }
        public string CourseName { get; set; }
        public string PlanName { get; set; }
        public string Type { get; set; }
        public string Review { get; set; }
    }
    public class TreatmentPlan
    {
        public string Type { get; set; }
        public string CourseName { get; set; }
        public string PlanName { get; set; }
        public DateTimeClass Review { get; set; }
    }
    public class DataMiningReTreat
    {
        public static List<TreatmentPlan> FilterTreatmentPlansByDays(List<TreatmentPlan> treatmentPlans, int days=90)
        {
            if (treatmentPlans == null || treatmentPlans.Count == 0)
                return new List<TreatmentPlan>();

            var plansToKeep = new List<TreatmentPlan>();

            foreach (var plan in treatmentPlans)
            {
                DateTime currentDate = ConvertToDateTime(plan.Review);

                // Check if this plan is >90 days from at least one other plan
                bool hasDistantPlan = treatmentPlans.Any(otherPlan =>
                {
                    if (otherPlan == plan) return false; // Skip comparing to itself

                    DateTime otherDate = ConvertToDateTime(otherPlan.Review);
                    double daysDifference = Math.Abs((currentDate - otherDate).TotalDays);

                    return daysDifference > days;
                });

                if (hasDistantPlan)
                {
                    plansToKeep.Add(plan);
                }
            }

            return plansToKeep;
        }

        // Helper method to convert DateTimeClass to DateTime
        private static DateTime ConvertToDateTime(DateTimeClass dtc)
        {
            if (dtc == null)
                return DateTime.MinValue;

            return new DateTime(dtc.Year, dtc.Month, dtc.Day, dtc.Hour, dtc.Minute, dtc.Second);
        }
        public static void CheckFractionDose(List<TreatmentPlan> treatmentPlans, FractionDoseClass fractionDose, TreatmentPlanClass planClass, string CourseName)
        {
            if (fractionDose == null)
            {
                return;
            }
            foreach (RegionOfInterestDose roiDose in fractionDose.DoseROIs)
            {
                if (roiDose.Name.ToLower().Contains("bladder") || roiDose.Name.ToLower().Contains("rectum"))
                {
                    if (roiDose.AbsoluteDose[0] > 250)
                    {
                        treatmentPlans.Add(new TreatmentPlan()
                        {
                            PlanName = planClass.PlanName,
                            CourseName = CourseName,
                            Review = planClass.Review.ReviewTime,
                            Type = planClass.PlanType
                        });
                        break;
                    }
                }
            }
        }
        public static List<ReTreatPatient> FindRetreatBrachypatients(List<PatientClass> patients)
        {
            List<ReTreatPatient> outPatients = new List<ReTreatPatient>();
            foreach (PatientClass patient in patients)
            {
                List<TreatmentPlan> treatmentPlans = new List<TreatmentPlan>();
                foreach (CourseClass course in patient.Courses)
                {
                    foreach (TreatmentPlanClass planClass in course.TreatmentPlans)
                    {
                        if (planClass.Review.ApprovalStatus != "TreatmentApproved")
                        {
                            continue;
                        }
                        if (planClass.ApplicatorSets != null)
                        {
                            foreach (ApplicatorSetClass applicatorSet in planClass.ApplicatorSets)
                            {
                                FractionDoseClass fractionDose = applicatorSet.FractionDose;
                                CheckFractionDose(treatmentPlans, fractionDose, planClass, course.Name);
                            }
                        }
                        if (planClass.BeamSets != null)
                        {
                            foreach (BeamSetClass beamSet in planClass.BeamSets)
                            {
                                FractionDoseClass fractionDose = beamSet.FractionDose;
                                CheckFractionDose(treatmentPlans, fractionDose, planClass, course.Name);
                            }
                        }
                    }
                }
                treatmentPlans = FilterTreatmentPlansByDays(treatmentPlans, 90);
                if (treatmentPlans.Count >= 2 && treatmentPlans.Any(tp => tp.Type == "Brachy"))
                {
                    foreach(TreatmentPlan treatmentPlan in treatmentPlans)
                    {
                        ReTreatPatient outPatient = new ReTreatPatient()
                        {
                            MRN = patient.MRN,
                            Review = treatmentPlan.Review.ToString(),
                            Type = treatmentPlan.Type,
                            CourseName = treatmentPlan.CourseName,
                            PlanName = treatmentPlan.PlanName,
                        };
                        outPatients.Add(outPatient);
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
            var reTreatPatients = FindRetreatBrachypatients(allPatients);
            string outputCsvPath = Path.Combine(dataDirectory, "ReTreatPatients.csv");
            CsvTools.WriteToCsv<ReTreatPatient>(reTreatPatients, outputCsvPath);
        }
    }
}
