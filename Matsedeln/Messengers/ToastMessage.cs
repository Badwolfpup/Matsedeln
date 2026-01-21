using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matsedeln.Messengers
{
    public class ToastMessage
    {
        public string Message { get; set; }
        public int DurationInSeconds { get; set; } = 3;
        public ToastMessage(string message, int durationInSeconds = 3)
        {
            Message = message;
            DurationInSeconds = durationInSeconds;
        }
    }
}
