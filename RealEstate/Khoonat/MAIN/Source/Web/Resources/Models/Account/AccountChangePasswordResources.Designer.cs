﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18010
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JahanJooy.RealEstate.Web.Resources.Models.Account {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class AccountChangePasswordResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AccountChangePasswordResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("JahanJooy.RealEstate.Web.Resources.Models.Account.AccountChangePasswordResources", typeof(AccountChangePasswordResources).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to کلمه عبور جدید (جهت تایید).
        /// </summary>
        public static string Label_ConfirmPassword {
            get {
                return ResourceManager.GetString("Label_ConfirmPassword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to کلمه عبور جدید.
        /// </summary>
        public static string Label_NewPassword {
            get {
                return ResourceManager.GetString("Label_NewPassword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to کلمه عبور فعلی.
        /// </summary>
        public static string Label_OldPassword {
            get {
                return ResourceManager.GetString("Label_OldPassword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to هر دو کلمه عبور وارد شده باید با هم برابر باشند..
        /// </summary>
        public static string Validation_Compare_ConfirmPassword {
            get {
                return ResourceManager.GetString("Validation_Compare_ConfirmPassword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to کلمه عبور نمی تواند کمتر از 4 حرف باشد، و بهتر است بیش از 6 حرف باشد..
        /// </summary>
        public static string Validation_NewPassword_Length {
            get {
                return ResourceManager.GetString("Validation_NewPassword_Length", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to لطفاً یک کلمه عبور مناسب انتخاب کنید..
        /// </summary>
        public static string Validation_NewPassword_Required {
            get {
                return ResourceManager.GetString("Validation_NewPassword_Required", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to کلمه عبور نمی تواند کمتر از 4 حرف باشد..
        /// </summary>
        public static string Validation_OldPassword_Length {
            get {
                return ResourceManager.GetString("Validation_OldPassword_Length", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to جهت امنیت بیشتر، ارائه کلمه عبور فعلی برای تغییر آن الزامی است..
        /// </summary>
        public static string Validation_OldPassword_Required {
            get {
                return ResourceManager.GetString("Validation_OldPassword_Required", resourceCulture);
            }
        }
    }
}
