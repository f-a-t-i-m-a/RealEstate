using System.Collections.Generic;
using System.Reflection;
using System.Web.Http.Dispatcher;

namespace JahanJooy.Common.Util.Web.Services
{
    public class LimitedAssembliesResolver : IAssembliesResolver
    {
        private readonly ICollection<Assembly> _assemblies;

        public LimitedAssembliesResolver(ICollection<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        public ICollection<Assembly> GetAssemblies()
        {
            return _assemblies;
        }
    }

}