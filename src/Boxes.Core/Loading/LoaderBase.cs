// Copyright 2012 - 2013 dbones.co.uk
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
namespace Boxes.Loading
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Module = Boxes.Module;

    public abstract class LoaderBase : ILoader
    {
        /// <summary>
        /// The Global list of loaded assemblies, these assemblies are SHARED
        /// between the packages and main program
        /// </summary>
        protected readonly IDictionary<Module, Assembly> Loaded;

        protected LoaderBase()
        {
            var assemblyNames = AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(x => new KeyValuePair<Module, Assembly>(new Module(x.GetName()), x));

            Loaded = new Dictionary<Module, Assembly>();

            foreach (var assemblyName in assemblyNames)
            {
                Loaded.Add(assemblyName);
            }
        }

        public virtual void LoadPackage(Package package)
        {
            //simply call the load
            foreach (var assemblyReference in package.AssembliesToLoad())
            {
                LoadAssembly(assemblyReference);
            }
            package.Loaded = true;
        }

        protected virtual void LoadAssembly(AssemblyReference assemblyReference)
        {
            //todo: see if we need to say, "are you already loaded?"
            Assembly assembly;
            if (Loaded.TryGetValue(assemblyReference.Module, out assembly))
            {
                //the assembly is already loaded in the AppDomain
                assemblyReference.AssociateWithExisting(assembly);
            }
            else
            {
                assemblyReference.LoadFromFile();
                Loaded.Add(assemblyReference.Module, assemblyReference.Assembly);
            }
        }
    }
}