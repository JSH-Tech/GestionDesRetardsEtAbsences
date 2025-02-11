using System;
using System.Collections.Generic;

namespace GestionDesRetardsEtAbseneces.Model;

public partial class Demandeconge
{
    public int IdDemande { get; set; }

    public int IdEmploye { get; set; }

    public string? TypeConge { get; set; }

    public DateTime? DateDebut { get; set; }

    public DateTime? DateFin { get; set; }

    public string? Justification { get; set; }

    public string? Statut { get; set; }

    public virtual Employe IdEmployeNavigation { get; set; } = null!;
}
