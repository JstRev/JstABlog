using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Dto
{
    public record PostPersistenceDto(
        string Id,
        string Title,
        string Content,
        long TimeStamp
        );
}
