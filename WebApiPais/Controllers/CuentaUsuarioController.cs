using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Tokens;
using WebApiPais.Models;

namespace WebApiPais.Controllers
{
    [Produces("application/json")]
    [Route("api/CuentaUsuario")]
    public class CuentaUsuarioController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public CuentaUsuarioController(UserManager<ApplicationUser> userManager,
             SignInManager<ApplicationUser> signInManager,
             IConfiguration configuration
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] InformacionUsuario model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return BuildToken(model);
                }
                else
                {
                    return BadRequest("Usuario o contraseña invalida, por favor verifique.");
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [Route("Logueo")]
        [HttpPost]
        public async Task<IActionResult> Logueo([FromBody] InformacionUsuario userInfo)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(userInfo.Email, userInfo.Password, true, false);
                if (result.Succeeded)
                {
                    return BuildToken(userInfo);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Logueo invalido, por favor verifique.");
                    return BadRequest(ModelState);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        private IActionResult BuildToken(InformacionUsuario informacionUsuario)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, informacionUsuario.Email),
                new Claim("miValor", "lo que yo quiera"), //se puede pasar cualquier valor 
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) //valor unico por token, identificador para poder revocar el token si se desea.
            };


            //la clave secrete debe tener una longitud de mas de 128 bit.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["va_clave_super_secreta"]));
            var credencial = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(1);

            JwtSecurityToken token = new JwtSecurityToken(
                                                           issuer: "yourdomain.com",
                                                           audience: "yourdomain.com",
                                                           claims: claims,
                                                           expires: expiration,
                                                           signingCredentials: credencial
                                    );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiracion= expiration             
            });
        }
    }
}