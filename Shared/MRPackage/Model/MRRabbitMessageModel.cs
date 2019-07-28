using Newtonsoft.Json;
using System;
using System.Text;

namespace MRPackage.Model
{
    [Serializable]
    public class MRRabbitMessageModel
    {
        public string CallService { get; set; }
        public string CallEnv { get; set; }
        public string Body { get; set; }

        public static byte[] Serialize(object body, string callService, string callEnv)
        {
            var message = new MRRabbitMessageModel
            {
                CallEnv = callEnv,
                CallService = callService,
                Body = JsonConvert.SerializeObject(body)
            };

            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
        }

        public static (MRRabbitMessageModel, TModel) Parse<TModel>(byte[] byteMessage)
            where TModel: class, new()
        {
            var textMessage = Encoding.UTF8.GetString(byteMessage);
            var message = JsonConvert.DeserializeObject<MRRabbitMessageModel>(textMessage);

            if (string.IsNullOrWhiteSpace(message.Body))
            {
                return (message, null);
            }

            return (message, JsonConvert.DeserializeObject<TModel>(message.Body));
        }
    }
}
