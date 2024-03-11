using System;
using System.Collections.Generic;

namespace HelloDocAdmin.Entity.Models;

public partial class Encounterform
{
    public int Encounterformid { get; set; }

    public int? Requestid { get; set; }

    public string? Historyofpresentillnessorinjury { get; set; }

    public string? Medicalhistory { get; set; }

    public string? Medications { get; set; }

    public string? Allergies { get; set; }

    public string? Temp { get; set; }

    public string? Hr { get; set; }

    public string? Rr { get; set; }

    public string? Bloodpressuresystolic { get; set; }

    public string? Bloodpressurediastolic { get; set; }

    public string? O2 { get; set; }

    public string? Pain { get; set; }

    public string? Heent { get; set; }

    public string? Cv { get; set; }

    public string? Chest { get; set; }

    public string? Abd { get; set; }

    public string? Extremities { get; set; }

    public string? Skin { get; set; }

    public string? Neuro { get; set; }

    public string? Other { get; set; }

    public string? Diagnosis { get; set; }

    public string? TreatmentPlan { get; set; }

    public string? Medicaldispensed { get; set; }

    public string? Procedures { get; set; }

    public string? Followup { get; set; }

    public int? Adminid { get; set; }

    public int? Physicianid { get; set; }

    public bool Isfinalize { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? Modified { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Physician? Physician { get; set; }

    public virtual Request? Request { get; set; }
   
}
