using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using joystick_feedback.Audio;
using log4net;
using Newtonsoft.Json;
using SharpDX.DirectInput;

// ReSharper disable StringLiteralTypo

namespace joystick_feedback
{
    public class ButtonData
    {
        public string SoundFile { get; set; }
        //public List<string> Actions { get; set; }
        [JsonIgnore]
        public CachedSound Sound { get; set; }

    }

    public class AxisData
    {
        public int Deadband { get; set; }
        public string InDeadzoneSoundFile { get; set; }
        public string OutDeadzoneSoundFile { get; set; }

        [JsonIgnore]
        public CachedSound InDeadzoneSound { get; set; }
        [JsonIgnore]
        public CachedSound OutDeadzoneSound { get; set; }

    }
    public class JoystickData
    {
        public string PID { get; set; }
        public string VID { get; set; }

        public AxisData X { get; set; }
        public AxisData Y { get; set; }
        public AxisData Z { get; set; }

        public Dictionary<int, ButtonData> Buttons { get; set; }

        [JsonIgnore]
        public Joystick Joystick { get; set; }
        [JsonIgnore]
        public Task JoystickTask { get; set; }
        [JsonIgnore]
        public CancellationTokenSource JoystickTokenSource = new CancellationTokenSource();
        [JsonIgnore]
        public int LastX = -1;
        [JsonIgnore]
        public int LastY = -1;
        [JsonIgnore]
        public int LastZ = -1;
        [JsonIgnore]
        public bool[] LastButtonState = new bool[256];

    }

    public partial class App : Application
    {
        public static bool IsShuttingDown { get; set; }

        private static Mutex _mutex;

        private TaskbarIcon _notifyIcon;

        private static FifoExecution ButtonJob = new FifoExecution(); // language is changed for thread

        private static List<JoystickData> JoystickList = new List<JoystickData>();

        public static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Initialize DirectInput
        private static readonly DirectInput DirectInput = new DirectInput();

        public static string ExePath;

