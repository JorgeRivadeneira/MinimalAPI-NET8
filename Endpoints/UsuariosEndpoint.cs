using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Filtros;
using MinimalAPIPeliculas.Servicios;
using MinimalAPIPeliculas.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MinimalAPIPeliculas.Endpoints
{
    public static class UsuariosEndpoint
    {
        public static RouteGroupBuilder MapUsuarios(this RouteGroupBuilder group)
        {
            group.MapPost("/registrar", Registrar).AddEndpointFilter<FiltroValidaciones<CredencialesUsuarioDTO>>();
            group.MapPost("/login", Login).AddEndpointFilter<FiltroValidaciones<CredencialesUsuarioDTO>>();

            group.MapPost("/haceradmin", HacerAdmin)
                .AddEndpointFilter<FiltroValidaciones<EditarClaimDTO>>()
                .RequireAuthorization("isAdmin");

            group.MapPost("/removerradmin", RemoverAdmin)
                .AddEndpointFilter<FiltroValidaciones<EditarClaimDTO>>()
                .RequireAuthorization("isAdmin");

            group.MapGet("/renovarToken", RenovarToken).RequireAuthorization();
            return group;
        }

        static async Task<Results<Ok<RespuestaAutenticacionDTO>, BadRequest<IEnumerable<IdentityError>>>> Registrar(CredencialesUsuarioDTO credencialesUsuarioDTO,
            [FromServices] UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            var usuario = new IdentityUser
            {
                UserName = credencialesUsuarioDTO.Email,
                Email = credencialesUsuarioDTO.Email
            };

            var resultado = await userManager.CreateAsync(usuario, credencialesUsuarioDTO.Password);

            if(resultado.Succeeded)
            {
                var credencialesRespuesta = await ConstruirToken(credencialesUsuarioDTO, configuration, userManager);
                return TypedResults.Ok(credencialesRespuesta);
            }
            else
            {
                return TypedResults.BadRequest(resultado.Errors);
            }
        }

        public static async Task<Results<Ok<RespuestaAutenticacionDTO>, NotFound>> RenovarToken(IServicioUsuario servicioUsuario,
            IConfiguration configuration, UserManager<IdentityUser> userManager)
        {
            var usuario = await servicioUsuario.ObtenerUsuario();
            if(usuario is null)
            {
                return TypedResults.NotFound();
            }

            var credencialesUsuarioDTO = new CredencialesUsuarioDTO { Email = usuario.Email! };

            var respuestaAutenticacionDTO = await ConstruirToken(credencialesUsuarioDTO, configuration, userManager);
            return TypedResults.Ok(respuestaAutenticacionDTO);

        }

        static async Task<Results<NoContent, NotFound>> HacerAdmin(EditarClaimDTO editarClaimDTO,
            [FromServices] UserManager<IdentityUser> userManager)
        {
            var usuario = await userManager.FindByEmailAsync(editarClaimDTO.Email);
            if(usuario is null)
            {
                return TypedResults.NotFound();
            }

            await userManager.AddClaimAsync(usuario, new Claim("isAdmin", "true"));
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> RemoverAdmin(EditarClaimDTO editarClaimDTO,
            [FromServices] UserManager<IdentityUser> userManager)
        {
            var usuario = await userManager.FindByEmailAsync(editarClaimDTO.Email);
            if (usuario is null)
            {
                return TypedResults.NotFound();
            }

            await userManager.RemoveClaimAsync(usuario, new Claim("isAdmin", "true"));
            return TypedResults.NoContent();
        }

        static async Task<Results<Ok<RespuestaAutenticacionDTO>, BadRequest<string>>> Login(CredencialesUsuarioDTO credencialesUsuarioDTO,
            [FromServices] SignInManager<IdentityUser> signInManager,
            [FromServices] UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            var usuario = await userManager.FindByEmailAsync(credencialesUsuarioDTO.Email);

            if(usuario is null)
            {
                return TypedResults.BadRequest("Login Incorrecto");
            }

            var resultado = await signInManager.CheckPasswordSignInAsync(usuario, credencialesUsuarioDTO.Password, lockoutOnFailure: false);

            if(resultado.Succeeded)
            {
                var respuestaAutenticacion = await ConstruirToken(credencialesUsuarioDTO, configuration, userManager);
                return TypedResults.Ok(respuestaAutenticacion);
            }
            else
            {
                return TypedResults.BadRequest("Login Incorrecto");
            }
        }

        private static async Task<RespuestaAutenticacionDTO> ConstruirToken(CredencialesUsuarioDTO credencialesUsuarioDTO, 
            IConfiguration configuration, UserManager<IdentityUser> userManager)
        {
            var claims = new List<Claim>
            {
                new Claim("email", credencialesUsuarioDTO.Email),
                new Claim("any", "any other value")
            };

            var usuario = await userManager.FindByNameAsync(credencialesUsuarioDTO.Email);
            var claimsDB = await userManager.GetClaimsAsync(usuario);
            claims.AddRange(claimsDB);

            var llave = Llaves.ObtenerLlave(configuration);
            var creds = new SigningCredentials(llave.First(), SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddMonths(1);

            var tokenSeguridad = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiracion, signingCredentials: creds);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenSeguridad);

            return new RespuestaAutenticacionDTO
            {
                Token = token,
                Expiracion = expiracion,
            };
        }
    }


}
