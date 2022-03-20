using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using joystick_feedback.Audio;
using log4net;
using SharpDX.DirectInput;

// ReSharper disable StringLiteralTypo

namespace joystick_feedback
{


    public partial class App : Application
    {
        public static bool IsShuttingDown { get; set; }

        private static Task _joystickTask;
        private static CancellationTokenSource _joystickTokenSource = new CancellationTokenSource();

        private static Mutex _mutex;

        private TaskbarIcon _notifyIcon;
        
        public static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        // Initialize DirectInput
        private static readonly DirectInput DirectInput = new DirectInput();

        private static Joystick _joystick;

        private static string _pid;
        private static string _vid;
        private static int _deadband;

        public static string ExePath;

        private static CachedSound _inYDeadzoneSound = null;
        private static CachedSound _outYDeadzoneSound = null;

        public static void PlayInYDeadzoneSound()
        {
            if (_inYDeadzoneSound != null)
            {
                try
                {
                    AudioPlaybackEngine.Instance.PlaySound(_inYDeadzoneSound);
                }
                catch (Exception ex)
                {
                    Log.Error( $"PlayInYDeadzoneSound: {ex}");
                }
            }
        }

        public static void PlayOutYDeadzoneSound()
        {
            if (_outYDeadzoneSound != null)
            {
                try
                {
                    AudioPlaybackEngine.Instance.PlaySound(_outYDeadzoneSound);
                }
                catch (Exception ex)
                {
                    Log.Error($"PlayOutYDeadzoneSound: {ex}");
                }
            }
        }

        private static void GetExePath()
        {
            var strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            ExePath = Path.GetDirectoryName(strExeFilePath);
        }



        private static void RunProcess(string fileName)
        {
            var process = new Process();
            // Configure the process using the StartInfo properties.
            process.StartInfo.FileName = fileName;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();
        }

        private static void MigrateSettings()
        {
            try
            {
                if (!joystick_feedback.Properties.Settings.Default.Upgraded)
                {
                    joystick_feedback.Properties.Settings.Default.Upgrade();
                    joystick_feedback.Properties.Settings.Default.Upgraded = true;
                    joystick_feedback.Properties.Settings.Default.Save();
                }
            }
            catch
            {
                // ignored
            }
        }


        void AppDomainUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs ea)
        {
            Exception ex = (Exception)ea.ExceptionObject;

            Log.Error($"AppDomainUnhandledExceptionHandler: {ex}");
        }