        public static void PlaySound(CachedSound deadzoneSound)
        {
            if (deadzoneSound != null)
            {
                try
                {
                    AudioPlaybackEngine.Instance.PlaySound(deadzoneSound);
                }
                catch (Exception ex)
                {
                    Log.Error( $"PlayDeadzoneSound: {ex}");
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

        private static void ButtonCallback(Object threadContext)
        {
            var param = (KeyValuePair<int, ButtonData>)threadContext;

            PlaySound(param.Value.Sound);
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

            var buttonPath = Path.Combine(ExePath, "Data\\Joysticks\\");

            var fileEntries = Directory.GetFiles(buttonPath);
            foreach (var fileName in fileEntries)
            {
                JoystickList.Add(JsonConvert
                    .DeserializeObject<JoystickData>(File.ReadAllText(fileName)));
            }

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            _notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");

            _notifyIcon.IconSource = new BitmapImage(new Uri("pack://application:,,,/joystick-feedback;component/joystick_22830.ico"));
            _notifyIcon.ToolTipText = "Joystick Feedback";

            var splashScreen = new SplashScreenWindow();
            splashScreen.Show();

            Task.Run(() =>
            {

                splashScreen.Dispatcher.Invoke(() =>
                    splashScreen.ProgressText.Text = "Looking for Joystick...");

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

                        var joystick = JoystickList.FirstOrDefault(x => x.Joystick == null && deviceInstance.ProductGuid.ToString().ToUpper().StartsWith(x.PID.ToUpper() + x.VID.ToUpper()));

                        if (joystick != null)
                        {
                            Log.Info(
                                $"Using Joystick {deviceInstance.InstanceName.Trim().Replace("\0", "")} with Instance Guid {deviceInstance.InstanceGuid}");

                            if (!string.IsNullOrEmpty(joystick.Y?.InDeadzoneSoundFile) &&
                                File.Exists(Path.Combine(ExePath, "Sounds", joystick.Y.InDeadzoneSoundFile)))
                            {
                                try
                                {
                                    joystick.Y.InDeadzoneSound =
                                        new CachedSound(Path.Combine(ExePath, "Sounds",
                                            joystick.Y.InDeadzoneSoundFile));
                                }
                                catch (Exception ex)
                                {
                                    joystick.Y.InDeadzoneSound = null;

                                    Log.Error($"CachedSound: {ex}");
                                }
                            }

                            if (!string.IsNullOrEmpty(joystick.Y?.OutDeadzoneSoundFile) &&
                                File.Exists(Path.Combine(ExePath, "Sounds", joystick.Y.OutDeadzoneSoundFile)))
                            {
                                try
                                {
                                    joystick.Y.OutDeadzoneSound =
                                        new CachedSound(Path.Combine(ExePath, "Sounds",
                                            joystick.Y.OutDeadzoneSoundFile));
                                }
                                catch (Exception ex)
                                {
                                    joystick.Y.OutDeadzoneSound = null;

                                    Log.Error($"CachedSound: {ex}");
                                }
                            }

                            if (!string.IsNullOrEmpty(joystick.X?.InDeadzoneSoundFile) &&
                                File.Exists(Path.Combine(ExePath, "Sounds", joystick.X.InDeadzoneSoundFile)))
                            {
                                try
                                {
                                    joystick.X.InDeadzoneSound =
                                        new CachedSound(Path.Combine(ExePath, "Sounds",
                                            joystick.X.InDeadzoneSoundFile));
                                }
                                catch (Exception ex)
                                {
                                    joystick.X.InDeadzoneSound = null;

                                    Log.Error($"CachedSound: {ex}");
                                }
                            }

                            if (!string.IsNullOrEmpty(joystick.X?.OutDeadzoneSoundFile) &&
                                File.Exists(Path.Combine(ExePath, "Sounds", joystick.X.OutDeadzoneSoundFile)))
                            {
                                try
                                {
                                    joystick.X.OutDeadzoneSound =
                                        new CachedSound(Path.Combine(ExePath, "Sounds",
                                            joystick.X.OutDeadzoneSoundFile));
                                }
                                catch (Exception ex)
                                {
                                    joystick.X.OutDeadzoneSound = null;

                                    Log.Error($"CachedSound: {ex}");
                                }
                            }

                            if (!string.IsNullOrEmpty(joystick.Z?.InDeadzoneSoundFile) &&
                                File.Exists(Path.Combine(ExePath, "Sounds", joystick.Z.InDeadzoneSoundFile)))
                            {
                                try
                                {
                                    joystick.Z.InDeadzoneSound =
                                        new CachedSound(Path.Combine(ExePath, "Sounds",
                                            joystick.Z.InDeadzoneSoundFile));
                                }
                                catch (Exception ex)
                                {
                                    joystick.Z.InDeadzoneSound = null;

                                    Log.Error($"CachedSound: {ex}");
                                }
                            }

                            if (!string.IsNullOrEmpty(joystick.Z?.OutDeadzoneSoundFile) &&
                                File.Exists(Path.Combine(ExePath, "Sounds", joystick.Z.OutDeadzoneSoundFile)))
                            {
                                try
                                {
                                    joystick.Z.OutDeadzoneSound =
                                        new CachedSound(Path.Combine(ExePath, "Sounds",
                                            joystick.Z.OutDeadzoneSoundFile));
                                }
                                catch (Exception ex)
                                {
                                    joystick.Z.OutDeadzoneSound = null;

                                    Log.Error($"CachedSound: {ex}");
                                }
                            }

                            joystick.Joystick = new Joystick(DirectInput, deviceInstance.InstanceGuid);

                            //joystick.Properties.BufferSize = 128;

                            /*
                            joystick.SetCooperativeLevel(wih,
                              CooperativeLevel.Background | CooperativeLevel.NonExclusive);*/

                            joystick.Joystick.Acquire();

                            var joystickToken = joystick.JoystickTokenSource.Token;

                            joystick.JoystickTask = Task.Run(async () =>
                            {
                                Log.Info(
                                    $"joystick task started with PID={joystick.PID} and VID={joystick.VID}");

                                while (true)
                                {
                                    if (joystickToken.IsCancellationRequested)
                                    {
                                        joystickToken.ThrowIfCancellationRequested();
                                    }

                                    joystick.Joystick.Poll();

                                    var state = joystick.Joystick.GetCurrentState();

                                    if (joystick.Buttons?.Any() == true)
                                    {
                                        foreach (var button in joystick.Buttons)
                                        {
                                            if (button.Value.Sound == null && !string.IsNullOrEmpty(button.Value.SoundFile) &&
                                                File.Exists(Path.Combine(ExePath, "Sounds", button.Value.SoundFile)))
                                            {
                                                try
                                                {
                                                    button.Value.Sound =
                                                        new CachedSound(Path.Combine(ExePath, "Sounds",
                                                            button.Value.SoundFile));
                                                }
                                                catch (Exception ex)
                                                {
                                                    button.Value.Sound = null;
                                                    button.Value.SoundFile = null;

                                                    Log.Error($"CachedSound: {ex}");
                                                }
                                            }


                                            var buttonId = button.Key;

                                            if (buttonId > 0 && state.Buttons.Length >= buttonId)
                                            {
                                                var buttonState = state.Buttons[buttonId - 1];
                                                var oldButtonState = joystick.LastButtonState[buttonId - 1];

                                                if (buttonState && !oldButtonState)
                                                {
                                                    ButtonJob.QueueUserWorkItem(ButtonCallback, button);
                                                }

                                                joystick.LastButtonState[buttonId - 1] = buttonState;
                                            }
                                        }
                                    }

                                    // y axis of vkb joystick with 5% deadband configured in the joystick  : 0 - 30750 32767  34370 - 65535

                                    if (joystick.Y?.Deadband > 0)
                                    {
                                        if (!joystick.LastY.IsBetweenII(32767 - joystick.Y.Deadband,
                                                32767 + joystick.Y.Deadband) &&
                                            state.Y.IsBetweenEE(32767 - joystick.Y.Deadband,
                                                32767 + joystick.Y.Deadband))
                                        {
                                            //Log.Info($"{state.Y}");
                                            PlaySound(joystick.Y.InDeadzoneSound);
                                        }
                                        else if (joystick.LastY.IsBetweenII(32767 - joystick.Y.Deadband,
                                                     32767 + joystick.Y.Deadband) &&
                                                 !state.Y.IsBetweenEE(32767 - joystick.Y.Deadband,
                                                     32767 + joystick.Y.Deadband))

                                        {
                                            //Log.Info($"{state.Y}");
                                            PlaySound(joystick.Y.OutDeadzoneSound);
                                        }
                                    }

                                    if (joystick.X?.Deadband > 0)
                                    {
                                        if (!joystick.LastX.IsBetweenII(32767 - joystick.X.Deadband,
                                                32767 + joystick.X.Deadband) &&
                                            state.X.IsBetweenEE(32767 - joystick.X.Deadband,
                                                32767 + joystick.X.Deadband))
                                        {
                                            //Log.Info($"{state.X}");
                                            PlaySound(joystick.X.InDeadzoneSound);
                                        }
                                        else if (joystick.LastX.IsBetweenII(32767 - joystick.X.Deadband,
                                                     32767 + joystick.X.Deadband) &&
                                                 !state.X.IsBetweenEE(32767 - joystick.X.Deadband,
                                                     32767 + joystick.X.Deadband))

                                        {
                                            //Log.Info($"{state.X}");
                                            PlaySound(joystick.X.OutDeadzoneSound);
                                        }
                                    }

                                    if (joystick.Z?.Deadband > 0)
                                    {
                                        if (!joystick.LastZ.IsBetweenII(32767 - joystick.Z.Deadband,
                                                32767 + joystick.Z.Deadband) &&
                                            state.Z.IsBetweenEE(32767 - joystick.Z.Deadband,
                                                32767 + joystick.Z.Deadband))
                                        {
                                            //Log.Info($"{state.Z}");
                                            PlaySound(joystick.Z.InDeadzoneSound);
                                        }
                                        else if (joystick.LastZ.IsBetweenII(32767 - joystick.Z.Deadband,
                                                     32767 + joystick.Z.Deadband) &&
                                                 !state.Z.IsBetweenEE(32767 - joystick.Z.Deadband,
                                                     32767 + joystick.Z.Deadband))

                                        {
                                            //Log.Info($"{state.Z}");
                                            PlaySound(joystick.Z.OutDeadzoneSound);
                                        }
                                    }

                                    joystick.LastX = state.X;
                                    joystick.LastY = state.Y;
                                    joystick.LastZ = state.Z;

                                    await Task.Delay(50, joystick.JoystickTokenSource.Token);
                                }

                            }, joystickToken);
                        }
                    }
                }
           
                foreach (var joystick in JoystickList.Where(x => x.Joystick == null))
                {
                    Log.Info($"No joystick found with PID={joystick.PID} and VID={joystick.VID}");
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

            foreach (var joystick in JoystickList.Where(x => x.Joystick != null))
            {
                joystick.JoystickTokenSource.Cancel();

                var joystickToken = joystick.JoystickTokenSource.Token;

                try
                {
                    joystick.JoystickTask?.Wait(joystickToken);
                }
                catch (OperationCanceledException)
                {
                    Log.Info($"joystick background task ended with PID={joystick.PID} and VID={joystick.VID}");
                }
                finally
                {
                    joystick.JoystickTokenSource.Dispose();

                    joystick.Joystick?.Unacquire();
                    joystick.Joystick?.Dispose();
                }
            }

            Log.Info("exiting");

            base.OnExit(e);
        }
    }
}
