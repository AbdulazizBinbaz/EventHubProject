using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Utility
{
    public static class SD
    {
        public const string Role_User_Individual = "Individual";
        public const string Role_User_EventManager = "EventManager";
        public const string Role_Admin = "Admin";


        public const string EventStatus_WaitingForApproval = "WaitingForApproval";
        public const string EventStatus_Approved = "Approved";
        public const string EventStatus_Declined = "Declined";
        public const string EventStatus_DeleteRequest = "DeleteRequest";


        public const string PaymentStatus_Approved = "Approved";
        public const string PaymentStatus_Declined = "Declined";
        public const string PaymentStatus_Refunded = "Refunded";


    }
}
