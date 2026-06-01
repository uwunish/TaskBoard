using System;
using System.Collections.Generic;
using System.Text;

namespace TaskBoard.Application.Common.Exceptions
{
    public class ForbiddenException:Exception
    {
        public ForbiddenException() 
            : base("You do not have permission to perform this action.") { }
    }
}
