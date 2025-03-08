using System;
using System.Collections.Generic;

namespace GestionDesRetardsEtAbseneces.Model;

public partial class Absence
{
    public int IdAbsence { get; set; }

    public int IdEmploye { get; set; }

    public DateTime? DateAbsence { get; set; }

    public string? TypeAbsence { get; set; }

    public string? Justification { get; set; }

    public bool Valide { get; set; }

    public virtual Employe IdEmployeNavigation { get; set; } = null!;

    public string StatutText => Valide ? "Valide" : "Non Valide";

}
