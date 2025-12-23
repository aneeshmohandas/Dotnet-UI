using System;
using System.Collections.Generic;

namespace DotnetUI.Interfaces
{
    public interface IActivityService
    {
        bool Save(Activities entity);
        List<Activities> GetList(DateTime FromDate, DateTime ToDate);
        List<Activities> GetAll();        
    }
}