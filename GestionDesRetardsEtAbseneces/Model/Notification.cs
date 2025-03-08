using System;
using System.Collections.Generic;

namespace GestionDesRetardsEtAbseneces.Model;

public partial class Notification
{
    public int IdNotification { get; set; }

    public int IdEmploye { get; set; }

    public string? TypeNotification { get; set; }

    public string? MessageNotification { get; set; }

    public DateTime? DateEnvoi { get; set; }

    public bool Statut { get; set; }

    public virtual Employe IdEmployeNavigation { get; set; } = null!;

    public string StatutText => Statut ? "Lue" : "Non Lue";

}
