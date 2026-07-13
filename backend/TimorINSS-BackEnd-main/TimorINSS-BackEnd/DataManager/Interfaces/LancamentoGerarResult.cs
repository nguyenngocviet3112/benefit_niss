namespace TimorINSSBackEnd.DataManager.Interfaces
{
    // Kết quả của GerarSeChuaCo — cho phép nơi gọi thông báo lên người dùng khi
    // 1 bút toán vừa được tự sinh (để kiểm tra lại nếu sai) hoặc khi bị bỏ qua
    // vì thiếu cấu hình tài khoản Nợ/Có (để biết đường đi cấu hình bổ sung).
    public class LancamentoGerarResult
    {
        public bool Gerado { get; set; }
        public bool FaltaConfiguracao { get; set; }
    }
}
