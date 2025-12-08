using System;

namespace SUSDK.Console
{
  public class DebugCommand : DebugCommandBase
  {
    private readonly Action _command;

    public DebugCommand(string id, string description, string format, Action command) : base(id, description, format)
    {
      this._command = command;
    }

    public void Invoke()
    {
      _command.Invoke();
    }
  }

  public class DebugCommand<T1> : DebugCommandBase
  {
    private readonly Action<T1> _command;

    public DebugCommand(string id, string description, string format, Action<T1> command) : base(id, description, format)
    {
      this._command = command;
    }

    public void Invoke(T1 value)
    {
      _command.Invoke(value);
    }
  }

  public class DebugCommand<T1, T2> : DebugCommandBase
  {
    private readonly Action<T1, T2> _command;

    public DebugCommand(string id, string description, string format, Action<T1, T2> command) : base(id, description,
      format)
    {
      this._command = command;
    }

    public void Invoke(T1 value, T2 secondvalue)
    {
      _command.Invoke(value, secondvalue);
    }
  }
}