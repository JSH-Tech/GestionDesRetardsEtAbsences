using System;
using System.Collections.Generic;

namespace GestionDesRetardsEtAbseneces.Model;

public partial class Rapportassiduite
{
    public int IdRapport { get; set; }

    public int IdEmploye { get; set; }

    public string? PeriodeRapport { get; set; }

    public DateTime? DateGeneration { get; set; }

    public string? ContenuRapport { get; set; }

    public virtual Employe IdEmployeNavigation { get; set; } = null!;
}
