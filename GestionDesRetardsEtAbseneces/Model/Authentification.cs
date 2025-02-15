using System;
using System.Collections.Generic;

namespace GestionDesRetardsEtAbseneces.Model;

public partial class Authentification
{
    /// <summary>
    /// Primary Key
    /// </summary>
    public int IdAuthentification { get; set; }

    /// <summary>
    /// Foreign Key
    /// </summary>
    public int IdEmploye { get; set; }

    /// <summary>
    /// Hashed Password
    /// </summary>
    public string MotDePasseHash { get; set; } = null!;

    /// <summary>
    /// Last Connection Date
    /// </summary>
    public DateTime DateDerniereConnexion { get; set; }

    /// <summary>
    /// Session Token
    /// </summary>
    public string TokenSession { get; set; } = null!;

    /// <summary>
    /// Expiration Date
    /// </summary>
    public DateTime DateExpiration { get; set; }

    /// <summary>
    /// Access Level
    /// </summary>
    public int NiveauAcces { get; set; }

    public virtual Employe IdEmployeNavigation { get; set; } = null!;
}
