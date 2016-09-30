using System;
using System.Windows.Input;
using System.IO;
using Legross.Audio;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using Legross.ChatClient.Helper;
using System.Windows;

namespace Legross.ChatClient
{
    public class RecorderViewModel : ViewModelBase, IView
    {

        private readonly RelayCommand beginRecordingCommand;
        private readonly RelayCommand stopCommand;
        private readonly RelayCommand sendMessageCommand;
        private readonly RelayCommand signInCommand;
        private readonly IAudioRecorder recorder;
        private float lastPeak;
        private string waveFileName;
        public const string ViewName = "RecorderView";
        private MessageHub messageHub;


        public RecorderViewModel(IAudioRecorder recorder)
        {
            messageHub = new MessageHub();

            this.recorder = recorder;
            this.recorder.Stopped += OnRecorderStopped;
            beginRecordingCommand = new RelayCommand(BeginRecording,
                () => recorder.RecordingState == RecordingState.Stopped ||
                      recorder.RecordingState == RecordingState.Monitoring);
            stopCommand = new RelayCommand(Stop,
                () => recorder.RecordingState == RecordingState.Recording);
            signInCommand = new RelayCommand(SignedIn);
            sendMessageCommand = new RelayCommand(Sent);

            recorder.SampleAggregator.MaximumCalculated += OnRecorderMaximumCalculated;
            Messenger.Default.Register<ShuttingDownMessage>(this, OnShuttingDown);
        }

        void OnRecorderStopped(object sender, EventArgs e)
        {
            //get text
            var s2t = GoogleTranscribeAsync.Transcribe(waveFileName);

            //send to chat bot


            //Messenger.Default.Send(new NavigateMessage(SaveViewModel.ViewName,
            //    new VoiceRecorderState(waveFileName, null)));

        }

        void OnRecorderMaximumCalculated(object sender, MaxSampleEventArgs e)
        {
            lastPeak = Math.Max(e.MaxSample, Math.Abs(e.MinSample));
            RaisePropertyChanged("CurrentInputLevel");
            RaisePropertyChanged("RecordedTime");
        }

        public ICommand BeginRecordingCommand { get { return beginRecordingCommand; } }
        public ICommand StopCommand { get { return stopCommand; } }
        public ICommand SendMessage { get { return sendMessageCommand; } }
        public ICommand SignIn { get { return signInCommand; } }


        public void Activated(object state)
        {
            BeginMonitoring((int)state);
        }
        private void Sent()
        {
            messageHub.Send(ChatMessage);
            //todo: loging here
            ChatMessage = String.Empty;
            IsChatMessageFocused = true;
            RaisePropertyChanged("ChatMessage");
            RaisePropertyChanged("IsChatMessageFocused");
        }
        private void SignedIn()
        {
            messageHub.SignIn(UserName);
            //SignInPanelVisibility = Visibility.Collapsed;

            //Show chat UI; hide login UI
            //SignInPanel.Visibility = Visibility.Collapsed;
            //ChatPanel.Visibility = Visibility.Visible;
            //ButtonSend.IsEnabled = true;
            //TextBoxMessage.Focus();
            //RichTextBoxConsole.AppendText("Connected to server at " + ServerURI + "\r");
            StatusVisibility = true;
            StatusContent = "Connecting to server...";

            RaisePropertyChanged("StatusVisibility");
            RaisePropertyChanged("StatusContent");
        }
        private void ConnectionClosed()
        {

        }
        private void OnShuttingDown(ShuttingDownMessage message)
        {
            if (message.CurrentViewName == ViewName)
            {
                recorder.Stop();
            }
        }
        public string RichTextBoxConsoleText { get; set; }
        public bool IsChatMessageFocused { get; set; }
        public string StatusContent { get; set; }
        public string ChatMessage { get; set; }
        public bool StatusVisibility { get; set; }
        public bool ButtonSendEnabled { get; set; }
        public bool ChatPanelVisibility { get; set; }
        public bool SignInPanelVisibility { get; set; }

        public string RecordedTime
        {
            get
            {
                var current = recorder.RecordedTime;
                return String.Format("{0:D2}:{1:D2}.{2:D3}",
                    current.Minutes, current.Seconds, current.Milliseconds);
            }
        }

        private void BeginMonitoring(int recordingDevice)
        {
            recorder.BeginMonitoring(recordingDevice);
            RaisePropertyChanged("MicrophoneLevel");
        }

        private void BeginRecording()
        {
            waveFileName = DateTime.Now.ToString("yyMMdd-HHmmss") + ".wav";
            recorder.BeginRecording(waveFileName);
            RaisePropertyChanged("MicrophoneLevel");
            RaisePropertyChanged("ShowWaveForm");
        }

        private void Stop()
        {
            recorder.Stop();
        }

        public double MicrophoneLevel
        {
            get { return recorder.MicrophoneLevel; }
            set { recorder.MicrophoneLevel = value; }
        }
        public string UserName
        {
            get { return messageHub.UserName; }
            set { messageHub.UserName = value; }
        }

        public bool ShowWaveForm
        {
            get
            {
                return recorder.RecordingState == RecordingState.Recording ||
              recorder.RecordingState == RecordingState.RequestedStop;
            }
        }

        // multiply by 100 because the Progress bar's default maximum value is 100
        public float CurrentInputLevel { get { return lastPeak * 100; } }

        public SampleAggregator SampleAggregator
        {
            get
            {
                return recorder.SampleAggregator;
            }
        }
    }
}
