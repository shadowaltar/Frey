using Automata.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automata.Mechanisms
{
    public static class SecurityUniverse
    {
        public static T Lookup<T>(SecurityIdentifier securityIdentifier, string value) where T : Security
        {
            throw new NotImplementedException();
        }
    }
}
