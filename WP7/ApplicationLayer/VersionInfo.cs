using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;

namespace MonkeySpace
{
    internal class VersionInfo
    {
        static public System.Version AssemblyVersion
        {
            get
            {
                //return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                string name = Assembly.GetExecutingAssembly().FullName;
                AssemblyName asmName = new AssemblyName(name);
                Version version = asmName.Version;
                return version;// String.Format("{0}.{1}", version.Major, version.Minor);
            }
        }
        static public string AssemblyVersionString
        {
            get
            {
                //return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                string name = Assembly.GetExecutingAssembly().FullName;
                AssemblyName asmName = new AssemblyName(name);
                Version version = asmName.Version;
                return String.Format("{0}.{1}", version.Major, version.Minor);
            }
        }
        // etc. for other fields 
    } 


}
