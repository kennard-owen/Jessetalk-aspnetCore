using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using MvcCookieAuthSample.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCookieAuthSample.Services
{
    public class ConsentServices
    {
        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;
        private readonly IIdentityServerInteractionService _identityServerInteractionServer;
        public ConsentServices(IClientStore clientStore,
            IResourceStore resourceStore,
            IIdentityServerInteractionService identityServerInteractionServer
            )
        {
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _identityServerInteractionServer = identityServerInteractionServer;
        }
        public async Task<ConsentViewModel> BuildConsentViewModel(string returnUrl,InputConsentViewModel model=null)
        {
            var request = await _identityServerInteractionServer.GetAuthorizationContextAsync(returnUrl);
            if (request == null) return null;
            var client = await _clientStore.FindClientByIdAsync(request.ClientId);
            var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);
            var vm = CreateConsentViewModel(request, client, resources, model);
            vm.ReturnUrl = returnUrl;
            return vm;
        }
        private ConsentViewModel CreateConsentViewModel(AuthorizationRequest request, Client client, Resources resources,InputConsentViewModel model)
        {
            var remeberConsent = model?.RemeberConsent ?? true;
            var selectedScopes = model?.ScopesConsented ?? Enumerable.Empty<string>();
            var vm = new ConsentViewModel();
            vm.ClientName = client.ClientName;
            vm.ClientUri = client.ClientUri;
            vm.ClientLogoUrl = client.LogoUri;
            vm.RemeberConsent = remeberConsent;// client.AllowRememberConsent;
            vm.IdentityScopes = resources.IdentityResources.Select(i => CreateScopeVideModel(i, selectedScopes.Contains(i.Name)||model==null));
            vm.ResourceScopes = resources.ApiResources.SelectMany(i => i.Scopes).Select(x => CreateScopeVideModel(x, selectedScopes.Contains(x.Name)||model == null));
            return vm;
        }
        private ScopeViewModel CreateScopeVideModel(Scope scope,bool check)
        {
            return new ScopeViewModel
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Description = scope.Description,
                Required = scope.Required,
                Checked = check|| scope.Required,
                Emphasize = scope.Emphasize,

            };
        }
        private ScopeViewModel CreateScopeVideModel(IdentityResource identityResource, bool check)
        {
            return new ScopeViewModel
            {
                Name = identityResource.Name,
                DisplayName = identityResource.DisplayName,
                Description = identityResource.Description,
                Required = identityResource.Required,
                Checked = check|| identityResource.Required,
                Emphasize = identityResource.Emphasize,
            };
        }
        public async Task<ProcessConsentResult> PorcessConsent(InputConsentViewModel model)
        {
            ConsentResponse consentResponse = null;
            var result =new ProcessConsentResult();
            if (model.Button == "no")
            {
                consentResponse = ConsentResponse.Denied;
            }
            else if (model.Button == "yes")
            {
                if (model.ScopesConsented != null && model.ScopesConsented.Any())
                {
                    consentResponse = new ConsentResponse
                    {
                        RememberConsent = model.RemeberConsent,
                        ScopesConsented = model.ScopesConsented
                    };
                }
            }
            if (consentResponse != null)
            {
                var request = await _identityServerInteractionServer.GetAuthorizationContextAsync(model.ReturnUrl);
                await _identityServerInteractionServer.GrantConsentAsync(request, consentResponse);
                result.RedirectUrl= model.ReturnUrl;
            }
            else
            {
                result.ValidationError = "请至少选择一个权限";
                var consentViewModel =await BuildConsentViewModel(model.ReturnUrl);
                result.consentViewModel = consentViewModel;
            }
            return result;
        }
    }
}
