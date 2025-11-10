namespace QLCF_NewVer
{
    public static class Session
    {
        // Lưu trữ thông tin người dùng đang đăng nhập
        public static NguoiDung CurrentUser { get; private set; }

        // Hàm gọi khi đăng nhập thành công
        public static void Login(NguoiDung user)
        {
            CurrentUser = user;
        }

        // Hàm gọi khi đăng xuất
        public static void Logout()
        {
            CurrentUser = null;
        }

        // Kiểm tra xem đã đăng nhập hay chưa
        public static bool IsLoggedIn()
        {
            return CurrentUser != null;
        }

        // Kiểm tra xem có phải Admin không
        public static bool IsAdmin()
        {
            return IsLoggedIn() && CurrentUser.ViTri == "Admin";
        }
    }
}
