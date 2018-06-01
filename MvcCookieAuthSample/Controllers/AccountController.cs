using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using MvcCookieAuthSample.ViewModels;
using Microsoft.AspNetCore.Identity;
using MvcCookieAuthSample.Models;
using IdentityServer4.Test;
using IdentityServer4.Services;

namespace MvcCookieAuthSample.Controllers
{


    public class AccountController : Controller
    {
        //public readonly TestUserStore _users;
        //public AccountController(TestUserStore testUserStore)
        //{
        //    _users = testUserStore;
        //}
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private IIdentityServerInteractionService _interaction;
        public AccountController(UserManager<ApplicationUser> userManager
            , SignInManager<ApplicationUser> signInManager
            , IIdentityServerInteractionService interaction)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _interaction = interaction;
        }
        /// <summary>
        /// 注册GET
        /// </summary>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        public IActionResult Register( string ReturnUrl = null)
        {
            ViewData["ResultUrl"] = ReturnUrl;
            return View();
            
        }
        /// <summary>
        /// 注册POST
        /// </summary>
        /// <param name="registerViewModel"></param>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public async  Task<IActionResult> Register(RegisterViewModel registerViewModel, string ReturnUrl = null)
        {
            if (ModelState.IsValid)
            {
                ViewData["ResultUrl"] = ReturnUrl;
                var register = new ApplicationUser
                {
                    UserName = registerViewModel.Email.Split('@').First(),
                    Email = registerViewModel.Email,
                    PasswordHash = registerViewModel.Password

                };
                var idrntityResult = await _userManager.CreateAsync(register, registerViewModel.ConfirmedPassword);
                if (idrntityResult.Succeeded)
                {
                    await _signInManager.SignInAsync(register, new AuthenticationProperties() { IsPersistent = true });
                    return RedirectToLocal(ReturnUrl);
                }
                else
                {
                    AddError(idrntityResult);
                }
            }
            return View();
        }
        /// <summary>
        /// 登录GET
        /// </summary>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        public IActionResult Login(string ReturnUrl = null)
        {
            ViewData["ResultUrl"] = ReturnUrl;
            return View();
        }
        /// <summary>
        /// 登录POST
        /// </summary>
        /// <param name="loginModel"></param>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                ViewData["ResultUrl"] = returnUrl;
                var user = await _userManager.FindByEmailAsync(loginModel.Email);
                if (user == null)
                {
                    ModelState.AddModelError(nameof(LoginViewModel.Email), "Email not exists");
                }
                else
                {
                    if ( await _userManager.CheckPasswordAsync(user, loginModel.Password))
                    {
                        AuthenticationProperties props = null;
                        if (loginModel.RememberMe)
                        {
                            props = new AuthenticationProperties
                            {
                                IsPersistent = true,
                                ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(30))
                            };
                        }
                         
                        // await Microsoft.AspNetCore.Http.AuthenticationManagerExtensions.SignInAsync(
                        //    HttpContext,
                        //    user.SubjectId,
                        //    user.Username,
                        //    props
                        //);
                        await _signInManager.SignInAsync(user, props);
                        if (_interaction.IsValidReturnUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        return Redirect("~/");
                    } 
                    else
                    {
                        ModelState.AddModelError(nameof(LoginViewModel.Password), "Password not exists");
                    }
                }
            }
            return View(loginModel);
            

        }
        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        public async Task< IActionResult> Logout()
        {
             await _signInManager.SignOutAsync();
            //HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult MakeLogin()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,"Anker"),
                new Claim(ClaimTypes.Role,"Admin")
            };
            var claimIdrntity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdrntity));
            return Ok("OK");
        }
        //public IActionResult LogOut()
        //{
        //    HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    return Ok("NO");
        //}
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index),"Home");
            }
        }
        private void AddError(IdentityResult identityResult)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty,error.Description);
            }   
        }
    }
}