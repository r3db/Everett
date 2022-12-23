using System;
using System.Collections.Generic;
using System.Linq;

namespace Everett.Interop
{
    // Todo: Introduce Cache!
    public static class ModuleResolver
    {
        // Internal Instance Data
        private static readonly IDictionary<string, List<ModuleResolverMapping>> _mappings = new Dictionary<string, List<ModuleResolverMapping>>();
        
        // Methods
        public static void Register(string moniker, IEnumerable<ModuleResolverMapping> mappings)
        {
            if (moniker is null)
            {
                throw new ArgumentNullException(nameof(moniker));
            }

            if (mappings is null)
            {
                throw new ArgumentNullException(nameof(mappings));
            }

            // Todo: Use this instead!
            //if (_mappings.ContainsKey(moniker) is false)
            //{
            //    _mappings.Add(moniker, mappings.ToList());
            //}
            //else 
            //{
            //    _mappings[moniker].AddRange(mappings.ToList());
            //}

            foreach (var item in mappings)
            {
                if (_mappings.ContainsKey(moniker) is false)
                {
                    _mappings.Add(moniker, new List<ModuleResolverMapping>());
                }

                _mappings[moniker].Add(item);
            }
        }

        // Methods - Internal
        internal static string Resolve(string moniker)
        {
            if (moniker is null)
            {
                throw new ArgumentNullException(nameof(moniker));
            }

            var platform = PlatformUtility.Platform;
            var architecture = PlatformUtility.Architecture;

            var mapping = _mappings.ContainsKey(moniker)
                ? _mappings[moniker].FirstOrDefault(x => x.Platform == platform && x.Architecture == architecture)
                : null;

            if (mapping is null)
            {
                throw new ArgumentException(string.Format("The specified moniker '{0}' is not valid.", moniker), nameof(moniker));
            }

            return mapping.Name;
        }
    }
}