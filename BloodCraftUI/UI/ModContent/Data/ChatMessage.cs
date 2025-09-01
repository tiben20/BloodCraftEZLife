using ProjectM;
using Il2CppSystem;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodCraftEZLife.UI.ModContent.Data
{
    public class ChatUser
    {
        public string Username { get; set; }
        public List<ChatMessage> Messages { get; set; } = new();
        
    }

    public class ChatMessage
    {
        public long Timestamp { get; set; }
        public string TextMessage { get; set; }
        public bool From { get; set; }
    }
}