namespace XperienceCommunity.CSP.Services
{
    public interface ICspNonceService
    {
        string Nonce { get; }
    }

    public class CspNonceService : ICspNonceService
    {
        private readonly string _nonce;

        public CspNonceService()
        {
            _nonce = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        public string Nonce => _nonce;
    }
}
