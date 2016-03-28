using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;


namespace TaskService.Controllers
{
    [Authorize]
    public class TasksController : ApiController
    {
        string obIDClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        private static List<Models.Task> tasks = new List<Models.Task>()
        {
            new Models.Task() { owner="bc19576c-dcae-472f-a11e-846f445ccc4e", task="generic task" },
            new Models.Task() { owner="bc19576c-dcae-472f-a11e-846f445ccc4e", task="another generic task" },
        };

        public IEnumerable<Models.Task> Get()
        {
            string owner = ClaimsPrincipal.Current.FindFirst(obIDClaimType).Value;
            IEnumerable<Models.Task> userTasks = tasks.Where(t => t.owner == owner);
            return userTasks;
        }

        public void Post(Models.Task task)
        {
            if (task.task == null || task.task == string.Empty)
                throw new WebException("Please provide a task description");

            string owner = ClaimsPrincipal.Current.FindFirst(obIDClaimType).Value;
            task.owner = owner;
            task.completed = false;
            task.date = DateTime.UtcNow;
            tasks.Add(task);
        }

        public void Delete(int id)
        {
            string owner = ClaimsPrincipal.Current.FindFirst(obIDClaimType).Value;
            Models.Task task = tasks.Where(t => t.owner.Equals(owner) && t.TaskID.Equals(id)).FirstOrDefault();
            tasks.Remove(task);
        }
    }
}
