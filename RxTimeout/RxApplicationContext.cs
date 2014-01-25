using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using RxTimeout.Forms;

namespace RxTimeout
{
  class RxApplicationContext : ApplicationContext
  {
    int _formCount;

    public RxApplicationContext()
    {
      var alex = new AlexForm { StartPosition = FormStartPosition.CenterScreen };
      Show(alex);

      var mike = new MikesForm
                 {
                   StartPosition = FormStartPosition.Manual,
                   Location = new Point(alex.Location.X + alex.Size.Height, alex.Location.Y)
                 };
      Show(mike);

      new Coordinator().Start();
    }

    void Show(Form form)
    {
      form.Closed += OnFormClosed;

      Interlocked.Increment(ref _formCount);
      form.Show();
    }

    void OnFormClosed(object sender, EventArgs e)
    {
      if (Interlocked.Decrement(ref _formCount) <= 0)
      {
        ExitThread();
      }
    }
  }
}
