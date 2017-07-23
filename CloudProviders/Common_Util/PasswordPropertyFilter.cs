public class PasswordPropertyFilter : PropertyFilter
{
    public static PasswordPropertyFilter GlobalInstance = new PasswordPropertyFilter();
    private MachineScopeCryptor _Cryptor;

    public PasswordPropertyFilter()
    {
        this._Cryptor = new MachineScopeCryptor();
    }

    public override void PostGetProperty(string name, ref object value)
    {
        Password password = value as Password;
        if (password == null)
            return;
        value = (object)new Password(this._Cryptor.Decrypt(password.Text));
    }

    public override void PreSetProperty(string name, ref object value)
    {
        Password password = value as Password;
        if (password == null)
            return;
        value = (object)new Password(this._Cryptor.Encrypt(password.Text));
    }
}
