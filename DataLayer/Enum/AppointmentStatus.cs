using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Enum
{
    public enum AppointmentStatus
    {
        Pending = 0,             // Đợi duyệt
        Confirmed = 1,           // Đã xác nhận và thanh toán xong
        Cancelled = 2,           // Hủy
        Completed = 3,           // Hoàn thành
        ReArranged = 4,          // Dời lịch
        CheckedIn = 5,          // Dã check-in
    }
}
