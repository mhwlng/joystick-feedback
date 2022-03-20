﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace joystick_feedback
{
    /// <summary>
    /// Provides bindable properties and commands for the NotifyIcon. In this sample, the
    /// view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
    /// in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
    /// </summary>
    public class NotifyIconViewModel
    {
        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand {CommandAction = () =>
                    {
                        /*if (App.JsonTask?.Status.Equals(TaskStatus.Running) == true)
                        {
                            MessageBox.Show("Waiting for background tasks to complete...","joystick-feedback", MessageBoxButton.OK,
                                MessageBoxImage.Information, MessageBoxResult.OK,
                                MessageBoxOptions.DefaultDesktopOnly);
                        }
                        else
                        {*/
                            App.IsShuttingDown = true;
                            Application.Current.Shutdown();
                        //}
                    }
                };
            }
        }
    }


    /// <summary>
    /// Simplistic delegate command for the demo.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; set; }
        public Func<bool> CanExecuteFunc { get; set; }

        public void Execute(object parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null  || CanExecuteFunc();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
