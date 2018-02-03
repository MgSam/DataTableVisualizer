using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DataTableViewer
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand CopyWithHeaders = new RoutedUICommand(
            text: "Copy With Headers",
            name: "CopyWithHeaders",
            ownerType: typeof(CustomCommands),
            inputGestures:new InputGestureCollection()
            {
                new KeyGesture(Key.C, ModifierKeys.Control | ModifierKeys.Shift)
            }
        );
    }
}
