using Microsoft.AspNetCore.Mvc;

namespace ECommerce513.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class WelcomeController : Controller
    {
        public IActionResult Index()
        {
            string name = "Mohamed";
            int age = 25;
            string address = "Mansoura";

            //Person person = new()
            //{
            //    Name = name,
            //    Age = age,
            //    Address = address
            //};

            List<string> skills = [];
            skills.Add("C++");
            skills.Add("C#");
            skills.Add("SQL Server");

            var person = new
            {
                Name = name,
                Age = age,
                Address = address,
                Skills = skills
            };


            return View(person);
        }
    }
}
