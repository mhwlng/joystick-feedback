using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace joystick_feedback
{
    internal static class CommandTools
    {
        [DllImport("user32.dll")]
        private static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handleWindow, out int lpdwProcessID);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetKeyboardLayout(int WindowsThreadProcessID);

        internal const char MACRO_START_CHAR = '{';
        internal const string MACRO_END = "}}";
        internal const string REGEX_MACRO = @"^\{(\{[^\{\}]+\})+\}$";
        internal const string REGEX_SUB_COMMAND = @"(\{[^\{\}]+\})";

        internal static string ExtractMacro(string text, int position)
        {
            try
            {
                var endPosition = text.IndexOf(MACRO_END, position);

                // Found an end, let's verify it's actually a macro
                if (endPosition > position)
                {
                    // Use Regex to verify it's really a macro
                    var match = Regex.Match(text.Substring(position, endPosition - position + MACRO_END.Length), REGEX_MACRO);
                    if (match.Length > 0)
                    {
                        return match.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                App.Log.Error( $"ExtractMacro Exception: {ex}");
            }

            return null;
        }

        internal static List<DirectInputKeyCode> ExtractKeyStrokes(string macroText)
        {
            var keyStrokes = new List<DirectInputKeyCode>();


            try
            {
                var matches = Regex.Matches(macroText, REGEX_SUB_COMMAND);
                foreach (var match in matches)
                {
                    var matchText = match.ToString().ToUpperInvariant().Replace("{", "").Replace("}", "");

                    if (App.Binding[BindingType.OnFoot].KeyboardLayout == "en-US")
                    {
                        // http://kbdlayout.info/kbdusx/shiftstates+scancodes/base

                        // FIRST ROW  DIKGRAVE          DIKMINUS        DIKEQUALS
                        // SECOND ROW DIKLEFTBRACKET    DIKRIGHTBRACKET DIKBACKSLASH
                        // THIRD ROW  DIKSEMICOLON      DIKAPOSTROPHE
                        // FOURTH ROW DIKCOMMA          DIKPERIOD       DIKSLASH
                    }
                    else if (App.Binding[BindingType.OnFoot].KeyboardLayout == "es-ES")
                    {
                        // http://kbdlayout.info/kbdsp/shiftstates+scancodes/base

                        // FIRST ROW
                        // SECOND ROW 
                        // THIRD ROW
                        // FOURTH ROW

                        // all the keys are the same as en-US in binding file , for some reason ????
                    }
                    else if (App.Binding[BindingType.OnFoot].KeyboardLayout == "en-GB")
                    {
                        // http://kbdlayout.info/kbduk/shiftstates+scancodes/base

                        switch (matchText)
                        {
                            // second row
                            case "DIKHASH":
                                matchText = "DIKBACKSLASH";
                                break;
                        }
                    }
                    else if (App.Binding[BindingType.OnFoot].KeyboardLayout == "fr-FR")
                    {
                        // http://kbdlayout.info/kbdfr/shiftstates+scancodes/base

                        switch (matchText)
                        {
                            // FIRST ROW
                            case "DIKSUPERSCRIPTTWO":
                                matchText = "DIKGRAVE";
                                break;
                            case "DIKAMPERSAND":
                                matchText = "DIK1";
                                break;
                            case "DIKÉ":
                                matchText = "DIK2";
                                break;
                            case "DIKDOUBLEQUOTE":
                                matchText = "DIK3";
                                break;
                            case "DIKAPOSTROPHE":
                                matchText = "DIK4";
                                break;
                            case "DIKLEFTPARENTHESIS":
                                matchText = "DIK5";
                                break;
                            case "DIKMINUS":
                                matchText = "DIK6";
                                break;
                            case "DIKÈ":
                                matchText = "DIK7";
                                break;
                            case "DIKUNDERLINE":
                                matchText = "DIK8";
                                break;
                            case "DIKÇ":
                                matchText = "DIK9";
                                break;
                            case "DIKÀ":
                                matchText = "DIK0";
                                break;
                            case "DIKRIGHTPARENTHESIS":
                                matchText = "DIKMINUS";
                                break;

                            // SECOND ROW
                            case "DIKA":
                                matchText = "DIKQ";
                                break;
                            case "DIKZ":
                                matchText = "DIKW";
                                break;
                            case "DIKCIRCUMFLEX":
                                matchText = "DIKLEFTBRACKET";
                                break;
                            case "DIKDOLLAR":
                                matchText = "DIKRIGHTBRACKET";
                                break;
                            case "DIKASTERISK":
                                matchText = "DIKBACKSLASH";
                                break;

                            // THIRD ROW

                            case "DIKQ":
                                matchText = "DIKA";
                                break;

                            case "DIKM":
                                matchText = "DIKSEMICOLON";
                                break;
                            case "DIKÙ":
                                matchText = "DIKAPOSTROPHE";
                                break;

                            // FOURTH ROW
                            case "DIKW":
                                matchText = "DIKZ";
                                break;
                            case "DIKCOMMA":
                                matchText = "DIKM";
                                break;
                            case "DIKSEMICOLON":
                                matchText = "DIKCOMMA";
                                break;
                            case "DIKCOLON":
                                matchText = "DIKPERIOD";
                                break;
                            case "DIKEXCLAMATIONPOINT":
                                matchText = "DIKSLASH";
                                break;
                        }

                    }
                    else if (App.Binding[BindingType.OnFoot].KeyboardLayout == "de-DE")
                    {
                        // http://kbdlayout.info/kbdgr/shiftstates+scancodes/base

                        switch (matchText)
                        {
                            // FIRST ROW
                            case "DIKCIRCUMFLEX":
                                matchText = "DIKGRAVE";
                                break;
                            case "DIKß":
                                matchText = "DIKMINUS";
                                break;
                            case "DIKACUTE":
                                matchText = "DIKEQUALS";
                                break;

                            // SECOND ROW 
                            case "DIKZ":
                                matchText = "DIKY";
                                break;
                            case "DIKÜ":
                                matchText = "DIKLEFTBRACKET";
                                break;
                            case "DIKPLUS":
                                matchText = "DIKRIGHTBRACKET";
                                break;
                            case "DIKHASH":
                                matchText = "DIKBACKSLASH";
                                break;

                            // THIRD ROW
                            case "DIKÖ":
                                matchText = "DIKSEMICOLON";
                                break;
                            case "DIKÄ":
                                matchText = "DIKAPOSTROPHE";
                                break;

                            // FOURTH ROW
                            case "DIKY":
                                matchText = "DIKZ";
                                break;
                            case "DIKMINUS":
                                matchText = "DIKSLASH";
                                break;
                        }

                    }
                    else if (App.Binding[BindingType.OnFoot].KeyboardLayout == "de-CH")
                    {
                        // http://kbdlayout.info/kbdsg/shiftstates+scancodes/base

                        switch (matchText)
                        {
                            // FIRST ROW
                            case "DIK§":
                                matchText = "DIKGRAVE";
                                break;
                            case "DIKAPOSTROPHE":
                                matchText = "DIKMINUS";
                                break;
                            case "DIKCIRCUMFLEX":
                                matchText = "DIKEQUALS";
                                break;

                            // SECOND ROW 
                            case "DIKZ":
                                matchText = "DIKY";
                                break;
                            case "DIKÜ":
                                matchText = "DIKLEFTBRACKET";
                                break;
                            case "DIKUMLAUT":
                                matchText = "DIKRIGHTBRACKET";
                                break;
                            case "DIKDOLLAR":
                                matchText = "DIKBACKSLASH";
                                break;

                            // THIRD ROW
                            case "DIKÖ":
                                matchText = "DIKSEMICOLON";
                                break;
                            case "DIKÄ":
                                matchText = "DIKAPOSTROPHE";
                                break;

                            // FOURTH ROW
                            case "DIKY":
                                matchText = "DIKZ";
                                break;
                            case "DIKMINUS":
                                matchText = "DIKSLASH";
                                break;

                        }

                    }
                    else if (App.Binding[BindingType.OnFoot].KeyboardLayout == "da-DK")
                    {
                        // http://kbdlayout.info/kbdda/shiftstates+scancodes/base

                        switch (matchText)
                        {
                            // FIRST ROW
                            case "DIKHALF":
                                matchText = "DIKGRAVE";
                                break;
                            case "DIKPLUS":
                                matchText = "DIKMINUS";
                                break;
                            case "DIKACUTE":
                                matchText = "DIKEQUALS";
                                break;

                            // SECOND ROW 
                            case "DIKÅ":
                                matchText = "DIKLEFTBRACKET";
                                break;
                            case "DIKUMLAUT":
                                matchText = "DIKRIGHTBRACKET";
                                break;
                            case "DIKAPOSTROPHE":
                                matchText = "DIKBACKSLASH";
                                break;

                            // THIRD ROW
                            case "DIKÆ":
                                matchText = "DIKSEMICOLON";
                                break;
                            case "DIKØ":
                                matchText = "DIKAPOSTROPHE";
                                break;

                            // FOURTH ROW
                            case "DIKMINUS":
                                matchText = "DIKSLASH";
                                break;
                        }

                    }
                    else if (App.Binding[BindingType.OnFoot].KeyboardLayout == "it-IT")
                    {
                        // http://kbdlayout.info/kbdit/shiftstates+scancodes/base

                        switch (matchText)
                        {
                            // FIRST ROW
                            case "DIKBACKSLASH":
                                matchText = "DIKGRAVE";
                                break;
                            case "DIKAPOSTROPHE":
                                matchText = "DIKMINUS";
                                break;
                            case "DIKÌ":
                                matchText = "DIKEQUALS";
                                break;

                            // SECOND ROW 
                            case "DIKÈ":
                                matchText = "DIKLEFTBRACKET";
                                break;
                            case "DIKPLUS":
                                matchText = "DIKRIGHTBRACKET";
                                break;
                            case "DIKÙ":
                                matchText = "DIKBACKSLASH";
                                break;

                            // THIRD ROW
                            case "DIKÒ":
                                matchText = "DIKSEMICOLON";
                                break;
                            case "DIKÀ":
                                matchText = "DIKAPOSTROPHE";
                                break;

                            // FOURTH ROW
                            case "DIKMINUS":
                                matchText = "DIKSLASH";
                                break;
                        }

                    }

                    else if (App.Binding[BindingType.OnFoot].KeyboardLayout == "pt-PT")
                    {
                        // http://kbdlayout.info/kbdpo/shiftstates+scancodes/base

                        switch (matchText)
                        {
                            // FIRST ROW
                            case "DIKBACKSLASH":
                                matchText = "DIKGRAVE";
                                break;
                            case "DIKAPOSTROPHE":
                                matchText = "DIKMINUS";
                                break;
                            case "DIK«":
                                matchText = "DIKEQUALS";
                                break;

                            // SECOND ROW 
                            case "DIKPLUS":
                                matchText = "DIKLEFTBRACKET";
                                break;
                            case "DIKACUTE":
                                matchText = "DIKRIGHTBRACKET";
                                break;
                            case "DIKTILDE":
                                matchText = "DIKBACKSLASH";
                                break;

                            // THIRD ROW
                            case "DIKÇ":
                                matchText = "DIKSEMICOLON";
                                break;
                            case "DIKº":
                                matchText = "DIKAPOSTROPHE";
                                break;

                            // FOURTH ROW
                            case "DIKMINUS":
                                matchText = "DIKSLASH";
                                break;
                        }

                    }


                    var stroke = (DirectInputKeyCode)Enum.Parse(typeof(DirectInputKeyCode), matchText, true);

                    keyStrokes.Add(stroke);
                }
            }
            catch (Exception ex)
            {
                App.Log.Error( $"ExtractKeyStrokes Exception: {ex}");
            }

            return keyStrokes;
        }



        public static DirectInputKeyCode ConvertLocaleScanCode(DirectInputKeyCode scanCode)
        {
            //german

            // http://kbdlayout.info/KBDGR/shiftstates+scancodes/base

            // french
            // http://kbdlayout.info/kbdfr/shiftstates+scancodes/base

            // usa
            // http://kbdlayout.info/kbdusx/shiftstates+scancodes/base

            if (App.Binding[BindingType.OnFoot].KeyboardLayout != "en-US")
            {
                App.Log.Info( scanCode.ToString() + " " + ((ushort)scanCode).ToString("X"));

                int lpdwProcessId;
                IntPtr hWnd = GetForegroundWindow();
                int WinThreadProcId = GetWindowThreadProcessId(hWnd, out lpdwProcessId);
                var hkl = GetKeyboardLayout(WinThreadProcId);

                App.Log.Info( ((long)hkl).ToString("X"));

                //hkl = (IntPtr)67568647; // de-DE 4070407

                // Maps the virtual scanCode to key code for the current locale
                var virtualKeyCode = MapVirtualKeyEx((ushort)scanCode, 3, hkl);

                if (virtualKeyCode > 0)
                {
                    // map key code back to en-US scan code :

                    hkl = (IntPtr)67699721; // en-US 4090409

                    var virtualScanCode = MapVirtualKeyEx((ushort)virtualKeyCode, 4, hkl);

                    if (virtualScanCode > 0)
                    {
                        App.Log.Info(
                            "keycode " + virtualKeyCode.ToString("X") + " scancode " + virtualScanCode.ToString("X") +
                            " keyboard code " + hkl.ToString("X"));

                        return (DirectInputKeyCode)(virtualScanCode & 0xff); // only use low byte
                    }
                }
            }

            return scanCode;
        }


        public static void HandleMacro(string macro)
        {
            var keyStrokes = ExtractKeyStrokes(macro);

            // Actually initiate the keystrokes
            if (keyStrokes.Count > 0)
            {
                var iis = new InputSimulator();
                var keyCode = keyStrokes.Last();
                keyStrokes.Remove(keyCode);

                if (keyStrokes.Count > 0)
                {
                    //iis.Keyboard.ModifiedKeyStroke(keyStrokes.Select(ks => ks).ToArray(), keyCode);

                    iis.Keyboard.DelayedModifiedKeyStroke(keyStrokes.Select(ks => ks), keyCode, 40);

                }
                else // Single Keycode
                {
                    //iis.Keyboard.KeyPress(keyCode);

                    iis.Keyboard.DelayedKeyPress(keyCode, 40);
                }
            }
        }

        public static void HandleMacroDown(string macro)
        {
            var keyStrokes = ExtractKeyStrokes(macro);

            // Actually initiate the keystrokes
            if (keyStrokes.Count > 0)
            {
                var iis = new InputSimulator();
                var keyCode = keyStrokes.Last();
                keyStrokes.Remove(keyCode);

                if (keyStrokes.Count > 0)
                {
                    iis.Keyboard.DelayedModifiedKeyStrokeDown(keyStrokes.Select(ks => ks), keyCode, 40);

                }
                else // Single Keycode
                {
                    iis.Keyboard.DelayedKeyPressDown(keyCode, 40);
                }
            }
        }


        public static void HandleMacroUp(string macro)
        {
            var keyStrokes = ExtractKeyStrokes(macro);

            // Actually initiate the keystrokes
            if (keyStrokes.Count > 0)
            {
                var iis = new InputSimulator();
                var keyCode = keyStrokes.Last();
                keyStrokes.Remove(keyCode);

                if (keyStrokes.Count > 0)
                {
                    iis.Keyboard.DelayedModifiedKeyStrokeUp(keyStrokes.Select(ks => ks), keyCode, 40);

                }
                else // Single Keycode
                {
                    iis.Keyboard.DelayedKeyPressUp(keyCode, 40);
                }
            }
        }

        public static string BuildInputText(StandardBindingInfo keyInfo)
        {
            var inputText = "";

            if (keyInfo.Primary.Device == "Keyboard")
            {
                inputText =
                    "{" + keyInfo.Primary.Key.Replace("Key_", "DIK") + "}";
                foreach (var m in keyInfo.Primary.Modifier)
                {
                    if (m.Device == "Keyboard")
                    {
                        inputText =
                            "{" + m.Key.Replace("Key_", "DIK") +
                            "}" + inputText;
                    }
                }

            }
            else if (keyInfo.Secondary.Device == "Keyboard")
            {
                inputText =
                    "{" + keyInfo.Secondary.Key.Replace("Key_", "DIK") + "}";
                foreach (var m in keyInfo.Secondary.Modifier)
                {
                    if (m.Device == "Keyboard")
                    {
                        inputText =
                            "{" + m.Key.Replace("Key_", "DIK") +
                            "}" + inputText;
                    }
                }
            }

            if (!string.IsNullOrEmpty(inputText))
            {
                inputText = inputText.Replace("_", "")

                    .Replace("Subtract", "MINUS") //DIKNumpadSubtract   -> DikNumpadMinus
                    .Replace("Add", "PLUS") //DIKNumpadAdd        -> DikNumpadPlus
                    .Replace("Divide", "SLASH") //DIKNumpadDivide     -> DikNumpadSlash
                    .Replace("Decimal", "PERIOD") //DIKNumpadDecimal    -> DikNumpadPeriod
                    .Replace("Multiply", "STAR") //DIKNumpadMultiply   -> DikNumpadStar
                    .Replace("DIKEnter", "DIKRETURN")  // don't affect DIKNumpadEnter
                    .Replace("Backspace", "BACK")
                    .Replace("UpArrow", "UP")
                    .Replace("DownArrow", "DOWN")
                    .Replace("LeftArrow", "LEFT")
                    .Replace("RightArrow", "RIGHT")
                    .Replace("LeftAlt", "LMENU")
                    .Replace("RightAlt", "RMENU")
                    .Replace("RightControl", "RCONTROL")
                    .Replace("LeftControl", "LCONTROL")
                    .Replace("RightShift", "RSHIFT")
                    .Replace("LeftShift", "LSHIFT");

                //Logger.Instance.LogMessage(TracingLevel.DEBUG, $"{inputText}");

            }

            return inputText;
        }

        public static void SendInput(string inputText)
        {
            var text = inputText;

            for (var idx = 0; idx < text.Length ; idx++)
            {
                var macro = ExtractMacro(text, idx);
                idx += macro.Length - 1;
                macro = macro.Substring(1, macro.Length - 2);

                HandleMacro(macro);
            }
        }

        public static void SendInputDown(string inputText)
        {
            var text = inputText;

            for (var idx = 0; idx < text.Length; idx++)
            {
                var macro = ExtractMacro(text, idx);
                idx += macro.Length - 1;
                macro = macro.Substring(1, macro.Length - 2);

                HandleMacroDown(macro);
            }
        }

        public static void SendInputUp(string inputText)
        {
            var text = inputText;

            for (var idx = 0; idx < text.Length; idx++)
            {
                var macro = ExtractMacro(text, idx);
                idx += macro.Length - 1;
                macro = macro.Substring(1, macro.Length - 2);

                HandleMacroUp(macro);
            }
        }


        public static void SendKeypress(StandardBindingInfo keyInfo, int repeatCount = 1)
        {
            var inputText = BuildInputText(keyInfo);

            if (!string.IsNullOrEmpty(inputText))
            {

                //Logger.Instance.LogMessage(TracingLevel.DEBUG, $"{inputText}");

                for (var i = 0; i < repeatCount; i++)
                {
                    if (repeatCount > 1 && i > 0)
                    {
                        Thread.Sleep(20);
                    }
                    SendInput("{" + inputText + "}");

                }

                // keyboard test page : https://w3c.github.io/uievents/tools/key-event-viewer.html
            }

        }

        public static  void SendKeypressDown(StandardBindingInfo keyInfo)
        {
            var inputText = BuildInputText(keyInfo);

            if (!string.IsNullOrEmpty(inputText))
            {
                SendInputDown("{" + inputText + "}");
            }
        }


        public static void SendKeypressUp(StandardBindingInfo keyInfo)
        {
            var inputText = BuildInputText(keyInfo);

            if (!string.IsNullOrEmpty(inputText))
            {
                SendInputUp("{" + inputText + "}");
            }
        }

        public static void HandleFireGroup(int fireGroup)
        {
            var isDisabled = (EliteData.StatusData.OnFoot ||
                              EliteData.StatusData.InSRV ||
                              EliteData.StatusData.Docked ||
                              EliteData.StatusData.Landed ||
                              EliteData.StatusData.LandingGearDown ||
                              EliteData.StatusData.FsdJump //||

                //EliteData.StatusData.Supercruise ||
                //EliteData.StatusData.CargoScoopDeployed ||
                //EliteData.StatusData.SilentRunning ||
                //EliteData.StatusData.ScoopingFuel ||
                //EliteData.StatusData.IsInDanger ||
                //EliteData.StatusData.BeingInterdicted ||
                //EliteData.StatusData.HudInAnalysisMode ||
                //EliteData.StatusData.FsdMassLocked ||
                //EliteData.StatusData.FsdCharging ||
                //EliteData.StatusData.FsdCooldown ||

                //EliteData.StatusData.HardpointsDeployed
                );

            if (!isDisabled)
            {
                var cycle = fireGroup - EliteData.StatusData.Firegroup;

                if (cycle < 0)
                {
                    for (var f = 0; f < -cycle; f++)
                    {
                        SendKeypress(App.Binding[BindingType.Ship].CycleFireGroupPrevious);
                        Thread.Sleep(70);
                    }
                }
                else if (cycle > 0)
                {
                    for (var f = 0; f < cycle; f++)
                    {
                        SendKeypress(App.Binding[BindingType.Ship].CycleFireGroupNext);
                        Thread.Sleep(70);
                    }
                }
            }
        }

        public static void KeyPressed(string payload)
        {
            if (App.Binding == null)
            {
                return;
            }

            var isPrimary = false;

            switch (payload)
            {
                case "OrderFocusTarget":
                    SendKeypress(App.Binding[BindingType.Ship].OrderFocusTarget);
                    break;
                case "OrderAggressiveBehaviour":
                    SendKeypress(App.Binding[BindingType.Ship].OrderAggressiveBehaviour);
                    break;
                case "OrderDefensiveBehaviour":
                    SendKeypress(App.Binding[BindingType.Ship].OrderDefensiveBehaviour);
                    break;
                case "OpenOrders":
                    SendKeypress(App.Binding[BindingType.Ship].OpenOrders);
                    break;
                case "OrderRequestDock":
                    SendKeypress(App.Binding[BindingType.Ship].OrderRequestDock);
                    break;
                case "OrderFollow":
                    SendKeypress(App.Binding[BindingType.Ship].OrderFollow);
                    break;
                case "OrderHoldFire":
                    SendKeypress(App.Binding[BindingType.Ship].OrderHoldFire);
                    break;
                case "OrderHoldPosition":
                    SendKeypress(App.Binding[BindingType.Ship].OrderHoldPosition);
                    break;

                case "HeadLookPitchDown":
                    SendKeypress(App.Binding[BindingType.Ship].HeadLookPitchDown);
                    break;
                case "HeadLookYawLeft":
                    SendKeypress(App.Binding[BindingType.Ship].HeadLookYawLeft);
                    break;
                case "HeadLookYawRight":
                    SendKeypress(App.Binding[BindingType.Ship].HeadLookYawRight);
                    break;
                case "HeadLookPitchUp":
                    SendKeypress(App.Binding[BindingType.Ship].HeadLookPitchUp);
                    break;
                case "HeadLookReset":
                    SendKeypress(App.Binding[BindingType.Ship].HeadLookReset);
                    break;
                case "OpenCodexGoToDiscovery":
                    SendKeypress(App.Binding[BindingType.Ship].OpenCodexGoToDiscovery);
                    break;
                case "FriendsMenu":
                    SendKeypress(App.Binding[BindingType.Ship].FriendsMenu);
                    break;
                case "Pause":
                    SendKeypress(App.Binding[BindingType.Ship].Pause);
                    break;
                case "MicrophoneMute":
                    SendKeypress(App.Binding[BindingType.Ship].MicrophoneMute);
                    break;

                case "HMDReset":
                    SendKeypress(App.Binding[BindingType.Ship].HMDReset);
                    break;
                case "OculusReset":
                    SendKeypress(App.Binding[BindingType.Ship].OculusReset);
                    break;
                case "RadarDecreaseRange":
                    SendKeypress(App.Binding[BindingType.Ship].RadarDecreaseRange);
                    break;
                case "RadarIncreaseRange":
                    SendKeypress(App.Binding[BindingType.Ship].RadarIncreaseRange);
                    break;
                case "MultiCrewThirdPersonFovInButton":
                    SendKeypress(App.Binding[BindingType.Ship].MultiCrewThirdPersonFovInButton);
                    break;
                case "MultiCrewThirdPersonFovOutButton":
                    SendKeypress(App.Binding[BindingType.Ship].MultiCrewThirdPersonFovOutButton);
                    break;
                case "MultiCrewPrimaryFire":
                    SendKeypress(App.Binding[BindingType.Ship].MultiCrewPrimaryFire);
                    break;
                case "MultiCrewSecondaryFire":
                    SendKeypress(App.Binding[BindingType.Ship].MultiCrewSecondaryFire);
                    break;
                case "MultiCrewToggleMode":
                    SendKeypress(App.Binding[BindingType.Ship].MultiCrewToggleMode);
                    break;
                case "MultiCrewThirdPersonPitchDownButton":
                    SendKeypress(App.Binding[BindingType.Ship].MultiCrewThirdPersonPitchDownButton);
                    break;
                case "MultiCrewThirdPersonPitchUpButton":
                    SendKeypress(App.Binding[BindingType.Ship].MultiCrewThirdPersonPitchUpButton);
                    break;
                case "MultiCrewPrimaryUtilityFire":
                    SendKeypress(App.Binding[BindingType.Ship].MultiCrewPrimaryUtilityFire);
                    break;
                case "MultiCrewSecondaryUtilityFire":
                    SendKeypress(App.Binding[BindingType.Ship].MultiCrewSecondaryUtilityFire);
                    break;
                case "MultiCrewCockpitUICycleBackward":
                    SendKeypress(App.Binding[BindingType.Ship].MultiCrewCockpitUICycleBackward);
                    break;
                case "MultiCrewCockpitUICycleForward":
                    SendKeypress(App.Binding[BindingType.Ship].MultiCrewCockpitUICycleForward);
                    break;
                case "MultiCrewThirdPersonYawLeftButton":
                    SendKeypress(App.Binding[BindingType.Ship].MultiCrewThirdPersonYawLeftButton);
                    break;
                case "MultiCrewThirdPersonYawRightButton":
                    SendKeypress(App.Binding[BindingType.Ship].MultiCrewThirdPersonYawRightButton);
                    break;
                case "SAAThirdPersonFovInButton":
                    SendKeypress(App.Binding[BindingType.Ship].SAAThirdPersonFovInButton);
                    break;
                case "SAAThirdPersonFovOutButton":
                    SendKeypress(App.Binding[BindingType.Ship].SAAThirdPersonFovOutButton);
                    break;
                case "ExplorationFSSEnter":
                    SendKeypress(App.Binding[BindingType.Ship].ExplorationFSSEnter);
                    break;
                case "ExplorationSAAExitThirdPerson":
                    SendKeypress(App.Binding[BindingType.Ship].ExplorationSAAExitThirdPerson);
                    break;
                case "ExplorationFSSQuit":
                    SendKeypress(App.Binding[BindingType.Ship].ExplorationFSSQuit);
                    break;
                case "ExplorationFSSShowHelp":
                    SendKeypress(App.Binding[BindingType.Ship].ExplorationFSSShowHelp);
                    break;
                case "ExplorationSAANextGenus":
                    SendKeypress(App.Binding[BindingType.Ship].ExplorationSAANextGenus);
                    break;
                case "ExplorationSAAPreviousGenus":
                    SendKeypress(App.Binding[BindingType.Ship].ExplorationSAAPreviousGenus);
                    break;
                case "ExplorationFSSDiscoveryScan":
                    SendKeypress(App.Binding[BindingType.Ship].ExplorationFSSDiscoveryScan);
                    break;
                case "ExplorationFSSCameraPitchDecreaseButton":
                    SendKeypress(App.Binding[BindingType.Ship].ExplorationFSSCameraPitchDecreaseButton);
                    break;
                case "ExplorationFSSCameraPitchIncreaseButton":
                    SendKeypress(App.Binding[BindingType.Ship].ExplorationFSSCameraPitchIncreaseButton);
                    break;
                case "ExplorationFSSRadioTuningX_Decrease":
                    SendKeypress(App.Binding[BindingType.Ship].ExplorationFSSRadioTuningX_Decrease);
                    break;
                case "ExplorationFSSRadioTuningX_Increase":
                    SendKeypress(App.Binding[BindingType.Ship].ExplorationFSSRadioTuningX_Increase);
                    break;
                case "ExplorationFSSCameraYawDecreaseButton":
                    SendKeypress(App.Binding[BindingType.Ship].ExplorationFSSCameraYawDecreaseButton);
                    break;
                case "ExplorationFSSCameraYawIncreaseButton":
                    SendKeypress(App.Binding[BindingType.Ship].ExplorationFSSCameraYawIncreaseButton);
                    break;
                case "SAAThirdPersonPitchDownButton":
                    SendKeypress(App.Binding[BindingType.Ship].SAAThirdPersonPitchDownButton);
                    break;
                case "SAAThirdPersonPitchUpButton":
                    SendKeypress(App.Binding[BindingType.Ship].SAAThirdPersonPitchUpButton);
                    break;
                case "ExplorationFSSMiniZoomIn":
                    SendKeypress(App.Binding[BindingType.Ship].ExplorationFSSMiniZoomIn);
                    break;
                case "ExplorationFSSMiniZoomOut":
                    SendKeypress(App.Binding[BindingType.Ship].ExplorationFSSMiniZoomOut);
                    break;
                case "ExplorationFSSTarget":
                    SendKeypress(App.Binding[BindingType.Ship].ExplorationFSSTarget);
                    break;
                case "ExplorationSAAChangeScannedAreaViewToggle":
                    SendKeypress(App.Binding[BindingType.Ship].ExplorationSAAChangeScannedAreaViewToggle);
                    break;
                case "SAAThirdPersonYawLeftButton":
                    SendKeypress(App.Binding[BindingType.Ship].SAAThirdPersonYawLeftButton);
                    break;
                case "SAAThirdPersonYawRightButton":
                    SendKeypress(App.Binding[BindingType.Ship].SAAThirdPersonYawRightButton);
                    break;
                case "ExplorationFSSZoomIn":
                    SendKeypress(App.Binding[BindingType.Ship].ExplorationFSSZoomIn);
                    break;
                case "ExplorationFSSZoomOut":
                    SendKeypress(App.Binding[BindingType.Ship].ExplorationFSSZoomOut);
                    break;
                case "FocusCommsPanel":
                    SendKeypress(App.Binding[BindingType.Ship].FocusCommsPanel);
                    break;
                case "FocusLeftPanel":
                    SendKeypress(App.Binding[BindingType.Ship].FocusLeftPanel);
                    break;
                case "QuickCommsPanel":
                    SendKeypress(App.Binding[BindingType.Ship].QuickCommsPanel);
                    break;
                case "FocusRadarPanel":
                    SendKeypress(App.Binding[BindingType.Ship].FocusRadarPanel);
                    break;
                case "FocusRightPanel":
                    SendKeypress(App.Binding[BindingType.Ship].FocusRightPanel);
                    break;
                case "UIFocus":
                    SendKeypress(App.Binding[BindingType.Ship].UIFocus);
                    break;
                case "TargetWingman0":
                    SendKeypress(App.Binding[BindingType.Ship].TargetWingman0);
                    break;
                case "TargetWingman1":
                    SendKeypress(App.Binding[BindingType.Ship].TargetWingman1);
                    break;
                case "TargetWingman2":
                    SendKeypress(App.Binding[BindingType.Ship].TargetWingman2);
                    break;
                case "WingNavLock":
                    SendKeypress(App.Binding[BindingType.Ship].WingNavLock);
                    break;
                case "SelectTargetsTarget":
                    SendKeypress(App.Binding[BindingType.Ship].SelectTargetsTarget);
                    break;
                case "FireChaffLauncher":
                    SendKeypress(App.Binding[BindingType.Ship].FireChaffLauncher);
                    break;
                case "ChargeECM":
                    SendKeypress(App.Binding[BindingType.Ship].ChargeECM);
                    break;
                case "IncreaseEnginesPower":
                    SendKeypress(App.Binding[BindingType.Ship].IncreaseEnginesPower);
                    break;
                case "PrimaryFire":
                    SendKeypress(App.Binding[BindingType.Ship].PrimaryFire);
                    break;
                case "SecondaryFire":
                    SendKeypress(App.Binding[BindingType.Ship].SecondaryFire);
                    break;
                case "DeployHeatSink":
                    SendKeypress(App.Binding[BindingType.Ship].DeployHeatSink);
                    break;
                case "SelectHighestThreat":
                    SendKeypress(App.Binding[BindingType.Ship].SelectHighestThreat);
                    break;
                case "CycleNextTarget":
                    SendKeypress(App.Binding[BindingType.Ship].CycleNextTarget);
                    break;
                case "CycleFireGroupNext":
                    SendKeypress(App.Binding[BindingType.Ship].CycleFireGroupNext);
                    break;
                case "CycleNextHostileTarget":
                    SendKeypress(App.Binding[BindingType.Ship].CycleNextHostileTarget);
                    break;
                case "CycleNextSubsystem":
                    SendKeypress(App.Binding[BindingType.Ship].CycleNextSubsystem);
                    break;
                case "CyclePreviousTarget":
                    SendKeypress(App.Binding[BindingType.Ship].CyclePreviousTarget);
                    break;
                case "CycleFireGroupPrevious":
                    SendKeypress(App.Binding[BindingType.Ship].CycleFireGroupPrevious);
                    break;
                case "CyclePreviousHostileTarget":
                    SendKeypress(App.Binding[BindingType.Ship].CyclePreviousHostileTarget);
                    break;
                case "CyclePreviousSubsystem":
                    SendKeypress(App.Binding[BindingType.Ship].CyclePreviousSubsystem);
                    break;
                case "ResetPowerDistribution":
                    SendKeypress(App.Binding[BindingType.Ship].ResetPowerDistribution);
                    break;
                case "UseShieldCell":
                    SendKeypress(App.Binding[BindingType.Ship].UseShieldCell);
                    break;
                case "IncreaseSystemsPower":
                    SendKeypress(App.Binding[BindingType.Ship].IncreaseSystemsPower);
                    break;
                case "SelectTarget":
                    SendKeypress(App.Binding[BindingType.Ship].SelectTarget);
                    break;
                case "IncreaseWeaponsPower":
                    SendKeypress(App.Binding[BindingType.Ship].IncreaseWeaponsPower);
                    break;
                case "ShowPGScoreSummaryInput":
                    SendKeypress(App.Binding[BindingType.Ship].ShowPGScoreSummaryInput);
                    break;
                case "EjectAllCargo":
                    SendKeypress(App.Binding[BindingType.Ship].EjectAllCargo);
                    break;
                case "EngineColourToggle":
                    SendKeypress(App.Binding[BindingType.Ship].EngineColourToggle);
                    break;
                case "OrbitLinesToggle":
                    SendKeypress(App.Binding[BindingType.Ship].OrbitLinesToggle);
                    break;
                case "MouseReset":
                    SendKeypress(App.Binding[BindingType.Ship].MouseReset);
                    break;
                case "HeadLookToggle":
                    SendKeypress(App.Binding[BindingType.Ship].HeadLookToggle);
                    break;
                case "WeaponColourToggle":
                    SendKeypress(App.Binding[BindingType.Ship].WeaponColourToggle);
                    break;
                case "SetSpeedMinus100":
                    SendKeypress(App.Binding[BindingType.Ship].SetSpeedMinus100);
                    break;
                case "SetSpeed100":
                    SendKeypress(App.Binding[BindingType.Ship].SetSpeed100);
                    break;
                case "SetSpeedMinus25":
                    SendKeypress(App.Binding[BindingType.Ship].SetSpeedMinus25);
                    break;
                case "SetSpeed25":
                    SendKeypress(App.Binding[BindingType.Ship].SetSpeed25);
                    break;
                case "SetSpeedMinus50":
                    SendKeypress(App.Binding[BindingType.Ship].SetSpeedMinus50);
                    break;
                case "SetSpeed50":
                    SendKeypress(App.Binding[BindingType.Ship].SetSpeed50);
                    break;
                case "SetSpeedMinus75":
                    SendKeypress(App.Binding[BindingType.Ship].SetSpeedMinus75);
                    break;
                case "SetSpeed75":
                    SendKeypress(App.Binding[BindingType.Ship].SetSpeed75);
                    break;
                case "SetSpeedZero":
                    SendKeypress(App.Binding[BindingType.Ship].SetSpeedZero);
                    break;
                case "UseAlternateFlightValuesToggle":
                    SendKeypress(App.Binding[BindingType.Ship].UseAlternateFlightValuesToggle);
                    break;
                case "UseBoostJuice":
                    SendKeypress(App.Binding[BindingType.Ship].UseBoostJuice);
                    break;
                case "ForwardKey":
                    SendKeypress(App.Binding[BindingType.Ship].ForwardKey);
                    break;
                case "ForwardThrustButton":
                    SendKeypress(App.Binding[BindingType.Ship].ForwardThrustButton);
                    break;
                case "ForwardThrustButton_Landing":
                    SendKeypress(App.Binding[BindingType.Ship].ForwardThrustButton_Landing);
                    break;
                case "GalaxyMapOpen":
                    SendKeypress(App.Binding[BindingType.Ship].GalaxyMapOpen);
                    break;
                case "HyperSuperCombination":
                    SendKeypress(App.Binding[BindingType.Ship].HyperSuperCombination);
                    break;
                case "TargetNextRouteSystem":
                    SendKeypress(App.Binding[BindingType.Ship].TargetNextRouteSystem);
                    break;
                case "PitchDownButton":
                    SendKeypress(App.Binding[BindingType.Ship].PitchDownButton);
                    break;
                case "PitchDownButton_Landing":
                    SendKeypress(App.Binding[BindingType.Ship].PitchDownButton_Landing);
                    break;
                case "PitchUpButton":
                    SendKeypress(App.Binding[BindingType.Ship].PitchUpButton);
                    break;
                case "PitchUpButton_Landing":
                    SendKeypress(App.Binding[BindingType.Ship].PitchUpButton_Landing);
                    break;
                case "ToggleReverseThrottleInput":
                    SendKeypress(App.Binding[BindingType.Ship].ToggleReverseThrottleInput);
                    break;
                case "BackwardKey":
                    SendKeypress(App.Binding[BindingType.Ship].BackwardKey);
                    break;
                case "BackwardThrustButton":
                    SendKeypress(App.Binding[BindingType.Ship].BackwardThrustButton);
                    break;
                case "BackwardThrustButton_Landing":
                    SendKeypress(App.Binding[BindingType.Ship].BackwardThrustButton_Landing);
                    break;
                case "RollLeftButton":
                    SendKeypress(App.Binding[BindingType.Ship].RollLeftButton);
                    break;
                case "RollLeftButton_Landing":
                    SendKeypress(App.Binding[BindingType.Ship].RollLeftButton_Landing);
                    break;
                case "RollRightButton":
                    SendKeypress(App.Binding[BindingType.Ship].RollRightButton);
                    break;
                case "RollRightButton_Landing":
                    SendKeypress(App.Binding[BindingType.Ship].RollRightButton_Landing);
                    break;
                case "DisableRotationCorrectToggle":
                    SendKeypress(App.Binding[BindingType.Ship].DisableRotationCorrectToggle);
                    break;
                case "SystemMapOpen":
                    SendKeypress(App.Binding[BindingType.Ship].SystemMapOpen);
                    break;
                case "DownThrustButton":
                    SendKeypress(App.Binding[BindingType.Ship].DownThrustButton);
                    break;
                case "DownThrustButton_Landing":
                    SendKeypress(App.Binding[BindingType.Ship].DownThrustButton_Landing);
                    break;
                case "LeftThrustButton":
                    SendKeypress(App.Binding[BindingType.Ship].LeftThrustButton);
                    break;
                case "LeftThrustButton_Landing":
                    SendKeypress(App.Binding[BindingType.Ship].LeftThrustButton_Landing);
                    break;
                case "RightThrustButton":
                    SendKeypress(App.Binding[BindingType.Ship].RightThrustButton);
                    break;
                case "RightThrustButton_Landing":
                    SendKeypress(App.Binding[BindingType.Ship].RightThrustButton_Landing);
                    break;
                case "UpThrustButton":
                    SendKeypress(App.Binding[BindingType.Ship].UpThrustButton);
                    break;
                case "UpThrustButton_Landing":
                    SendKeypress(App.Binding[BindingType.Ship].UpThrustButton_Landing);
                    break;
                case "YawLeftButton":
                    SendKeypress(App.Binding[BindingType.Ship].YawLeftButton);
                    break;
                case "YawLeftButton_Landing":
                    SendKeypress(App.Binding[BindingType.Ship].YawLeftButton_Landing);
                    break;
                case "YawRightButton":
                    SendKeypress(App.Binding[BindingType.Ship].YawRightButton);
                    break;
                case "YawRightButton_Landing":
                    SendKeypress(App.Binding[BindingType.Ship].YawRightButton_Landing);
                    break;
                case "YawToRollButton":
                    SendKeypress(App.Binding[BindingType.Ship].YawToRollButton);
                    break;


                case "Supercruise":
                    SendKeypress(App.Binding[BindingType.Ship].Supercruise);
                    break;
                case "Hyperspace":
                    SendKeypress(App.Binding[BindingType.Ship].Hyperspace);
                    break;

                case "PlayerHUDModeToggle-AM":
                    isPrimary = EliteData.StatusData.HudInAnalysisMode;
                    if (!isPrimary)
                    {
                        SendKeypress(App.Binding[BindingType.Ship].PlayerHUDModeToggle);
                    }
                    break;
                case "PlayerHUDModeToggle-CM":
                    isPrimary = EliteData.StatusData.HudInAnalysisMode;
                    if (isPrimary)
                    {
                        SendKeypress(App.Binding[BindingType.Ship].PlayerHUDModeToggle);
                    }
                    break;

                case "ToggleCargoScoop-ON":
                    isPrimary = EliteData.StatusData.CargoScoopDeployed;
                    if (!isPrimary)
                    {
                        SendKeypress(App.Binding[BindingType.Ship].ToggleCargoScoop);
                    }
                    break;
                case "ToggleCargoScoop-OFF":
                    isPrimary = EliteData.StatusData.CargoScoopDeployed;
                    if (isPrimary)
                    {
                        SendKeypress(App.Binding[BindingType.Ship].ToggleCargoScoop);
                    }
                    break;

                case "ToggleFlightAssist-ON":
                    isPrimary = !EliteData.StatusData.FlightAssistOff;
                    if (!isPrimary)
                    {
                        SendKeypress(App.Binding[BindingType.Ship].ToggleFlightAssist);
                    }
                    break;
                case "ToggleFlightAssist-OFF":
                    isPrimary = !EliteData.StatusData.FlightAssistOff;
                    if (isPrimary)
                    {
                        SendKeypress(App.Binding[BindingType.Ship].ToggleFlightAssist);
                    }
                    break;

                case "DeployHardpointToggle-ON":
                    isPrimary = EliteData.StatusData.HardpointsDeployed;
                    if (!isPrimary)
                    {
                        SendKeypress(App.Binding[BindingType.Ship].DeployHardpointToggle);
                    }
                    break;
                case "DeployHardpointToggle-OFF":
                    isPrimary = EliteData.StatusData.HardpointsDeployed;
                    if (isPrimary)
                    {
                        SendKeypress(App.Binding[BindingType.Ship].DeployHardpointToggle);
                    }
                    break;

                case "LandingGearToggle-ON":
                    isPrimary = EliteData.StatusData.LandingGearDown;
                    if (!isPrimary)
                    {
                        SendKeypress(App.Binding[BindingType.Ship].LandingGearToggle);
                    }
                    break;
                case "LandingGearToggle-OFF":
                    isPrimary = EliteData.StatusData.LandingGearDown;
                    if (isPrimary)
                    {
                        SendKeypress(App.Binding[BindingType.Ship].LandingGearToggle);
                    }
                    break;

                case "ShipSpotLightToggle-ON":
                    isPrimary = EliteData.StatusData.LightsOn || EliteData.StatusData.SrvHighBeam;
                    if (!isPrimary)
                    {
                        SendKeypress(App.Binding[BindingType.Ship].ShipSpotLightToggle);
                    }
                    break;
                case "ShipSpotLightToggle-OFF":
                    isPrimary = EliteData.StatusData.LightsOn || EliteData.StatusData.SrvHighBeam;
                    if (isPrimary)
                    {
                        SendKeypress(App.Binding[BindingType.Ship].ShipSpotLightToggle);
                    }
                    break;

                case "NightVisionToggle-ON":
                    isPrimary = EliteData.StatusData.NightVision;
                    if (!isPrimary)
                    {
                        SendKeypress(App.Binding[BindingType.Ship].NightVisionToggle);
                    }
                    break;
                case "NightVisionToggle-OFF":
                    isPrimary = EliteData.StatusData.NightVision;
                    if (isPrimary)
                    {
                        SendKeypress(App.Binding[BindingType.Ship].NightVisionToggle);
                    }
                    break;

                case "ToggleButtonUpInput-ON":
                    isPrimary = EliteData.StatusData.SilentRunning;
                    if (!isPrimary)
                    {
                        SendKeypress(App.Binding[BindingType.Ship].ToggleButtonUpInput);
                    }
                    break;
                case "ToggleButtonUpInput-OFF":
                    isPrimary = EliteData.StatusData.SilentRunning;
                    if (isPrimary)
                    {
                        SendKeypress(App.Binding[BindingType.Ship].ToggleButtonUpInput);
                    }
                    break;

                // fire groups 

                case "FireGroup-A":
                    HandleFireGroup(0);
                    break;
                case "FireGroup-B":
                    HandleFireGroup(1);
                    break;
                case "FireGroup-C":
                    HandleFireGroup(2);
                    break;
                case "FireGroup-D":
                    HandleFireGroup(3);
                    break;
                case "FireGroup-E":
                    HandleFireGroup(4);
                    break;
                case "FireGroup-F":
                    HandleFireGroup(5);
                    break;
                case "FireGroup-G":
                    HandleFireGroup(6);
                    break;
                case "FireGroup-H":
                    HandleFireGroup(7);
                    break;

                // general

                case "CycleNextPage":
                    SendKeypress(App.Binding[BindingType.General].CycleNextPage);
                    break;
                case "CycleNextPanel":
                    SendKeypress(App.Binding[BindingType.General].CycleNextPanel);
                    break;
                case "CyclePreviousPage":
                    SendKeypress(App.Binding[BindingType.General].CyclePreviousPage);
                    break;
                case "CyclePreviousPanel":
                    SendKeypress(App.Binding[BindingType.General].CyclePreviousPanel);
                    break;
                case "UI_Back":
                    SendKeypress(App.Binding[BindingType.General].UI_Back);
                    break;
                case "UI_Down":
                    SendKeypress(App.Binding[BindingType.General].UI_Down);
                    break;
                case "UI_Left":
                    SendKeypress(App.Binding[BindingType.General].UI_Left);
                    break;
                case "UI_Right":
                    SendKeypress(App.Binding[BindingType.General].UI_Right);
                    break;
                case "UI_Select":
                    SendKeypress(App.Binding[BindingType.General].UI_Select);
                    break;
                case "UI_Toggle":
                    SendKeypress(App.Binding[BindingType.General].UI_Toggle);
                    break;
                case "UI_Up":
                    SendKeypress(App.Binding[BindingType.General].UI_Up);
                    break;

                case "CamTranslateBackward":
                    SendKeypress(App.Binding[BindingType.General].CamTranslateBackward);
                    break;
                case "CamTranslateDown":
                    SendKeypress(App.Binding[BindingType.General].CamTranslateDown);
                    break;
                case "CamTranslateForward":
                    SendKeypress(App.Binding[BindingType.General].CamTranslateForward);
                    break;
                case "CamTranslateLeft":
                    SendKeypress(App.Binding[BindingType.General].CamTranslateLeft);
                    break;
                case "CamPitchDown":
                    SendKeypress(App.Binding[BindingType.General].CamPitchDown);
                    break;
                case "CamPitchUp":
                    SendKeypress(App.Binding[BindingType.General].CamPitchUp);
                    break;
                case "CamTranslateRight":
                    SendKeypress(App.Binding[BindingType.General].CamTranslateRight);
                    break;
                case "CamTranslateUp":
                    SendKeypress(App.Binding[BindingType.General].CamTranslateUp);
                    break;
                case "CamYawLeft":
                    SendKeypress(App.Binding[BindingType.General].CamYawLeft);
                    break;
                case "CamYawRight":
                    SendKeypress(App.Binding[BindingType.General].CamYawRight);
                    break;
                case "CamTranslateZHold":
                    SendKeypress(App.Binding[BindingType.General].CamTranslateZHold);
                    break;
                case "CamZoomIn":
                    SendKeypress(App.Binding[BindingType.General].CamZoomIn);
                    break;
                case "CamZoomOut":
                    SendKeypress(App.Binding[BindingType.General].CamZoomOut);
                    break;

                case "MoveFreeCamBackwards":
                    SendKeypress(App.Binding[BindingType.General].MoveFreeCamBackwards);
                    break;
                case "MoveFreeCamDown":
                    SendKeypress(App.Binding[BindingType.General].MoveFreeCamDown);
                    break;
                case "MoveFreeCamForward":
                    SendKeypress(App.Binding[BindingType.General].MoveFreeCamForward);
                    break;
                case "MoveFreeCamLeft":
                    SendKeypress(App.Binding[BindingType.General].MoveFreeCamLeft);
                    break;
                case "ToggleReverseThrottleInputFreeCam":
                    SendKeypress(App.Binding[BindingType.General].ToggleReverseThrottleInputFreeCam);
                    break;
                case "MoveFreeCamRight":
                    SendKeypress(App.Binding[BindingType.General].MoveFreeCamRight);
                    break;
                case "MoveFreeCamUp":
                    SendKeypress(App.Binding[BindingType.General].MoveFreeCamUp);
                    break;
                case "FreeCamSpeedDec":
                    SendKeypress(App.Binding[BindingType.General].FreeCamSpeedDec);
                    break;
                case "ToggleFreeCam":
                    SendKeypress(App.Binding[BindingType.General].ToggleFreeCam);
                    break;
                case "FreeCamSpeedInc":
                    SendKeypress(App.Binding[BindingType.General].FreeCamSpeedInc);
                    break;
                case "FreeCamToggleHUD":
                    SendKeypress(App.Binding[BindingType.General].FreeCamToggleHUD);
                    break;
                case "FreeCamZoomIn":
                    SendKeypress(App.Binding[BindingType.General].FreeCamZoomIn);
                    break;
                case "FreeCamZoomOut":
                    SendKeypress(App.Binding[BindingType.General].FreeCamZoomOut);
                    break;

                case "PhotoCameraToggle":
                    SendKeypress(App.Binding[BindingType.General].PhotoCameraToggle);
                    break;
                case "StorePitchCameraDown":
                    SendKeypress(App.Binding[BindingType.General].StorePitchCameraDown);
                    break;
                case "StorePitchCameraUp":
                    SendKeypress(App.Binding[BindingType.General].StorePitchCameraUp);
                    break;
                case "StoreEnableRotation":
                    SendKeypress(App.Binding[BindingType.General].StoreEnableRotation);
                    break;
                case "StoreYawCameraLeft":
                    SendKeypress(App.Binding[BindingType.General].StoreYawCameraLeft);
                    break;
                case "StoreYawCameraRight":
                    SendKeypress(App.Binding[BindingType.General].StoreYawCameraRight);
                    break;
                case "StoreCamZoomIn":
                    SendKeypress(App.Binding[BindingType.General].StoreCamZoomIn);
                    break;
                case "StoreCamZoomOut":
                    SendKeypress(App.Binding[BindingType.General].StoreCamZoomOut);
                    break;
                case "StoreToggle":
                    SendKeypress(App.Binding[BindingType.General].StoreToggle);
                    break;
                case "ToggleAdvanceMode":
                    SendKeypress(App.Binding[BindingType.General].ToggleAdvanceMode);
                    break;
                case "VanityCameraEight":
                    SendKeypress(App.Binding[BindingType.General].VanityCameraEight);
                    break;
                case "VanityCameraTwo":
                    SendKeypress(App.Binding[BindingType.General].VanityCameraTwo);
                    break;
                case "VanityCameraOne":
                    SendKeypress(App.Binding[BindingType.General].VanityCameraOne);
                    break;
                case "VanityCameraThree":
                    SendKeypress(App.Binding[BindingType.General].VanityCameraThree);
                    break;
                case "VanityCameraFour":
                    SendKeypress(App.Binding[BindingType.General].VanityCameraFour);
                    break;
                case "VanityCameraFive":
                    SendKeypress(App.Binding[BindingType.General].VanityCameraFive);
                    break;
                case "VanityCameraSix":
                    SendKeypress(App.Binding[BindingType.General].VanityCameraSix);
                    break;
                case "VanityCameraSeven":
                    SendKeypress(App.Binding[BindingType.General].VanityCameraSeven);
                    break;
                case "VanityCameraNine":
                    SendKeypress(App.Binding[BindingType.General].VanityCameraNine);
                    break;
                case "VanityCameraTen":
                    SendKeypress(App.Binding[BindingType.General].VanityCameraTen);
                    break;
                case "PitchCameraDown":
                    SendKeypress(App.Binding[BindingType.General].PitchCameraDown);
                    break;
                case "PitchCameraUp":
                    SendKeypress(App.Binding[BindingType.General].PitchCameraUp);
                    break;
                case "RollCameraLeft":
                    SendKeypress(App.Binding[BindingType.General].RollCameraLeft);
                    break;
                case "RollCameraRight":
                    SendKeypress(App.Binding[BindingType.General].RollCameraRight);
                    break;
                case "ToggleRotationLock":
                    SendKeypress(App.Binding[BindingType.General].ToggleRotationLock);
                    break;
                case "YawCameraLeft":
                    SendKeypress(App.Binding[BindingType.General].YawCameraLeft);
                    break;
                case "YawCameraRight":
                    SendKeypress(App.Binding[BindingType.General].YawCameraRight);
                    break;
                case "FStopDec":
                    SendKeypress(App.Binding[BindingType.General].FStopDec);
                    break;
                case "QuitCamera":
                    SendKeypress(App.Binding[BindingType.General].QuitCamera);
                    break;
                case "FocusDistanceInc":
                    SendKeypress(App.Binding[BindingType.General].FocusDistanceInc);
                    break;
                case "FocusDistanceDec":
                    SendKeypress(App.Binding[BindingType.General].FocusDistanceDec);
                    break;
                case "FStopInc":
                    SendKeypress(App.Binding[BindingType.General].FStopInc);
                    break;
                case "FixCameraRelativeToggle":
                    SendKeypress(App.Binding[BindingType.General].FixCameraRelativeToggle);
                    break;
                case "FixCameraWorldToggle":
                    SendKeypress(App.Binding[BindingType.General].FixCameraWorldToggle);
                    break;
                case "VanityCameraScrollRight":
                    SendKeypress(App.Binding[BindingType.General].VanityCameraScrollRight);
                    break;
                case "VanityCameraScrollLeft":
                    SendKeypress(App.Binding[BindingType.General].VanityCameraScrollLeft);
                    break;

                case "CommanderCreator_Redo":
                    SendKeypress(App.Binding[BindingType.General].CommanderCreator_Redo);
                    break;
                case "CommanderCreator_Rotation":
                    SendKeypress(App.Binding[BindingType.General].CommanderCreator_Rotation);
                    break;
                case "CommanderCreator_Rotation_MouseToggle":
                    SendKeypress(App.Binding[BindingType.General].CommanderCreator_Rotation_MouseToggle);
                    break;
                case "CommanderCreator_Undo":
                    SendKeypress(App.Binding[BindingType.General].CommanderCreator_Undo);
                    break;

                case "GalnetAudio_ClearQueue":
                    SendKeypress(App.Binding[BindingType.General].GalnetAudio_ClearQueue);
                    break;
                case "GalnetAudio_SkipForward":
                    SendKeypress(App.Binding[BindingType.General].GalnetAudio_SkipForward);
                    break;
                case "GalnetAudio_Play_Pause":
                    SendKeypress(App.Binding[BindingType.General].GalnetAudio_Play_Pause);
                    break;
                case "GalnetAudio_SkipBackward":
                    SendKeypress(App.Binding[BindingType.General].GalnetAudio_SkipBackward);
                    break;

                // in srv

                case "ToggleDriveAssist":
                    SendKeypress(App.Binding[BindingType.Srv].ToggleDriveAssist);
                    break;
                case "SteerLeftButton":
                    SendKeypress(App.Binding[BindingType.Srv].SteerLeftButton);
                    break;
                case "SteerRightButton":
                    SendKeypress(App.Binding[BindingType.Srv].SteerRightButton);
                    break;
                case "IncreaseSpeedButtonMax":
                    SendKeypress(App.Binding[BindingType.Srv].IncreaseSpeedButtonMax);
                    break;
                case "DecreaseSpeedButtonMax":
                    SendKeypress(App.Binding[BindingType.Srv].DecreaseSpeedButtonMax);
                    break;
                case "IncreaseSpeedButtonPartial":
                    SendKeypress(App.Binding[BindingType.Srv].IncreaseSpeedButtonPartial);
                    break;
                case "DecreaseSpeedButtonPartial":
                    SendKeypress(App.Binding[BindingType.Srv].DecreaseSpeedButtonPartial);
                    break;
                case "RecallDismissShip":
                    SendKeypress(App.Binding[BindingType.Srv].RecallDismissShip);
                    break;
                case "VerticalThrustersButton":
                    SendKeypress(App.Binding[BindingType.Srv].VerticalThrustersButton);
                    break;

                case "PhotoCameraToggle_Buggy":
                    SendKeypress(App.Binding[BindingType.Srv].PhotoCameraToggle_Buggy);
                    break;
                case "FocusCommsPanel_Buggy":
                    SendKeypress(App.Binding[BindingType.Srv].FocusCommsPanel_Buggy);
                    break;
                case "EjectAllCargo_Buggy":
                    SendKeypress(App.Binding[BindingType.Srv].EjectAllCargo_Buggy);
                    break;
                case "FocusLeftPanel_Buggy":
                    SendKeypress(App.Binding[BindingType.Srv].FocusLeftPanel_Buggy);
                    break;
                case "QuickCommsPanel_Buggy":
                    SendKeypress(App.Binding[BindingType.Srv].QuickCommsPanel_Buggy);
                    break;
                case "FocusRadarPanel_Buggy":
                    SendKeypress(App.Binding[BindingType.Srv].FocusRadarPanel_Buggy);
                    break;
                case "FocusRightPanel_Buggy":
                    SendKeypress(App.Binding[BindingType.Srv].FocusRightPanel_Buggy);
                    break;
                case "HeadLookToggle_Buggy":
                    SendKeypress(App.Binding[BindingType.Srv].HeadLookToggle_Buggy);
                    break;
                case "UIFocus_Buggy":
                    SendKeypress(App.Binding[BindingType.Srv].UIFocus_Buggy);
                    break;
                case "IncreaseEnginesPower_Buggy":
                    SendKeypress(App.Binding[BindingType.Srv].IncreaseEnginesPower_Buggy);
                    break;
                case "BuggyPrimaryFireButton":
                    SendKeypress(App.Binding[BindingType.Srv].BuggyPrimaryFireButton);
                    break;
                case "ResetPowerDistribution_Buggy":
                    SendKeypress(App.Binding[BindingType.Srv].ResetPowerDistribution_Buggy);
                    break;
                case "BuggySecondaryFireButton":
                    SendKeypress(App.Binding[BindingType.Srv].BuggySecondaryFireButton);
                    break;
                case "IncreaseSystemsPower_Buggy":
                    SendKeypress(App.Binding[BindingType.Srv].IncreaseSystemsPower_Buggy);
                    break;
                case "SelectTarget_Buggy":
                    SendKeypress(App.Binding[BindingType.Srv].SelectTarget_Buggy);
                    break;
                case "BuggyTurretPitchDownButton":
                    SendKeypress(App.Binding[BindingType.Srv].BuggyTurretPitchDownButton);
                    break;
                case "BuggyTurretYawLeftButton":
                    SendKeypress(App.Binding[BindingType.Srv].BuggyTurretYawLeftButton);
                    break;
                case "ToggleBuggyTurretButton":
                    SendKeypress(App.Binding[BindingType.Srv].ToggleBuggyTurretButton);
                    break;
                case "BuggyTurretYawRightButton":
                    SendKeypress(App.Binding[BindingType.Srv].BuggyTurretYawRightButton);
                    break;
                case "BuggyTurretPitchUpButton":
                    SendKeypress(App.Binding[BindingType.Srv].BuggyTurretPitchUpButton);
                    break;
                case "IncreaseWeaponsPower_Buggy":
                    SendKeypress(App.Binding[BindingType.Srv].IncreaseWeaponsPower_Buggy);
                    break;
                case "ToggleCargoScoop_Buggy":
                    SendKeypress(App.Binding[BindingType.Srv].ToggleCargoScoop_Buggy);
                    break;
                case "GalaxyMapOpen_Buggy":
                    SendKeypress(App.Binding[BindingType.Srv].GalaxyMapOpen_Buggy);
                    break;
                case "AutoBreakBuggyButton":
                    SendKeypress(App.Binding[BindingType.Srv].AutoBreakBuggyButton);
                    break;
                case "HeadlightsBuggyButton":
                    SendKeypress(App.Binding[BindingType.Srv].HeadlightsBuggyButton);
                    break;
                case "BuggyPitchDownButton":
                    SendKeypress(App.Binding[BindingType.Srv].BuggyPitchDownButton);
                    break;
                case "BuggyPitchUpButton":
                    SendKeypress(App.Binding[BindingType.Srv].BuggyPitchUpButton);
                    break;
                case "BuggyToggleReverseThrottleInput":
                    SendKeypress(App.Binding[BindingType.Srv].BuggyToggleReverseThrottleInput);
                    break;
                case "BuggyRollLeft":
                    SendKeypress(App.Binding[BindingType.Srv].BuggyRollLeft);
                    break;
                case "BuggyRollLeftButton":
                    SendKeypress(App.Binding[BindingType.Srv].BuggyRollLeftButton);
                    break;
                case "BuggyRollRight":
                    SendKeypress(App.Binding[BindingType.Srv].BuggyRollRight);
                    break;
                case "BuggyRollRightButton":
                    SendKeypress(App.Binding[BindingType.Srv].BuggyRollRightButton);
                    break;
                case "SystemMapOpen_Buggy":
                    SendKeypress(App.Binding[BindingType.Srv].SystemMapOpen_Buggy);
                    break;

                // on foot

                case "HumanoidClearAuthorityLevel":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidClearAuthorityLevel);
                    break;
                case "HumanoidHealthPack":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidHealthPack);
                    break;
                case "HumanoidBattery":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidBattery);
                    break;
                case "HumanoidSelectFragGrenade":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidSelectFragGrenade);
                    break;
                case "HumanoidSelectEMPGrenade":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidSelectEMPGrenade);
                    break;
                case "HumanoidSelectShieldGrenade":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidSelectShieldGrenade);
                    break;

                case "PhotoCameraToggle_Humanoid":
                    SendKeypress(App.Binding[BindingType.OnFoot].PhotoCameraToggle_Humanoid);
                    break;
                case "HumanoidForwardButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidForwardButton);
                    break;
                case "HumanoidBackwardButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidBackwardButton);
                    break;
                case "HumanoidStrafeLeftButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidStrafeLeftButton);
                    break;
                case "HumanoidStrafeRightButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidStrafeRightButton);
                    break;
                case "HumanoidSprintButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidSprintButton);
                    break;
                case "HumanoidCrouchButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidCrouchButton);
                    break;
                case "HumanoidJumpButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidJumpButton);
                    break;
                case "HumanoidPrimaryInteractButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidPrimaryInteractButton);
                    break;
                case "HumanoidItemWheelButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidItemWheelButton);
                    break;
                case "HumanoidEmoteWheelButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidEmoteWheelButton);
                    break;
                case "HumanoidUtilityWheelCycleMode":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidUtilityWheelCycleMode);
                    break;

                case "HumanoidPrimaryFireButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidPrimaryFireButton);
                    break;
                case "HumanoidSelectPrimaryWeaponButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidSelectPrimaryWeaponButton);
                    break;
                case "HumanoidSelectSecondaryWeaponButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidSelectSecondaryWeaponButton);
                    break;
                case "HumanoidHideWeaponButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidHideWeaponButton);
                    break;
                case "HumanoidZoomButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidZoomButton);
                    break;
                case "HumanoidReloadButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidReloadButton);
                    break;
                case "HumanoidThrowGrenadeButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidThrowGrenadeButton);
                    break;
                case "HumanoidMeleeButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidMeleeButton);
                    break;
                case "HumanoidOpenAccessPanelButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidOpenAccessPanelButton);
                    break;
                case "HumanoidSecondaryInteractButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidSecondaryInteractButton);
                    break;
                case "HumanoidSwitchToRechargeTool":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidSwitchToRechargeTool);
                    break;
                case "HumanoidSwitchToCompAnalyser":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidSwitchToCompAnalyser);
                    break;
                case "HumanoidToggleToolModeButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidToggleToolModeButton);
                    break;
                case "HumanoidToggleNightVisionButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidToggleNightVisionButton);
                    break;
                case "HumanoidSwitchToSuitTool":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidSwitchToSuitTool);
                    break;
                case "HumanoidToggleFlashlightButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidToggleFlashlightButton);
                    break;
                case "GalaxyMapOpen_Humanoid":
                    SendKeypress(App.Binding[BindingType.OnFoot].GalaxyMapOpen_Humanoid);
                    break;
                case "SystemMapOpen_Humanoid":
                    SendKeypress(App.Binding[BindingType.OnFoot].SystemMapOpen_Humanoid);
                    break;
                case "FocusCommsPanel_Humanoid":
                    SendKeypress(App.Binding[BindingType.OnFoot].FocusCommsPanel_Humanoid);
                    break;
                case "QuickCommsPanel_Humanoid":
                    SendKeypress(App.Binding[BindingType.OnFoot].QuickCommsPanel_Humanoid);
                    break;
                case "HumanoidConflictContextualUIButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidConflictContextualUIButton);
                    break;
                case "HumanoidToggleShieldsButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidToggleShieldsButton);
                    break;

                case "HumanoidRotateLeftButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidRotateLeftButton);
                    break;
                case "HumanoidRotateRightButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidRotateRightButton);
                    break;
                case "HumanoidPitchUpButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidPitchUpButton);
                    break;
                case "HumanoidPitchDownButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidPitchDownButton);
                    break;
                case "HumanoidSwitchWeapon":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidSwitchWeapon);
                    break;
                case "HumanoidSelectUtilityWeaponButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidSelectUtilityWeaponButton);
                    break;
                case "HumanoidSelectNextWeaponButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidSelectNextWeaponButton);
                    break;
                case "HumanoidSelectPreviousWeaponButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidSelectPreviousWeaponButton);
                    break;
                case "HumanoidSelectNextGrenadeTypeButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidSelectNextGrenadeTypeButton);
                    break;
                case "HumanoidSelectPreviousGrenadeTypeButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidSelectPreviousGrenadeTypeButton);
                    break;
                case "HumanoidToggleMissionHelpPanelButton":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidToggleMissionHelpPanelButton);
                    break;
                case "HumanoidPing":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidPing);
                    break;

                case "HumanoidEmoteSlot1":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidEmoteSlot1);
                    break;
                case "HumanoidEmoteSlot2":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidEmoteSlot2);
                    break;
                case "HumanoidEmoteSlot3":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidEmoteSlot3);
                    break;
                case "HumanoidEmoteSlot4":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidEmoteSlot4);
                    break;
                case "HumanoidEmoteSlot5":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidEmoteSlot5);
                    break;
                case "HumanoidEmoteSlot6":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidEmoteSlot6);
                    break;
                case "HumanoidEmoteSlot7":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidEmoteSlot7);
                    break;
                case "HumanoidEmoteSlot8":
                    SendKeypress(App.Binding[BindingType.OnFoot].HumanoidEmoteSlot8);
                    break;

                //-----------------------
                
                case "PrimaryFire-DOWN":
                    SendKeypressDown(App.Binding[BindingType.Ship].PrimaryFire);
                    break;
                case "PrimaryFire-UP":
                    SendKeypressUp(App.Binding[BindingType.Ship].PrimaryFire);
                    break;

                case "SecondaryFire-DOWN":
                    SendKeypressDown(App.Binding[BindingType.Ship].SecondaryFire);
                    break;
                case "SecondaryFire-UP":
                    SendKeypressUp(App.Binding[BindingType.Ship].SecondaryFire);
                    break;


            }

        }


    }
}
