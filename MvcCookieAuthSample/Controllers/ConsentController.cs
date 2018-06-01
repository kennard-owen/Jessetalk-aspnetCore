using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcCookieAuthSample.ViewModels;
using IdentityServer4.Stores;
using IdentityServer4.Services;
using IdentityServer4.Models;
using MvcCookieAuthSample.Services;

namespace MvcCookieAuthSample.Controllers
{
    public class ConsentController : Controller
    {
        private readonly ConsentServices _consentServices;
        public ConsentController(ConsentServices consentServices)
        {
            _consentServices = consentServices;
        }
       
        
        [HttpGet]
        public async Task< IActionResult> Index(string returnUrl)
        {
            ConsentViewModel consentViewModel = await _consentServices.BuildConsentViewModel(returnUrl);
            Console.WriteLine(consentViewModel.ClientLogoUrl);
            if (consentViewModel == null)
            {

            }
            return View(consentViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Index(InputConsentViewModel viewModel)
        {
            var result =await _consentServices.PorcessConsent(viewModel);
            if (result.IsRedirect)
            {
                return Redirect(result.RedirectUrl);
            }
            if (!string.IsNullOrEmpty(result.ValidationError))
            {
                ModelState.AddModelError("", result.ValidationError);
            }
            return View(result.consentViewModel);
        }
    }
}