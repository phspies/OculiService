public class Password
{
    public string Text;

    public Password()
    {
    }

    public Password(string text)
    {
        this.Text = text;
    }

    public static implicit operator string(Password p)
    {
        if (p == null)
            return (string)null;
        return p.Text;
    }

    public override string ToString()
    {
        return this.Text;
    }
}
