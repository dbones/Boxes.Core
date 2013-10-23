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
namespace Boxes.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class Util
    {
        public static IEnumerable<Assembly> AllLoadedAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        public static IEnumerable<Assembly> BoxesLoadedAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return assemblies.Where(x => x.GetName().Name.ToLower().Contains("test.box"));
        }

        /// <summary>
        /// because .Net lazy loads assemblies.
        /// </summary>
        /// <param name="packageRegistry"></param>
        public static void ForceLoadOfInternals(PackageRegistry packageRegistry)
        {
            var assemblies = packageRegistry
                .Packages
                .SelectMany(x => x.LoadedAssemblies);

            var types = assemblies.SelectMany(x => x.GetTypes());

            foreach (var type in types)
            {
                try
                {
                    var obj = Activator.CreateInstance(type);
                }
// ReSharper disable EmptyGeneralCatchClause
                catch (Exception)
// ReSharper restore EmptyGeneralCatchClause
                {
                    //do not care, just wanted to force the load 
                    //of the assembly and of it dependent assemblies
                }
                
            }
        }


        public static ObjectInstance CreateObject(this PackageRegistry packageRegistry, string className)
        {
            Type type = packageRegistry.Packages
                .SelectMany(x => x.LoadedAssemblies)
                .SelectMany(x => x.GetExportedTypes())
                .First(x => x.Name == className);

            object instance = Activator.CreateInstance(type);
            return new ObjectInstance(instance);
        }
    }


    public class ObjectInstance
    {
        private readonly object _instance;
        Type _type;

        public ObjectInstance(object instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            _instance = instance;
            _type = instance.GetType();
        }

        public T GetPropertyValue<T>(string methodName)
        {
            PropertyInfo propertyInfo = _type.GetProperty(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
            MethodInfo methodInfo = propertyInfo.GetGetMethod();
            var result = methodInfo.Invoke(_instance, new object[] { });
            return (T)result;
        }
    }



}