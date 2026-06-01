using System;
using System.Collections.Generic;
using System.Text;

namespace TaskBoard.Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string entityName, Guid id) 
            :base($"{entityName} with id '{id}' was not found.")
        { 
        }
    }
}
