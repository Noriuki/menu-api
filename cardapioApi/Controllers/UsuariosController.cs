using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cardapioApi.Models;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace cardapioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly cardapioDBContext _context;

        public UsuariosController(cardapioDBContext context, IOptions<JwtSettings> appSettings)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<dynamic>> Login([FromBody]Usuario reqDados)
        {
            var usuario =  await _context.Usuario.SingleOrDefaultAsync(x => x.Email == reqDados.Email && x.Senha == reqDados.Senha);

            // Retorna null se usuario invalido
            if (usuario == null)
                return null;

            var TokenString = GerarToken();

            usuario.Senha = null;

            return new
            {
                Autenticado = true,
                Usuario = usuario.Nome,
                Token = TokenString
            };
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<dynamic>> Registrar([FromBody]Usuario reqDados)
        {

            // Validacao dos dados
            if (string.IsNullOrWhiteSpace(reqDados.Senha))
                return null;

            if (_context.Usuario.Any(x => x.Email == reqDados.Email))
                return null;

            _context.Usuario.Add(reqDados);
            await _context.SaveChangesAsync();

            var TokenString = GerarToken();

            return new
            {
                Usuario = reqDados,
                Token = TokenString
            };
        }

        public static string GerarToken()
        {
            // Gerar o Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JwtSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddHours(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
