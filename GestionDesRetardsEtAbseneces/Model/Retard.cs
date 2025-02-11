using System;
using System.Collections.Generic;

namespace GestionDesRetardsEtAbseneces.Model;

public partial class Retard
{
    public int IdRetard { get; set; }

    public int IdEmploye { get; set; }

    public DateTime? DateRetard { get; set; }

    public TimeSpan? HeureDebut { get; set; }

    public TimeSpan? HeureFin { get; set; }

    public string? Justification { get; set; }

    public bool? Valide { get; set; }

    public virtual Employe IdEmployeNavigation { get; set; } = null!;
}
