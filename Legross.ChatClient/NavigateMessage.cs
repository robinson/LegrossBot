namespace Legross.ChatClient
{
    class NavigateMessage
    {
        public string TargetView { get; private set; }
        public object State { get; private set; }

        public NavigateMessage(string targetView, object state)
        {
            this.TargetView = targetView;
            this.State = state;
        }
    }
}
