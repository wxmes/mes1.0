using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace EIP.Common.WebApi.Provider
{
    public class RequiredBindingMetadataProvider : IBindingMetadataProvider
    {
        public void CreateBindingMetadata(BindingMetadataProviderContext context)
        {
            if (context.PropertyAttributes?.OfType<RequiredAttribute>().Any() ?? false)
            {
                context.BindingMetadata.IsBindingRequired = true;
            }
        }
    }
}