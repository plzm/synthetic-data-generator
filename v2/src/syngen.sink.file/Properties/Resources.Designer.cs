﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace pelazem.syngen.sink.file.Properties {
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
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("pelazem.syngen.sink.file.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Error: could not create sink file directory.
        /// </summary>
        public static string Error_DirectoryCreate {
            get {
                return ResourceManager.GetString("Error_DirectoryCreate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error: Failure writing data to sink!.
        /// </summary>
        public static string Error_FailureWritingDataToSink {
            get {
                return ResourceManager.GetString("Error_FailureWritingDataToSink", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error: data NOT sent to sink, as file already existed and behavior was set to raise error if file already existed..
        /// </summary>
        public static string Error_FileAlreadyExistedSoDataNotSentToSink {
            get {
                return ResourceManager.GetString("Error_FileAlreadyExistedSoDataNotSentToSink", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error: param not of required type: .
        /// </summary>
        public static string Error_ParamTypeWrong {
            get {
                return ResourceManager.GetString("Error_ParamTypeWrong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error: param was null: .
        /// </summary>
        public static string Error_ParamWasNull {
            get {
                return ResourceManager.GetString("Error_ParamWasNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error: param was unreadable: .
        /// </summary>
        public static string Error_ParamWasUnreadable {
            get {
                return ResourceManager.GetString("Error_ParamWasUnreadable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Warning: sink file already existed and was deleted, as behavior was set to delete file if existed..
        /// </summary>
        public static string Warn_FileAlreadyExistedAndWasDeleted {
            get {
                return ResourceManager.GetString("Warn_FileAlreadyExistedAndWasDeleted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Warning: data NOT sent to sink, as file already existed and behavior was set to do nothing if file already existed..
        /// </summary>
        public static string Warn_FileAlreadyExistedSoDataNotSentToSink {
            get {
                return ResourceManager.GetString("Warn_FileAlreadyExistedSoDataNotSentToSink", resourceCulture);
            }
        }
    }
}
