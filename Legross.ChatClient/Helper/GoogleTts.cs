using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Legross.ChatClient.Helper
{

    public class GoogleTts
    {
        bool waiting = false;
        AutoResetEvent stop = new AutoResetEvent(false);
        private void PlayMp3FromUrl(string url)
        {
            using (Stream ms = new MemoryStream())
            {
                using (Stream stream = WebRequest.Create(url)
                    .GetResponse().GetResponseStream())
                {
                    byte[] buffer = new byte[32768];
                    int read;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                }

                ms.Position = 0;
                using (WaveStream blockAlignedStream =
                    new BlockAlignReductionStream(
                        WaveFormatConversionStream.CreatePcmStream(
                            new Mp3FileReader(ms))))
                {
                    using (WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                    {
                        waveOut.Init(blockAlignedStream);
                        waveOut.PlaybackStopped += (sender, e) =>
                        {
                            waveOut.Stop();
                        };

                        waveOut.Play();
                        waiting = true;
                        stop.WaitOne(10000);
                        waiting = false;
                    }
                }
            }
        }
        public void PlayText(string text)
        {
            var translateText = HttpUtility.UrlEncode(text);
            var playThread = new Thread(() => PlayMp3FromUrl("http://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen=32&client=tw-ob&tl=vi-vn&q=" + translateText));
            playThread.IsBackground = true;
            playThread.Start();
        }        
    }
}
