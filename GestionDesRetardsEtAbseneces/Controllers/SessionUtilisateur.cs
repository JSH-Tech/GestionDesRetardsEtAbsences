namespace GestionDesRetardsEtAbseneces.Model
{
    public static class SessionUtilisateur
    {


        //propriete
        public static int IdEmploye { get; set; }
        public static string? Nom { get; set; }
        public static string? Prenom { get; set; }
        public static string? Role { get; set; }
        public static string? Email { get; set; }
        public static DateTime HeureConnexion { get; set; }
    }
}
