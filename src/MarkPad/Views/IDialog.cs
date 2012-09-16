using System;

namespace MarkPad.Views
{
    public interface IDialog
    {
        Action Cancelled { get; set; }
    }
}