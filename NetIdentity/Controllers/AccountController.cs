using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetIdentity.Models;

namespace NetIdentity.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        
        private const string CLAIM_GENERO = "Genero";
        private const string CLAIM_FECHA_NACIMIENTO = "FechaNacimiento";

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewBag.Error = "Email o contraseña incorrectos";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, DateTime fechaNacimiento, string nombreCompleto, string genero)
        {
            if (string.IsNullOrEmpty(genero))
            {
                ModelState.AddModelError("genero", "El género es requerido");
                return View();
            }
            
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FechaNacimiento = fechaNacimiento,
                NombreCompleto = nombreCompleto,
                genero = genero
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(user,
                    new System.Security.Claims.Claim(CLAIM_FECHA_NACIMIENTO, fechaNacimiento.ToString("yyyy-MM-dd")));
                
                await _userManager.AddClaimAsync(user,
                    new System.Security.Claims.Claim(CLAIM_GENERO, genero));

                await _userManager.AddToRoleAsync(user, "Usuario");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
