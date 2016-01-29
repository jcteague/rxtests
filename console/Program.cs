using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console
{
    class Program
    {
        static void Main(string[] args)
        {
            var timeObservable = CreateObservableTimePoller()
                .Select(MapToString)
                .Select(MapAsDate)
                .Subscribe(x => Console.WriteLine(x.ToLocalTime().ToLongTimeString()),
                    (ex) => Console.WriteLine("error occurred"));
            Console.ReadLine();
            timeObservable.Dispose();
        }

        private static DateTime MapAsDate(string arg)
        {
            return DateTime.Parse(arg);
        }

        private static string MapToString(byte[] arg)
        {
            return System.Text.Encoding.UTF8.GetString(arg);
        }

        private static IObservable<byte[]> CreateObservableTimePoller()
        {
            return Observable.Create<byte[]>(observer =>
            {
                var scheduler = Scheduler.Default;
                return scheduler.ScheduleAsync(async (schedulerController, cancelletionToken) =>
                {
                    while (!cancelletionToken.IsCancellationRequested)
                    {
                        var client = new WebClient();
                        client.Headers.Add("user-agent",
                            @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)");
                        try
                        {
                            var result = await client.DownloadDataTaskAsync("http://www.timeapi.org/utc/now");
                            observer.OnNext(result);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            observer.OnError(ex);
                        }
                        await schedulerController.Sleep(TimeSpan.FromMilliseconds(500), cancelletionToken);
                    }
                });
            });
        }
    }
}

