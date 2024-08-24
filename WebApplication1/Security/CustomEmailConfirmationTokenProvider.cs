using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace WebApplication1.Security
{
    public class CustomEmailConfirmationTokenProvider<Tuser> : DataProtectorTokenProvider<Tuser> where Tuser : class
    {
        public CustomEmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider,
            IOptions<CustomEmailConfirmationTokenProviderOptions> options
            , ILogger<DataProtectorTokenProvider<Tuser>> logger) : base(dataProtectionProvider,options,logger)
        {
             

        
        }
    }
}
