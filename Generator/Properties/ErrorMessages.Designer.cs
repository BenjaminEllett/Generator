﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Generator.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ErrorMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ErrorMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Generator.Properties.ErrorMessages", typeof(ErrorMessages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The password length command line parameter contains invalid data because it does not contain a positive integer number.  Here is what the parameter contains: {0}..
        /// </summary>
        internal static string PasswordLengthDoesNotContainAValidNumber {
            get {
                return ResourceManager.GetString("PasswordLengthDoesNotContainAValidNumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The password length command line parameter contains a number which is either too low or too high.  This program can generate a password which has {0} and {1} characters (inclusive).  Here is the parameter&apos;s value: {2}..
        /// </summary>
        internal static string PasswordLengthOutOfRangeFormatString {
            get {
                return ResourceManager.GetString("PasswordLengthOutOfRangeFormatString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This program does not support passwords longer than 256 characters..
        /// </summary>
        internal static string PasswordLengthTooLong {
            get {
                return ResourceManager.GetString("PasswordLengthTooLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A valid password must have at least 1 character..
        /// </summary>
        internal static string PasswordLengthTooShort {
            get {
                return ResourceManager.GetString("PasswordLengthTooShort", resourceCulture);
            }
        }
    }
}