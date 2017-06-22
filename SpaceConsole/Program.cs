using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mastonet;

namespace SpaceConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        static private async Task RunAsync()
        {
            string Email = Constant.InfoEmail;
            string Password = Constant.Password;

            DateTime today = DateTime.UtcNow.AddHours(9);

            var list = System.IO.File.ReadAllLines("theme.txt").ToList();
            string firstTheme = list.First();
            list.RemoveAt(0);
            list.Add(firstTheme);
            string secondTheme = list.First();
            System.IO.File.WriteAllLines("theme.txt", list);

            string message = new StringBuilder()
                .AppendLine(today.ToString("M月d日(dddd)"))
                .AppendLine("本日のテーマ")
                .AppendLine("#" + secondTheme)
                .AppendLine("#" + firstTheme)
                .ToString();

            var authenticationClient = new AuthenticationClient(Constant.Instance);
            var registration = await authenticationClient.CreateApp("SpaceConsole", Scope.Read | Scope.Write | Scope.Follow);
            var auth = await authenticationClient.ConnectWithPassword(Email, Password);
            var MastodonClient = new MastodonClient(registration, auth);

            var post = await MastodonClient.PostStatus(message, Visibility.Public);

            var statuses = await MastodonClient.GetAccountStatuses(post.Account.Id, post.Id);
            foreach (var status in statuses)
            {
                await MastodonClient.DeleteStatus(status.Id);
            }
        }
    }
}
