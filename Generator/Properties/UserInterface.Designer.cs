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
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class UserInterface {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal UserInterface() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Generator.Properties.UserInterface", typeof(UserInterface).Assembly);
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
        ///   Looks up a localized string similar to This password can be used to protect any data..
        /// </summary>
        internal static string AdequateForProtectingAllOfYourData {
            get {
                return ResourceManager.GetString("AdequateForProtectingAllOfYourData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New password: {0}.
        /// </summary>
        internal static string NewPassword {
            get {
                return ResourceManager.GetString("NewPassword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New password strength: {0} .
        /// </summary>
        internal static string NewPasswordStrength {
            get {
                return ResourceManager.GetString("NewPasswordStrength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New password strength in bits: {0}.
        /// </summary>
        internal static string NewPasswordStrengthInBits {
            get {
                return ResourceManager.GetString("NewPasswordStrengthInBits", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Generator [/AKB | /AN | /PIN] [new password length]
        ///Generator /GeneratePasswordUsingAnyCharacterOnAKeyboard] [new password length]
        ///Generator /GenerateAlphaNumericPassword [new password length]
        ///Generator /GeneratePersonalIdentificationNumber [new password length]
        ///Generator /?
        ///Generator /Help
        ///
        ////AKB
        ////GeneratePasswordUsingAnyCharacterOnAKeyboard
        ///     If this command is specified, the created password can have any character
        ///     which can be typed on a keyboard.  Each character in the password can be         /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Usage {
            get {
                return ResourceManager.GetString("Usage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This password should only be used for PINs and other passwords where the number of guesses can be limited.  If the number of guesses cannot be limited, it should not be used because it can easily be brute forced (i.e. a computer program may be able to try all of possible password values and find the right password)..
        /// </summary>
        internal static string WeakPassword {
            get {
                return ResourceManager.GetString("WeakPassword", resourceCulture);
            }
        }
    }
}
