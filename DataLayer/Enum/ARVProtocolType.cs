using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Enum
{
    public enum ARVProtocolType
    {
        TDF_3TC_EFV = 0,    /* A - Tenofovir + Lamivudine + Efavirenz (Phác đồ bậc 1 phổ biến) */
        TDF_3TC_DTG = 1,    /* B - Tenofovir + Lamivudine + Dolutegravir (Hiệu quả cao, ít kháng) */
        AZT_3TC_NVP = 2,    /* C - Zidovudine + Lamivudine + Nevirapine (Phác đồ cũ) */
        AZT_3TC_EFV = 3,    /* D - Zidovudine + Lamivudine + Efavirenz (Thay thế TDF) */
        ABC_3TC_LPVr = 4,   /* E - Abacavir + Lamivudine + Lopinavir/Ritonavir (Dành cho trẻ em) */
        TDF_3TC_LPVr = 5,   /* F - Tenofovir + Lamivudine + Lopinavir/Ritonavir (Phác đồ bậc 2) */
        TDF_3TC_NVP = 6,    /* G - Tenofovir + Lamivudine + Nevirapine (Thay thế khi không dùng được EFV) */
        ABC_3TC_DTG = 7,    /* H - Abacavir + Lamivudine + Dolutegravir (Dành cho người bệnh thận) */
        TDF_FTC_EFV = 8,    /* I - Tenofovir + Emtricitabine + Efavirenz (Biệt dược Atripla) */
        TDF_FTC_DTG = 9,    /* J - Tenofovir + Emtricitabine + Dolutegravir (Phác đồ mới, hiệu quả cao) */
    }
}
