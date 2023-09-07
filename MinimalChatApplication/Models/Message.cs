﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MinimalChatApplication.Models
{
    public class Message
    {
        
        public int Id { get; set; }
        
      
        public string SenderId { get; set; }
       
        public string ReceiverId { get; set; }
      
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        public bool Seen { get; set; }
        public bool IsRead { get; set; }

        // Navigation properties
        public IdentityUser Sender { get; set; }
        public IdentityUser Receiver { get; set; }
        public DateTime SeenTimestamp { get;  set; }
        public string SeenByUserId { get;  set; }
    }
}
