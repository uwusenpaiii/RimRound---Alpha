﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Utilities
{
    public struct PatchCollection 
    {
        public MethodInfo prefix;
        public MethodInfo postfix;
        public MethodInfo transpiler;
        public MethodInfo finalizer;
    };

    public struct ModPatchInfo 
    {
        public ModPatchInfo(string modName, string typeName, string methodName, MethodType methodType, List<string> methodParams = null) 
        {
            ModName = modName;
            TypeName = typeName;
            MethodName = methodName;
            MethodType = methodType;
            methodParameters = methodParams;
        }
        public string ModName;
        public string TypeName;
        public string MethodName;
        public MethodType MethodType;
        public List<string> methodParameters;
    };

    [StaticConstructorOnStartup]
    public static class ModCompatibilityUtility 
    {
        static ModCompatibilityUtility() 
        {
            Assembly mscorlib = Assembly.GetAssembly(typeof(int));

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly a in assemblies)
            {
                if (a == mscorlib)
                    continue;

                types.AddRange(a.GetTypes());
            }
        }

        public static BindingFlags majorFlags =
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Static | BindingFlags.Instance |
            BindingFlags.FlattenHierarchy;

        static List<Type> types = new List<Type>();

        public static void TryPatch(Harmony harmonyinstance, ModPatchInfo modPatchInfo, PatchCollection patchCollection)
        {
            if (!CheckModInstalled(modPatchInfo.ModName) ||
                (patchCollection.prefix is null && patchCollection.postfix is null && patchCollection.transpiler is null && patchCollection.finalizer is null))
                return;

            MethodInfo methodToPatchMI;

            switch (modPatchInfo.MethodType)
            {
                case MethodType.Normal:
                    methodToPatchMI = GetMethodInfo(modPatchInfo.ModName, modPatchInfo.TypeName, modPatchInfo.MethodName, modPatchInfo.methodParameters);
                    break;
                case MethodType.Getter:
                    methodToPatchMI = GetPropertyInfo(modPatchInfo.ModName, modPatchInfo.TypeName, modPatchInfo.MethodName)?.GetGetMethod(true);
                    break;
                case MethodType.Setter:
                    methodToPatchMI = GetPropertyInfo(modPatchInfo.ModName, modPatchInfo.TypeName, modPatchInfo.MethodName)?.GetSetMethod(true);
                    break;
                case MethodType.Constructor:
                    throw new NotImplementedException();
                case MethodType.StaticConstructor:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }

            harmonyinstance.Patch(methodToPatchMI, 
                patchCollection.prefix     is null ? null : new HarmonyMethod(patchCollection.prefix), 
                patchCollection.postfix    is null ? null : new HarmonyMethod(patchCollection.postfix),  
                patchCollection.transpiler is null ? null : new HarmonyMethod(patchCollection.transpiler),
                patchCollection.finalizer  is null ? null : new HarmonyMethod(patchCollection.finalizer));

            Log.Message($"RimRound successfully patched {modPatchInfo.ModName}'s {modPatchInfo.MethodName}!");
        }

        public static bool CheckModInstalled(string modname) 
        {
            if (ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name == modname)) 
            {
                return true;
            }
            return false;
        }

        public static MethodInfo GetMethodInfo(string modname, string typeName, string methodName, List<string> types = null) 
        {
            if (CheckModInstalled(modname))
            {
                foreach (Type t in ModCompatibilityUtility.types) 
                {
                    if (t.Name == typeName)
                    {
                        MethodInfo m;
                        if (types is null)
                            m = t.GetMethod(methodName, majorFlags);
                        else
                        {
                            List<Type> typeParameters = new List<Type>();
                            foreach (string s in types)
                            {
                                foreach (Type type in ModCompatibilityUtility.types) 
                                {
                                    if (s == type.Name) 
                                    {
                                        typeParameters.Add(type);
                                    }
                                }
                            }
                            if (typeParameters.Count != types.Count)
                                throw new Exception("Was unable to match types in ModCompatibilityUtility.GetMethodInfo!");
                            
                            //This was added for testing and needs altered later
                            Type[] methodParams = typeParameters.ToArray();
                            methodParams[1] = methodParams[1].MakeByRefType();
                            //---------------------------------------------
                            m = t.GetMethod(methodName, majorFlags, null, methodParams, null);
                        }
                            

                        if (m is null)
                        {
                            Log.Error($"Could not get method {methodName} from {t.Name}");
                        }

                        return m;
                    }
                }
            }

            return null;
        }

        public static PropertyInfo GetPropertyInfo(string modname, string typeName, string propertyName) 
        {
            if (CheckModInstalled(modname))
            {
                foreach (Type t in types)
                {
                    if (t.Name == typeName)
                    {
                        PropertyInfo m = t.GetProperty(propertyName, majorFlags);

                        if (m is null)
                        {
                            Log.Error($"Could not get method {propertyName} from {t.Name}");
                        }

                        return m;
                    }
                }
            }

            return null;
        }
   
        
    }
}
