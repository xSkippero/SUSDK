namespace SUSDK.Console
{
    public class DebugCommandBase
    {
        protected DebugCommandBase(string id, string description, string format)
        {
            CommandId = id;
            CommandDescription = description;
            CommandFormat = format;
        }

        public string CommandId { get; }

        public string CommandDescription { get; }

        public string CommandFormat { get; }
    }
}