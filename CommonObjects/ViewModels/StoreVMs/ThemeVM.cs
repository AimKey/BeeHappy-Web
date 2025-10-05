using MongoDB.Bson;

namespace CommonObjects.ViewModels.StoreVMs;

public class ThemeVM
{
    public ObjectId Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}