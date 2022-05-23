using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsyncAwait
{
    public partial class Main : Form
    {
        private string uri1 = "https://api.jikan.moe/v3/search/anime?q=naruto"; // json abbastanza grande
        private string uri2 = "https://house-stock-watcher-data.s3-us-west-2.amazonaws.com/data/all_transactions.json"; // json enorme
        private string uri3 = "https://www.boredapi.com/api/activity"; // poco testo
        private Stopwatch stopWatch;
        SynchronizationContext ctx;

        public Main()
        {
            ctx = SynchronizationContext.Current;
            stopWatch = new Stopwatch();
            InitializeComponent();
        }

        private void FakeAsync_Click(object sender, EventArgs e)
        {
            StartWatchAndShowOnElapsed();



            var finalText = FormatTexts(CallApiAsync(uri1).Result, // <-- Result blocca il main thread
                                         CallApiAsync(uri2).Result, // <-- Result blocca il main thread
                                         CallApiAsync(uri3).Result); // <-- Result blocca il main thread
            SetMainTextArea(finalText);



            stopWatch.Stop();
            SetElapsedText($"Tempo totale: {stopWatch.Elapsed.ToString(@"m\:ss\.fff")}");
        }

        private void ParallelNotAsync_Click(object sender, EventArgs e)
        {
            StartWatchAndShowOnElapsed();


            var threadFinishEvents = new List<EventWaitHandle>();
            var texts = new List<string>();
            var uris = new[] { uri1, uri2, uri3 };

            foreach (var uri in uris)
            {
                var threadFinish = new EventWaitHandle(false, EventResetMode.ManualReset);
                threadFinishEvents.Add(threadFinish);
                var thread = new Thread(
                    delegate ()
                    {
                        texts.Add(CallApi(uri));
                        threadFinish.Set();
                    }
                );
                thread.Start();
            }

            foreach (var thread in threadFinishEvents) thread.WaitOne(60000);
            var finalText = FormatTexts(texts.ToArray());
            SetMainTextArea(finalText);


            stopWatch.Stop();
            SetElapsedText($"Tempo totale: {stopWatch.Elapsed.ToString(@"m\:ss\.fff")}");
        }

        private async void ParallelFakeAsync_Click(object sender, EventArgs e)
        {
            StartWatchAndShowOnElapsed();


            //var threadFinishEvents = new List<EventWaitHandle>();
            //var texts = new List<string>();
            //var uris = new[] { uri1, uri2, uri3 };

            //foreach (var uri in uris)
            //{
            //    var threadFinish = new EventWaitHandle(false, EventResetMode.ManualReset);
            //    threadFinishEvents.Add(threadFinish);
            //    var thread = new Thread(
            //        async () =>
            //        {
            //            texts.Add(await Task.FromResult(CallApiAsync(uri).Result));
            //            threadFinish.Set();
            //        }
            //    );
            //    thread.Start();
            //}

            //foreach (var thread in threadFinishEvents) thread.WaitOne(60000);
            //var finalText = FormatTexts(texts.ToArray());


            // stessa cosa di sopra, blocca il main thread comunque. Ma il codice giù è più divertente
            var task1 = CallApiAsync(uri1);
            var task2 = CallApiAsync(uri2);
            var task3 = CallApiAsync(uri3);
            SetElapsedText($"Tempo parziale pre Wait: {stopWatch.Elapsed.ToString(@"m\:ss\.fff")}");
            task1.Wait(); // <-- Wait blocca il main thread
            task2.Wait(); // <-- Wait blocca il main thread
            task3.Wait(); // <-- Wait blocca il main thread
            SetElapsedText($"Tempo parziale pre Result: {stopWatch.Elapsed.ToString(@"m\:ss\.fff")}");
            var _ = FormatTexts(task1.Result, task2.Result, task3.Result); // <-- questi Result servono solo a srotolare Task->string i task son già finiti grazie ai Wait
            var finalText = await Task.FromResult(_).ConfigureAwait(false);
            SetElapsedText($"Tempo parziale post Result: {stopWatch.Elapsed.ToString(@"m\:ss\.fff")}");
            // ma come mai questo blocco di codice sarebbe "parallelo" ?_? 
            // la chiamata FakeAsync_Click è simile eppure non è parallela... Q_Q
            // hint -> chiamate async


            SetMainTextArea(finalText);


            stopWatch.Stop();
            SetElapsedText($"Tempo totale: {stopWatch.Elapsed.ToString(@"m\:ss\.fff")}");
        }

        private async void AsyncCall_Click(object sender, EventArgs e)
        {
            StartWatchAndShowOnElapsed();


            var task1 = CallApiAsync(uri1);
            var task2 = CallApiAsync(uri2);
            var task3 = CallApiAsync(uri3);
            //var tasks = await Task.WhenAll(task1, task2, task3).ConfigureAwait(false); // non necessario
            var finalText = FormatTexts(await task1.ConfigureAwait(false), await task2.ConfigureAwait(false), await task3.ConfigureAwait(false));


            //var tasks = await Task.WhenAll(task1, task2, task3).ConfigureAwait(false); // funzionerebbe anche così
            //var finalText = FormatTexts(task1.Result, task2.Result, task3.Result); // sappiamo perchè qui Result non è "dannoso"


            SetMainTextArea(finalText);


            stopWatch.Stop();
            SetElapsedText($"Tempo totale: {stopWatch.Elapsed.ToString(@"m\:ss\.fff")}");
        }

        public async Task<string> CallApiAsync(string uri)
        {
            var partialStopWatch = new Stopwatch();
            partialStopWatch.Start();
            using (var httpResponse = await ConfigureHttpClient(uri).GetAsync(uri).ConfigureAwait(false))
            {
                var response = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                SetElapsedText($"Tempo parziale - chiamata url {uri}{Environment.NewLine}-- {partialStopWatch.Elapsed.ToString(@"m\:ss\.fff")}");
                partialStopWatch.Stop();
                return response.Length < 500 ? response : response.Substring(0, 500);
            }
        }

        public string CallApi(string uri)
        {
            var partialStopWatch = new Stopwatch();
            partialStopWatch.Start();
            using (var httpResonse = ConfigureHttpClient(uri).GetAsync(uri).GetAwaiter().GetResult()) // <-- GetAwaiter().GetResult() blocca il main thread
            {
                var _ = httpResonse.Content.ReadAsStringAsync().GetAwaiter().GetResult(); // <-- GetAwaiter().GetResult() blocca il main thread
                SetElapsedText($"Tempo parziale - chiamata url {uri}{Environment.NewLine}-- {partialStopWatch.Elapsed.ToString(@"m\:ss\.fff")}");
                partialStopWatch.Stop();
                return _.Length < 500 ? _ : _.Substring(0, 500);
            }
        }

        #region chissenefrega
        private static HttpClient ConfigureHttpClient(string uri)
        {
            HttpClient client = new HttpClient { BaseAddress = new Uri(uri) };

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                   | SecurityProtocolType.Tls11
                   | SecurityProtocolType.Tls12
                   | SecurityProtocolType.Ssl3;
            return client;
        }

        private void TimeDialog_Click(object sender, EventArgs e)
        {
            Form dlg1 = new Form { Text = DateTime.Now.ToString() };
            dlg1.ShowDialog();
        }

        private void ClearPanel_Click(object sender, EventArgs e)
        {
            MainTextArea.Clear();
            Elapsed.Clear();
        }

        private void StartWatchAndShowOnElapsed()
        {
            stopWatch.Reset();
            SetElapsedText("Inizio delle chiamate alle API");
            stopWatch.Start();
        }

        private void SetElapsedText(string text)
        {
            MethodInvoker action = delegate { Elapsed.Text += Environment.NewLine + Environment.NewLine + text; };
            Elapsed.BeginInvoke(action);
        }

        private void SetMainTextArea(string text)
        {
            MethodInvoker action = delegate { MainTextArea.Text += Environment.NewLine + Environment.NewLine + text; };
            Elapsed.BeginInvoke(action);
        }

        private string FormatTexts(params string[] texts)
        {
            SetElapsedText($"Parziale inizio formattazione testo: {stopWatch.Elapsed.ToString(@"m\:ss\.fff")}");
            var _ = string.Join(Environment.NewLine + Environment.NewLine, texts);
            SetElapsedText($"Parziale fine formattazione testo: {stopWatch.Elapsed.ToString(@"m\:ss\.fff")}");
            return _;
        }
        #endregion
    }
}
