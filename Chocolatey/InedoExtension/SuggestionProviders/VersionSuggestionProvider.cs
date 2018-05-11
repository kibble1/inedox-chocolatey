﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using NuGet;
#if Otter
using Inedo.Otter.Extensibility;
using Inedo.Otter.Web.Controls;
#elif BuildMaster
using Inedo.BuildMaster.Extensibility;
using Inedo.BuildMaster.Web.Controls;
#else
using Inedo.Extensibility;
using Inedo.Web;
#endif

namespace Inedo.Extensions.Chocolatey.SuggestionProviders
{
    internal sealed class VersionSuggestionProvider : ISuggestionProvider
    {
        public Task<IEnumerable<string>> GetSuggestionsAsync(IComponentConfiguration config)
        {
            return Task.Run(() => this.GetSuggestions(config["PackageName"], config["Version"], AH.CoalesceString(config["Source"], "https://chocolatey.org/api/v2")));
        }

        private IEnumerable<string> GetSuggestions(string packageName, string partialVersion, string source)
        {
            if (SpecialSourceSuggestionProvider.SpecialSources.Contains(source))
                return Enumerable.Empty<string>();

            var repository = PackageRepositoryFactory.Default.CreateRepository(source);
            return repository.FindPackagesById(packageName).Select(pkg => pkg.Version.ToOriginalString()).Where(v => v.Contains(partialVersion));
        }
    }
}