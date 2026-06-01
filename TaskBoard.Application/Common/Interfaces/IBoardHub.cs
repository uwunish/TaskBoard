using System;
using System.Collections.Generic;
using System.Text;

namespace TaskBoard.Application.Common.Interfaces
{

    // Marker interface - gives IHubContext a type to work with
    // without Infrastructure needing to know about the real BoardHub class in the API project
    public interface IBoardHub
    {
    }
}
