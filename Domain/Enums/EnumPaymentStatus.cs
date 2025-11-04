namespace Domain.Enums
{
    public enum EnumPaymentStatus
    {
        Pending,   // Đã tạo link thanh toán, chưa trả về
        Success,   // Thanh toán thành công
        Failed,    // Thanh toán thất bại
        Canceled,  // Người dùng hủy
        Expired    // Hết hạn link
    }
}
