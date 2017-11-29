using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Threading;
using NostreetsExtensions.Utilities;
using NostreetsExtensions;
using System.Reflection;
using System.Linq.Expressions;

namespace Nostreets_Services.Helpers
{
    public class NostreetsWinService<T> : ServiceBase, IDisposable
    {
        public ServiceController ServiceController { get { return _serviceController; } }
        public string TargetedDirectory { get; set; }
        protected IEnumerable<object> Params { get; set; }

        protected FileManager _fileManager = null;
        protected Thread _thread = null;
        private ServiceController _serviceController = null;
        private bool _updateComplete = false;
        private string[] _args = null;
        private ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private Tuple<
                        Action<ManualResetEvent>,
                        Func<ManualResetEvent, object>,
                        Func<ManualResetEvent, IEnumerable<object>>
                        >
            _method = null;

        public NostreetsWinService(string serviceName, string directoryPath, Action<ManualResetEvent> method)
        {
            //if (!serviceName.IsWindowsServiceInstalled())
            //{
            //    string cmd = "SC CREATE \"" + serviceName + "\" binpath= \"" + directoryPath + "\"";
            //    cmd.RunConsoleCommand(System.Diagnostics.ProcessWindowStyle.Maximized);
            //}

            _serviceController = new ServiceController(serviceName, Environment.MachineName);
            _method = new Tuple<
                        Action<ManualResetEvent>,
                        Func<ManualResetEvent, object>,
                        Func<ManualResetEvent, IEnumerable<object>>
                        >
                (method, null, null);

            TargetedDirectory = directoryPath;

        }

        public NostreetsWinService(string serviceName, string directoryPath, object parameter, Action<ManualResetEvent> method)
        {
            //if (!serviceName.IsWindowsServiceInstalled())
            //{
            //    string cmd = "SC CREATE \"" + serviceName + "\" binpath= \"" + directoryPath + "\"";
            //    cmd.RunConsoleCommand(System.Diagnostics.ProcessWindowStyle.Maximized);
            //}

            _serviceController = new ServiceController(serviceName, Environment.MachineName);
            _method = new Tuple<
                        Action<ManualResetEvent>,
                        Func<ManualResetEvent, object>,
                        Func<ManualResetEvent, IEnumerable<object>>
                        >
                (method, null, null);
            Params = new[] { parameter };
            TargetedDirectory = directoryPath;

        }

        public NostreetsWinService(string serviceName, string directoryPath, IEnumerable<object> parameters, Action<ManualResetEvent> method)
        {

            //if (!serviceName.IsWindowsServiceInstalled())
            //{
            //    string cmd = "SC CREATE \"" + serviceName + "\" binpath= \"" + directoryPath + "\"";
            //    cmd.RunConsoleCommand(System.Diagnostics.ProcessWindowStyle.Maximized);
            //}

            _serviceController = new ServiceController(serviceName, Environment.MachineName);
            _method = new Tuple<
                        Action<ManualResetEvent>,
                        Func<ManualResetEvent, object>,
                        Func<ManualResetEvent, IEnumerable<object>>
                        >
                (method, null, null);
            TargetedDirectory = directoryPath;
            Params = parameters;
        }

        public NostreetsWinService(string serviceName, string directoryPath, Func<ManualResetEvent, object> method)
        {

            //if (!serviceName.IsWindowsServiceInstalled())
            //{
            //    string cmd = "SC CREATE \"" + serviceName + "\" binpath= \"" + directoryPath + "\"";
            //    cmd.RunConsoleCommand(System.Diagnostics.ProcessWindowStyle.Maximized);
            //}

            _serviceController = new ServiceController(serviceName, Environment.MachineName);
            _method = new Tuple<
                        Action<ManualResetEvent>,
                        Func<ManualResetEvent, object>,
                        Func<ManualResetEvent, IEnumerable<object>>
                        >
                (null, method, null);
            TargetedDirectory = directoryPath;

        }

        public NostreetsWinService(string serviceName, string directoryPath, object parameter, Func<ManualResetEvent, object> method)
        {

            //if (!serviceName.IsWindowsServiceInstalled())
            //{
            //    string cmd = "SC CREATE \"" + serviceName + "\" binpath= \"" + directoryPath + "\"";
            //    cmd.RunConsoleCommand(System.Diagnostics.ProcessWindowStyle.Maximized);
            //}

            _serviceController = new ServiceController(serviceName, Environment.MachineName);
            _method = new Tuple<
                        Action<ManualResetEvent>,
                        Func<ManualResetEvent, object>,
                        Func<ManualResetEvent, IEnumerable<object>>
                        >
                        (null, method, null);

            TargetedDirectory = directoryPath;
            Params = new[] { parameter };
        }

        public NostreetsWinService(string serviceName, string directoryPath, IEnumerable<object> parameters, Func<ManualResetEvent, object> method)
        {
            //if (!serviceName.IsWindowsServiceInstalled())
            //{
            //    string cmd = "SC CREATE \"" + serviceName + "\" binpath= \"" + directoryPath + "\"";
            //    cmd.RunConsoleCommand(System.Diagnostics.ProcessWindowStyle.Maximized);
            //}

            _serviceController = new ServiceController(serviceName, Environment.MachineName);
            _method = new Tuple<
                        Action<ManualResetEvent>,
                        Func<ManualResetEvent, object>,
                        Func<ManualResetEvent, IEnumerable<object>>
                        >
                (null, method, null);
            TargetedDirectory = directoryPath;
            Params = parameters;
        }

