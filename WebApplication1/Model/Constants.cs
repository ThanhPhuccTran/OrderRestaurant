﻿namespace OrderRestaurant.Model
{
    public static class Constants
    {
        /// <summary>
        /// Đơn hàng vừa được khởi tạo (đang chờ duyệt)
        /// </summary>
        public const int ORDER_INIT = 1;
        /// <summary>
        /// Đơn hàng đã được duyệt
        /// </summary>
        public const int ORDER_APPROVE = 2;
        /// <summary>
        /// Đơn hàng đã được thanh toán
        /// </summary>
        public const int ORDER_PAYMENT = 3;
        /// <summary>
        /// Đơn hàng đã bị từ chối
        /// </summary>
        public const int ORDER_REFUSE = 4;
        /// <summary>
        /// Bàn trống
        /// </summary>
        public const int TABLE_EMPTY = 5;
        /// <summary>
        /// Bàn đang có khách ngồi
        /// </summary>
        public const int TABLE_GUESTS = 6;
        /// <summary>
        /// Bàn đã có khách đặt
        /// </summary>
        public const int TABLE_BOOKING = 7;
        /// <summary>
        /// Yêu cầu mới
        /// </summary>
        public const int REQUEST_INIT = 8;
        /// <summary>
        /// Yêu cầu nhân viên đã hoàn thành
        /// </summary>
        public const int REQUEST_COMPLETE = 9;
        /// <summary>
        /// Yêu cầu bị từ chối
        /// </summary>
        public const int REQUEST_REFUSE = 10;

    }
}
