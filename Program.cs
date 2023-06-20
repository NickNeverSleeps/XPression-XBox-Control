// See https://aka.ms/new-console-template for more information
using SharpDX;
using SharpDX.XInput;
using System.Numerics;
using System.Threading;
using XPression;


xpEngine engine = new xpEngine();
xpSequencer sequencer = engine.Sequencer;


while (true)
{
    Controller controller = new Controller(UserIndex.One);
    State state = controller.GetState();
    Thread.Sleep(150);

if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.A))
{
        xpBaseTakeItem takeItem;
        sequencer.GetFocusedTakeItem(out takeItem);
        takeItem.Execute();
}

    if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.B))
    {
        xpBaseTakeItem takeItem;
        sequencer.GetFocusedTakeItem(out takeItem);
        takeItem.SetOffline();
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
        sequencer.GetFocusedTakeItem(out  currentItem);
        newItem = (xpTakeItem)currentItem;
        for (int i = 0; i < sequencer.ItemCount - 1; i++){
            sequencer.GetTakeItemByIndex(i, out intItem);
            if(intItem.ID == currentItem.ID)
            {
                sequencer.GetTakeItemByIndex(i+1, out newItem);
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
    if(rightTrigger != 0)
    {
        Console.WriteLine(rightTrigger.ToString());
    }
    float rightThumbstickX = state.Gamepad.RightThumbX;

}
