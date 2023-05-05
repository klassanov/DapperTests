using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Identity.Client;

namespace DapperTests.GroupByTests
{
    public class Tests
    {
        public static void GroupStundentsByBranch()
        {
            var students = Student.GetStudents();

            //Method syntax
            IEnumerable<IGrouping<string, Student>> groupByMethodSyntax = students.GroupBy(x => x.Branch);

            //Query syntax
            IEnumerable<IGrouping<string, Student>> groupByQuerySyntax =
                (from std in students
                 group std by std.Branch);


            foreach (var group in groupByMethodSyntax)
            {
                Console.WriteLine($"{group.Key}: {group.Count()}");
                foreach (var std in group)
                {
                    Console.WriteLine($"Name: {std.Name}, Age: {std.Age}, Gender: {std.Gender}");
                }
                Console.WriteLine();
            }
        }

        public static void GroupStundentsByGenderWithOrderBy()
        {
            //Method syntax
            var customGroupByMs = Student.GetStudents().GroupBy(x => x.Gender)
                                                       .OrderByDescending(group => group.Key)
                                                       .Select(group => new
                                                       {
                                                           Key = group.Key,
                                                           Students = group.OrderBy(x => x.Name)
                                                       });


            //Query syntax
            var customGroupQs = from std in Student.GetStudents()
                                group std by std.Gender into stdGroup
                                orderby stdGroup.Key descending
                                select new
                                {
                                    Key = stdGroup.Key,
                                    Students = stdGroup.OrderBy(x => x.Name)
                                };




            foreach (var group in customGroupQs)
            {
                Console.WriteLine($"{group.Key}: {group.Students.Count()}");
                foreach (var std in group.Students)
                {
                    Console.WriteLine($"Name: {std.Name}, Age: {std.Age}, Gender: {std.Gender}");
                }

                Console.WriteLine();
            }
        }

        public static void GroupByMultipleKeys()
        {
            //Method syntax
            var customGroupByMs = Student.GetStudents().GroupBy(x => new { x.Branch, x.Gender })
                                                       .Select(group => new
                                                       {
                                                           Branch = group.Key.Branch,
                                                           Gender = group.Key.Gender,
                                                           Students = group.OrderBy(x => x.Name),
                                                           Entities = group.ToList() //plain list of the components that compound the result
                                                       });


            //Query syntax
            var customGroupByQs = from std in Student.GetStudents()
                                  group std by new { std.Branch, std.Gender } into stdGroup
                                  select new
                                  {
                                      Branch = stdGroup.Key.Branch,
                                      Gender = stdGroup.Key.Gender,
                                      Students = stdGroup.OrderBy(x => x.Name),
                                      Entities = stdGroup.ToList()
                                  };



            foreach (var group in customGroupByQs)
            {
                Console.WriteLine($"{group.Branch} and {group.Gender}: {group.Students.Count()}");
                foreach (var std in group.Students)
                {
                    Console.WriteLine($"Name: {std.Name}, Age: {std.Age}, Branch: {std.Branch}, Gender: {std.Gender}");
                }

                Console.WriteLine();
            }
        }

        public static void GroupByMultipleKeysWithOrderBy()
        {
            //Method syntax
            var customGroupByMs = Student.GetStudents().GroupBy(x => new { x.Branch, x.Gender })
                                                       .OrderByDescending(group => group.Key.Branch)
                                                       .ThenBy(group => group.Key.Gender)
                                                       .Select(group => new
                                                       {
                                                           Branch = group.Key.Branch,
                                                           Gender = group.Key.Gender,
                                                           Students = group.OrderBy(x => x.Name)
                                                       });

            //Query syntax
            var customGroupByQs = from std in Student.GetStudents()
                                  group std by new { std.Branch, std.Gender } into stdGroup
                                  orderby stdGroup.Key.Branch descending,
                                          stdGroup.Key.Gender ascending
                                  select new
                                  {
                                      Branch = stdGroup.Key.Branch,
                                      Gender = stdGroup.Key.Gender,
                                      Students = stdGroup.OrderBy(x => x.Name)
                                  };


            foreach (var group in customGroupByQs)
            {
                Console.WriteLine($"{group.Branch} and {group.Gender}: {group.Students.Count()}");
                foreach (var std in group.Students)
                {
                    Console.WriteLine($"Name: {std.Name}, Age: {std.Age}, Branch: {std.Branch}, Gender: {std.Gender}");
                }

                Console.WriteLine();
            }
        }

        public static void NestedGroups1()
        {
            //Group by gender and then by age           

            //Query syntax
            var nestedGroupsQueryQs = from std in Student.GetStudents()
                                      group std by std.Gender into genderGroup

                                      from ageGroup in (
                                          from s in genderGroup
                                          group s by s.Age
                                      )

                                      group ageGroup by genderGroup.Key;


            foreach (var outerGroup in nestedGroupsQueryQs)
            {
                Console.WriteLine($"Group Gender: {outerGroup.Key}");

                foreach (var innerGroup in outerGroup)
                {
                    Console.WriteLine($"    Subgroup Age: {innerGroup.Key}");

                    foreach (var std in innerGroup)
                    {
                        Console.WriteLine($"        Gender: {std.Gender} Age: {std.Age} Name: {std.Name}");
                    }
                }
            }


            //var queryNestedGroups =
            //                        from e in source
            //                        group e by e.Key1 into g1
            //                        from e1 in
            //                            (from e in g1
            //                             group e by e.Key2 into g2
            //                             from e2 in
            //                                (from e in g2
            //                                 group e by e.Key3)
            //                             group e2 by g2.Key)
            //                        group e1 by g1.Key;
        }

        public static void NestedGroups2()
        {
            //Group students by gender, age and branch
            //Query syntax
            var nestedGroupsQueryQs = from std in Student.GetStudents()
                                      group std by std.Gender into genderGroup

                                      from std1 in (
                                          from s in genderGroup
                                          group s by s.Age into ageGroup

                                          from std2 in (
                                            from s in ageGroup
                                            group s by s.Branch
                                          )
                                          group std2 by ageGroup.Key
                                      )
                                      group std1 by genderGroup.Key;


            foreach (var outerGroup in nestedGroupsQueryQs)
            {
                Console.WriteLine($"Group Gender: {outerGroup.Key}");

                foreach (var innerGroup in outerGroup)
                {
                    Console.WriteLine($"    Subgroup Age: {innerGroup.Key}");

                    foreach (var innerestGroup in innerGroup)
                    {
                        Console.WriteLine($"        Subgroup Branch: {innerestGroup.Key}");

                        foreach (var std in innerestGroup)
                        {
                            Console.WriteLine($"            Gender: {std.Gender} Age: {std.Age} Branch: {std.Branch} Name: {std.Name}");
                        }
                    }
                }
            }
        }
    }
}
