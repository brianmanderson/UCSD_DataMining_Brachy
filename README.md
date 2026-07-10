# UCSD_DataMining

Internal C# console scripts for finding patient cohorts in UCSD Radiation Medicine data —
primarily brachytherapy cases. Each "finder" walks a local JSON snapshot of the ARIA
oncology information system (or queries ARIA directly) and writes the matching patients
to a CSV for review. Working scratch repo, not a packaged tool: queries are enabled or
commented out in `Program.Main` per question being asked.

## Finders (`DataMiningBrachy/DataMiningBrachy/`)

- `DataMiningCylinder.cs` — brachy cylinder cases, matched by depth-type prescriptions
  of 600 cGy x 5 or 700 cGy x 3
- `DataMiningCervix.cs` — cervix cases via ICD-10 diagnosis codes C53.0/C53.1 on
  treatment-approved plans
- `DataMiningSkin.cs` — brachy skin treatments (also duplicated in the older
  `UCSD_FindingBrachySkin` project)
- `DataMiningReTreat.cs`, `DataMiningImaging.cs`, `HNFinder.cs` — re-treatment,
  imaging, and head-and-neck queries
- `Program.cs` also runs a direct ARIA SQL activity query (`QueryAria`) to CSV

## How it works

Patient data comes from year-partitioned JSON databases exported from ARIA (network
share `\\ad.ucsd.edu\...\DataBases`, synced to a local folder). Finders deserialize
those into `PatientClass` objects and filter on courses, plans, prescriptions, and
diagnosis codes.

## Requirements

- .NET 8; NuGet: Microsoft.Data.SqlClient, System.Data.Odbc, Newtonsoft.Json
- Sibling repos checked out next to this one (referenced by relative path in
  `DataMining.csproj`): `Make_Raystation_Data_StructureCSharp` (DataBaseStructure,
  DataWritingTools, DataBaseFileManager), `SendEmailCSharp`, `UCSD_DataMiningHighMod`,
  and a local `FindingHAPatients` project
- Access to the ARIA JSON database snapshots; hard-coded UCSD paths in the finders
