public class RoomInf
{
    public string Name;
    public int Id;
    public string Password;

    public RoomInf(string name, int id, string password = "")
    {
        Name = name;
        Id = id;
        Password = password;
    }
}