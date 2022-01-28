using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineTicketing.Data.ViewModel;
using OnlineTicketing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineTicketing.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signManager = signManager;
            _roleManager = roleManager;
        }
        
        public IActionResult LogIn(string returnUrl)
        {
            return View(new LogInViewModel()
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LogInViewModel logInViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(logInViewModel);
            }
            var user = await _userManager.FindByEmailAsync(logInViewModel.EmailAddress);

            if(user!= null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, logInViewModel.Password);
                if (passwordCheck)
                {
                    var result = await _signManager.PasswordSignInAsync(user, logInViewModel.Password, false, false);
                    if (result.Succeeded)
                    {
                        if (string.IsNullOrEmpty(logInViewModel.ReturnUrl))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        return Redirect(logInViewModel.ReturnUrl);
                    }
                }
            }
            ModelState.AddModelError("", "Username or Password is Invalid!!!");
            return View(logInViewModel);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);
                if (user != null)
                {
                    ModelState.AddModelError("","Email already in use.");
                    return View(registerViewModel);

                }
                var newUser = new ApplicationUser()
                {
                    FullName = registerViewModel.FullName,
                    Email = registerViewModel.EmailAddress,
                    UserName = registerViewModel.EmailAddress
                };
                var result = await _userManager.CreateAsync(newUser, registerViewModel.Password);

                if (result.Succeeded)
                {
                    if(!await _roleManager.RoleExistsAsync("Admin"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    }
                    if (!await _roleManager.RoleExistsAsync("Customer"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Customer"));
                    }
                    //await _userManager.AddToRoleAsync(newUser, "Admin");
                    await _userManager.AddToRoleAsync(newUser, "Customer");
                    return View("RegisterCompleted");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create the account.");
                    return View(registerViewModel);
                }
                
            }
            return View(registerViewModel);
        }

        public async Task<IActionResult> Logout()
        {
            await _signManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied(string ReturnUrl)
        {
            return View();
        }
    }
}
