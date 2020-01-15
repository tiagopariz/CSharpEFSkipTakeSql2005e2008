using System;
using System.Linq;
 
namespace CSharpEFSkipTakeSql2005e2008.Prompt
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbContext = new SkipTakeContext();
            var people = dbContext.People
                                    .OrderBy(x => x.Name)
                                    .Take(10)
                                    .Skip(2);
 
            foreach (var person in people)
            {
                Console.WriteLine($"Name: {person.Name}\t | Age: {person.Age}\t | Id: {person.Id}");
            }            
        }
    }
}