        void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Log.Error($"Application_ThreadException: {e.Exception}");

        }

        void AppDispatcherUnhandledException(object
            sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error($"AppDispatcherUnhandledException: {e.Exception}");
        }

        protected override void OnStartup(StartupEventArgs evtArgs)
        {

            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledExceptionHandler;

            System.Windows.Forms.Application.ThreadException += Application_ThreadException;

            Application.Current.DispatcherUnhandledException += AppDispatcherUnhandledException;

            const string appName = "joystick_feedback";

            _mutex = new Mutex(true, appName, out var createdNew);

            if (!createdNew)
            {
                //app is already running! Exiting the application  
                Current.Shutdown();
            }

            GetExePath();

            base.OnStartup(evtArgs);

            log4net.Config.XmlConfigurator.Configure();

            MigrateSettings();
            
            _inYDeadzoneSound = null;
            _outYDeadzoneSound = null;

            if (File.Exists(Path.Combine(ExePath, "appSettings.config")) &&
                ConfigurationManager.GetSection("appSettings") is NameValueCollection appSection)
            {
                if (File.Exists(Path.Combine(ExePath, "Sounds", appSection["InYDeadzoneSound"])))
                {
                    try
                    {
                        _inYDeadzoneSound = new CachedSound(Path.Combine(ExePath, "Sounds", appSection["InYDeadzoneSound"]));
                    }
                    catch (Exception ex)
                    {
                        _inYDeadzoneSound = null;

                        Log.Error($"CachedSound: {ex}");
                    }

                }

                if (File.Exists(Path.Combine(ExePath, "Sounds", appSection["OutYDeadzoneSound"])))
                {
                    try
                    {
                        _outYDeadzoneSound = new CachedSound(Path.Combine(ExePath, "Sounds", appSection["OutYDeadzoneSound"]));
                    }
                    catch (Exception ex)
                    {
                        _outYDeadzoneSound = null;

                        Log.Error($"CachedSound: {ex}");
                    }

                }
            }

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            _notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");

            _notifyIcon.IconSource = new BitmapImage(new Uri("pack://application:,,,/joystick-feedback;component/joystick_22830.ico"));
            _notifyIcon.ToolTipText = "Joystick Feedback";

            var splashScreen = new SplashScreenWindow();
            splashScreen.Show();

            Task.Run(() =>
            {

                if (File.Exists(Path.Combine(ExePath, "joystickSettings.config")) && ConfigurationManager.GetSection("joystickSettings") is NameValueCollection joystickSection)
                {
                    _pid = joystickSection["PID"];
                    _vid = joystickSection["VID"];
                    
                    _deadband = Convert.ToInt32(joystickSection["Deadband"]);

                    if (!string.IsNullOrEmpty(_pid) && !string.IsNullOrEmpty(_vid))
                    {
                        splashScreen.Dispatcher.Invoke(() => splashScreen.ProgressText.Text = "Looking for Joystick...");

                        Log.Info($"Looking for directinput devices with PID={_pid} and VID={_vid}");


                        foreach (var deviceInstance in DirectInput.GetDevices())
                        {

                            //Device = 17,
                            //Mouse = 18,
                            //Keyboard = 19,

                            //Joystick = 20,
                            //Gamepad = 21,
                            //Driving = 22,
                            //Flight = 23,
                            //FirstPerson = 24,

                            //ControlDevice = 25,
                            //ScreenPointer = 26,
                            //Remote = 27,
                            //Supplemental = 28

                            if (deviceInstance.Type >= DeviceType.Joystick &&
                                deviceInstance.Type <= DeviceType.FirstPerson)
                            {
                                Log.Info("PID:" + deviceInstance.ProductGuid.ToString().Substring(0, 4) + " - VID:" +
                                         deviceInstance.ProductGuid.ToString().Substring(4, 4) + " - " +
                                         deviceInstance.Type.ToString().PadRight(11) + " - " +
                                         deviceInstance.ProductGuid + " - " + deviceInstance.InstanceGuid + " - " +
                                         deviceInstance.InstanceName.Trim().Replace("\0", ""));

                                if (_joystick == null &&
                                    deviceInstance.ProductGuid.ToString().ToUpper().StartsWith(_pid.ToUpper() + _vid.ToUpper()))
                                {
                                    Log.Info(
                                        $"Using Joystick {deviceInstance.InstanceName.Trim().Replace("\0", "")} with Instance Guid {deviceInstance.InstanceGuid}");

                                    _joystick = new Joystick(DirectInput, deviceInstance.InstanceGuid);

                                    //joystick.Properties.BufferSize = 128;

                                    /*
                                    joystick.SetCooperativeLevel(wih,
                                      CooperativeLevel.Background | CooperativeLevel.NonExclusive);*/

                                    _joystick.Acquire();

                                    var joystickToken = _joystickTokenSource.Token;

                                    var lastY = -1;

                                    _joystickTask = Task.Run(async () =>
                                    {
                                        Log.Info("joystick task started");

                                        while (true)
                                        {
                                            if (joystickToken.IsCancellationRequested)
                                            {
                                                joystickToken.ThrowIfCancellationRequested();
                                            }

                                            _joystick.Poll();

                                            var state = _joystick.GetCurrentState();

                                            // y axis of vkb joystick with 5% deadband configured in the joystick  : 0 - 30750 32767  34370 - 65535
                                            
                                            if  (!lastY.IsBetweenII(32767-_deadband,32767+_deadband) && state.Y.IsBetweenEE(32767-_deadband,32767+_deadband))
                                            {
                                                //Log.Info($"{state.Y}");
                                                PlayInYDeadzoneSound();
                                            }
                                            else if (lastY.IsBetweenII(32767 - _deadband, 32767 + _deadband) && !state.Y.IsBetweenEE(32767 - _deadband, 32767 + _deadband))
                                            
                                            {
                                                //Log.Info($"{state.Y}");
                                                PlayOutYDeadzoneSound();
                                            }

                                            lastY = state.Y;

                                            await Task.Delay(50, _joystickTokenSource.Token);
                                        }

                                    }, joystickToken);
                                }
                            }
                        }

                        if (_joystick == null)
                        {
                            Log.Info($"No joystick found with PID={_pid} and VID={_vid}");
                        }
                    }
                }

                Dispatcher.Invoke(() =>
                {
                    var window = Current.MainWindow = new MainWindow();
                    window.ShowActivated = false;
                    var im = (System.Windows.Controls.Image)window.FindName("im");
                    /*if (im != null && WindowWidth > 0 && WindowHeight > 0)
                    {
                        im.Width = WindowWidth;
                        im.Height = WindowHeight;
                    }*/
                });


                Log.Info("joystick_feedback started");

                Dispatcher.Invoke(() =>
                {
                    var window = Current.MainWindow;

                    if (window != null && joystick_feedback.Properties.Settings.Default.Visible)
                    {
                        window.Show();
                        //FipHandler.RefreshDevicePages();
                    }
                    else
                        window.Hide();
                });

                Dispatcher.Invoke(() => { splashScreen.Close(); });
                
            });

        }
      

        protected override void OnExit(ExitEventArgs e)
        {
            joystick_feedback.Properties.Settings.Default.Save();

            _notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner

            if (_joystick != null)
            {
                _joystickTokenSource.Cancel();

                var joystickToken = _joystickTokenSource.Token;

                try
                {
                    _joystickTask?.Wait(joystickToken);
                }
                catch (OperationCanceledException)
                {
                    Log.Info("joystick background task ended");
                }
                finally
                {
                    _joystickTokenSource.Dispose();

                    _joystick?.Unacquire();
                    _joystick?.Dispose();
                }
            }

            Log.Info("exiting");

            base.OnExit(e);
        }
    }
}
