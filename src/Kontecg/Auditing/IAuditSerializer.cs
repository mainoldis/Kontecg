namespace Kontecg.Auditing
{
    public interface IAuditSerializer
    {
        string Serialize(object obj);
    }
}
