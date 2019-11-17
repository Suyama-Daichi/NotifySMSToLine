using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace NotifySMSToLine
{
    public class Function
    {
        private static HttpClient client = new HttpClient();

        /// <summary>
        /// SMSが来たらLINE Notifyに通知する
        /// </summary>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<object> FunctionHandler(SMSInfo input, ILambdaContext context)
        {
            // リージョンを日本に設定
            var culture = System.Globalization.CultureInfo.GetCultureInfo("ja-JP");

            string sentDate = DateTimeOffset.FromUnixTimeSeconds(input.sentTimestamp).AddHours(9).ToString("yyyy/MM/dd(ddd) HH:mm:ss", culture);

            // 環境変数は「Environment.GetEnvironmentVariable("accessToken")」で取得できる
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Environment.GetEnvironmentVariable("accessToken")}");

            var result = await client.PostAsync($"https://notify-api.line.me/api/notify/?message=\n【内容】\n{input.messageContent}\n\n【受信時刻】\n{sentDate}\n\n【送信元】\n{input.senderPhoneNumber}", null);
            client = new HttpClient();
            return result;
        }
    }
}
