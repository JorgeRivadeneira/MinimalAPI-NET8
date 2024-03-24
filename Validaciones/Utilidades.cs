namespace MinimalAPIPeliculas.Validaciones
{
    public static class Utilidades
    {
        public static string CampoRequeridoMensaje = "El campo {PropertyName} es requerido";
        public static string MaximaLongitudPermitida = "El campo {PropertyName} debe tener menos de {MaxLength} caracteres";
        public static string PrimeraLetraMayusculaMensaje = "El campo {PropertyName} debe comenzar con mayúsculas";
        public static string EmailMensaje = "El campo {PropertyName} debe ser un Email válido";

        public static bool ValidarPrimeraLetraMayuscula(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return true;
            }

            var primeraLetra = valor[0].ToString();
            return primeraLetra == primeraLetra.ToUpper();
        }

        public static string GreatherThanOrEqualToMensaje(DateTime fechaMinima) => "El campo {PropertyName} debe ser posterior a " + fechaMinima.ToString("yyyy-MM-dd");
    }
}
