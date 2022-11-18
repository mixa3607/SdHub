using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SdHub.Database.Shared;

public interface IDbSeeder<in T> where T : DbContext
{
    Task Seed(T db);
}