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
        private GoogleTts Tts;


        public RecorderViewModel(IAudioRecorder recorder)
        {
            messageHub = new MessageHub();
            messageHub.OnCallBack = (string name, string message) => OnHubCallBack(name, message);
            messageHub.ConnectionClosed = () => OnHubClosed();
            Tts = new GoogleTts();

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

            PrepareUI();
        }
        private void PrepareUI()
        {
            ChatPanelVisibility = false;
            ButtonSendEnabled = false;
            SignInPanelVisibility = true;
            StatusVisibility = false;
            RaisePropertyChanged("ChatPanelVisibility");
            RaisePropertyChanged("SignInPanelVisibility");
            RaisePropertyChanged("ButtonSendEnabled");
            RaisePropertyChanged("StatusVisibility");
        }
        private void OnHubClosed()
        {
            if (messageHub.Connection != null)
            {
                messageHub.Connection.Stop();
                messageHub.Connection.Dispose();

            }
        }
        void OnRecorderStopped(object sender, EventArgs e)
        {
            //get text
            var s2t = GoogleTranscribeAsync.Transcribe(waveFileName);
            if (s2t != null)
            {
                foreach (string item in s2t)
                {
                    ChatMessage += item;
                }
                RaisePropertyChanged("ChatMessage");
            }
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
        public ICommand SendMessageCommand { get { return sendMessageCommand; } }
        public ICommand SignInCommand { get { return signInCommand; } }

        void OnHubCallBack(string name, string message)
        {
            RichTextBoxConsoleText += String.Format("{0}: {1}\r", name, message);
            RaisePropertyChanged("RichTextBoxConsoleText");
            Tts.PlayText(name + "trả lời: "+ message);
        }
        public void Activated(object state)
        {
            BeginMonitoring((int)state);
        }
        private void Sent()
        {
            messageHub.Send("LegrossChat", ChatMessage);
            //todo: loging here
            
            RichTextBoxConsoleText += String.Format("{0}: {1}\r", "ChatClient", ChatMessage);
            ChatMessage = String.Empty;
            IsChatMessageFocused = true;
            RaisePropertyChanged("ChatMessage");
            RaisePropertyChanged("IsChatMessageFocused");
            RaisePropertyChanged("RichTextBoxConsoleText");
        }
        private void SignedIn()
        {
            StatusVisibility = true;
            StatusContent = "Connecting to server...";
            RaisePropertyChanged("StatusContent");
            RaisePropertyChanged("StatusVisibility");
            messageHub.SignIn(UserName);

            //Show chat UI; hide login UI
            SignInPanelVisibility = false;
            ChatPanelVisibility = true;
            ButtonSendEnabled = true;

            RichTextBoxConsoleText = "Connected to server" + "\r";
            
            

            RaisePropertyChanged("SignInPanelVisibility");
            RaisePropertyChanged("ChatPanelVisibility");
            
            RaisePropertyChanged("ButtonSendEnabled");
            RaisePropertyChanged("RichTextBoxConsoleText");
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
