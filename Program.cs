// See https://aka.ms/new-console-template for more information
using SharpDX.XInput;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XPression;

class XPControl
{
    static Controller controller = new Controller(UserIndex.One);
    static GamepadButtonFlags previousButtonState = GamepadButtonFlags.None;
    static Stopwatch timer = new Stopwatch();
    static int debounceThresholdMs = 200;
    static Dictionary<GamepadButtonFlags, ButtonInfo> buttonStates = new Dictionary<GamepadButtonFlags, ButtonInfo>();



    static void Main()
    {
        InitializeButtonStates();

        // Game loop or update loop where you read the button state
        while (true)
        {
            State state = controller.GetState();

            foreach (var kvp in buttonStates)
            {
                GamepadButtonFlags button = kvp.Key;
                ButtonInfo buttonInfo = kvp.Value;

                if (state.Gamepad.Buttons != buttonInfo.PreviousButtonState)
                {
                    if (state.Gamepad.Buttons.HasFlag(button))
                    {
                        if (!buttonInfo.Timer.IsRunning)
                        {
                            // Start the timer on button press
                            buttonInfo.Timer.Restart();
                            ProcessButtonPress(state);
                        }
                    }
                    else
                    {
                        // Reset the timer on button release
                        buttonInfo.Timer.Reset();
                    }
                }
                else if (buttonInfo.Timer.IsRunning && buttonInfo.Timer.ElapsedMilliseconds >= buttonInfo.DebounceThresholdMs)
                {
                    // Timer expired, process the button press
                    buttonInfo.Timer.Stop();
                    ProcessButtonPress(state);
                }

                buttonInfo.PreviousButtonState = state.Gamepad.Buttons;
            }
        }
    }

    static void InitializeButtonStates()
    {
        buttonStates.Add(GamepadButtonFlags.A, new ButtonInfo { DebounceThresholdMs = 200 });
        buttonStates.Add(GamepadButtonFlags.B, new ButtonInfo { DebounceThresholdMs = 150 });
        buttonStates.Add(GamepadButtonFlags.X, new ButtonInfo { DebounceThresholdMs = 200 });
        buttonStates.Add(GamepadButtonFlags.Y, new ButtonInfo { DebounceThresholdMs = 200 });
        buttonStates.Add(GamepadButtonFlags.DPadLeft, new ButtonInfo { DebounceThresholdMs = 200 });
        buttonStates.Add(GamepadButtonFlags.DPadRight, new ButtonInfo { DebounceThresholdMs = 200 });
        buttonStates.Add(GamepadButtonFlags.DPadUp, new ButtonInfo { DebounceThresholdMs = 200 });
        buttonStates.Add(GamepadButtonFlags.DPadDown, new ButtonInfo { DebounceThresholdMs = 200 });
    }

    class ButtonInfo
    {
        public Stopwatch Timer { get; } = new Stopwatch();
        public GamepadButtonFlags PreviousButtonState { get; set; }
        public int DebounceThresholdMs { get; set; }
    }

    static void ProcessButtonPress(State state)
    {
        xpEngine engine = new xpEngine();
        xpSequencer sequencer = engine.Sequencer;
        if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.A))
        {
            xpBaseTakeItem takeItem;
            sequencer.GetFocusedTakeItem(out takeItem);
            takeItem.Execute();
            vibrateShort(controller);
        }

        if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.B))
        {
            xpBaseTakeItem takeItem;
            sequencer.GetFocusedTakeItem(out takeItem);
            takeItem.SetOffline();
            vibrateMed(controller);
        }
        if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Y))
        {
            xpOutputFrameBuffer preview;
            engine.GetPreviewFrameBuffer(out preview);
            xpBaseTakeItem takeItem;
            sequencer.GetFocusedTakeItem(out takeItem);
            //future: run preview
        }
        if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadDown))
        {
            xpBaseTakeItem currentItem;
            xpTakeItem intItem;
            xpTakeItem newItem;
            sequencer.GetFocusedTakeItem(out currentItem);
            newItem = (xpTakeItem)currentItem;
            for (int i = 0; i < sequencer.ItemCount - 1; i++)
            {
                sequencer.GetTakeItemByIndex(i, out intItem);
                if (intItem.ID == currentItem.ID)
                {
                    sequencer.GetTakeItemByIndex(i + 1, out newItem);
                }
            }
            newItem.SetFocus();
        }

        if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadLeft))
        {
            xpBaseTakeItem focused;
            xpTakeItemGroup group;
            sequencer.GetFocusedTakeItem(out focused);
            if (focused.GetType() == typeof(xpTakeItemGroup))
            {
                group = (xpTakeItemGroup)focused;
                group.Expanded = false;
            }
            else
            {
                xpBaseTakeItem currentItem;
                xpTakeItem intItem;
                xpTakeItem newItem;
                sequencer.GetFocusedTakeItem(out currentItem);
                newItem = (xpTakeItem)currentItem;
                newItem.GetGroup(out group);
                group.SetFocus();
            }
            
        }


        if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadUp))
        {
            xpBaseTakeItem currentItem;
            xpTakeItem intItem;
            xpTakeItem newItem;
            sequencer.GetFocusedTakeItem(out currentItem);
            newItem = (xpTakeItem)currentItem;
            for (int i = 0; i < sequencer.ItemCount - 1; i++)
            {
                sequencer.GetTakeItemByIndex(i, out intItem);
                if (intItem.ID == currentItem.ID)
                {
                    sequencer.GetTakeItemByIndex(i - 1, out newItem);
                }
            }
            newItem.SetFocus();
        }
        if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadLeft))
        {
            xpBaseTakeItem currentItem;
            xpBaseTakeItem newItem;
            sequencer.GetFocusedTakeItem(out currentItem);
            sequencer.GetTakeItemByID(currentItem.ID - 1, out newItem);
            newItem.SetFocus();
        }

        float rightTrigger = state.Gamepad.RightTrigger;
        if (rightTrigger != 0)
        {
            Console.WriteLine(rightTrigger.ToString());
        }
        float rightThumbstickX = state.Gamepad.RightThumbX;
    }

    private static void vibrateShort(Controller controller)
    {
        Vibration vibrationOn = new Vibration();
        Vibration vibrationOff = new Vibration();
        vibrationOn.RightMotorSpeed = 25000;
        vibrationOn.LeftMotorSpeed = 25000;
        vibrationOff.RightMotorSpeed = 0;
        vibrationOff.LeftMotorSpeed = 0;
        controller.SetVibration(vibrationOn);
        Thread.Sleep(250);
        controller.SetVibration(vibrationOff);
    }

    private static void vibrateMed(Controller controller)
    {
        Vibration vibrationOn = new Vibration();
        Vibration vibrationOff = new Vibration();
        vibrationOn.RightMotorSpeed = 45000;
        vibrationOn.LeftMotorSpeed = 45000;
        vibrationOff.RightMotorSpeed = 0;
        vibrationOff.LeftMotorSpeed = 0;
        controller.SetVibration(vibrationOn);
        Thread.Sleep(350);
        controller.SetVibration(vibrationOff);
    }
}