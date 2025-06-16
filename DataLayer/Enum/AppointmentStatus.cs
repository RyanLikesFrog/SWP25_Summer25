using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Enum
{
    public enum AppointmentStatus
    {
        Pending = 0,             
        Confirmed = 1,           // Đã xác nhận và thanh toán xong
        Cancelled = 2,
        Completed = 3,
    }
}
