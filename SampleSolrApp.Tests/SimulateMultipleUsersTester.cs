using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using NUnit.Framework;

using SharpTestsEx;

namespace SampleSolrApp.Tests
{
    [TestFixture]
    public class SimulateMultipleUsersTester
    {
        private const int _threadsNumber = 10;
        private static ManualResetEvent[] _resetEvents;
        private static int _errorsCount = 0;
        private static int _requestsCount = 0;

        [Test]
        public void Simulate_10_Users_Should_Crash_Application()
        {
            _resetEvents = new ManualResetEvent[_threadsNumber];

            for (int s = 0; s < _threadsNumber; s++)
            {
                _resetEvents[s] = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem(new WaitCallback(DoWork), (object)s);
            }

            Debug.WriteLine("Waiting...");

            WaitHandle.WaitAll(_resetEvents);

            Debug.WriteLine(string.Format("Done {0} requests.", _requestsCount));
            if (_errorsCount > 0)
                Debug.WriteLine(string.Format("{0} errors. Please check the logs in the web application Logs directory.", _errorsCount));

            true.Should().Be.True();
        }

        private static void DoWork(object o)
        {
            int index = (int)o;
            try
            {
                for (int i = 0; i < 20; i++)
                {
                    try
                    {
                        Interlocked.Increment(ref _requestsCount);
                        var request = WebRequest.Create("http://localhost:25827/home/add");
                        request.Method = "GET";
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        Debug.WriteLine(string.Format("[Thread {0}:{1}] {2}", index, i, response.StatusCode));
                        if (response.StatusCode != HttpStatusCode.OK)
                            Interlocked.Increment(ref _errorsCount);
                    }
                    catch
                    {
                        Interlocked.Increment(ref _errorsCount);
                    }

                }
            }
            catch
            {
                Interlocked.Increment(ref _errorsCount);
            }
            finally
            {
                _resetEvents[index].Set();
            }
        }
    }
}