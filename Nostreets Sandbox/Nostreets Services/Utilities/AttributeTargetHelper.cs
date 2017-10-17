using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Utilities
{
    public enum ClassTypes
    {
        Any = 1,
        Assembly = 2,
        Methods = 4,
        Constructors = 8,
        Properties = 16,
        OtherFields = 32,
        Type = 64,
        Parameters = 128
    }

    public static class AttributeTargetHelper<TAttribute> where TAttribute : Attribute
    {
        static AttributeTargetHelper()
        {
            targetMap = new List<Tuple<TAttribute, object>>();


            skipAssemblies = new List<string>(typeof(TAttribute).Assembly.GetReferencedAssemblies().Select(c => c.FullName));
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.Contains("System")) { skipAssemblies.Add(assembly.FullName); }
            }

            ScanAllAssemblies();
        }

        private static List<Tuple<TAttribute, object>> targetMap;

        private static List<string> skipAssemblies;

        private static void Add(TAttribute attribute, object item)
        {
            targetMap.Add(new Tuple<TAttribute, object>(attribute, item));
        }

        private static void ScanAllAssemblies()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                ScanAssembly(assembly);
            }
        }

        private static void ScanAssembly(Assembly assembly, ClassTypes part = ClassTypes.Any)
        {
            const BindingFlags memberInfoBinding = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

            if (!skipAssemblies.Contains(assembly.FullName))
            {
                skipAssemblies.Add(assembly.FullName);

                Debug.WriteLine("Loading attribute targets for " + typeof(TAttribute).Name + " from assembly " + assembly.FullName);

                if (part == ClassTypes.Any || part == ClassTypes.Assembly)
                {
                    foreach (TAttribute attr in assembly.GetCustomAttributes(typeof(TAttribute), false))
                    { Add(attr, assembly); }
                }

                foreach (Type type in assembly.GetTypes())
                {
                    if (part == ClassTypes.Any || part == ClassTypes.Type)
                    {
                        foreach (TAttribute attr in type.GetCustomAttributes(typeof(TAttribute), false))
                        { Add(attr, type); }
                    }

                    foreach (MemberInfo member in type.GetMembers(memberInfoBinding))
                    {
                        if (member.MemberType == MemberTypes.Property && (part == ClassTypes.Properties | part == ClassTypes.Any))
                        {
                            foreach (TAttribute attr in member.GetCustomAttributes(typeof(TAttribute), false))
                            { Add(attr, member); }
                        }

                        if (member.MemberType == MemberTypes.Method && (part == ClassTypes.Methods | part == ClassTypes.Any))
                        {
                            foreach (TAttribute attr in member.GetCustomAttributes(typeof(TAttribute), false))
                            { Add(attr, member); }


                        }

                        if (member.MemberType == MemberTypes.Method && (part == ClassTypes.Parameters | part == ClassTypes.Any))
                        {
                            foreach (ParameterInfo parameter in ((MethodInfo)member).GetParameters())
                            {
                                foreach (TAttribute attr in parameter.GetCustomAttributes(typeof(TAttribute), false))
                                { Add(attr, parameter); }
                            }
                        }

                    }
                }
            }

            foreach (var assemblyName in assembly.GetReferencedAssemblies())
            {
                if (!skipAssemblies.Contains(assemblyName.FullName))
                {
                    try
                    {
                        ScanAssembly(Assembly.Load(assemblyName));
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
        }

        public static List<Tuple<TAttribute, object>> GetAttributes(ClassTypes section = ClassTypes.Any)
        {
            return targetMap;
        }

        public static void RescanAssemblies(ClassTypes section = ClassTypes.Any)
        {
            StackTrace stackTrace = new StackTrace();           // get call stack
            StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)

            // write call stack method names
            foreach (StackFrame stackFrame in stackFrames)
            {
                Console.WriteLine(stackFrame.GetMethod().Name);   // write method name
                ScanAssembly(stackFrame.GetMethod().GetType().Assembly, section);
            }

           
        }

    }
}
