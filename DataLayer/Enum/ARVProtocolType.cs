namespace DataLayer.Enum
{
    public enum ARVProtocolType
    {
        SimpleHIV = 0,        // HIV đơn thuần (không có bệnh phối hợp)
        HIV_TB = 1,           // HIV đồng nhiễm Lao (Tuberculosis)
        HIV_HBV = 2,          // HIV đồng nhiễm Viêm gan B
        HIV_HCV = 3,          // HIV đồng nhiễm Viêm gan C
        HIV_Sepsis = 4,       // HIV có nhiễm trùng máu
        HIV_Kidney = 5,       // HIV + Bệnh thận
        HIV_Liver = 6,        // HIV + Suy gan
        HIV_Cancer = 7,       // HIV + Ung thư (Kaposi, lymphoma...)
        HIV_Pregnancy = 8,    // HIV + Phụ nữ mang thai
        HIV_Pediatric = 9,    // HIV ở trẻ em
    }
}
