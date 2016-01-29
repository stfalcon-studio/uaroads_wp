using Caliburn.Micro;

namespace UR.Core.WP81.Common
{
    //public class MsgDataHandlerStatusChanged
    //{
    //    public bool IsStarted { get; private set; }

    //    public MsgDataHandlerStatusChanged(bool isStarted)
    //    {
    //        IsStarted = isStarted;
    //    }
    //}


    public class MsgAppState
    {
        public EGlobalState State { get; private set; }

        public MsgAppState(EGlobalState newState)
        {
            State = newState;
        }

        public static void Publish(EGlobalState newState)
        {
            IoC.Get<IEventAggregator>().PublishOnUIThread(new MsgAppState(newState));
        }
    }
}
