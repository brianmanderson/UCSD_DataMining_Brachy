using DataBaseStructure;
using DataBaseStructure.AriaBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMiningBrachy
{
    public class OutPatientHN
    {
        public string PatientID { get; set; }
        public int YearTreated { get; set; }
        public int MonthTreated { get; set; }
        public string TreatmentType { get; set; }
        public string PlanName { get; set; }
    }
    class HNFinder
    {

        static List<PatientClass> FindHNPatients(List<PatientClass> patients)
        {
            List<PatientClass> filteredPatients = new List<PatientClass>();
            foreach (PatientClass patient in patients)
            {
                bool has_mr = false;
                foreach (ExaminationClass exam in patient.Examinations)
                {
                    if (exam.EquipmentInfo.Modality == "MR")
                    {
                        has_mr = true;
                    }
                }
                //if (!has_mr)
                //{
                //    continue;
                //}
                List<string> diagnosticConditions = new List<string>() { "C00", "C01", "C02", "C03", "C04", "C05", "C06", "C07", "C08", "C09", "C10", "C11", "C13", "C12", "C14" };
                if (patient.Courses.Count == 0)
                {
                    continue;
                }
                foreach (CourseClass course in patient.Courses)
                {
                    foreach (DiagnosisCodeClass diagnosis in course.DiagnosisCodes)
                    {
                        if (diagnosticConditions.Contains(diagnosis.DiagnosisCode))
                        {
                            Console.WriteLine($"{patient.MRN},{course.Name}");
                        }
                    }
                    foreach (TreatmentPlanClass treatment in course.TreatmentPlans)
                    {
                        if (treatment.Review.ApprovalStatus != "TreatmentApproved")
                        {
                            continue;
                        }
                        if (treatment.FractionNumber < 3)
                        {
                            continue;
                        }
                        bool addPatient = false;
                        if (treatment.PlanType == "ExternalBeam")
                        {
                            if (treatment.PlanName.ToLower().Contains("hip"))
                            {
                                Console.WriteLine($"{patient.MRN},{course.Name},{treatment.PlanName}");
                                addPatient = true;
                            }
                            continue;
                            foreach (BeamSetClass beamSet in treatment.BeamSets)
                            {
                                FractionDoseClass fractionDose = beamSet.FractionDose;
                                if (fractionDose is null)
                                {
                                    continue;
                                }
                                foreach (RegionOfInterestDose doseROI in fractionDose.DoseROIs)
                                {
                                    if (doseROI.Name.ToLower().Contains("paro"))
                                    {
                                        if (doseROI.AbsoluteDose[0] > 800)
                                        {
                                            addPatient = true;
                                            Console.WriteLine($"{patient.MRN},{course.Name},{treatment.PlanName}");
                                            break;
                                        }
                                    }

                                }
                            }
                        }
                        if (treatment.PlanType.ToLower().Contains("brachy"))
                        {
                            continue;
                            if (treatment.PlanName.ToLower().Contains("cylinder") ||
                                treatment.PlanName.ToLower().Contains("skin") ||
                                treatment.PlanName.ToLower().Contains("scalp") ||
                                treatment.PlanName.ToLower().Contains("nose"))
                            {
                                addPatient = true;
                            }
                            foreach (DiagnosisCodeClass diagnosis in course.DiagnosisCodes)
                            {
                                if (diagnosticConditions.Contains(diagnosis.DiagnosisCode))
                                {
                                    addPatient = true;
                                }
                            }
                        }
                        if (addPatient)
                        {
                            filteredPatients.Add(patient);
                        }
                    }
                }
            }
            return filteredPatients;
        }
        static void MainHN(string[] args)
        {
            List<string> jsonFiles = new List<string>();
            jsonFiles = AriaDataBaseJsonReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2025", jsonFiles, "*.json", SearchOption.AllDirectories);
            jsonFiles = AriaDataBaseJsonReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2024", jsonFiles, "*.json", SearchOption.AllDirectories);
            //jsonFiles = AriaDataBaseReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2023", jsonFiles, "*.json", SearchOption.AllDirectories);
            //jsonFiles = AriaDataBaseReader.ReturnPatientFileNames(@"C:\Users\BRA008\Modular_Projects\LocalDatabases\2023", jsonFiles, "*.json", SearchOption.AllDirectories);
            List<PatientClass> allPatients = new List<PatientClass>();
            allPatients = AriaDataBaseJsonReader.ReadPatientFiles(jsonFiles);
            List<PatientClass> brachyPatients = FindHNPatients(allPatients);
        }
    }
}
