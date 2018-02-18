namespace T4Template.Utilities.TemplateProcess
{
    using Microsoft.VisualStudio.TextTemplating;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    ///The text template transformation engine is responsible for running   
    ///the transformation process.  
    ///The host is responsible for all input and output, locating files,   
    ///and anything else related to the external environment.  
    /// </summary>
    public class CustomTextTemplatingEngineHost : ITextTemplatingEngineHost, ITextTemplatingSessionHost
    {
        private const string APP_DOMAIN_NAME = "Custom T4 Transform Domain";

        private string _templateFilePathValue;
        /// <summary>
        ///The path and file name of the text template that is being processed  
        /// </summary>
        public string TemplateFile
        {
            get { return _templateFilePathValue; }
            set { _templateFilePathValue = value; }
        }

        private string _fileExtensionValue = ".txt";
        /// <summary>
        ///The extension of the generated text output file.  
        ///The host can provide a default by setting the value of the field here.  
        ///The engine can change this value based on the optional output directive  
        ///if the user specifies it in the text template.
        /// </summary>
        public string FileExtension
        {
            get { return _fileExtensionValue; }
            set { _fileExtensionValue = value; }
        }

        private Encoding fileEncodingValue = Encoding.UTF8;
        /// <summary>
        ///The encoding of the generated text output file.  
        ///The host can provide a default by setting the value of the field here.  
        ///The engine can change this value based on the optional output directive  
        ///if the user specifies it in the text template.  
        /// </summary>
        public Encoding FileEncoding
        {
            get { return fileEncodingValue; }
        }

        private CompilerErrorCollection _errorsValue;
        /// <summary>
        ///The errors that occur when the engine processes a template.  
        ///The engine passes the errors to the host when it is done processing,  
        ///and the host can decide how to display them. For example, the host   
        ///can display the errors in the UI or write them to a file.  
        /// </summary>
        public CompilerErrorCollection Errors
        {
            get { return _errorsValue; }
        }

        private ITextTemplatingSession _sessionValue;
        public ITextTemplatingSession Session
        {
            get
            {
                return _sessionValue;
            }
            set
            {
                _sessionValue = value;
            }
        }

        /// <summary>
        ///The host can provide standard assembly references.  
        ///The engine will use these references when compiling and  
        ///executing the generated transformation class.  
        /// </summary>
        public IList<string> StandardAssemblyReferences
        {
            get
            {
                return new string[]
                {
                    typeof(System.Uri).Assembly.Location
                };
            }
        }

        /// <summary>
        ///The host can provide standard imports or using statements.  
        ///The engine will add these statements to the generated   
        ///transformation class.  
        /// </summary>
        public IList<string> StandardImports
        {
            get
            {
                return new string[]
                {
                    "System"
                };
            }
        }

        private bool _useCurrentAppDomain;

        public CustomTextTemplatingEngineHost(bool useCurrentAppDomain = false)
        {
            this._useCurrentAppDomain = useCurrentAppDomain;
        }

        /// <summary>
        ///The included text is returned in the context parameter.  
        ///If the host searches the registry for the location of include files,  
        ///or if the host searches multiple locations by default, the host can  
        ///return the final path of the include file in the location parameter.  
        /// </summary>
        /// <param name="requestFileName"></param>
        /// <param name="content"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool LoadIncludeText(string requestFileName, out string content, out string location)
        {
            content = System.String.Empty;
            location = System.String.Empty;

            if (File.Exists(requestFileName))
            {
                content = File.ReadAllText(requestFileName);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///Called by the Engine to enquire about   
        ///the processing options you require.   
        ///If you recognize that option, return an   
        ///appropriate value.   
        ///Otherwise, pass back NULL.  
        /// </summary>
        /// <param name="optionName"></param>
        /// <returns></returns>
        public object GetHostOption(string optionName)
        {
            object returnObject;
            switch (optionName)
            {
                case "CacheAssemblies":
                    returnObject = true;
                    break;
                default:
                    returnObject = null;
                    break;
            }
            return returnObject;
        }

        /// <summary>
        ///The engine calls this method to resolve assembly references used in  
        ///the generated transformation class project and for the optional   
        ///assembly directive if the user has specified it in the text template.  
        ///This method can be called 0, 1, or more times.  
        /// </summary>
        /// <param name="assemblyReference"></param>
        /// <returns></returns>
        public string ResolveAssemblyReference(string assemblyReference)
        {
            if (File.Exists(assemblyReference))
            {
                return assemblyReference;
            }

            string candidate = Path.Combine(Path.GetDirectoryName(this.TemplateFile), assemblyReference);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            return null;
        }

        /// <summary>
        ///The engine calls this method based on the directives the user has   
        ///specified in the text template.  
        ///This method can be called 0, 1, or more times.  
        /// </summary>
        /// <param name="processorName"></param>
        /// <returns></returns>
        public Type ResolveDirectiveProcessor(string processorName)
        {
            if (string.Compare(processorName, "XYZ", StringComparison.OrdinalIgnoreCase) == 0)
            {
                //return typeof();  
            }

            throw new Exception("Directive Processor not found");
        }

        /// <summary>
        ///A directive processor can call this method if a file name does not   
        ///have a path.  
        ///The host can attempt to provide path information by searching   
        ///specific paths for the file and returning the file and path if found.  
        ///This method can be called 0, 1, or more times.  
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string ResolvePath(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("the file name cannot be null");
            }

            if (File.Exists(fileName))
            {
                return fileName;
            }

            string candidate = Path.Combine(Path.GetDirectoryName(this.TemplateFile), fileName);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            return null;
        }

        /// <summary>
        ///If a call to a directive in a text template does not provide a value  
        ///for a required parameter, the directive processor can try to get it  
        ///from the host by calling this method.  
        ///This method can be called 0, 1, or more times.  
        /// </summary>
        /// <param name="directiveId"></param>
        /// <param name="processorName"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public string ResolveParameterValue(string directiveId, string processorName, string parameterName)
        {
            if (directiveId == null)
            {
                throw new ArgumentNullException("the directiveId cannot be null");
            }
            if (processorName == null)
            {
                throw new ArgumentNullException("the processorName cannot be null");
            }
            if (parameterName == null)
            {
                throw new ArgumentNullException("the parameterName cannot be null");
            }

            return String.Empty;
        }

        /// <summary>
        ///The engine calls this method to change the extension of the   
        ///generated text output file based on the optional output directive   
        ///if the user specifies it in the text template.  
        /// </summary>
        /// <param name="extension"></param>
        public void SetFileExtension(string extension)
        {
            //The parameter extension has a '.' in front of it already.  
            _fileExtensionValue = extension;
        }

        /// <summary>
        ///The engine calls this method to change the encoding of the   
        ///generated text output file based on the optional output directive   
        ///if the user specifies it in the text template.  
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="fromOutputDirective"></param>
        public void SetOutputEncoding(System.Text.Encoding encoding, bool fromOutputDirective)
        {
            fileEncodingValue = encoding;
        }

        /// <summary>
        ///The engine calls this method when it is done processing a text  
        ///template to pass any errors that occurred to the host.  
        ///The host can decide how to display them.  
        /// </summary>
        /// <param name="errors"></param>
        public void LogErrors(CompilerErrorCollection errors)
        {
            _errorsValue = errors;
        }

        /// <summary>
        ///This is the application domain that is used to compile and run  
        ///the generated transformation class to create the generated text output.  
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public AppDomain ProvideTemplatingAppDomain(string content)
        {
            //This host will provide a new application domain each time the   
            if (_useCurrentAppDomain)
            {
                return AppDomain.CurrentDomain;
            }
            else
            {
                return AppDomain.CreateDomain(APP_DOMAIN_NAME);
            }
            //This could be changed to return the current appdomain, but new   
            //assemblies are loaded into this AppDomain on a regular basis.  
            //If the AppDomain lasts too long, it will grow indefintely,   
            //which might be regarded as a leak.  
            //This could be customized to cache the application domain for   
            //a certain number of text template generations (for example, 10).  
            //This could be customized based on the contents of the text   
            //template, which are provided as a parameter for that purpose.  
        }

        public ITextTemplatingSession CreateSession()
        {
            return new TextTemplatingSession();
        }
    }
}
