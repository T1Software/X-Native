using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using XNative.Core.Abstractions;

namespace XNative.App
{
    /// <summary>
    /// Development host of X-Native. It exists so that the composition root
    /// can be executed and verified outside of the Xbox environment.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Entry point of the development host.
        /// </summary>
        public static void Main()
        {
            using ServiceProvider provider = CompositionRoot.Build();

            IEnumerable<IModuleDescriptor> modules = provider.GetServices<IModuleDescriptor>();

            Console.WriteLine("X-Native modules:");
            foreach (IModuleDescriptor module in modules)
            {
                Console.WriteLine($"  {module.Id} - {module.DisplayName}");
            }
        }
    }
}
