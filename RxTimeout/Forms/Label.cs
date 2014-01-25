using System;
using System.Drawing;
using System.Windows.Forms;

namespace RxTimeout.Forms
{
  public static class Label
  {
    public static System.Windows.Forms.Label Create()
    {
      return new System.Windows.Forms.Label
             {
               Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
               Name = "message" + Guid.NewGuid(),
               Padding = new Padding(2),
               TextAlign = ContentAlignment.BottomLeft,
               UseMnemonic = false,
               Visible = false
             };
    }

    public static System.Windows.Forms.Label Create(string text, Form parent)
    {
      var label = Create();
      label.Text = text;

      label.Size = label.GetPreferredSize(parent.Size);
      if (label.Width < parent.ClientRectangle.Width)
      {
        label.Width += 15;
      }

      label.Visible = true;
      parent.Controls.Add(label);
      parent.Controls.SetChildIndex(label, 0);

      return label;
    }
  }
}
