namespace Vysotsky.API.Controllers.Categories
{
    public record Category(int Id, string Name, SubCategory[] SubCategories);

    public record SubCategory(int Id, string Name);
}