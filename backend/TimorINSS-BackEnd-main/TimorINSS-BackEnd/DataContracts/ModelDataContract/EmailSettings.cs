namespace TimorINSSBackEnd.DataContracts.ModelDataContract
{
    public class EmailSettings
    {
        public string PrimaryDomain { get; set; }
        public int PrimaryPort { get; set; }
        public string UsernameEmail { get; set; }
        public string UsernamePassword { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string CcEmail { get; set; }
        public string RecoverUrl { get; set; }
        public string FirstAcessUrl { get; set; }
        public string InternalRecoverUrl { get; set; }
        public string InternalFirstAcessUrl { get; set; }
    }
}