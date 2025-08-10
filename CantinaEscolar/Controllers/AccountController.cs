using CantinaEscolar.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CantinaEscolar.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        // Método auxiliar para popular lista de roles
        private List<SelectListItem> GetRolesForCurrentUser()
        {
            if (User.IsInRole("Admin"))
            {
                return new List<SelectListItem>
                {
                    new SelectListItem { Value = "Admin", Text = "Administrador" },
                    new SelectListItem { Value = "Prop", Text = "Proprietário" },
                    new SelectListItem { Value = "Aluno", Text = "Aluno" },
                    new SelectListItem { Value = "Responsavel", Text = "Responsável" }
                };
            }
            if (User.IsInRole("Prop"))
            {
                return new List<SelectListItem>
                {
                    new SelectListItem { Value = "Aluno", Text = "Aluno" },
                    new SelectListItem { Value = "Responsavel", Text = "Responsável" }
                };
            }
            return new List<SelectListItem>();
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Prop")]
        public IActionResult Register()
        {
            var model = new RegisterViewModel();

            if (!User.IsInRole("Admin") && !User.IsInRole("Prop"))
                return Forbid();

            model.Roles = GetRolesForCurrentUser();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Prop")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            _logger.LogInformation("Entrou no POST Register");

            if (User.IsInRole("Prop") && model.Role != "Aluno" && model.Role != "Responsavel")
            {
                ModelState.AddModelError("Role", "Você só pode cadastrar Alunos ou Responsáveis.");
            }

            if (!ModelState.IsValid)
            {
                model.Roles = GetRolesForCurrentUser();

                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        _logger.LogWarning($"Erro no campo '{key}': {error.ErrorMessage}");
                    }
                }

                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Nome = model.Nome,
                RA = model.RA
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            model.Roles = GetRolesForCurrentUser();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = "")
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Login inválido.");
            return View(model);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin,Prop")]
        public async Task<IActionResult> Usuarios()
        {
            var usuarios = _userManager.Users.ToList();
            var lista = new List<UsuarioViewModel>();

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);

                lista.Add(new UsuarioViewModel
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Fone = usuario.PhoneNumber,
                    RA = usuario.RA,
                    Roles = roles.ToList()
                });
            }

            return View(lista);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Prop")]
        public async Task<IActionResult> Editar(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(usuario);
            var model = new UsuarioViewModel
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Fone = usuario.PhoneNumber,
                RA = usuario.RA,
                Roles = roles.ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Prop")]
        public async Task<IActionResult> Editar(UsuarioViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _userManager.FindByIdAsync(model.Id);
            if (usuario == null)
                return NotFound();

            usuario.Nome = model.Nome;
            usuario.Email = model.Email;
            usuario.PhoneNumber = model.Fone;
            usuario.RA = model.RA;

            var result = await _userManager.UpdateAsync(usuario);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            return RedirectToAction("Usuarios");
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Prop")]
        public async Task<IActionResult> Excluir(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Id inválido");

            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(usuario);

            var model = new UsuarioViewModel
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Fone = usuario.PhoneNumber,
                RA = usuario.RA,
                Roles = roles.ToList()
            };

            return View(model);
        }

        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Prop")]
        public async Task<IActionResult> ExcluirConfirmado(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(usuario);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View("Excluir", new UsuarioViewModel { Id = usuario.Id, Nome = usuario.Nome });
            }

            return RedirectToAction("Usuarios");
        }
    }
}
