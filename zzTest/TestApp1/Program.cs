
//using Microsoft.Expression.Framework.UserInterface;
//using Shopdrawing.App;
//using System;
//using System.Diagnostics;
//using System.Globalization;
//using System.Reflection;
//using System.Runtime.Remoting.Channels;
//using System.Runtime.Remoting.Channels.Ipc;

//namespace Shopdrawing
//{
//    internal static class Application
//    {
//        [LoaderOptimization(LoaderOptimization.MultiDomain)]
//        [STAThread]
//        private static void Main(string[] args)
//        {
//            int num = 159;
//            Process[] processes = Process.GetProcesses();
//            Process currentProcess = Process.GetCurrentProcess();
//            string[] commandLineArgs = Environment.GetCommandLineArgs();
//            ChannelServices.RegisterChannel(new IpcClientChannel(), false);
//            Process[] processArray = processes;
//            int num1 = 0;
//            while (true)
//            {
//                if (num1 < (int)processArray.Length)
//                {
//                    Process process = processArray[num1];
//                    if (process.ProcessName.StartsWith("Blend", StringComparison.OrdinalIgnoreCase) && process.Id != currentProcess.Id && process.StartInfo.UserName == currentProcess.StartInfo.UserName)
//                    {
//                        try
//                        {
//                            CultureInfo invariantCulture = CultureInfo.InvariantCulture;
//                            object[] str = new object[2];
//                            int id = process.Id;
//                            str[0] = id.ToString(CultureInfo.InvariantCulture);
//                            str[1] = typeof(IBlendServer).Name;
//                            string str1 = string.Format(invariantCulture, "ipc://BlendLocalPipe{0}/{1}", str);
//                            IBlendServer obj = (IBlendServer)Activator.GetObject(typeof(IBlendServer), str1);
//                            if (obj.CanProcessCommandLineArgs(commandLineArgs))
//                            {
//                                obj.ProcessCommandLineArgs(commandLineArgs);
//                                break;
//                            }
//                        }
//                        catch
//                        {
//                        }
//                    }
//                    num1++;
//                }
//                else
//                {
//                    SplashScreen splashScreen = new SplashScreen(typeof(Application).Module, num);
//                    splashScreen.Show();
//                    Shopdrawing.App.BlendApplication blendApplication = new Shopdrawing.App.BlendApplication(splashScreen);
//                    splashScreen.Close();
//                    break;
//                }
//            }
//        }
//    }
//}