using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
namespace DotnetUI.Service
{
    public class ActivityService : IActivityService
    {

        IWebHostEnvironment _env;
        public ActivityService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public List<Activities> GetAll()
        {
            var logs = DotnetUiHelper.GetActivities(_env);
            return logs;
        }

        public List<Activities> GetList(DateTime FromDate, DateTime ToDate)
        {
            var logs = DotnetUiHelper.GetActivities(_env);
            return logs.Where(x => x.CreatedOn <= ToDate && x.CreatedOn >= FromDate)?.ToList();
        }

        public bool Save(Activities entity)
        {
            var logs = DotnetUiHelper.GetActivities(_env);
            logs.Add(entity);
            return DotnetUiHelper.UpdateActivities(_env, logs);
        }
    }
}