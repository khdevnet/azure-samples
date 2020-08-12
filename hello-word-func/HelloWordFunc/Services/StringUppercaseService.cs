using System;
using System.Collections.Generic;
using System.Text;

namespace HelloWordFunc.Services
{
    public interface IStringUppercaseService
    {
        string Apply(string text);
    }

    public class StringUppercaseService : IStringUppercaseService
    {
        public string Apply(string text) => text.ToUpper();
    }
}
