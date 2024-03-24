using Microsoft.IdentityModel.Tokens;

namespace MinimalAPIPeliculas.Utilities
{
    public static class Llaves
    {
        public const string IssuerPropio = "my-web-api";
        private const string SeccionLlaves = "Authentication:Schemes:Bearer:SigningKeys";
        private const string SeccionLlavesEmisor = "Issuer";
        private const string SeccionLlavesValor = "Value";

        public static IEnumerable<SecurityKey> ObtenerLlave(IConfiguration configuration) => ObtenerLlave(configuration, IssuerPropio);

        public static IEnumerable<SecurityKey> ObtenerLlave(IConfiguration configuration, string issuer)
        {
            var signInKey = configuration.GetSection(SeccionLlaves)
                .GetChildren()
                .SingleOrDefault(llave => llave[SeccionLlavesEmisor] == issuer);
            if(signInKey is not null && signInKey[SeccionLlavesValor] is string valorLlave)
            {
                yield return new SymmetricSecurityKey(Convert.FromBase64String(valorLlave));
            }
        }

        public static IEnumerable<SecurityKey> ObtenerTodasLasLlave(IConfiguration configuration)
        {
            var signInKeys = configuration.GetSection(SeccionLlaves)
                .GetChildren();

            foreach(var key in signInKeys)
            {
                if (key[SeccionLlavesValor] is string valorLlave)
                {
                    yield return new SymmetricSecurityKey(Convert.FromBase64String(valorLlave));
                }
            }


        }
    }
}
