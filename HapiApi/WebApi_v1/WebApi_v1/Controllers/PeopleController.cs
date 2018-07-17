using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi_v1.Models;

namespace WebApi_v1.Controllers
{
    public class PeopleController : ApiController
    {
        List<Person> people = new List<Person>();

        PeopleController()
        {
            people.Add(new Person { FirstName = "Blaine", LastName = "Harris", Id = 1 });
            people.Add(new Person { FirstName = "JK", LastName = "Rowling", Id = 2 });
            people.Add(new Person { FirstName = "Ell", LastName = "Shep", Id = 3, Age = 25 });
        }

        [Route("api/People/GetFirstNames/{userId:int?}/{age:int?}")]
        [HttpGet]
        public List<string> GetFirstNames(int userId = -1, int age = -1)
        {
            List<string> output = new List<string>();

            foreach (var p in people)
            {
                if (userId != -1 && age != -1)
                {
                    if (p.Id == userId && p.Age == age)
                        output.Add(p.FirstName);
                }
                else if (userId != -1 && age == -1)
                {
                    if (p.Id == userId)
                        output.Add(p.FirstName);
                }
                else
                {
                    output.Add(p.FirstName);
                }
            }

            return output;
        }

        // GET: api/People
        public List<Person> Get()
        {
            return people;
        }

        // GET: api/People/5
        public Person Get(int id)
        {
            return people.Where(x => x.Id == id).FirstOrDefault();
        }

        // POST: api/People
        public void Post([FromBody]Person val)
        {
            people.Add(val);
        }

        // PUT: api/People/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/People/5
        public void Delete(int id)
        {
        }
    }
}
