namespace com.doosan.fms.model.Auth
{
    public class AuthData
    {
        private string _jwt;
        private string _name;
        private string _id;
        private string _message;

        public AuthData()
        {
        }

        public AuthData(string jwt, string name, string id)
        {
            _jwt = jwt;
            _name = name;
            _id = id;
        }

        public string Jwt { get => _jwt; set => _jwt = value; }
        public string Name { get => _name; set => _name = value; }
        public string Id { get => _id; set => _id = value; }
        public string Message { get => _message; set => _message = value; }
    }
}
