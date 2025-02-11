using System;
using System.Collections.Generic;

namespace GestionDesRetardsEtAbseneces.Model;

public partial class Employe
{
    public int IdEmploye { get; set; }

    public string? Nom { get; set; }

    public string? Prenom { get; set; }

    public string? Email { get; set; }

    public string? Poste { get; set; }

    public string? Departement { get; set; }

    public string? Statut { get; set; }

    public string? RoleEmploye { get; set; }

    public virtual ICollection<Absence> Absences { get; set; } = new List<Absence>();

    public virtual ICollection<Demandeconge> Demandeconges { get; set; } = new List<Demandeconge>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Rapportassiduite> Rapportassiduites { get; set; } = new List<Rapportassiduite>();

    public virtual ICollection<Retard> Retards { get; set; } = new List<Retard>();
}
