// See https://aka.ms/new-console-template for more information
using SharpDX.XInput;
using XPression;

internal class XPControl
{
    private static void Main()
    {
        while (true)
        {
            Controller controller = new Controller(UserIndex.One);
            State state = controller.GetState();
            doTasks(controller, state);
            //Thread.Sleep(150);
        }
    }

    private static void doTasks(Controller controller, State state)
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