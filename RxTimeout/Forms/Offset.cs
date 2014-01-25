using System.Drawing;
using System.Windows.Forms;

namespace RxTimeout.Forms
{
  class Offset
  {
    readonly Form _parent;
    int _offset;

    public Offset(Form parent)
    {
      _parent = parent;
    }

    public void Apply(Control control)
    {
      control.Location = new Point(1, _parent.ClientRectangle.Height - control.Height - 1 - _offset);
      _offset += 15;
    }
  }
}
