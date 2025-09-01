using BloodCraftEZLife.UI;
using BloodCraftEZLife.UI.ModContent;
using BloodCraftEZLife.UI.ModContent.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Unity.Collections;
using static BloodCraftEZLife.Services.TeleportsService;
using static Il2CppMono.Security.X509.X520;
using static ProjectM.Metrics;
using static ProjectM.StunAnalytics.Shared;

namespace BloodCraftEZLife.Config
{
    
    public class SettingsChatMessage
    {
        public Dictionary<string, ChatUser> Users { get; set; } = new();
        private List<string> _users;
        private List<ChatMessage> _messages = new();

        [JsonIgnore]
        public List<string> GetUsers => _users ??= GetListUsers();
        [JsonIgnore]
        public string CurrentUser { get; private set; } = "";
        
        public List<string> GetListUsers()
        {
            List<string> result=new List<string>();
            foreach (var usr in Users)
            {
                result.Add(usr.Key);

            }
            return result;
        }

        public List<ChatMessage> GetMessages(string user = "")
        {
            if (CurrentUser == user || user == "")
                return _messages;
            _messages.Clear();
            foreach (var usr in Users[user].Messages)
            {
                _messages.Add(usr);

            }
            
            CurrentUser = user;
            return _messages;
        }

        public void AddMessage(string username, string message,long timestamp,bool from)
        {
            try
            {
                if (!GetUsers.Contains(username))
                {
                    Users[username] = new ChatUser { Username = username };
                    _users.Add(username);
                }
                ChatMessage newmsg = new ChatMessage
                {
                    Timestamp = timestamp,
                    TextMessage = message,
                    From = from
                };
                Users[username].Messages.Add(newmsg);
                if (username == CurrentUser)
                {
                    _messages.Add(newmsg);
                    var panel = Plugin.UIManager.GetPanel<ChatPanel>();
                    panel.RefreshMessage(username);
                }
                }
            catch (System.Exception)
            {

                
            }
        }
      
    }
}
