using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMessages.Commands
{
    public interface IGetProductList
    {
        public int Id { get; }
        public string Name { get; }
    }
}