        public NostreetsWinService(string serviceName, string directoryPath, Func<ManualResetEvent, IEnumerable<object>> method)
        {
            //if (!serviceName.IsWindowsServiceInstalled())
            //{
            //    string cmd = "SC CREATE \"" + serviceName + "\" binpath= \"" + directoryPath + "\"";
            //    cmd.RunConsoleCommand(System.Diagnostics.ProcessWindowStyle.Maximized);
            //}

            _serviceController = new ServiceController(serviceName, Environment.MachineName);
            _method = new Tuple<
                        Action<ManualResetEvent>,
                        Func<ManualResetEvent, object>,
                        Func<ManualResetEvent, IEnumerable<object>>
                        >
                (null, null, method);
            TargetedDirectory = directoryPath;

        }

        public NostreetsWinService(string serviceName, string directoryPath, object parameter, Func<ManualResetEvent, IEnumerable<object>> method)
        {
            //if (!serviceName.IsWindowsServiceInstalled())
            //{
            //    string cmd = "SC CREATE \"" + serviceName + "\" binpath= \"" + directoryPath + "\"";
            //    cmd.RunConsoleCommand(System.Diagnostics.ProcessWindowStyle.Maximized);
            //}

            _serviceController = new ServiceController(serviceName, Environment.MachineName);
            _method = new Tuple<
                        Action<ManualResetEvent>,
                        Func<ManualResetEvent, object>,
                        Func<ManualResetEvent, IEnumerable<object>>
                        >
                        (null, null, method);

            TargetedDirectory = directoryPath;
            Params = new[] { parameter };
        }

        public NostreetsWinService(string serviceName, string directoryPath, IEnumerable<object> parameters, Func<ManualResetEvent, IEnumerable<object>> method)
        {
            //if (!serviceName.IsWindowsServiceInstalled())
            //{
            //    string cmd = "SC CREATE \"" + serviceName + "\" binpath= \"" + directoryPath + "\"";
            //    cmd.RunConsoleCommand(System.Diagnostics.ProcessWindowStyle.Maximized);
            //}

            _serviceController = new ServiceController(serviceName, Environment.MachineName);
            _method = new Tuple<
                        Action<ManualResetEvent>,
                        Func<ManualResetEvent, object>,
                        Func<ManualResetEvent, IEnumerable<object>>
                        >
                (null, null, method);
            TargetedDirectory = directoryPath;
            Params = parameters;
        }

        protected override void OnStart(string[] args)
        {

            _thread = new Thread(() => Process());
            _thread.IsBackground = true;
            _thread.Name = "Worker_Bee";

            _args = args;

            if (args.Length > 0)
            {
                _fileManager = new FileManager(args[0]);
                _fileManager.CreateFile("Worker_Bee_LOG_" + DateTime.Now.Timestamp() + ".txt");
            }


            _thread.Start();

            base.OnStart(args);
        }

        protected override void OnStop()
        {
            _shutdownEvent.Set();
            if (!_thread.Join(15000))
            {
                _thread.Abort();
            }
            base.OnStop();
        }

        protected void Process()
        {
            try
            {

                PropertyInfo[] propsInfo = _method.GetType().GetProperties();
                object func = null,
                       result = null;


                foreach (PropertyInfo prop in propsInfo)
                {
                    func = prop.GetValue(_method);
                    if (func != null) { break; }
                }


                if (func.GetType() == typeof(Action<ManualResetEvent>) && Params.Count() == 0)
                {
                    ((Action<ManualResetEvent>)func).Invoke(_shutdownEvent);
                }
                else if (func.GetType() == typeof(Action<ManualResetEvent>) && Params.Count() == 1)
                {

                    ((Action<ManualResetEvent, object>)func).Invoke(_shutdownEvent, Params.GetEnumerator().Current);
                }
                else if (func.GetType() == typeof(Action<ManualResetEvent>) && Params.Count() > 1)
                {

                    ((Action<ManualResetEvent, IEnumerable<object>>)func).Invoke(_shutdownEvent, Params);
                }

                else if (func.GetType() == typeof(Func<ManualResetEvent, IEnumerable<object>>) && Params.Count() == 0)
                {
                    result = ((Func<ManualResetEvent, IEnumerable<object>>)func).Invoke(_shutdownEvent);
                }
                else if (func.GetType() == typeof(Func<ManualResetEvent, IEnumerable<object>>) && Params.Count() == 1)
                {

                    result = ((Func<ManualResetEvent, object, IEnumerable<object>>)func).Invoke(_shutdownEvent, Params.GetEnumerator().Current);
                }
                else if (func.GetType() == typeof(Func<ManualResetEvent, IEnumerable<object>>) && Params.Count() > 1)
                {

                    result = ((Func<ManualResetEvent, IEnumerable<object>, IEnumerable<object>>)func).Invoke(_shutdownEvent, Params);
                }
                else if (func.GetType() == typeof(Func<ManualResetEvent, object>) && Params.Count() == 0)
                {
                    result = ((Func<ManualResetEvent, object>)func).Invoke(_shutdownEvent);
                }
                else if (func.GetType() == typeof(Func<ManualResetEvent, object>) && Params.Count() == 1)
                {

                    result = ((Func<ManualResetEvent, object, object>)func).Invoke(_shutdownEvent, Params.GetEnumerator().Current);
                }
                else if (func.GetType() == typeof(Func<ManualResetEvent, object>) && Params.Count() > 1)
                {

                    result = ((Func<ManualResetEvent, IEnumerable<object>, object>)func).Invoke(_shutdownEvent, Params);
                }



            }
            catch (Exception ex)
            {

                _shutdownEvent.Set();
                if (!_thread.Join(15000))
                {
                    _thread.Abort();
                }
            }
        }

    }
}
