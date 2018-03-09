/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Lucene.Net.Support;
using System;
using System.Collections.Generic;

namespace Lucene.Net.Messages
{
    /// <summary> MessageBundles classes extend this class, to implement a bundle.
    ///
    /// For Native Language Support (NLS), system of software internationalization.
    ///
    /// This interface is similar to the NLS class in eclipse.osgi.util.NLS class -
    /// initializeMessages() method resets the values of all static strings, should
    /// only be called by classes that extend from NLS (see TestMessages.java for
    /// reference) - performs validation of all message in a bundle, at class load
    /// time - performs per message validation at runtime - see NLSTest.java for
    /// usage reference
    ///
    /// MessageBundle classes may subclass this type.
    /// </summary>
    public class NLS
    {
        public interface IPriviligedAction
        {
            /// <summary>
            /// Performs the priviliged action.
            /// </summary>
            /// <returns>A value that may represent the result of the action.</returns>
            object Run();
        }

        private class AnonymousClassPrivilegedAction : IPriviligedAction
        {
            public AnonymousClassPrivilegedAction(System.Reflection.FieldInfo field)
            {
                InitBlock(field);
            }

            private void InitBlock(System.Reflection.FieldInfo field)
            {
                this.field = field;
            }

            private System.Reflection.FieldInfo field;

            public virtual object Run()
            {
                // field.setAccessible(true); // {{Aroush-2.9}} java.lang.reflect.AccessibleObject.setAccessible
                return null;
            }
        }

        private static IDictionary<string, Type> bundles = new HashMap<string, Type>(0);

        protected internal NLS()
        {
            // Do not instantiate
        }

        public static string GetLocalizedMessage(string key)
        {
            return GetLocalizedMessage(key, System.Threading.Thread.CurrentThread.CurrentCulture);
        }

        public static string GetLocalizedMessage(string key, System.Globalization.CultureInfo locale)
        {
            object message = GetResourceBundleObject(key, locale);
            if (message == null)
            {
                return "Message with key:" + key + " and locale: " + locale + " not found.";
            }
            return message.ToString();
        }

        public static string GetLocalizedMessage(string key, System.Globalization.CultureInfo locale, params object[] args)
        {
            string str = GetLocalizedMessage(key, locale);

            if (args.Length > 0)
            {
                str = string.Format(str, args);
            }

            return str;
        }

        public static string GetLocalizedMessage(string key, params object[] args)
        {
            return GetLocalizedMessage(key, System.Threading.Thread.CurrentThread.CurrentCulture, args);
        }

        /// <summary> Initialize a given class with the message bundle Keys Should be called from
        /// a class that extends NLS in a static block at class load time.
        ///
        /// </summary>
        /// <param name="bundleName">Property file with that contains the message bundle
        /// </param>
        /// <param name="clazz">where constants will reside
        /// </param>
        //@SuppressWarnings("unchecked")
        protected internal static void InitializeMessages<T>(string bundleName)
        {
            try
            {
                Load<T>();
                if (!bundles.ContainsKey(bundleName))
                    bundles[bundleName] = typeof(T);
            }
            catch (System.Exception)
            {
                // ignore all errors and exceptions
                // because this function is supposed to be called at class load time.
            }
        }

        private static object GetResourceBundleObject(string messageKey, System.Globalization.CultureInfo locale)
        {
            // slow resource checking
            // need to loop thru all registered resource bundles
            for (var it = bundles.Keys.GetEnumerator(); it.MoveNext();)
            {
                Type clazz = bundles[it.Current];
                System.Threading.Thread.CurrentThread.CurrentUICulture = locale;
                System.Resources.ResourceManager resourceBundle = System.Resources.ResourceManager.CreateFileBasedResourceManager(clazz.Name, "Messages", null); //{{Lucene.Net-2.9.1}} Can we make resourceDir "Messages" more general?
                if (resourceBundle != null)
                {
                    try
                    {
                        object obj = resourceBundle.GetObject(messageKey);
                        if (obj != null)
                            return obj;
                    }
                    catch (System.Resources.MissingManifestResourceException)
                    {
                        // just continue it might be on the next resource bundle
                    }
                }
            }
            // if resource is not found
            return null;
        }

        private static void Load<T>()
        {
            var clazz = typeof(T);
            System.Reflection.FieldInfo[] fieldArray = clazz.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Static);

            bool isFieldAccessible = clazz.IsPublic;

            // build a map of field names to Field objects
            int len = fieldArray.Length;
            var fields = new HashMap<string, System.Reflection.FieldInfo>(len * 2);
            for (int i = 0; i < len; i++)
            {
                fields[fieldArray[i].Name] = fieldArray[i];
                LoadfieldValue<T>(fieldArray[i], isFieldAccessible);
            }
        }

        /// <param name="field"></param>
        /// <param name="isFieldAccessible"></param>
        private static void LoadfieldValue<T>(System.Reflection.FieldInfo field, bool isFieldAccessible)
        {
            var clazz = typeof(T);
            /*
			int MOD_EXPECTED = Modifier.PUBLIC | Modifier.STATIC;
			int MOD_MASK = MOD_EXPECTED | Modifier.FINAL;
			if ((field.getModifiers() & MOD_MASK) != MOD_EXPECTED)
				return ;
            */
            if (!(field.IsPublic || field.IsStatic))
                return;

            // Set a value for this empty field.
            if (!isFieldAccessible)
                MakeAccessible(field);
            try
            {
                field.SetValue(null, field.Name);
                ValidateMessage<T>(field.Name);
            }
            catch (ArgumentException)
            {
                // should not happen
            }
            catch (System.UnauthorizedAccessException)
            {
                // should not happen
            }
        }

        /// <param name="key">- Message Key
        /// </param>
        private static void ValidateMessage<T>(string key)
        {
            // Test if the message is present in the resource bundle
            var clazz = typeof(T);
            try
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                System.Resources.ResourceManager resourceBundle = System.Resources.ResourceManager.CreateFileBasedResourceManager(clazz.FullName, "", null);
                if (resourceBundle != null)
                {
                    object obj = resourceBundle.GetObject(key);
                    if (obj == null)
                    {
                        System.Console.Error.WriteLine("WARN: Message with key:" + key + " and locale: " + System.Threading.Thread.CurrentThread.CurrentCulture + " not found.");
                    }
                }
            }
            catch (System.Resources.MissingManifestResourceException)
            {
                System.Console.Error.WriteLine("WARN: Message with key:" + key + " and locale: " + System.Threading.Thread.CurrentThread.CurrentCulture + " not found.");
            }
            catch (System.Exception)
            {
                // ignore all other errors and exceptions
                // since this code is just a test to see if the message is present on the
                // system
            }
        }

        /*
		* Make a class field accessible
		*/

        //@SuppressWarnings("unchecked")
        private static void MakeAccessible(System.Reflection.FieldInfo field)
        {
            if (System.Security.SecurityManager.SecurityEnabled)
            {
                //field.setAccessible(true);   // {{Aroush-2.9}} java.lang.reflect.AccessibleObject.setAccessible
            }
            else
            {
                //AccessController.doPrivileged(new AnonymousClassPrivilegedAction(field));     // {{Aroush-2.9}} java.security.AccessController.doPrivileged
            }
        }
    }
}