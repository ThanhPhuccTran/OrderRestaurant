namespace OrderRestaurant.Model
{
    public static class Constants
    {
        /// <summary>
        /// Đơn hàng vừa được khởi tạo (đang chờ duyệt)
        /// </summary>
        public const int ORDER_INIT = 1;
        /// <summary>
        /// Đơn hàng đã được duyệt (đang chờ chuyển hàng)
        /// </summary>
        public const int ORDER_ACCEPTED = 2;
        /// <summary>
        /// Đơn hàng đang được chuyển hàng
        /// Đơn hàng đã hoàn tất
        /// </summary>
        public const int ORDER_FINISHED = 3;
        /// <summary>
        /// Đơn hàng bị hủy
        /// </summary>
        public const int ORDER_CANCEL = -1;
        /// <summary>
        /// Đơn hàng bị từ chối
        /// </summary>
        public const int ORDER_REJECTED = -2;
    }
}
