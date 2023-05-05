using System;
using System.Data;
using System.Linq;
using Dapper;
using DapperTests.DbModels;
using DapperTests.GroupByTests;
using Microsoft.Data.SqlClient;

namespace DapperTests
{
    internal class Program
    {
        static string ConnectionString = "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = Spark; Integrated Security = True;";//  Connect Timeout = 30; Encrypt = False; Trust Server Certificate=False; Application Intent = ReadWrite; Multi Subnet Failover=False
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            //UsersQuery();
            //UserRolesQuery();
            UserRolesPagesQuery();

            //Tests.GroupStundentsByBranch();
            //Tests.GroupStundentsByGenderWithOrderBy();
            //Tests.GroupByMultipleKeys();
            //Tests.GroupByMultipleKeysWithOrderBy();
            //Tests.NestedGroups1();
            //Tests.NestedGroups2();
        }

        private static void UsersQuery()
        {
            var sql = "SELECT * FROM USERS";

            using (var connection = new SqlConnection(ConnectionString))
            {

                var users = connection.Query<User>(sql);

                foreach (var user in users)
                {
                    Console.WriteLine($"{user.Id}, {user.UserName}");
                }
            }
        }

        private static void UserRolesQuery()
        {
            var sql = "SELECT u.Id, u.UserName, r.Id, r.Name FROM Users u JOIN UsersRoles ur on u.Id=ur.UserId JOIN Roles r on r.Id=ur.RoleId order by u.Id";

            using (var connection = new SqlConnection(ConnectionString))
            {

                var users = connection.Query<User, Role, User>(sql, (user, role) =>
                {
                    user.Roles.Add(role);
                    return user;
                });

                var result2 = users.GroupBy(u => u.Id);

                var result = users.GroupBy(u => u.Id)
                                  .Select(g =>
                                  {
                                      var groupedUser = g.First();
                                      groupedUser.Roles = g.Select(user => user.Roles.Single()).ToList();
                                      return groupedUser;
                                  });

                foreach (var user in result)
                {
                    Console.Write($"{user.UserName}: ");

                    foreach (var role in user.Roles)
                    {
                        Console.Write($"{role.Name} ");
                    }

                    Console.WriteLine();
                }
            }
        }

        private static void UserRolesPagesQuery()
        {
            var procedure = "GetRolesPagesByUserId";
            var values = new { userId = 1 };
            using (var connection = new SqlConnection(ConnectionString))
            { 
                var results = connection.Query(procedure, values, commandType: CommandType.StoredProcedure).ToList();
            }
        }
    }
}
