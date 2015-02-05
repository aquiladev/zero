using System;
using System.Reflection;
using System.Runtime.InteropServices;

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

[assembly: AssemblyProduct("Zero")]
[assembly: AssemblyCompany("Sergii Bomko")]
[assembly: AssemblyCopyright("Copyright © Sergii Bomko 2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Revision
//      Build
//
// You can specify all the values or you can default the Revision and Build Numbers by using the '*' as shown below:
[assembly: AssemblyVersion("1.0.0.*")]
