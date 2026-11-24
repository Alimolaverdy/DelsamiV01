using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Security;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ExtendedCustomer.Services
{
    public class ZarIranPermissionService : PermissionService, IPermissionService
    {
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        public ZarIranPermissionService(ICustomerService customerService,
                                         ILocalizationService localizationService,
                                         IRepository<PermissionRecord> permissionRecordRepository,
                                         IRepository<PermissionRecordCustomerRoleMapping> permissionRecordCustomerRoleMappingRepository,
                                         IStaticCacheManager staticCacheManager,
                                         IWorkContext workContext) : base(customerService,
                                                                          localizationService,
                                                                          permissionRecordRepository,
                                                                          permissionRecordCustomerRoleMappingRepository,
                                                                          staticCacheManager,
                                                                          workContext)
        {
            _customerService = customerService;
            _workContext = workContext;
        }

        public override async Task<bool> AuthorizeAsync(PermissionRecord permission)
        {
            return await AuthorizeAsync(permission, await _workContext.GetCurrentCustomerAsync());
        }
        public override async Task<bool> AuthorizeAsync(string permissionRecordSystemName, Customer customer)
        {
            if (string.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            var customerRoles = await _customerService.GetCustomerRolesAsync(customer);

            if (!customerRoles.Any())
            {
                var guestCustomerRole = await _customerService.GetCustomerRoleBySystemNameAsync("Guests");
                customerRoles.Add(guestCustomerRole);
            }
            foreach (var role in customerRoles)
                if (await AuthorizeAsync(permissionRecordSystemName, role.Id))
                    //yes, we have such permission
                    return true;

            //no permission found
            return false;
        }
    }
}
