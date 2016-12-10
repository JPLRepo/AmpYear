/**
 * AmpYear power management.
 * (C) Copyright 2015, Jamie Leighton
 * The original code and concept of AmpYear rights go to SodiumEyes on the Kerbal Space Program Forums, which was covered by GNU License GPL (no version stated).
 * As such this code continues to be covered by GNU GPL license.
 * (C) Copyright 2015, Jamie Leighton
 *
 * Kerbal Space Program is Copyright (C) 2013 Squad. See http://kerbalspaceprogram.com/. This
 * project is in no way associated with nor endorsed by Squad.
 *
 *
 */

using System;
using System.Reflection;

namespace AY
{
    /// <summary>
    /// The Wrapper class to access USILS
    /// </summary>
    public class USILSWrapper
    {
        public static System.Type USIMLSSType;
        public static System.Type USIMLSMSCType;
        /// <summary>
        /// Whether we found the USILS assembly in the loadedassemblies.
        ///
        /// SET AFTER INIT
        /// </summary>
        public static Boolean AssemblyExists { get { return USIMLSSType != null; } }

        /// <summary>
        /// Whether we managed to wrap all the methods/functions from the instance.
        ///
        /// SET AFTER INIT
        /// </summary>
        private static Boolean _USILSWrapped;

        /// <summary>
        /// Whether the object has been wrapped
        /// </summary>
        public static Boolean APIReady { get { return _USILSWrapped; } }

        /// <summary>
        /// This method will set up the USILS object and wrap all the methods/functions
        /// </summary>
        /// <returns></returns>
        public static Boolean InitUSILSWrapper()
        {
            //reset the internal objects
            _USILSWrapped = false;
            LogFormatted_DebugOnly("Attempting to Grab USI LS Types...");

            //find the USILS part module type
            USIMLSSType = getType("LifeSupport.ModuleLifeSupportSystem"); 

            if (USIMLSSType == null)
            {
                return false;
            }

            //find the USILS part swappable module type
            USIMLSMSCType = getType("USITools.ModuleSwappableConverter");

            if (USIMLSMSCType == null)
            {
                return false;
            }

            LogFormatted("USI LS Version:{0}", USIMLSSType.Assembly.GetName().Version.ToString());

            _USILSWrapped = true;
            return true;
        }

        internal static Type getType(string name)
        {
            Type type = null;
            AssemblyLoader.loadedAssemblies.TypeOperation(t =>

            {
                if (t.FullName == name)
                    type = t;
            }
            );

            if (type != null)
            {
                return type;
            }
            return null;
        }

        public class ModuleSwappableConverter : PartModule
        {
            internal ModuleSwappableConverter(Object a)
            {
                actualModuleSwappableConverter = a;

                SwappableConverterCurrentLoadoutField = USIMLSMSCType.GetField("currentLoadout");
                SwappableConverterBayNameField = USIMLSMSCType.GetField("bayName");

            }

            private Object actualModuleSwappableConverter;

            private FieldInfo SwappableConverterCurrentLoadoutField;

            public int currentLoadout
            {
                get
                {
                    try
                    {
                        return (int)SwappableConverterCurrentLoadoutField.GetValue(actualModuleSwappableConverter);
                    }
                    catch (Exception ex)
                    {
                        LogFormatted("SwappableConverter current loadout Field failed: {0}", ex.Message);
                        return 0;
                    }
                }
            }

            private FieldInfo SwappableConverterBayNameField;

            public string bayName
            {
                get
                {
                    try
                    {
                        return (string)SwappableConverterBayNameField.GetValue(actualModuleSwappableConverter);
                    }
                    catch (Exception ex)
                    {
                        LogFormatted("SwappableConverter bayName Field failed: {0}", ex.Message);
                        return null;
                    }
                }
            }
        }

        public class ModuleLifeSupportSystem
        {
            internal ModuleLifeSupportSystem(Object a)
            {
                actualModuleLifeSupportSystem = a;

                LifeSupportRecipeMethod = USIMLSSType.GetMethod("get_ECRecipe", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            }

            private Object actualModuleLifeSupportSystem;

            private MethodInfo LifeSupportRecipeMethod;

            /// <summary>
            /// Get the Resource Converter Recipe
            /// </summary>
            /// <returns>ConversionRecipe or NULL</returns>
            public ConversionRecipe LifeSupportRecipe
            {
                get
                {
                    try
                    {
                        return (ConversionRecipe)LifeSupportRecipeMethod.Invoke(actualModuleLifeSupportSystem, null);
                    }
                    catch (Exception ex)
                    {
                        LogFormatted("Invoke LifeSupportRecipeField failed: {0}", ex.Message);
                        return null;
                    }
                }
            }
        }

        #region Logging Stuff

        /// <summary>
        /// Some Structured logging to the debug file - ONLY RUNS WHEN DLL COMPILED IN DEBUG MODE
        /// </summary>
        /// <param name="Message">Text to be printed - can be formatted as per String.format</param>
        /// <param name="strParams">Objects to feed into a String.format</param>
        internal static void LogFormatted_DebugOnly(String Message, params Object[] strParams)
        {
            if (RSTUtils.Utilities.debuggingOn)
                LogFormatted(Message, strParams);
        }

        /// <summary>
        /// Some Structured logging to the debug file
        /// </summary>
        /// <param name="Message">Text to be printed - can be formatted as per String.format</param>
        /// <param name="strParams">Objects to feed into a String.format</param>
        internal static void LogFormatted(String Message, params Object[] strParams)
        {
            Message = String.Format(Message, strParams);
            String strMessageLine = String.Format("{0},{2}-{3},{1}",
                DateTime.Now, Message, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);
            UnityEngine.Debug.Log(strMessageLine);
        }

        #endregion Logging Stuff
    }
}
